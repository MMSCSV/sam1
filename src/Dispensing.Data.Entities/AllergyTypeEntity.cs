using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class AllergyTypeEntity : IContractConvertible<AllergyType>
    {
        #region Implementation of IContractConvertible<AllergyType>

        public AllergyType ToContract()
        {
            return new AllergyType(Key)
                {
                    Code = AllergyTypeCode,
                    Description = DescriptionText,
                    ExternalSystemKey = ExternalSystemKey,
                    SortOrder = SortValue,
                    LastModified = LastModifiedBinaryValue.ToArray()
                };
        }

        #endregion
    }
}
