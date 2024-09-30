using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class ClinicalDataCategoryEntity : IContractConvertible<ClinicalDataCategory>
    {
        #region IContractConvertible<ClinicalDataCategory> Members

        public ClinicalDataCategory ToContract()
        {
            return new ClinicalDataCategory(Key)
                {
                    Description = DescriptionText,
                    LastModified = LastModifiedBinaryValue.ToArray()
                };
        }

        #endregion
    }
}
