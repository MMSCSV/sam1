using System;
using CareFusion.Dispensing.Resources;
using CareFusion.Dispensing.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a kind of item that is dispensed, administered, stored
    /// or tracked in inventory.
    /// </summary>
    /// <remarks>
    /// Each instance represents a kind of item, not a single, actual
    /// physical item.
    /// </remarks>
    [Serializable]
    [HasSelfValidation]
    public class Item : Entity<Guid>
    {
        #region Constructors

        public Item()
        {
            // NOTE: Default is MED
            BusinessDomain = BusinessDomainInternalCode.MED;
        }

        public Item(Guid key)
        {
            Key = key;
        }

        public Item(Guid key, Guid snapshotKey)
        {
            Key = key;
            SnapshotKey = snapshotKey;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator Item(Guid key)
        {
            return FromKey(key);
        }

        public static Item FromKey(Guid key)
        {
            return new Item(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of a medication item snapshot.
        /// </summary>
        public Guid SnapshotKey { get; private set; }

        /// <summary>
        /// Gets or sets the business domain.
        /// </summary>
        public BusinessDomainInternalCode BusinessDomain { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of an external system.
        /// </summary>
        public Guid? PharmacyInformationSystemKey { get; set; }

        /// <summary>
        /// Gets the value that indicates the name of an external system.
        /// </summary>
        public string PharmacyInformationSystemName { get; internal set; }

        /// <summary>
        /// Gets the value that indicates whether a PIS provides medication classes.
        /// </summary>
        public bool PharmacyInformationSystemProvidesMedicationClass { get; internal set; }

        /// <summary>
        /// Gets the value that indicates whether a PIS provides therapeutic classes.
        /// </summary>
        public bool PharmacyInformationSystemProvidesTherapeuticClass { get; internal set; }

        /// <summary>
        /// Gets the value that indicates whether a PIS provides pure generic names.
        /// </summary>
        public bool PharmacyInformationSystemProvidesPureGenericName { get; internal set; }

        /// <summary>
        /// Gets or sets the surrogate key of a facility.
        /// </summary>
        public Guid? FacilityKey { get; set; }

        /// <summary>
        /// Gets the value that indicates the name of a facility.
        /// </summary>
        public string FacilityCode { get; internal set; }

        /// <summary>
        /// Gets the value that indicates the name of a facility.
        /// </summary>
        public string FacilityName { get; internal set; }

        /// <summary>
        /// Gets or sets the Id of an item.
        /// </summary>
        [DispensingStringLengthValidator(ValidationConstants.ItemIdUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "ItemIdOutOfBounds")]
        [RequiredStringValidator(MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "ItemIdRequired")]
        public string ItemId { get; set; }

        /// <summary>
        /// Gets or sets the alternate ID of an item at the external-system (such as PIS) level.
        /// </summary>
        public string AlternateItemId { get; set; }

        /// <summary>
        /// Gets or sets the name of an item at the external-system (such as PIS) level.
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// Gets or sets the description text of an item at the external-system (such as PIS) level.
        /// </summary>
        public string DescriptionText { get; set; }

        /// <summary>
        /// Gets or sets the item type internal code of an item at the external-system (such as PIS) level.
        /// </summary>
        public string ItemTypeInternalCode { get; set; }

        /// <summary>
        /// Gets or sets the item sub type internal code an item at the external-system (such as PIS) level.
        /// </summary>
        public string ItemSubTypeInternalCode { get; set; }


        /// <summary>
        /// Gets or sets a value that indicates whether an item is a medication item.
        /// </summary>
        public bool IsMedItem { get; set; }

        /// <summary>
        /// Gets or sets the UTC date and time an item is deleted in an external system.
        /// </summary>
        public DateTime? ExternalSystemDeleteUtcDateTime { get; set; }

        /// <summary>
        /// Gets or sets the local date and time an item is deleted in an external system.
        /// </summary>
        public DateTime? ExternalSystemDeleteDateTime { get; set; }

        /// <summary>
        /// Gets or sets the first custom field.
        /// </summary>
        public string CustomField1 { get; set; }

        /// <summary>
        /// Gets or sets the second custom field.
        /// </summary>
        public string CustomField2 { get; set; }
        
        /// <summary>
        /// Gets or sets the third custom field.
        /// </summary>
        public string CustomField3 { get; set; }

        /// <summary>
        /// Gets or sets the enterprise item id field.
        /// </summary>
        public string EnterpriseItemId { get; set; }

        /// <summary>
        /// Gets or sets the item equivalency sets.
        /// </summary>
        public ItemEquivalencySet[] ItemEquivalencySets { get; set; }

        /// <summary>
        /// Gets or sets the deleted flag for an item.
        /// </summary>
        public bool IsDeleted { get; set; }

        #endregion

        #region Public Members

        public bool IsLocal()
        {
            return (FacilityKey != null);
        }

        #endregion

        #region SelfValidation Logic

        [SelfValidation]
        public void CheckExternalSystemKeyAndFacilityKey(ValidationResults results)
        {
            if (PharmacyInformationSystemKey == null && FacilityKey == null)
                results.AddResult(new ValidationResult(ValidationStrings.ItemExternalKeyFacilityKeyRule, this, "", "", null));
        }

        #endregion
    }
}
