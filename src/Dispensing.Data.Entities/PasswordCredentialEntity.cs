using CareFusion.Dispensing.Contracts;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class PasswordCredentialEntity : IContractConvertible<PasswordCredential>
    {
        #region IContractConvertible<PasswordCredential> Members

        public PasswordCredential ToContract()
        {
            return new PasswordCredential(Key)
            {
                CreatedUtcDateTime = StartUTCDateTime,
                Password = null, // Hash value is the only one stored.
                PasswordHash = PasswordHashValue,
                Salt = PasswordSaltValue,
                EncryptionAlgorithm = EncryptionAlgorithmInternalCode.FromInternalCode<EncryptionAlgorithmInternalCode>(),
                UserChangedOwnPasswordUtcDate = UserChangedOwnPasswordUTCDateTime,
                UserChangedOwnPasswordDate = UserChangedOwnPasswordLocalDateTime,
                LastModified = LastModifiedBinaryValue.ToArray()
            };
        }

        #endregion
    }
}
