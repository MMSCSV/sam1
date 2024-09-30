using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class UserFingerprintEntity : IContractConvertible<UserFingerprint>
    {
        #region IContractConvertible<UserFingerprint> Members

        public UserFingerprint ToContract()
        {
            return new UserFingerprint(Key)
                {
                    UserAccountKey = UserAccountKey,
                    Value1 = Fingerprint1Value.ToArray(),
                    Value1Length = Fingerprint1LengthQuantity,
                    Value2 = Fingerprint2Value.ToArray(),
                    Value2Length = Fingerprint2LengthQuantity,
                    Value3 = Fingerprint3Value.ToArray(),
                    Value3Length = Fingerprint3LengthQuantity,
                    LastModified = LastModifiedBinaryValue.ToArray()
                };
        }

        #endregion
    }
}
