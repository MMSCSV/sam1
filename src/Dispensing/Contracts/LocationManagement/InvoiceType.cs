using System;

namespace CareFusion.Dispensing.Contracts.LocationManagement
{
    /// <summary>
    /// Represents an Invoice Type
    /// </summary>
    [Serializable]
    public class InvoiceType : Entity<Guid>
    {
        #region Constructors

        public InvoiceType() { }

        public InvoiceType(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator InvoiceType(Guid key)
        {
            return FromKey(key);
        }

        public static InvoiceType FromKey(Guid key)
        {
            return new InvoiceType(key);
        }

        #endregion

        #region Public Properties
        

        /// <summary>
        /// Gets or sets the surrogate key of a facility.
        /// </summary>
        public Guid FacilityKey { get; set; }

        /// <summary>
        /// Gets the name of the facility.
        /// </summary>
        public string FacilityName { get; internal set; }

        /// <summary>
        /// Gets or sets the name of an Invoice Type.
        /// </summary>
        public string InvoiceTypeName { get; set; }

        /// <summary>
        /// Gets ort sets the Description of an Invoice Type.
        /// </summary>
        public string DescriptionText { get; set; }

        /// <summary>
        /// Gets the value that indicates whether an Invoice Type is logically deleted
        /// </summary>
        public bool IsDeleted { get; set; }

        #endregion
    }
}
