using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class PatientIDTypeEntity : IContractConvertible<PatientIdentificationType>
    {
        #region IContractConvertible<PatientIdentificationType> Members

        public PatientIdentificationType ToContract()
        {
            return new PatientIdentificationType(Key)
            {
                DisplayCode = DisplayCode,
                InternalCode = InternalCode,
                Description = DescriptionText,
                IsActive = ActiveFlag,
                SortOrder = SortValue,
                LastModified = LastModifiedBinaryValue.ToArray()
            };
        }

        #endregion
    }
}
