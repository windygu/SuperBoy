using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Web.CacheManage.Memcached
{
	internal delegate T UseSocket<T>(PooledSocket socket);
	internal delegate void UseSocket(PooledSocket socket);

	/// <summary>
	/// The ServerPool encapsulates a collection of memcached servers and the associated SocketPool objects.
	/// This class contains the server-selection logic, and contains methods for executing a block of code on 
	/// a socket from the server corresponding to a given key.
	/// </summary>
	internal class ServerPool {
		private static LogAdapter _logger = LogAdapter.GetLogger(typeof(ServerPool));

		//Expose the socket pools.
		private SocketPool[] _hostList;
		internal SocketPool[] HostList { get { return _hostList; } }

		private Dictionary<uint, SocketPool> _hostDictionary;
		private uint[] _hostKeys;

		//Internal configuration properties
		private int _sendReceiveTimeout = 2000;
		private int _connectTimeout = 2000;
		private uint _maxPoolSize = 10;
		private uint _minPoolSize = 5;
		private TimeSpan _socketRecycleAge = TimeSpan.FromMinutes(30);
		internal int SendReceiveTimeout { get { return _sendReceiveTimeout; } set { _sendReceiveTimeout = value; } }
		internal int ConnectTimeout { get { return _connectTimeout; } set { _connectTimeout = value; } }
		internal uint MaxPoolSize { get { return _maxPoolSize; } set { _maxPoolSize = value; } }
		internal uint MinPoolSize { get { return _minPoolSize; } set { _minPoolSize = value; } }
		internal TimeSpan SocketRecycleAge { get { return _socketRecycleAge; } set { _socketRecycleAge = value; } }

		/// <summary>
		/// Internal constructor. This method takes the array of hosts and sets up an internal list of socketpools.
		/// </summary>
		internal ServerPool(string[] hosts) {
			_hostDictionary = new Dictionary<uint, SocketPool>();
			var pools = new List<SocketPool>();
			var keys = new List<uint>();
			foreach(var host in hosts) {
				//Create pool
				var pool = new SocketPool(this, host.Trim());

				//Create 250 keys for this pool, store each key in the hostDictionary, as well as in the list of keys.
				for (var i = 0; i < 250; i++) {
					var key = BitConverter.ToUInt32(new ModifiedFnv132().ComputeHash(Encoding.UTF8.GetBytes(host + "-" + i)), 0);
					if (!_hostDictionary.ContainsKey(key)) {
						_hostDictionary[key] = pool;
						keys.Add(key);
					}
				}

				pools.Add(pool);
			}

			//Hostlist should contain the list of all pools that has been created.
			_hostList = pools.ToArray();

			//Hostkeys should contain the list of all key for all pools that have been created.
			//This array forms the server key continuum that we use to lookup which server a
			//given item key hash should be assigned to.
			keys.Sort();
			_hostKeys = keys.ToArray();
		}

		/// <summary>
		/// Given an item key hash, this method returns the socketpool which is closest on the server key continuum.
		/// </summary>
		internal SocketPool GetSocketPool(uint hash) {
			//Quick return if we only have one host.
			if (_hostList.Length == 1) {
				return _hostList[0];
			}

			//New "ketama" host selection.
			var i = Array.BinarySearch(_hostKeys, hash);

			//If not exact match...
			if(i < 0) {
				//Get the index of the first item bigger than the one searched for.
				i = ~i;

				//If i is bigger than the last index, it was bigger than the last item = use the first item.
				if (i >= _hostKeys.Length) {
					i = 0;
				}
			}
			return _hostDictionary[_hostKeys[i]];
		}

		internal SocketPool GetSocketPool(string host) {
			return Array.Find(HostList, delegate(SocketPool socketPool) { return socketPool.Host == host; });
		}

		/// <summary>
		/// This method executes the given delegate on a socket from the server that corresponds to the given hash.
		/// If anything causes an error, the given defaultValue will be returned instead.
		/// This method takes care of disposing the socket properly once the delegate has executed.
		/// </summary>
		internal T Execute<T>(uint hash, T defaultValue, UseSocket<T> use) {
			return Execute(GetSocketPool(hash), defaultValue, use);
		}

		internal T Execute<T>(SocketPool pool, T defaultValue, UseSocket<T> use) {
			PooledSocket sock = null;
			try {
				//Acquire a socket
				sock = pool.Acquire();

				//Use the socket as a parameter to the delegate and return its result.
				if (sock != null) {
					return use(sock);
				}
			} catch(Exception e) {
				_logger.Error("Error in Execute<T>: " + pool.Host, e);

				//Socket is probably broken
				if (sock != null) {
					sock.Close();
				}
			} finally {
				if (sock != null) {
					sock.Dispose();
				}
			}
			return defaultValue;
		}

		internal void Execute(SocketPool pool, UseSocket use) {
			PooledSocket sock = null;
			try {
				//Acquire a socket
				sock = pool.Acquire();

				//Use the socket as a parameter to the delegate and return its result.
				if (sock != null) {
					use(sock);
				}
			} catch(Exception e) {
				_logger.Error("Error in Execute: " + pool.Host, e);

				//Socket is probably broken
				if (sock != null) {
					sock.Close();
				}
			}
			finally {
				if(sock != null) {
					sock.Dispose();
				}
			}
		}

		/// <summary>
		/// This method executes the given delegate on all servers.
		/// </summary>
		internal void ExecuteAll(UseSocket use) {
			foreach(var socketPool in _hostList){
				Execute(socketPool, use);
			}
		}
	}
}