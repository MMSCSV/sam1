using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class EncounterAdmissionStatusEntity : IContractConvertible<EncounterAdmissionStatus>
    {
        #region IContractConvertible<EncounterAdmissionStatus> Members

        public EncounterAdmissionStatus ToContract()
        {
            return new EncounterAdmissionStatus(InternalCode, DescriptionText)
                {
                    SortValue = SortValue
                };
        }

        #endregion
    }
}
