﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Drawing;

namespace Core.Threads
{
    /*
     *         
       private QueuedBackgroundWorker worker;

        public Form1()
        {
            InitializeComponent();

            worker = new QueuedBackgroundWorker();
            worker.IsBackground = true;
            worker.Threads = 1;
            worker.ProcessingMode = ProcessingMode.FIFO;
            worker.DoWork += new QueuedWorkerDoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunQueuedWorkerCompletedEventHandler(worker_RunWorkerCompleted);
        }

        
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            worker.DoWork -= worker_DoWork;
            worker.RunWorkerCompleted -= worker_RunWorkerCompleted;
            worker.Dispose();
        }

        private void GetVerifyImage()
        {
            worker.RunWorkerAsync("GetImage");
        }

        void worker_DoWork(object sender, QueuedWorkerDoWorkEventArgs e)
        {
            string verifyUrl = "http://ui.ptlogin2.qq.com/cgi-bin/getimage?aid=1000801&0.5723910723542236";
            string newverifyUrl = "http://captcha.qq.com/getimage?aid=1007901&0.5723910723542236";
            string referer = "http://reg.qq.com";

            cookieReg = new CookieContainer();
            HttpHelper httpHelper = new HttpHelper();
            using (Stream s = httpHelper.GetStream(newverifyUrl, cookieReg, referer))
            {
                if (s == null)
                {
                    MessageUtil.ShowWarning("获取登陆码错误，请检查您的网络!");
                    return;
                }
                e.Result = Image.FromStream(s);
            }
        }

        void worker_RunWorkerCompleted(object sender, QueuedWorkerCompletedEventArgs e)
        {
            Image image = e.Result as Image;
            if (image != null)
            {
                this.pictureBox1.Image = image;
            }
        }
     */

    /// <summary>
    /// A background worker with a work queue.
    /// </summary>
    [Description("A background worker with a work queue.")]
    [ToolboxBitmap(typeof(QueuedBackgroundWorker))]
    [DefaultEvent("DoWork")]
    public class QueuedBackgroundWorker : Component
    {
        #region Member Variables
        private readonly object _lockObject;

        private ProcessingMode _processingMode;
        private int _threadCount;
        private System.Threading.Thread[] _threads;
        private bool _stopping;
        private bool _started;
        private bool _disposed;

        private int _priorityQueues;
        private LinkedList<AsyncOperation>[] _items;
        private Dictionary<object, bool> _cancelledItems;

        private readonly SendOrPostCallback _workCompletedCallback;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="QueuedBackgroundWorker"/> class.
        /// </summary>
        public QueuedBackgroundWorker()
        {
            _lockObject = new object();
            _stopping = false;
            _started = false;
            _disposed = false;

            // Threads
            _threadCount = 5;
            CreateThreads();

            // Work items
            _processingMode = ProcessingMode.Fifo;
            _priorityQueues = 5;
            BuildWorkQueue();
            _cancelledItems = new Dictionary<object, bool>();

            // The loader complete callback
            _workCompletedCallback = new SendOrPostCallback(this.RunWorkerCompletedCallback);
        }
        #endregion

        #region RunWorkerAsync
        /// <summary>
        /// Starts processing a new background operation.
        /// </summary>
        /// <param name="argument">The argument of an asynchronous operation.</param>
        /// <param name="priority">A value between 0 and <see cref="PriorityQueues"/> indicating the priority of this item.
        /// An item with a higher priority will be processed before items with lower priority.</param>
        public void RunWorkerAsync(object argument, int priority)
        {
            if (priority < 0 || priority >= _priorityQueues)
                throw new ArgumentException("priority must be between 0 and " + (_priorityQueues - 1).ToString() + "  inclusive.", "priority");

            // Start the worker threads
            if (!_started)
            {
                // Start the thread
                for (var i = 0; i < _threadCount; i++)
                {
                    _threads[i].Start();
                    while (!_threads[i].IsAlive) ;
                }

                _started = true;
            }

            lock (_lockObject)
            {
                AddWork(argument, priority);
                Monitor.Pulse(_lockObject);
            }
        }
        /// <summary>
        /// Starts processing a new background operation.
        /// </summary>
        /// <param name="argument">The argument of an asynchronous operation.</param>
        public void RunWorkerAsync(object argument)
        {
            RunWorkerAsync(argument, 0);
        }
        /// <summary>
        /// Starts processing a new background operation.
        /// </summary>
        public void RunWorkerAsync()
        {
            RunWorkerAsync(null, 0);
        }
        #endregion

        #region Work Queue Access
        /// <summary>
        /// Determines if the work queue is empty.
        /// </summary>
        /// <returns>true if the work queue is empty; otherwise false.</returns>
        private bool IsWorkQueueEmpty()
        {
            foreach (var queue in _items)
            {
                if (queue.Count > 0)
                    return false;
            }

            return true;
        }
        /// <summary>
        /// Adds the operation to the work queue.
        /// </summary>
        /// <param name="argument">The argument of an asynchronous operation.</param>
        /// <param name="priority">A value between 0 and <see cref="PriorityQueues"/> indicating the priority of this item.
        /// An item with a higher priority will be processed before items with lower priority.</param>
        private void AddWork(object argument, int priority)
        {
            // Create an async operation for this work item
            var asyncOp = AsyncOperationManager.CreateOperation(argument);

            if (_processingMode == ProcessingMode.Fifo)
                _items[priority].AddLast(asyncOp);
            else
                _items[priority].AddFirst(asyncOp);
        }
        /// <summary>
        /// Gets a pending operation from the work queue.
        /// </summary>
        /// <returns>A 2-tuple whose first component is the the pending operation with
        /// the highest priority from the qork queue and the second component is the
        /// priority.</returns>
        private Tuple<AsyncOperation, int> GetWork()
        {
            AsyncOperation request = null;
            var priority = 0;

            for (var i = _priorityQueues - 1; i >= 0; i--)
            {
                if (_items[i].Count > 0)
                {
                    priority = i;
                    request = _items[i].First.Value;
                    _items[i].RemoveFirst();
                    break;
                }
            }

            return Tuple.Create(request, priority);
        }
        /// <summary>
        /// Rebuilds the work queue.
        /// </summary>
        private void BuildWorkQueue()
        {
            _items = new LinkedList<AsyncOperation>[_priorityQueues];
            for (var i = 0; i < _priorityQueues; i++)
                _items[i] = new LinkedList<AsyncOperation>();
        }
        /// <summary>
        /// Clears the work queue.
        /// </summary>
        private void ClearWorkQueue()
        {
            for (var i = 0; i < _priorityQueues; i++)
                ClearWorkQueue(i);
        }
        /// <summary>
        /// Clears the work queue with the given priority.
        /// </summary>
        /// <param name="priority">A value between 0 and <see cref="PriorityQueues"/>
        /// indicating the priority queue to cancel.</param>
        private void ClearWorkQueue(int priority)
        {
            while (_items[priority].Count > 0)
            {
                var asyncOp = _items[priority].First.Value;
                asyncOp.OperationCompleted();
                _items[priority].RemoveFirst();
            }
        }
        #endregion

        #region Worker Threads
        /// <summary>
        /// Creates the thread array.
        /// </summary>
        private void CreateThreads()
        {
            _threads = new System.Threading.Thread[_threadCount];
            for (var i = 0; i < _threadCount; i++)
            {
                _threads[i] = new System.Threading.Thread(new ThreadStart(Run));
                _threads[i].IsBackground = true;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Represents the mode in which the work items are processed.
        /// Processing mode cannot be changed after any work is added to the work queue.
        /// </summary>
        [Browsable(true), Category("Behaviour"), DefaultValue(typeof(ProcessingMode), "FIFO")]
        public ProcessingMode ProcessingMode
        {
            get { return _processingMode; }
            set
            {
                if (_started)
                    throw new System.Threading.ThreadStateException("The thread has already been started.");

                _processingMode = value;
                BuildWorkQueue();
            }
        }
        /// <summary>
        /// Gets or sets the number of priority queues. Number of queues
        /// cannot be changed after any work is added to the work queue.
        /// </summary>
        [Browsable(true), Category("Behaviour"), DefaultValue(5)]
        public int PriorityQueues
        {
            get { return _priorityQueues; }
            set
            {
                if (_started)
                    throw new System.Threading.ThreadStateException("The thread has already been started.");

                _priorityQueues = value;
                BuildWorkQueue();
            }
        }
        /// <summary>
        /// Determines whether the <see cref="QueuedBackgroundWorker"/> started working.
        /// </summary>
        [Browsable(false), Description("Determines whether the QueuedBackgroundWorker started working."), Category("Behavior")]
        public bool Started { get { return _started; } }
        /// <summary>
        /// Gets or sets a value indicating whether or not the worker thread is a background thread.
        /// </summary>
        [Browsable(true), Description("Gets or sets a value indicating whether or not the worker thread is a background thread."), Category("Behavior")]
        public bool IsBackground
        {
            get { return _threads[0].IsBackground; }
            set
            {
                for (var i = 0; i < _threadCount; i++)
                    _threads[i].IsBackground = value;
            }
        }
        /// <summary>
        /// Determines whether the <see cref="QueuedBackgroundWorker"/> is being stopped.
        /// </summary>
        private bool Stopping { get { lock (_lockObject) { return _stopping; } } }
        /// <summary>
        /// Gets or sets the number of worker threads. Number of threads
        /// cannot be changed after any work is added to the work queue.
        /// </summary>
        [Browsable(true), Category("Behaviour"), DefaultValue(5)]
        public int Threads
        {
            get { return _threadCount; }
            set
            {
                if (_started)
                    throw new System.Threading.ThreadStateException("The thread has already been started.");

                _threadCount = value;
                CreateThreads();
            }
        }
        #endregion

        #region Cancel
        /// <summary>
        /// Cancels all pending operations in all queues.
        /// </summary>
        public void CancelAsync()
        {
            lock (_lockObject)
            {
                ClearWorkQueue();
                Monitor.Pulse(_lockObject);
            }
        }
        /// <summary>
        /// Cancels all pending operations in the given queue.
        /// </summary>
        /// <param name="priority">A value between 0 and <see cref="PriorityQueues"/>
        /// indicating the priority queue to cancel.</param>
        public void CancelAsync(int priority)
        {
            if (priority < 0 || priority >= _priorityQueues)
                throw new ArgumentException("priority must be between 0 and " + (_priorityQueues - 1).ToString() + "  inclusive.", "priority");

            lock (_lockObject)
            {
                ClearWorkQueue(priority);
                Monitor.Pulse(_lockObject);
            }
        }
        /// <summary>
        /// Cancels processing the item with the given key.
        /// </summary>
        /// <param name="argument">The argument of an asynchronous operation.</param>
        public void CancelAsync(object argument)
        {
            lock (_lockObject)
            {
                if (!_cancelledItems.ContainsKey(argument))
                {
                    _cancelledItems.Add(argument, false);
                    Monitor.Pulse(_lockObject);
                }
            }
        }
        #endregion

        #region Delegate Callbacks
        /// <summary>
        /// Used to call <see cref="OnRunWorkerCompleted"/> by the synchronization context.
        /// </summary>
        /// <param name="arg">The argument.</param>
        private void RunWorkerCompletedCallback(object arg)
        {
            OnRunWorkerCompleted((QueuedWorkerCompletedEventArgs)arg);
        }
        #endregion

        #region Virtual Methods
        /// <summary>
        /// Raises the RunWorkerCompleted event.
        /// </summary>
        /// <param name="e">A <see cref="QueuedWorkerCompletedEventArgs"/> that contains event data.</param>
        protected virtual void OnRunWorkerCompleted(QueuedWorkerCompletedEventArgs e)
        {
            if (RunWorkerCompleted != null)
                RunWorkerCompleted(this, e);
        }
        /// <summary>
        /// Raises the DoWork event.
        /// </summary>
        /// <param name="e">A <see cref="QueuedWorkerDoWorkEventArgs"/> that contains event data.</param>
        protected virtual void OnDoWork(QueuedWorkerDoWorkEventArgs e)
        {
            if (DoWork != null)
                DoWork(this, e);
        }
        #endregion

        #region Get/Set Apartment State
        /// <summary>
        /// Gets the apartment state of the worker thread.
        /// </summary>
        public ApartmentState GetApartmentState()
        {
            return _threads[0].GetApartmentState();
        }
        /// <summary>
        /// Sets the apartment state of the worker thread. The apartment state
        /// cannot be changed after any work is added to the work queue.
        /// </summary>
        public void SetApartmentState(ApartmentState state)
        {
            for (var i = 0; i < _threadCount; i++)
                _threads[i].SetApartmentState(state);
        }
        #endregion

        #region Public Events
        /// <summary>
        /// Occurs when the background operation of an item has completed,
        /// has been canceled, or has raised an exception.
        /// </summary>
        [Category("Behavior"), Browsable(true), Description("Occurs when the background operation of an item has completed.")]
        public event RunQueuedWorkerCompletedEventHandler RunWorkerCompleted;
        /// <summary>
        /// Occurs when <see cref="RunWorkerAsync(object, int)" /> is called.
        /// </summary>
        [Category("Behavior"), Browsable(true), Description("Occurs when RunWorkerAsync is called.")]
        public event QueuedWorkerDoWorkEventHandler DoWork;
        #endregion

        #region Worker Method
        /// <summary>
        /// Used by the worker thread to process items.
        /// </summary>
        private void Run()
        {
            while (!Stopping)
            {
                lock (_lockObject)
                {
                    // Wait until we have pending work items
                    if (IsWorkQueueEmpty())
                        Monitor.Wait(_lockObject);
                }

                // Loop until we exhaust the queue
                var queueFull = true;
                while (queueFull && !Stopping)
                {
                    // Get an item from the queue
                    AsyncOperation asyncOp = null;
                    object request = null;
                    var priority = 0;
                    lock (_lockObject)
                    {
                        // Check queues
                        var work = GetWork();
                        asyncOp = work.Item1;
                        priority = work.Item2;
                        if (asyncOp != null)
                            request = asyncOp.UserSuppliedState;

                        // Check if the item was removed
                        if (request != null && _cancelledItems.ContainsKey(request))
                            request = null;
                    }

                    if (request != null)
                    {
                        Exception error = null;
                        // Start the work
                        var arg = new QueuedWorkerDoWorkEventArgs(request, priority);
                        try
                        {
                            // Raise the do work event
                            OnDoWork(arg);
                        }
                        catch (Exception e)
                        {
                            error = e;
                        }

                        // Raise the work complete event
                        var arg2 = new QueuedWorkerCompletedEventArgs(request,
                            arg.Result, priority, error, arg.Cancel);
                        if (!Stopping)
                            asyncOp.PostOperationCompleted(_workCompletedCallback, arg2);
                    }
                    else if (asyncOp != null)
                        asyncOp.OperationCompleted();

                    // Check if the cache is exhausted
                    lock (_lockObject)
                    {
                        queueFull = !IsWorkQueueEmpty();
                    }
                }
            }
        }
        #endregion

        #region Dispose
        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.ComponentModel.Component"/>
        /// and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources;
        /// false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (_disposed)
                return;

            lock (_lockObject)
            {
                if (!_stopping)
                {
                    _stopping = true;
                    ClearWorkQueue();
                    _cancelledItems.Clear();
                    Monitor.Pulse(_lockObject);
                }
            }

            _disposed = true;
        }
        #endregion
    }

    #region Tuples
    /// <summary>
    /// Represents a factory class for creating tuples.
    /// </summary>
    internal static class Tuple
    {
        /// <summary>
        /// Creates a new 1-tuple.
        /// </summary>
        /// <typeparam name="T1">The type of the only component of the tuple.</typeparam>
        /// <param name="item1">The value of the only component of the tuple.</param>
        /// <returns>A 1-tuple whose value is (<paramref name="item1"/>).</returns>
        public static Tuple<T1> Create<T1>(T1 item1)
        {
            return new Tuple<T1>(item1);
        }
        /// <summary>
        /// Creates a new 2-tuple.
        /// </summary>
        /// <typeparam name="T1">The type of the first component of the tuple.</typeparam>
        /// <typeparam name="T2">The type of the second component of the tuple.</typeparam>
        /// <param name="item1">The value of the first component of the tuple.</param>
        /// <param name="item2">The value of the second component of the tuple.</param>
        /// <returns>A 2-tuple whose value is (<paramref name="item1"/>, <paramref name="item2"/>).</returns>
        public static Tuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2)
        {
            return new Tuple<T1, T2>(item1, item2);
        }
    }
    /// <summary>
    /// Represents a tuple with one element.
    /// </summary>
    internal class Tuple<T1>
    {
        private T1 _mItem1;

        /// <summary>
        /// Gets the value of the first component.
        /// </summary>
        public T1 Item1 { get { return _mItem1; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tuple&lt;T1&gt;"/> class.
        /// </summary>
        /// <param name="item1">The value of the first component of the tuple.</param>
        public Tuple(T1 item1)
        {
            _mItem1 = item1;
        }
    }
    /// <summary>
    /// Represents a tuple with two elements.
    /// </summary>
    internal class Tuple<T1, T2> : Tuple<T1>
    {
        private T2 _mItem2;

        /// <summary>
        /// Gets the value of the second component.
        /// </summary>
        public T2 Item2 { get { return _mItem2; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tuple&lt;T1, T2&gt;"/> class.
        /// </summary>
        /// <param name="item1">The value of the first component of the tuple.</param>
        /// <param name="item2">The value of the second component of the tuple.</param>
        public Tuple(T1 item1, T2 item2)
            : base(item1)
        {
            _mItem2 = item2;
        }
    }
    #endregion

    #region QueuedBackgroundWorker Public Enums
    /// <summary>
    /// Represents the mode in which the work items of <see cref="QueuedBackgroundWorker"/> are processed.
    /// </summary>
    public enum ProcessingMode
    {
        /// <summary>
        /// Items are processed in the order they are received.
        /// </summary>
        Fifo,
        /// <summary>
        /// Items are processed in reverse order.
        /// </summary>
        Lifo,
    }
    #endregion
}
