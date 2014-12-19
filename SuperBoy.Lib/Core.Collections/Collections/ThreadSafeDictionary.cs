﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections;

namespace Core.Collections
{
	public interface IThreadSafeDictionary<TKey, TValue> : IDictionary<TKey, TValue>
	{
		/// <summary>
		/// Merge is similar to the SQL merge or upsert statement.  
		/// </summary>
		/// <param name="key">Key to lookup</param>
		/// <param name="newValue">New Value</param>
		void MergeSafe(TKey key, TValue newValue);


		/// <summary>
		/// This is a blind remove. Prevents the need to check for existence first.
		/// </summary>
		/// <param name="key">Key to Remove</param>
		void RemoveSafe(TKey key);
	}


	[Serializable]
	public class ThreadSafeDictionary<TKey, TValue> : IThreadSafeDictionary<TKey, TValue>
	{
		//This is the internal dictionary that we are wrapping
		IDictionary<TKey, TValue> _dict = new Dictionary<TKey, TValue>();


		[NonSerialized]
		ReaderWriterLockSlim _dictionaryLock = Locks.GetLockInstance(LockRecursionPolicy.NoRecursion); //setup the lock;


		/// <summary>
		/// This is a blind remove. Prevents the need to check for existence first.
		/// </summary>
		/// <param name="key">Key to remove</param>
		public void RemoveSafe(TKey key)
		{
			using(new ReadLock(this._dictionaryLock))
			{
				if(this._dict.ContainsKey(key))
				{
					using(new WriteLock(this._dictionaryLock))
					{
						this._dict.Remove(key);
					}
				}
			}
		}


		/// <summary>
		/// Merge does a blind remove, and then add.  Basically a blind Upsert.  
		/// </summary>
		/// <param name="key">Key to lookup</param>
		/// <param name="newValue">New Value</param>
		public void MergeSafe(TKey key, TValue newValue)
		{
			using(new WriteLock(this._dictionaryLock)) // take a writelock immediately since we will always be writing
			{
				if(this._dict.ContainsKey(key))
				{
					this._dict.Remove(key);
				}


				this._dict.Add(key, newValue);
			}
		}


		public virtual bool Remove(TKey key)
		{
			using(new WriteLock(this._dictionaryLock))
			{
				return this._dict.Remove(key);
			}
		}


		public virtual bool ContainsKey(TKey key)
		{
			using(new ReadOnlyLock(this._dictionaryLock))
			{
				return this._dict.ContainsKey(key);
			}
		}


		public virtual bool TryGetValue(TKey key, out TValue value)
		{
			using(new ReadOnlyLock(this._dictionaryLock))
			{
				return this._dict.TryGetValue(key, out value);
			}
		}


		public virtual TValue this[TKey key]
		{
			get
			{
				using(new ReadOnlyLock(this._dictionaryLock))
				{
					return this._dict[key];
				}
			}
			set
			{
				using(new WriteLock(this._dictionaryLock))
				{
					this._dict[key] = value;
				}
			}
		}


		public virtual ICollection<TKey> Keys
		{
			get
			{
				using(new ReadOnlyLock(this._dictionaryLock))
				{
					return new List<TKey>(this._dict.Keys);
				}
			}
		}


		public virtual ICollection<TValue> Values
		{
			get
			{
				using(new ReadOnlyLock(this._dictionaryLock))
				{
					return new List<TValue>(this._dict.Values);
				}
			}
		}


		public virtual void Clear()
		{
			using(new WriteLock(this._dictionaryLock))
			{
				this._dict.Clear();
			}
		}


		public virtual int Count
		{
			get
			{
				using(new ReadOnlyLock(this._dictionaryLock))
				{
					return this._dict.Count;
				}
			}
		}


		public virtual bool Contains(KeyValuePair<TKey, TValue> item)
		{
			using(new ReadOnlyLock(this._dictionaryLock))
			{
				return this._dict.Contains(item);
			}
		}


		public virtual void Add(KeyValuePair<TKey, TValue> item)
		{
			using(new WriteLock(this._dictionaryLock))
			{
				this._dict.Add(item);
			}
		}


		public virtual void Add(TKey key, TValue value)
		{
			using(new WriteLock(this._dictionaryLock))
			{
				this._dict.Add(key, value);
			}
		}


		public virtual bool Remove(KeyValuePair<TKey, TValue> item)
		{
			using(new WriteLock(this._dictionaryLock))
			{
				return this._dict.Remove(item);
			}
		}


		public virtual void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			using(new ReadOnlyLock(this._dictionaryLock))
			{
				this._dict.CopyTo(array, arrayIndex);
			}
		}


		public virtual bool IsReadOnly
		{
			get
			{
				using(new ReadOnlyLock(this._dictionaryLock))
				{
					return this._dict.IsReadOnly;
				}
			}
		}


		public virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			throw new NotSupportedException("Cannot enumerate a threadsafe dictionary.  Instead, enumerate the keys or values collection");
		}


		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotSupportedException("Cannot enumerate a threadsafe dictionary.  Instead, enumerate the keys or values collection");
		}
	}


	public static class Locks
	{
		public static void GetReadLock(ReaderWriterLockSlim locks)
		{
			var lockAcquired = false;
			while(!lockAcquired)
				lockAcquired = locks.TryEnterUpgradeableReadLock(1);
		}


		public static void GetReadOnlyLock(ReaderWriterLockSlim locks)
		{
			var lockAcquired = false;
			while(!lockAcquired)
				lockAcquired = locks.TryEnterReadLock(1);
		}


		public static void GetWriteLock(ReaderWriterLockSlim locks)
		{
			var lockAcquired = false;
			while(!lockAcquired)
				lockAcquired = locks.TryEnterWriteLock(1);
		}


		public static void ReleaseReadOnlyLock(ReaderWriterLockSlim locks)
		{
			if(locks.IsReadLockHeld)
				locks.ExitReadLock();
		}


		public static void ReleaseReadLock(ReaderWriterLockSlim locks)
		{
			if(locks.IsUpgradeableReadLockHeld)
				locks.ExitUpgradeableReadLock();
		}


		public static void ReleaseWriteLock(ReaderWriterLockSlim locks)
		{
			if(locks.IsWriteLockHeld)
				locks.ExitWriteLock();
		}


		public static void ReleaseLock(ReaderWriterLockSlim locks)
		{
			ReleaseWriteLock(locks);
			ReleaseReadLock(locks);
			ReleaseReadOnlyLock(locks);
		}


		public static ReaderWriterLockSlim GetLockInstance()
		{
			return GetLockInstance(LockRecursionPolicy.SupportsRecursion);
		}


		public static ReaderWriterLockSlim GetLockInstance(LockRecursionPolicy recursionPolicy)
		{
			return new ReaderWriterLockSlim(recursionPolicy);
		}
	}


	public abstract class BaseLock : IDisposable
	{
		protected ReaderWriterLockSlim Locks;


		public BaseLock(ReaderWriterLockSlim locks)
		{
			Locks = locks;
		}


		public abstract void Dispose();
	}


	public class ReadLock : BaseLock
	{
		public ReadLock(ReaderWriterLockSlim locks)
			: base(locks)
		{
			Collections.Locks.GetReadLock(this.Locks);
		}


		public override void Dispose()
		{
			Collections.Locks.ReleaseReadLock(this.Locks);
		}
	}


	public class ReadOnlyLock : BaseLock
	{
		public ReadOnlyLock(ReaderWriterLockSlim locks)
			: base(locks)
		{
			Collections.Locks.GetReadOnlyLock(this.Locks);
		}


		public override void Dispose()
		{
			Collections.Locks.ReleaseReadOnlyLock(this.Locks);
		}
	}


	public class WriteLock : BaseLock
	{
		public WriteLock(ReaderWriterLockSlim locks)
			: base(locks)
		{
			Collections.Locks.GetWriteLock(this.Locks);
		}


		public override void Dispose()
		{
			Collections.Locks.ReleaseWriteLock(this.Locks);
		}
	}

}
