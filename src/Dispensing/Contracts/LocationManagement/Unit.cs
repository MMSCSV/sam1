using System;
using CareFusion.Dispensing.Resources;
using CareFusion.Dispensing.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents the most general place normally witin a floor that
    /// a patient is located at.
    /// </summary>
    [Serializable]
    [HasSelfValidation]
    public class Unit : Entity<Guid>
    {
        #region Constructors

        public Unit()
        {
            // Defaults
            PreadmissionLeadDuration = 72;
            AdmissionProlongedInactivityDuration = 365;
            DischargeDelayDuration = 2;
            PreadmissionProlongedInactivityDuration = 72;
            TransferDelayDuration = 2;
        }

        public Unit(Guid key)
            : this()
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator Unit(Guid key)
        {
            return FromKey(key);
        }

        public static Unit FromKey(Guid key)
        {
            return new Unit(key);
        }

        #endregion

        #region Public Properties
        public Guid FacilityKey { get; set; }

        public string FacilityName { get; set; }

        /// <summary>
        /// Gets or sets the name of the facility.
        /// </summary>
        /// <value>The name.</value>
        [DispensingStringLengthValidator(ValidationConstants.AreaNameUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_NameOutOfBounds")]
        [RequiredStringValidator(MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_NameRequired")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the text that describes the facility.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the nonlocalizable code that identifies an auto-discharge mode.
        /// </summary>
        public AutoDischargeModeInternalCode? AutoDischargeMode { get; set; }

        /// <summary>
        /// Gets or sets the duration (in hours) after which encounters assigned to a unit
        /// are deemed to be auto-discharged.
        /// </summary>
        public short? AutoDischargeDuration { get; set; }

        /// <summary>
        /// Gets or sets the duration (in hours) after which encounters that have the alternative 
        /// auto discharge flag ON and are assignedto a unit are deemed to be auto-discharged.
        /// </summary>
        public short? AlternateAutoDischargeDuration { get; set; }
        
        /// <summary>
        /// Gets or sets a value that indicates whether preadmissions are shown.
        /// </summary>
        public bool ShowPreadmission { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether recurring admissions are shown for a unit.
        /// </summary>
        public bool ShowRecurringAdmission { get; set; }

        /// <summary>
        /// Gets or sets the duration (in hours) that is subtracted from the preadmit
        /// date time and that is used to determine whether an encounter is shown at
        /// dispensing devices.
        /// </summary>
        [DispensingRangeValidator(ValidationConstants.UnitPreadmissionLeadDurationMinValue,
            ValidationConstants.UnitPreadmissionLeadDurationMaxValue,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "UnitPreadmissionLeadDurationOutOfBounds")]
        public short? PreadmissionLeadDuration { get; set; }

        /// <summary>
        /// Gets or sets the duration (in hours) that is added to an expected admit date
        /// time and that is used to determine whether an encounter is displayed.
        /// </summary>
        [DispensingRangeValidator(ValidationConstants.UnitPreadmissionProlongedInactivityDurationMinValue,
            ValidationConstants.UnitPreadmissionProlongedInactivityDurationMaxValue,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "UnitPreadmissionProlongedInactivityDurationOutOfBounds")]
        public short? PreadmissionProlongedInactivityDuration { get; set; }

        /// <summary>
        /// Gets or sets the duration (in days) of inactivity after which an encounter
        /// is considered to be in a state of prolonged inactivity.
        /// </summary>
        [DispensingRangeValidator(ValidationConstants.UnitAdmissionProlongedInactivityDurationMinValue,
            ValidationConstants.UnitAdmissionProlongedInactivityDurationMaxValue,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "UnitAdmissionProlongedInactivityDurationOutOfBounds")]
        public short? AdmissionProlongedInactivityDuration { get; set; }

        /// <summary>
        /// Gets or sets the duration (in hours) that is added to a consider-as-discharged
        /// date time and that is used to determine whether dispensing activities may be
        /// performed against an encounter.
        /// </summary>
        [DispensingRangeValidator(ValidationConstants.UnitDischargeDelayDurationMinValue,
            ValidationConstants.UnitDischargeDelayDurationMaxValue,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "UnitDischargeDelayDurationOutOfBounds")]
        public short? DischargeDelayDuration { get; set; }

        /// <summary>
        /// Gets or sets the duration (in hours) that is used to determine whether
        /// an encounter is associated with a unit.
        /// </summary>
        [DispensingRangeValidator(ValidationConstants.UnitTransferDelayDurationMinValue,
            ValidationConstants.UnitTransferDelayDurationMaxValue,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "UnitTransferDelayDurationOutOfBounds")]
        public short? TransferDelayDuration { get; set; }

        /// <summary>
        /// Gets or sets the name of a printer for OMNL notices.
        /// </summary>
        public string OmnlNoticePrinterName { get; set; }

        public bool LongTermCare { get; set; }

        /// <summary>
        /// Gets or sets the areas associated with a unit.
        /// </summary>
        public Guid[] Areas { get; set; }

        /// <summary>
        /// Gets or sets the rooms associated with a unit.
        /// </summary>
        public Guid[] Rooms { get; set; }

        #endregion

        #region SelfValidation Logic

        [SelfValidation]
        public void CheckFacilityAssociationRule(ValidationResults results)
        {
            if (Areas == null || Areas.Length == 0)
                results.AddResult(new ValidationResult(ValidationStrings.UnitAreasRequired, this, "Areas", "", null));
        }

        #endregion
    }
}
