using System;
using CareFusion.Dispensing.Resources;
using CareFusion.Dispensing.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a record of an item scan code for a given period of time.
    /// </summary>
    public class ItemScanCode : Entity<Guid>
    {
        #region Constructors

        public ItemScanCode()
        {

        }

        public ItemScanCode(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator ItemScanCode(Guid key)
        {
            return FromKey(key);
        }

        public static ItemScanCode FromKey(Guid key)
        {
            return new ItemScanCode(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the item.
        /// </summary>
        [NotNullValidator(MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "ItemScanCodeItemRequired")]
        public Item Item { get; set; }

        /// <summary>
        /// Gets or sets the value that can be scanned for an item.
        /// </summary>
        [DispensingStringLengthValidator(ValidationConstants.ItemScanCodeUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "ItemScanCodeScanCodeOutOfBounds")]
        [RequiredStringValidator(MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "ItemScanCodeScanCodeRequired")]
        public string ScanCode { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of a user account that linked a scan code to an item.
        /// </summary>
        public Guid? LinkedByUserAccountKey { get; set; }

        /// <summary>
        /// Gets the full name of user account that linked a scan code to an item.
        /// </summary>
        public string LinkedByUserAccountFullName { get; internal set; }

        /// <summary>
        /// Gets the user ID of user account that linked a scan code to an item.
        /// </summary>
        public string LinkedByUserAccountUserId { get; internal set; }

        /// <summary>
        /// Gets or sets the UTC date and time when the link occurred.
        /// </summary>
        public DateTime LinkedUtcDateTime { get; set; }

        /// <summary>
        /// Gets or sets the local date and time when the link occurred.
        /// </summary>
        public DateTime LinkedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of a user account that verified a scan code as 
        /// being for an item.
        /// </summary>
        public Guid? VerifiedByUserAccountKey { get; set; }

        /// <summary>
        /// Gets the full name of a user account that verified a scan code as being for an item.
        /// </summary>
        public string VerifiedByUserAccountFullName { get; internal set; }

        /// <summary>
        /// Gets the user ID of a user account that verified a scan code as being for an item.
        /// </summary>
        public string VerifiedByUserAccountUserId { get; internal set; }

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
        /// Gets or sets the value that indicates that the record was added from the data manager.
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
