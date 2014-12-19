using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Core.Threads
{
    /// <summary>
    /// 线程池辅助操作类
    /// </summary>
    public class ThreadPoolHelper
    {
        /// <summary>
        /// The delegate for explicit method.
        /// </summary>
        public delegate void WaitCallbackNew();

        /// <summary>
        /// The WaitCallback delegate object in actual of our thread pool.
        /// </summary>
        /// <param name="state"></param>
        static void Callback(object state)
        {
            var wcbh = (state as WaitCallbackHelper);
            wcbh.Callback();
            (wcbh.WaitHandle as AutoResetEvent).Set();
        }

        /// <summary>
        /// Queues a method for execution.
        /// The method executes when a thread pool thread becomes available.
        /// </summary>
        /// <param name="proc"></param>
        /// <returns></returns>
        public static bool QueueUserWorkItem(WaitCallbackNew callback)
        {
            var wcbh = new WaitCallbackHelper();
            wcbh.Callback = callback;
            wcbh.WaitHandle = new AutoResetEvent(false);
            if (_autoResetEvents == null)
            {
                _autoResetEvents = new List<WaitHandle>();
            }
            _autoResetEvents.Add(wcbh.WaitHandle);

            return ThreadPool.QueueUserWorkItem(new WaitCallback(Callback), wcbh);
        }

        /// <summary>
        /// Queues a few method for execution.
        /// The method executes when a thread pool thread becomes available.
        /// </summary>
        /// <param name="proc"></param>
        /// <returns></returns>
        public static bool QueueUserWorkItems(params WaitCallbackNew[] proc)
        {
            var result = true;
            foreach (var tp in proc)
            {
                result &= QueueUserWorkItem(tp);
            }
            return result;
        }

        /// <summary>
        /// (Collection)Notifies a waiting thread that an event has occurred. This class cannot be inherited.
        /// </summary>
        private static List<WaitHandle> _autoResetEvents;

        /// <summary>
        /// Waits for all the elements in the specified array to receive a signal.
        /// </summary>
        /// <returns>Waits for all the elements in the specified array to receive a signal.</returns>
        public static bool WaitAll()
        {
            return WaitHandle.WaitAll(_autoResetEvents.ToArray());
        }

        /// <summary>
        /// Waits for any of the elements in the specified array to receive a signal.
        /// </summary>
        /// <returns>The array index of the object that satisfied the wait.</returns>
        public static int WaitAny()
        {
            return WaitHandle.WaitAny(_autoResetEvents.ToArray());
        }

        /// <summary>
        /// A callback container with a WaitHandle.
        /// </summary>
        class WaitCallbackHelper
        {
            private WaitCallbackNew _callback;
            /// <summary>
            ///
            /// </summary>
            public WaitCallbackNew Callback
            {
                get
                {
                    return this._callback;
                }
                set
                {
                    this._callback = value;
                }
            }

            private WaitHandle _waitHandle;
            public WaitHandle WaitHandle
            {
                get
                {
                    return this._waitHandle;
                }
                set
                {
                    this._waitHandle = value;
                }
            }
        }
    }
}
