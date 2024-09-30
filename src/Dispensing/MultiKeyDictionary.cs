using System.Collections.Generic;
using System.Threading;

namespace CareFusion.Dispensing
{
	/// <summary>
	/// Multi-Key Dictionary Class
	/// </summary>	
    /// <typeparam name="TKey">Primary Key Type</typeparam>
    /// <typeparam name="TSubKey">Sub Key Type</typeparam>
    /// <typeparam name="TValue">Value Type</typeparam>
	public class MultiKeyDictionary<TKey, TSubKey, TValue>
	{
		private readonly Dictionary<TKey, TValue> _baseDictionary = new Dictionary<TKey, TValue>();
        private readonly Dictionary<TSubKey, TKey> _subDictionary = new Dictionary<TSubKey, TKey>();
		private readonly Dictionary<TKey, TSubKey> _primaryToSubkeyMapping = new Dictionary<TKey, TSubKey>();
		private readonly ReaderWriterLockSlim _readerWriterLock = new ReaderWriterLockSlim();

		public TValue this[TSubKey subKey]
		{
			get
			{
				TValue item;
				if (TryGetValue(subKey, out item))
					return item;

				throw new KeyNotFoundException("sub key not found: " + subKey);
			}
		}

		public TValue this[TKey primaryKey]
		{
			get
			{
				TValue item;
				if (TryGetValue(primaryKey, out item))
					return item;

				throw new KeyNotFoundException("primary key not found: " + primaryKey);
			}
		}

        public int Count
        {
            get
            {
                _readerWriterLock.EnterReadLock();

                try
                {
                    return _baseDictionary.Count;
                }
                finally
                {
                    _readerWriterLock.ExitReadLock();
                }
            }
        }

        public Dictionary<TKey, TValue>.ValueCollection Values
        {
            get
            {
                _readerWriterLock.EnterReadLock();

                try
                {
                    return _baseDictionary.Values;
                }
                finally
                {
                    _readerWriterLock.ExitReadLock();
                }
            }
        }

        public Dictionary<TKey, TValue>.KeyCollection Keys
        {
            get
            {
                _readerWriterLock.EnterReadLock();

                try
                {
                    return _baseDictionary.Keys;
                }
                finally
                {
                    _readerWriterLock.ExitReadLock();
                }
            }
        }

        public bool ContainsKey(TKey primaryKey)
        {
            TValue value;

            return TryGetValue(primaryKey, out value);
        }

		public bool ContainsKey(TSubKey subKey)
		{
			TValue value;

			return TryGetValue(subKey, out value);
		}

        public bool TryGetValue(TKey primaryKey, out TValue value)
        {
            _readerWriterLock.EnterReadLock();

            try
            {
                return _baseDictionary.TryGetValue(primaryKey, out value);
            }
            finally
            {
                _readerWriterLock.ExitReadLock();
            }
        }

        public bool TryGetValue(TSubKey subKey, out TValue value)
        {
            value = default(TValue);

            _readerWriterLock.EnterReadLock();

            try
            {
                TKey primaryKey;
                if (_subDictionary.TryGetValue(subKey, out primaryKey))
                {
                    return _baseDictionary.TryGetValue(primaryKey, out value);
                }
            }
            finally
            {
                _readerWriterLock.ExitReadLock();
            }

            return false;
        }

        public void Add(TKey primaryKey, TValue value)
        {
            _readerWriterLock.EnterWriteLock();

            try
            {
                _baseDictionary.Add(primaryKey, value);
            }
            finally
            {
                _readerWriterLock.ExitWriteLock();
            }
        }

        public void Add(TKey primaryKey, TSubKey subKey, TValue value)
        {
            Add(primaryKey, value);

            Associate(subKey, primaryKey);
        }

        public void Associate(TSubKey subKey, TKey primaryKey)
        {
            _readerWriterLock.EnterUpgradeableReadLock();

            try
            {
                if (!_baseDictionary.ContainsKey(primaryKey))
                    throw new KeyNotFoundException(string.Format("The base dictionary does not contain the key '{0}'", primaryKey));

                if (_primaryToSubkeyMapping.ContainsKey(primaryKey)) // Remove the old mapping first
                {
                    _readerWriterLock.EnterWriteLock();

                    try
                    {
                        if (_subDictionary.ContainsKey(_primaryToSubkeyMapping[primaryKey]))
                        {
                            _subDictionary.Remove(_primaryToSubkeyMapping[primaryKey]);
                        }

                        _primaryToSubkeyMapping.Remove(primaryKey);
                    }
                    finally
                    {
                        _readerWriterLock.ExitWriteLock();
                    }
                }

                _subDictionary[subKey] = primaryKey;
                _primaryToSubkeyMapping[primaryKey] = subKey;
            }
            finally
            {
                _readerWriterLock.ExitUpgradeableReadLock();
            }
        }

		public void Remove(TKey primaryKey)
		{
			_readerWriterLock.EnterWriteLock();

			try
			{
				if (_primaryToSubkeyMapping.ContainsKey(primaryKey))
				{
					if (_subDictionary.ContainsKey(_primaryToSubkeyMapping[primaryKey]))
					{
						_subDictionary.Remove(_primaryToSubkeyMapping[primaryKey]);
					}

					_primaryToSubkeyMapping.Remove(primaryKey);
				}

				_baseDictionary.Remove(primaryKey);
			}
			finally
			{
				_readerWriterLock.ExitWriteLock();
			}
		}

		public void Remove(TSubKey subKey)
		{
			_readerWriterLock.EnterWriteLock();

			try
			{
				_baseDictionary.Remove(_subDictionary[subKey]);

				_primaryToSubkeyMapping.Remove(_subDictionary[subKey]);

				_subDictionary.Remove(subKey);
			}
			finally
			{
				_readerWriterLock.ExitWriteLock();
			}
		}

		public void Clear()
		{
			_readerWriterLock.EnterWriteLock();

			try
			{
				_baseDictionary.Clear();

				_subDictionary.Clear();

				_primaryToSubkeyMapping.Clear();
			}
			finally
			{
				_readerWriterLock.ExitWriteLock();
			}
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			_readerWriterLock.EnterReadLock();

			try
			{
				return _baseDictionary.GetEnumerator();
			}
			finally
			{
				_readerWriterLock.ExitReadLock();
			}
		}
	}
}
