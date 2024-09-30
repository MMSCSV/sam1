using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CareFusion.Dispensing.Contracts
{
    [Serializable]
    public abstract class EntitySet<TKey, TItem> : Collection<TItem>
    {
        #region Contructors

        protected EntitySet()
        {
        }

        protected EntitySet(IEnumerable<TItem> items)
            : base(new List<TItem>(items))
        {
        }

        protected EntitySet(TKey key)
        {
            Key = key;
        }

        protected EntitySet(TKey key, IEnumerable<TItem> items)
            : base(new List<TItem>(items))
        {
            Key = key;
        }

        #endregion

        #region Public Properties

        public TKey Key { get; set; }

        public byte[] LastModified { get; set; }

        #endregion

        #region Public Members

        /// <summary>
        /// Transient objects are not associated with an item already in storage.
        /// </summary>
        /// <returns></returns>
        public bool IsTransient()
        {
            Type keyType = typeof(TKey);

            if (keyType.IsValueType)
            {
                if (keyType.IsPrimitive)
                {
                    // For primitive types like int, long, etc. we want to check
                    // if the key equal or less than zero.
                    IComparable key = Key as IComparable;
                    if (key != null)
                    {
                        try
                        {
                            return key.CompareTo(default(TKey)) <= 0;
                        }
                        catch (ArgumentException)
                        { }
                    }
                }

                return Key.Equals(default(TKey));
            }

            return Equals(Key, default(TKey));
        }

        #endregion
    }
}
