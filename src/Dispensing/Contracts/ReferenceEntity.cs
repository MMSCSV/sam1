using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents the base class for a reference entity.
    /// </summary>
    /// <typeparam name="TInternalCodeEnum">The type of internal code enum for the reference entity.</typeparam>
    [Serializable]
    [DataContract(Name = "{0}ReferenceEntity", Namespace = ContractConstants.ContractsNamespaceV1)]
    public abstract class ReferenceEntity<TInternalCodeEnum>
        where TInternalCodeEnum : struct
    {
        #region Constructors

        protected ReferenceEntity()
        {
        }

        protected ReferenceEntity(string internalCode)
        {
            Guard.ArgumentNotNullOrEmptyString(internalCode, "internalCode");

            InternalCode = internalCode.FromInternalCode<TInternalCodeEnum>();
        }

        protected ReferenceEntity(string internalCode, string description)
        {
            Guard.ArgumentNotNullOrEmptyString(internalCode, "internalCode");

            InternalCode = internalCode.FromInternalCode<TInternalCodeEnum>();
            Description = description;
        }

        protected ReferenceEntity(TInternalCodeEnum internalCode)
        {
            InternalCode = internalCode;
        }

        protected ReferenceEntity(TInternalCodeEnum internalCode, string description)
        {
            InternalCode = internalCode;
            Description = description;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the nonlocalizable code that identifies the reference entity.
        /// </summary>
        [DataMember]
        public TInternalCodeEnum InternalCode { get; set; }

        /// <summary>
        /// Gets or sets text that descirbes the reference entity
        /// </summary>
        [DataMember]
        [Column("DescriptionText")]
        public string Description { get; set; }

        #endregion

        #region Equality Members

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (GetType() != obj.GetType())
                return false;

            ReferenceEntity<TInternalCodeEnum> entity = (ReferenceEntity<TInternalCodeEnum>)obj;
            return InternalCode.Equals(entity.InternalCode);
        }

        public override int GetHashCode()
        {
            return InternalCode.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0} | {1}", InternalCode, Description);
        }

        #endregion
    }
}
