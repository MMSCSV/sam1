using System;
using CareFusion.Dispensing.Models;
using CareFusion.Dispensing.Resources;
using CareFusion.Dispensing.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents an item that is either a drug, a fluid like a TPN, or a blood product
    /// such as albumin (but not a blood cell or platelet product).
    /// </summary>
    [Serializable]
    [HasSelfValidation]
    public class MedItem : Item
    {
        private readonly string _displayName;
        private readonly Guid _snapshotKey;

        private static readonly Func<decimal?, bool> HasValue = (value) => value.HasValue && value.Value > 0;

        #region Constructors

        public MedItem()
        {

        }

        public MedItem(Guid key)
        {
            Key = key;
        }

        public MedItem(Guid key, Guid itemSnapshotKey, Guid snapshotKey)
            : base(key, itemSnapshotKey)
        {
            _snapshotKey = snapshotKey;
        }

        public MedItem(Guid key, Guid itemSnapshotKey, Guid snapshotKey, string displayName)
            : base(key, itemSnapshotKey)
        {
            _snapshotKey = snapshotKey;
            _displayName = displayName;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator MedItem(Guid key)
        {
            return FromKey(key);
        }

        public static new MedItem FromKey(Guid key)
        {
            return new MedItem(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of a medication item snapshot.
        /// </summary>
        public Guid MedItemSnapshotKey
        {
            get { return _snapshotKey; }
        }

        /// <summary>
        /// Gets the display name of a medication item.
        /// </summary>
        public string DisplayName
        {
            get { return _displayName; }
        }

        /// <summary>
        /// Gets or sets the nonproprietary name.
        /// </summary>
        [DispensingStringLengthValidator(ValidationConstants.MedItemGenericNameUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "MedItemGenericNameOutOfBounds")]
        [RequiredStringValidator(MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "MedItemGenericNameRequired")]
        public string GenericName { get; set; }

        /// <summary>
        /// Gets or sets the name used in generic name searches.
        /// </summary>
        public string SearchGenericName { get; set; }

        /// <summary>
        /// Gets or sets the non-proprietary name that contains just the generic name.
        /// </summary>
        public string PureGenericName { get; set; }

        /// <summary>
        /// Gets or sets the name if a medication brand.
        /// </summary>
        [DispensingStringLengthValidator(ValidationConstants.MedItemBrandNameUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "MedItemBrandNameOutOfBounds")]
        public string BrandName { get; set; }

        /// <summary>
        /// Gets or sets the name used in brand name searches.
        /// </summary>
        public string SearchBrandName { get; set; }

        /// <summary>
        /// Gets or sets the text that contains the strength for a medication.
        /// </summary>
        public string Strength { get; set; }

        /// <summary>
        /// Gets or sets the numeric strength for either a single-ingredient
        /// medication or multi-ingredient medication's primary ingredient.
        /// </summary>
        public decimal? StrengthAmount { get; set; }

        /// <summary>
        /// Gets or sets the unit of measure that is used for a strength for either
        /// a single-ingredient medication of a multi-ingredient medication's primary ingredient.
        /// </summary>
        public UnitOfMeasure StrengthUnitOfMeasure { get; set; }

        /// <summary>
        /// Gets or sets an external unit of measure that is used for a strength for either
        /// a single-ingredient medication of a multi-ingredient medication's primary ingredient.
        /// </summary>
        public Guid? StrengthExternalUnitOfMeasureKey { get; set; }

        /// <summary>
        /// Gets or sets the numeric concentration volume.
        /// </summary>
        public decimal? ConcentrationVolumeAmount { get; set; }

        /// <summary>
        /// Gets or sets the unit of measure that is used for a medication's concentration.
        /// volume.
        /// </summary>
        public UnitOfMeasure ConcentrationVolumeUnitOfMeasure { get; set; }

        /// <summary>
        /// Gets or sets an external unit of measure that is used for a medication's concentration.
        /// volume.
        /// </summary>
        public Guid? ConcentrationVolumeExternalUnitOfMeasureKey { get; set; }

        /// <summary>
        /// Gets or sets the numeric total volume.
        /// </summary>
        public decimal? TotalVolumeAmount { get; set; }

        /// <summary>
        /// Gets or sets the unit of measure that is used for a medication's total volume.
        /// </summary>
        public UnitOfMeasure TotalVolumeUnitOfMeasure { get; set; }

        /// <summary>
        /// Gets or sets an external unit of measure that is used for a medication's total volume.
        /// </summary>
        public Guid? TotalVolumeExternalUnitOfMeasureKey { get; set; }

        /// <summary>
        /// Gets or sets the dosage form.
        /// </summary>
        public DosageForm DosageForm { get; set; }

        /// <summary>
        /// Gets or sets the medication class.
        /// </summary>
        public MedicationClass MedicationClass { get; set; }

        public MedItemTypeInternalCode? MedItemType { get; set; }

        public decimal? MinimumDoseAmount { get; set; }

        public decimal? MaximumDoseAmount { get; set; }

        public Guid? DoseUnitOfMeasure { get; set; }

        /// <summary>
        /// Gets or sets the therapeutic classes associated with an item.
        /// </summary>
        public TherapeuticClass[] TherapeuticClasses { get; set; }

        /// <summary>
        /// Gets or sets the varialbe dose group members for this med item.
        /// </summary>
        public VariableDoseGroupMember[] VariableDoseGroupMembers { get; set; }

        #endregion

        #region Conversion Methods

        public decimal? GetStrengthAmount()
        {
            return GetStrengthAmount(
                StrengthAmount,
                ConcentrationVolumeAmount,
                ConcentrationVolumeUnitOfMeasure,
                TotalVolumeAmount,
                TotalVolumeUnitOfMeasure);
        }


        public static decimal? GetStrengthAmount(decimal? strengthAmount, decimal? concentrationVolumeAmount, UnitOfMeasure concentrationVolumeUnitOfMeasure,
            decimal? totalVolumeAmount, UnitOfMeasure totalVolumeUnitOfMeasure)
        {
            // if all amount values set, and can convert volume uoms (if necessary)
            if (HasValue(strengthAmount) &&
                HasValue(concentrationVolumeAmount) &&
                HasValue(totalVolumeAmount) &&
                UnitOfMeasure.CanConvert(concentrationVolumeUnitOfMeasure, totalVolumeUnitOfMeasure))
            {
                // convert concentration volume to total volume
                decimal convertedConcentrationVolume = UnitOfMeasure.Convert(
                    concentrationVolumeAmount.Value,
                    concentrationVolumeUnitOfMeasure,
                    totalVolumeUnitOfMeasure);

                // convert via strength * total volume / concentration volume
                return (strengthAmount.Value * totalVolumeAmount.Value / convertedConcentrationVolume);
            }

            // otherwise, return med strength
            return strengthAmount;   
        }

        public decimal? GetVolumeAmount()
        {
            return GetVolumeAmount(
                ConcentrationVolumeAmount,
                ConcentrationVolumeUnitOfMeasure,
                TotalVolumeAmount,
                TotalVolumeUnitOfMeasure);
        }

        public static decimal? GetVolumeAmount(decimal? concentrationVolumeAmount, UnitOfMeasure concentrationVolumeUnitOfMeasure,
            decimal? totalVolumeAmount, UnitOfMeasure totalVolumeUnitOfMeasure)
        {
            // if concentration and total are set, and can convert uoms (if necessary)
            if (HasValue(concentrationVolumeAmount) &&
                HasValue(totalVolumeAmount) &&
                UnitOfMeasure.CanConvert(concentrationVolumeUnitOfMeasure, totalVolumeUnitOfMeasure))
            {
                // convert total volume to concentration volume units
                return UnitOfMeasure.Convert(
                    totalVolumeAmount.Value,
                    totalVolumeUnitOfMeasure,
                    concentrationVolumeUnitOfMeasure);
            }
            // concentration volume, but no total volume
            else if (
                HasValue(concentrationVolumeAmount) &&
                concentrationVolumeUnitOfMeasure != null &&
                !HasValue(totalVolumeAmount))
            {
                return concentrationVolumeAmount;
            }
            // total volume, but no concentration volume
            else if (
                HasValue(totalVolumeAmount) &&
                totalVolumeUnitOfMeasure != null &&
                !HasValue(concentrationVolumeAmount))
            {
                return totalVolumeAmount;
            }
            else
            {
                // neither set
                return default(decimal?);
            }
        }

        #endregion

        #region SelfValidation Logic

        [SelfValidation]
        public void CheckAdditionalRequiredFields(ValidationResults results)
        {
            if (IsLocal())
            {
                if (DosageForm == null)
                {
                    results.AddResult(new ValidationResult(ValidationStrings.MedItemDosageFormRequired, this,
                                                           "DosageForm", "", null));
                }

                if (StrengthAmount != null &&
                    StrengthAmount <= 0)
                {
                    results.AddResult(new ValidationResult(ValidationStrings.MedItemStrengthAmountOutOfBounds,
                        this, "StrengthAmount", "", null));
                }

                if (ConcentrationVolumeAmount != null &&
                    ConcentrationVolumeAmount <= 0)
                {
                    results.AddResult(new ValidationResult(ValidationStrings.MedItemConcentrationVolumeAmountOutOfBounds,
                        this, "ConcentrationVolumeAmount", "", null));
                }

                if (TotalVolumeAmount != null &&
                    TotalVolumeAmount <= 0)
                {
                    results.AddResult(new ValidationResult(ValidationStrings.MedItemTotalVolumeAmountOutOfBounds,
                        this, "TotalVolumeAmount", "", null));
                }
            }
        }

        #endregion
    }
}
