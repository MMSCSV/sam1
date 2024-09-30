using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareFusion.Dispensing.Contracts
{
    [Serializable]
    public class UserDirectoryGroup : IEntity<Guid>
    {
        #region Constructors

        public UserDirectoryGroup()
        {

        }

        public UserDirectoryGroup(Guid key)
        {
            Key = key;
        }
        
        #endregion

        #region Operator Overloads

        public static implicit operator UserDirectoryGroup(Guid key)
        {
            return FromKey(key);
        }

        public static UserDirectoryGroup FromKey(Guid key)
        {
            return new UserDirectoryGroup(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of a active directory domain.
        /// </summary>
        [Column("UserDirectoryGroupKey")]
        public Guid Key { get; set; }

        public Guid ActiveDirectoryDomainKey { get; set; }

        /// <summary>
        /// Gets or sets the name of the group.
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a group is only for support users.
        /// </summary>
        [Column("SupportUserFlag")]
        public bool SupportUser { get; set; }

        /// <summary>
        /// Gets or sets the binary value that is unique within a database and that is assigned a new
        /// value on insert and on update.
        /// </summary>
        [Column("LastModifiedBinaryValue")]
        public byte[] LastModified { get; set; }

        #endregion

        #region Public Members

        public bool IsTransient()
        {
            return Key == default(Guid);
        }

        #endregion
    }
}
