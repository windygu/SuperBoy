using System.Threading;

namespace Core.Threads
{
    /// <summary>
    /// WorkItem that stores the supplied WaitCallback and user state object,
    /// as well as the current ExecutionContext
    /// </summary>
    public sealed class WorkItem
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:WorkItem"/> class.
        /// </summary>
        /// <param name="callback">The wc.</param>
        /// <param name="state">The state.</param>
        /// <param name="context">The CTX.</param>
        internal WorkItem(WaitCallback callback, object state, ExecutionContext context)
        {
            _callback = callback;
            _state = state;
            _context = context;
        }
        #endregion

        #region Fields
        private WaitCallback _callback;
        private object _state;
        private ExecutionContext _context;

        #endregion

        #region Properties
        /// <summary>
        /// a callback method to be executed by a thread pool thread. 
        /// </summary>
        public WaitCallback Callback { get { return _callback; } }

        /// <summary>
        /// An object containing information to be used by the callback method. 
        /// </summary>
        public object State { get { return _state; } }

        /// <summary>
        /// the execution context for the current thread
        /// </summary>
        public ExecutionContext Context { get { return _context; } }
        #endregion
    }
}
