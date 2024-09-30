using System;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a record of a product ID for a given period of time.
    /// </summary>
    public class ItemProductId : Entity<Guid>
    {
        #region Constructors

        public ItemProductId()
        {

        }

        public ItemProductId(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator ItemProductId(Guid key)
        {
            return FromKey(key);
        }

        public static ItemProductId FromKey(Guid key)
        {
            return new ItemProductId(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the item.
        /// </summary>
        public Guid ItemKey { get; set; }

        /// <summary>
        /// Gets or sets the product ID that can be scanned for an item.
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates the user account that linked a product ID.
        /// </summary>
        public Guid? LinkedByUserAccountKey { get; set; }

        /// <summary>
        /// Gets or sets the UTC date and time when the link occurred.
        /// </summary>
        public DateTime LinkedUtcDateTime { get; set; }

        /// <summary>
        /// Gets or sets the local date and time when the link occurred.
        /// </summary>
        public DateTime LinkedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates the user account that verified a product ID.
        /// </summary>
        public Guid? VerifiedByUserAccountKey { get; set; }

        /// <summary>
        /// Gets or sets the UTC date and time when the verify occurred.
        /// </summary>
        public DateTime? VerifiedUtcDateTime { get; set; }

        /// <summary>
        /// Gets or sets the local date and time when the verify occurred.
        /// </summary>
        public DateTime? VerifiedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates that the record was added from an external system.
        /// </summary>
        public bool FromExternalSystem { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates that the record was added from a data manager.
        /// </summary>
        public bool FromDataManager { get; set; }

        /// <summary>
        /// Gets or sets the non-localizable code that identifies a scan product delete reason.
        /// </summary>
        public ScanProductDeleteReasonInternalCode? ScanProductDeleteReason { get; set; }

        /// <summary>
        /// Gets or sets the other item ID that is the reason why an item scan code is logically deleted.
        /// </summary>
        public string OtherItemId { get; set; }

        /// <summary>
        /// Gets or sets the name of an external system that caused a scan code to be created.
        /// </summary>
        public string CreatedByExternalSystemName { get; set; }

        /// <summary>
        /// Gets or sets the name of an external system that caused a scan code to be deleted.
        /// </summary>
        public string DeletedByExternalSystemName { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether an item scan code is logially deleted.
        /// </summary>
        public bool Deleted { get; set; }

        #endregion

        #region Public Members

        public bool IsVerified()
        {
            return (VerifiedByUserAccountKey != null ||
                    VerifiedDateTime != null ||
                    VerifiedUtcDateTime != null); 
        }

        #endregion
    }
}
