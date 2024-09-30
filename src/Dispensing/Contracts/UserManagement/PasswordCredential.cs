using System;
using CareFusion.Dispensing.Resources;
using CareFusion.Dispensing.Validators;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a secret credential for a user account for a given period
    /// of time.
    /// </summary>
    [Serializable]
    public class PasswordCredential : Entity<Guid>, ICloneable
    {
        #region Constructors

        public PasswordCredential()
        {
            EncryptionAlgorithm = EncryptionAlgorithmInternalCode.SHA_1;
        }

        public PasswordCredential(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator PasswordCredential(Guid key)
        {
            return FromKey(key);
        }

        public static PasswordCredential FromKey(Guid key)
        {
            return new PasswordCredential(key);
        }

        #endregion

        #region Public Properties

        public DateTime CreatedUtcDateTime { get; set; }

        /// <summary>
        /// Gets or sets the password credential.  This is only used when plain text password
        /// is passed to the services.
        /// </summary>
        /// <remarks>The plain text password will never be returned by the service layer.</remarks>
        [DispensingStringLengthValidator(ValidationConstants.UserPasswordUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "UserAccountPasswordOutOfBounds")]
        [RequiredStringValidator(MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "UserAccountPasswordRequired")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the encryption algorithm used for this password.
        /// </summary>
        public EncryptionAlgorithmInternalCode EncryptionAlgorithm { get; set; }

        /// <summary>
        /// Gets or sets the text that holds the output of a hash function that takes a password and
        /// a salt as input.
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// Gets or sets the value that is input, along with a password, to a hash function.
        /// </summary>
        public string Salt { get; set; }

        /// <summary>
        /// Gets or sets the UTC date and time of when a user last changed his/her password.
        /// </summary>
        /// <value>The password expiration UTC date.</value>
        public DateTime? UserChangedOwnPasswordUtcDate { get; set; }

        /// <summary>
        /// Gets or sets the local date and time of when a user last changed his/her password.
        /// </summary>
        /// <value>The password expiration date.</value>
        public DateTime? UserChangedOwnPasswordDate { get; set; }

        #endregion

        #region ICloneable Members

        /// <summary>
        /// Performs a deep-copy of the current instance.
        /// </summary>
        /// <returns></returns>
        public PasswordCredential Clone(bool makeTransient)
        {
            // There is nothing to deep-copy in this entity.
            return (PasswordCredential)ShallowClone(makeTransient);
        }

        /// <summary>
        /// Performs a deep-copy of the current instance.
        /// </summary>
        /// <returns></returns>
        public PasswordCredential Clone()
        {
            // There is nothing to deep-copy in this entity.
            return Clone(true);
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion
    }
}
