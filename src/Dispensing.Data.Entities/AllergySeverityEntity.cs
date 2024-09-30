using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class AllergySeverityEntity : IContractConvertible<AllergySeverity>
    {
        #region Implementation of IContractConvertible<AllergySeverity>

        public AllergySeverity ToContract()
        {
            return new AllergySeverity(Key)
                {
                    Code = AllergySeverityCode,
                    Description = DescriptionText,
                    ExternalSystemKey = ExternalSystemKey,
                    SortOrder = SortValue,
                    LastModified = LastModifiedBinaryValue.ToArray()
                };
        }

        #endregion
    }
}
