using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{
    public partial class NoticeType
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the internal code that identifies a business domain.
        /// </summary>
        public BusinessDomainInternalCode? BusinessDomain
        {
            get { return BusinessDomainInternalCode.FromNullableInternalCode<BusinessDomainInternalCode>(); }
        }

        #endregion
    }
}
