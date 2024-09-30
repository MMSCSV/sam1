using CareFusion.Dispensing.Contracts;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class SecurityGroupEntity : IContractConvertible<SecurityGroup>
    {
        #region IContractConvertible<SecurityGroup> Members

        public SecurityGroup ToContract()
        {
            return new SecurityGroup(Key)
                {
                    BusinessDomain = BusinessDomainInternalCode.FromInternalCode<BusinessDomainInternalCode>(),
                    DisplayCode = DisplayCode,
                    Description = DescriptionText,
                    SortOrder = SortValue,
                    IsActive = ActiveFlag,
                    LastModified = LastModifiedBinaryValue.ToArray()
                };
        }

        #endregion
    }
}
