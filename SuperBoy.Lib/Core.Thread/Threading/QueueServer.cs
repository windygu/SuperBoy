using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Core.Threads
{
    /// <summary>
    /// 提供一个队列的线程处理
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QueueServer<T> : DisposableObject
    {
        private System.Threading.Thread _thread = null;
        private Queue<T> _queue = new Queue<T>();
        private bool _isBackground = false;

        public QueueServer()
        {
        }

        private bool _disposed = false;
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (!this._disposed)
                {
                    this.ClearItems();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        #region  公共方法

        public void EnqueueItem(T item)
        {
            lock (this._queue)
            {
                this._queue.Enqueue(item);
            }
            if ((this._thread == null) || !(this._thread.IsAlive))
            {
                this.CreateThread();
                this._thread.Start();
            }
        }

        public void ClearItems()
        {
            lock (this._queue)
            {
                this._queue.Clear();
            }
        }

        #endregion

        #region 线程处理

        private void CreateThread()
        {
            this._thread = new System.Threading.Thread(new ThreadStart(this.ThreadProc));
            this._thread.IsBackground = this._isBackground;
        }

        private void ThreadProc()
        {
            var item = default(T);
            while (true)
            {
                lock (this._queue)
                {
                    if (this._queue.Count > 0)
                    {
                        item = this._queue.Dequeue();
                    }
                    else
                    {
                        break;
                    }
                }
                try
                {
                    this.OnProcessItem(item);
                }
                catch
                {
                }
            }
        }

        protected virtual void OnProcessItem(T item)
        {
            if (ProcessItem != null)
            {
                ProcessItem(item);
            }
        }

        public event Action<T> ProcessItem;

        #endregion

        #region  属性

        public bool IsBackground
        {
            get
            {
                return this._isBackground;
            }
            set
            {
                this._isBackground = true;
                if ((this._thread != null) && (this._thread.IsAlive))
                {
                    this._thread.IsBackground = this._isBackground;
                }
            }
        }

        public T[] Items
        {
            get
            {
                lock (this._queue)
                {
                    return this._queue.ToArray();
                }
            }
        }

        public int QueueCount
        {
            get
            {
                lock (this._queue)
                {
                    return this._queue.Count;
                }
            }
        }

        #endregion
    }
}
