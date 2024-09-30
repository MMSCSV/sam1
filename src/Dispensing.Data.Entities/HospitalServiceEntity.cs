using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class HospitalServiceEntity : IContractConvertible<HospitalService>
    {
        #region Implementation of IContractConvertible<HospitalService>

        public HospitalService ToContract()
        {
            return new HospitalService(Key)
                {
                    ExternalSystemKey = ExternalSystemKey,
                    Code = HospitalServiceCode,
                    Description = DescriptionText,
                    SortOrder = SortValue,
                    LastModified = LastModifiedBinaryValue.ToArray()
                };
        }

        #endregion
    }
}
