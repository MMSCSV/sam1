using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class ClinicalDataSubjectTypeEntity : IContractConvertible<ClinicalDataSubjectType>
    {
        #region IContractConvertible<ClinicalDataSubjectType> Members

        public ClinicalDataSubjectType ToContract()
        {
            return new ClinicalDataSubjectType(InternalCode, DescriptionText)
                {
                    SortValue = SortValue
                };
        }

        #endregion
    }
}
