using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;

namespace Core.Web.CacheManage.Memcached
{
	/// <summary>
	/// The SocketPool encapsulates the list of PooledSockets against one specific host, and contains methods for 
	/// acquiring or returning PooledSockets.
	/// </summary>
	[DebuggerDisplay("[ Host: {Host} ]")]
	internal class SocketPool {
		private static LogAdapter _logger = LogAdapter.GetLogger(typeof(SocketPool));

		/// <summary>
		/// If the host stops responding, we mark it as dead for this amount of seconds, 
		/// and we double this for each consecutive failed retry. If the host comes alive
		/// again, we reset this to 1 again.
		/// </summary>
		private int _deadEndPointSecondsUntilRetry = 1;
		private const int MaxDeadEndPointSecondsUntilRetry = 60*10; //10 minutes
		private ServerPool _owner;
		private IPEndPoint _endPoint;
		private Queue<PooledSocket> _queue;

		//Debug variables and properties
		private int _newsockets = 0;
		private int _failednewsockets = 0;
		private int _reusedsockets = 0;
		private int _deadsocketsinpool = 0;
		private int _deadsocketsonreturn = 0;
		private int _dirtysocketsonreturn = 0;
		private int _acquired = 0;
		public int NewSockets { get { return _newsockets; } }
		public int FailedNewSockets { get { return _failednewsockets; } }
		public int ReusedSockets { get { return _reusedsockets; } }
		public int DeadSocketsInPool { get { return _deadsocketsinpool; } }
		public int DeadSocketsOnReturn { get { return _deadsocketsonreturn; } }
		public int DirtySocketsOnReturn { get { return _dirtysocketsonreturn; } }
		public int Acquired { get { return _acquired; } }
		public int Poolsize { get { return _queue.Count; } }

		//Public variables and properties
		public readonly string Host;

		private bool _isEndPointDead = false;
		public bool IsEndPointDead { get { return _isEndPointDead; } }

		private DateTime _deadEndPointRetryTime;
		public DateTime DeadEndPointRetryTime { get { return _deadEndPointRetryTime; } }

		internal SocketPool(ServerPool owner, string host) {
			Host = host;
			this._owner = owner;
			_endPoint = GetEndPoint(host);
			_queue = new Queue<PooledSocket>();
		}

		/// <summary>
		/// This method parses the given string into an IPEndPoint.
		/// If the string is malformed in some way, or if the host cannot be resolved, this method will throw an exception.
		/// </summary>
		private static IPEndPoint GetEndPoint(string host) {
			//Parse port, default to 11211.
			var port = 11211;
			if(host.Contains(":")) {
				var split = host.Split(new char[] { ':' });
				if(!Int32.TryParse(split[1], out port)) {
					throw new ArgumentException("Unable to parse host: " + host);
				}
				host = split[0];
			}

			//Parse host string.
			IPAddress address;
			if(IPAddress.TryParse(host, out address)) {
				//host string successfully resolved as an IP address.
			} else {
				//See if we can resolve it as a hostname
				try {
					address = Dns.GetHostEntry(host).AddressList[0];
				} catch(Exception e) {
					throw new ArgumentException("Unable to resolve host: " + host, e);
				}
			}

			return new IPEndPoint(address, port);
		}

		/// <summary>
		/// Gets a socket from the pool.
		/// If there are no free sockets, a new one will be created. If something goes
		/// wrong while creating the new socket, this pool's endpoint will be marked as dead
		/// and all subsequent calls to this method will return null until the retry interval
		/// has passed.
		/// </summary>
		internal PooledSocket Acquire() {
			//Do we have free sockets in the pool?
			//if so - return the first working one.
			//if not - create a new one.
			Interlocked.Increment(ref _acquired);
			lock(_queue) {
				while(_queue.Count > 0) {
					var socket = _queue.Dequeue();
					if(socket != null && socket.IsAlive) {
						Interlocked.Increment(ref _reusedsockets);
						return socket;
					}
					Interlocked.Increment(ref _deadsocketsinpool);
				}
			}

			Interlocked.Increment(ref _newsockets);
			//If we know the endpoint is dead, check if it is time for a retry, otherwise return null.
			if (_isEndPointDead) {
				if (DateTime.Now > _deadEndPointRetryTime) {
					//Retry
					_isEndPointDead = false;
				} else {
					//Still dead
					return null;
				}
			} 

			//Try to create a new socket. On failure, mark endpoint as dead and return null.
			try {
				var socket = new PooledSocket(this, _endPoint, _owner.SendReceiveTimeout, _owner.ConnectTimeout);
				//Reset retry timer on success.
				_deadEndPointSecondsUntilRetry = 1;
				return socket;
			}
			catch (Exception e) {
				Interlocked.Increment(ref _failednewsockets);
				_logger.Error("Error connecting to: " + _endPoint.Address, e);
				//Mark endpoint as dead
				_isEndPointDead = true;
				//Retry in 2 minutes
				_deadEndPointRetryTime = DateTime.Now.AddSeconds(_deadEndPointSecondsUntilRetry);
				if (_deadEndPointSecondsUntilRetry < MaxDeadEndPointSecondsUntilRetry) {
					_deadEndPointSecondsUntilRetry = _deadEndPointSecondsUntilRetry * 2; //Double retry interval until next time
				}
				return null;
			}
		}

		/// <summary>
		/// Returns a socket to the pool.
		/// If the socket is dead, it will be destroyed.
		/// If there are more than MaxPoolSize sockets in the pool, it will be destroyed.
		/// If there are less than MinPoolSize sockets in the pool, it will always be put back.
		/// If there are something inbetween those values, the age of the socket is checked. 
		/// If it is older than the SocketRecycleAge, it is destroyed, otherwise it will be 
		/// put back in the pool.
		/// </summary>
		internal void Return(PooledSocket socket) {
			//If the socket is dead, destroy it.
			if (!socket.IsAlive) {
				Interlocked.Increment(ref _deadsocketsonreturn);
				socket.Close();
			} else {
				//Clean up socket
				if (socket.Reset()) {
					Interlocked.Increment(ref _dirtysocketsonreturn);
				}

				//Check pool size.
				if (_queue.Count >= _owner.MaxPoolSize) {
					//If the pool is full, destroy the socket.
					socket.Close();
				} else if (_queue.Count > _owner.MinPoolSize && DateTime.Now - socket.Created > _owner.SocketRecycleAge) {
					//If we have more than the minimum amount of sockets, but less than the max, and the socket is older than the recycle age, we destroy it.
					socket.Close();
				} else {
					//Put the socket back in the pool.
					lock (_queue) {
						_queue.Enqueue(socket);
					}
				}
			}
		}
	}
}
