using System;
using System.Runtime.Serialization;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a shape of a storage space for a given storage space type that does
    /// not determine storage space behavior (other than how much inventory is contained
    /// within the space).
    /// </summary>
    [Serializable]
    [DataContract(Namespace = ContractConstants.ContractsNamespaceV1)]
    public class StorageSpaceForm : Entity<Guid>
    {
        #region Constructors

        public StorageSpaceForm()
        {
        }

        public StorageSpaceForm(Guid key)
        {
            Key = key;
        }

        public StorageSpaceForm(Guid key, StorageSpaceFormInternalCode formCode, StorageSpaceTypeInternalCode typeCode)
        {
            Key = key;
            InternalCode = formCode;
            StorageSpaceType = typeCode;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator StorageSpaceForm(Guid key)
        {
            return FromKey(key);
        }

        public static StorageSpaceForm FromKey(Guid key)
        {
            return new StorageSpaceForm(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the internal code that identifies a storage space form.
        /// </summary>
        [DataMember]
        public StorageSpaceFormInternalCode? InternalCode { get; set; }

        /// <summary>
        /// Gets or sets the internal code that identifies a storage space type.
        /// </summary>
        [DataMember]
        public StorageSpaceTypeInternalCode StorageSpaceType { get; set; }

        /// <summary>
        /// Gets or sets the text that describes a storage space form.
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the short name of a form that is not necessarily unique.
        /// </summary>
        [DataMember]
        public string ShortName { get; set; }

        /// <summary>
        /// Gets or sets the value that is used to control the sort order.
        /// </summary>
        [DataMember]
        public int SortOrder { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a storage form is active.
        /// </summary>
        [DataMember]
        public bool ActiveFlag { get; set; }
       
        #endregion
    }
}
