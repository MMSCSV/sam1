using CareFusion.Dispensing.Contracts;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class UserTypeEntity : IContractConvertible<UserType>
    {
        #region IContractConvertible<UserType> Members

        public UserType ToContract()
        {
            return new UserType(Key)
            {
                DisplayCode = DisplayCode,
                InternalCode = InternalCode.FromNullableInternalCode<UserTypeInternalCode>(),
                Description = DescriptionText,
                StandardUserTypeInternalCode = StandardUserTypeInternalCode,
                StandardUserTypeDescription = StandardUserTypeDescriptionText,
                SortOrder = SortValue,
                LastModified = LastModifiedBinaryValue.ToArray()
            };
        }

        #endregion
    }
}
