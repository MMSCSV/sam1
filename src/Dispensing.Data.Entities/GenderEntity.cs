using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class GenderEntity : IContractConvertible<Gender>
    {
        #region IContractConvertible<Gender> Members

        public Gender ToContract()
        {
            return new Gender(Key)
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
