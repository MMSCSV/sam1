using System;
using System.Runtime.Serialization;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a fingerprint for a user account for a given period of time.
    /// </summary>
    [DataContract(Namespace = ContractConstants.ContractsNamespaceV1)]
    public class UserFingerprint : Entity<Guid>
    {
        #region Constructors

        public UserFingerprint()
        {
        }

        public UserFingerprint(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator UserFingerprint(Guid key)
        {
            return FromKey(key);
        }

        public static UserFingerprint FromKey(Guid key)
        {
            return new UserFingerprint(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of a user account.
        /// </summary>
        [DataMember]
        public Guid UserAccountKey { get; set; }

        /// <summary>
        /// Gets or sets the biometrics.
        /// </summary>
        [DataMember]
        public byte[] Value1 { get; set; }

        /// <summary>
        /// Gets or sets the length of value 1.
        /// </summary>
        [DataMember]
        public long Value1Length { get; set; }

        /// <summary>
        /// Gets or sets the biometrics.
        /// </summary>
        [DataMember]
        public byte[] Value2 { get; set; }

        /// <summary>
        /// Gets or sets the length of value 2.
        /// </summary>
        [DataMember]
        public long Value2Length { get; set; }

        /// <summary>
        /// Gets or sets the biometrics.
        /// </summary>
        [DataMember]
        public byte[] Value3 { get; set; }

        /// <summary>
        /// Gets or sets the length of value 3.
        /// </summary>
        [DataMember]
        public long Value3Length { get; set; }

        #endregion
    }
}
