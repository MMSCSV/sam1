using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class EncounterPatientClassEntity : IContractConvertible<PatientClass>
    {
        #region IContractConvertible<PatientClass> Members

        public PatientClass ToContract()
        {
            return new PatientClass(Key)
            {
                ExternalSystemKey = ExternalSystemKey,
                Code = EncounterPatientClassCode,
                Description = DescriptionText,
                SortOrder = SortValue,
                LastModified = LastModifiedBinaryValue.ToArray()
            };
        }

        #endregion
    }
}
