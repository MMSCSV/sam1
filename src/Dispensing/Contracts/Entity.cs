using System;
using System.Runtime.Serialization;
using ProtoBuf;

namespace CareFusion.Dispensing.Contracts
{
    public interface IEntity
    {
        object Key { get; }
    }
   
    public interface IEntity<TKey>
    {
        TKey Key { get; set; }

        byte[] LastModified { get; set; }

        bool IsTransient();
    }

    /// <summary>
    /// Represents the base class for an entity.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    [Serializable]
    [DataContract(Namespace = ContractConstants.ContractsNamespaceV1)]
    [ProtoContract(UseProtoMembersOnly = true)]
    public abstract class Entity<TKey> : IEntity<TKey>, IEntity
    {
        public static Entity<TKey> CreateGenericEntity(TKey key, byte[] lastModified = null)
        {
            return new GenericEntity<TKey>(key, lastModified);
        }

        public static implicit operator Entity<TKey>(TKey key)
        {
            return new GenericEntity<TKey>(key);
        }

        #region Public Properties

        /// <summary>
        /// Gets or sets the unique key of the entity.
        /// </summary>
        [DataMember]
        [ProtoMember(1)]
        public TKey Key { get; set; }

        /// <summary>
        /// Gets or sets the last modified value of the entity.
        /// </summary>
        [DataMember]
        [ProtoMember(2)]
        public byte[] LastModified { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether the entity is partially loaded.
        /// </summary>
        [DataMember]
        [ProtoMember(3)]
        public bool PartiallyLoaded { get; set; }

        #endregion

        #region Public Methods

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

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            
            var otherEntity = obj as Entity<TKey>;
            if (otherEntity == null) return false;
            
            return Equals(otherEntity);
        }

        public bool Equals(Entity<TKey> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return Equals(other.Key, Key);
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }
        #endregion

        #region IEntity Members

        object IEntity.Key
        {
            get { return Key; }
        }

        #endregion

        #region ICloneable Members

        /// <summary>
        /// Shallow copy of the current instance.
        /// </summary>
        /// <returns></returns>
        protected object ShallowClone(bool makeTransient)
        {
            Entity<TKey> clone = (Entity<TKey>)MemberwiseClone();

            if (makeTransient)
            {
                // Make it a transient
                clone.Key = default(TKey);
                clone.LastModified = null;
            }

            return clone;
        }

        #endregion
    }

    [Serializable]
    [DataContract(Namespace = ContractConstants.ContractsNamespaceV1)]
    internal class GenericEntity<TKey> : Entity<TKey>
    {
        public GenericEntity(TKey key)
            : this(key, null)
        {
            
        }

        public GenericEntity(TKey key, byte[] lastModified)
        {
            Key = key;
            LastModified = lastModified;
        }
    }
}
