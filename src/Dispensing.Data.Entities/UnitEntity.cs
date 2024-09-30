using System;
using System.Linq;
using CareFusion.Dispensing.Contracts;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class UnitEntity : IContractConvertible<Unit>
    {
        #region IContractConvertible<PatientCareArea> Members

        public Unit ToContract()
        {
            Facility facility = FacilityEntity != null
                ? new Facility(FacilityEntity.Key) { Name = FacilityEntity.FacilityName }
                : FacilityKey;

            Guid[] areas = AreaUnitEntities
                .Select(au => au.AreaKey)
                .ToArray();

            Guid[] rooms = UnitRoomEntities
                .Select(ur => ur.Key)
                .ToArray();

            return new Unit(Key)
            {
                FacilityKey = FacilityKey,
                FacilityName = FacilityEntity?.FacilityName,
                Name = UnitName,
                Description = DescriptionText,
                ShowPreadmission = ShowPreadmissionFlag,
                ShowRecurringAdmission = ShowRecurringAdmissionFlag,
                PreadmissionLeadDuration = PreadmissionLeadDurationAmount,
                PreadmissionProlongedInactivityDuration = PreadmissionProlongedInactivityDurationAmount,
                AdmissionProlongedInactivityDuration = AdmissionProlongedInactivityDurationAmount,
                DischargeDelayDuration = DischargeDelayDurationAmount,
                TransferDelayDuration = TransferDelayDurationAmount,
                OmnlNoticePrinterName = OMNLNoticePrinterName,
                AutoDischargeMode = AutoDischargeModeInternalCode.FromNullableInternalCode<AutoDischargeModeInternalCode>(),  
                AutoDischargeDuration = AutoDischargeDurationAmount,
                AlternateAutoDischargeDuration = AlternateAutoDischargeDurationAmount,
                LongTermCare = LongTermCareFlag,
                Areas = areas,
                Rooms = rooms,
                LastModified = LastModifiedBinaryValue.ToArray()
            };
        }

        #endregion
    }
}
