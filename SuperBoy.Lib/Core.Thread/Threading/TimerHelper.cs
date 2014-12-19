﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace WHC.OrderWater.Commons
{
    /// <summary>
    /// Provides the definition to create timers within their own threads and within critical sections.
    /// </summary>
    [Serializable]
    public class TimerHelper : System.ComponentModel.Component
    {
        private System.Threading.Timer _timer;
        private long _timerInterval;
        private TimerState _timerState;

        /// <summary>
        /// The function prototype for timer executions.
        /// </summary>
        public delegate void TimerExecution();

        /// <summary>
        /// Called when the timer is executed.
        /// </summary>
        public event TimerExecution Execute;

        /// <summary>
        /// Creates a timer with a specified interval, and starts after the specified delay.
        /// </summary>
        public TimerHelper()
        {
            _timerInterval = 100;
            _timerState = TimerState.Stopped;
            _timer = new System.Threading.Timer(new TimerCallback(Tick), null, Timeout.Infinite, _timerInterval);
        }


        /// <summary>
        /// Creates a timer with a specified interval, and starts after the specified delay.
        /// </summary>
        /// <param name="interval">Interval in milliseconds at which the timer will execute.</param>
        /// <param name="startDelay"></param>
        public TimerHelper(long interval, int startDelay)
        {
            _timerInterval = interval;
            _timerState = (startDelay == Timeout.Infinite) ? TimerState.Stopped : TimerState.Running;
            _timer = new System.Threading.Timer(new TimerCallback(Tick), null, startDelay, interval);
        }

        /// <summary>
        /// Creates a timer with a specified interval.
        /// </summary>
        /// <param name="interval"></param>
        public TimerHelper(long interval, bool start)
        {
            _timerInterval = interval;
            _timerState = (!start) ? TimerState.Stopped : TimerState.Running;
            _timer = new System.Threading.Timer(new TimerCallback(Tick), null, 0, interval);
        }

        /// <summary>
        /// Starts the timer with a specified delay.
        /// </summary>
        /// <param name="delayBeforeStart"></param>
        public void Start(int delayBeforeStart)
        {
            _timerState = TimerState.Running;
            _timer.Change(delayBeforeStart, _timerInterval);
        }

        /// <summary>
        /// Starts the timer instantly.
        /// </summary>
        public void Start()
        {
            _timerState = TimerState.Running;
            _timer.Change(0, _timerInterval);
        }

        /// <summary>
        /// Pauses the timer.
        /// Note: Running threads won't be closed.
        /// </summary>
        public void Pause()
        {
            _timerState = TimerState.Paused;
            _timer.Change(Timeout.Infinite, _timerInterval);
        }

        /// <summary>
        /// Stops the timer.
        /// Note: Running threads won't be closed.
        /// </summary>
        public void Stop()
        {
            _timerState = TimerState.Stopped;
            _timer.Change(Timeout.Infinite, _timerInterval);
        }

        public void Tick(object obj)
        {
            if (_timerState == TimerState.Running && Execute != null)
            {
                lock (this)
                {
                    Execute();
                }
            }
        }

        /// <summary>
        /// Gets the state of the timer.
        /// </summary>
        public TimerState State
        {
            get
            {
                return _timerState;
            }
        }

        /// <summary>
        /// Gets or sets the timer interval.
        /// </summary>
        public long Interval
        {
            get
            {
                return _timerInterval;
            }
            set
            {
                _timer.Change(((_timerState == TimerState.Running) ? value : Timeout.Infinite), value);
            }
        }
    }

    public enum TimerState
    {
        Stopped,
        Running,
        Paused
    }
}