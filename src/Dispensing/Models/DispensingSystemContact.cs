using System;
using System.ComponentModel.DataAnnotations.Schema;
using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Models
{
    /// <summary>
    /// Represents a dispensing system contact.
    /// </summary>
    [Serializable]
    public class DispensingSystemContact : IEntity<Guid>
    {
        public const int PrimaryContactRankValue = 10;
        public const int SecondaryContactRankValue = 20;

        #region Constructors

        public DispensingSystemContact()
        {
        }

        public DispensingSystemContact(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator DispensingSystemContact(Guid key)
        {
            return FromKey(key);
        }

        public static DispensingSystemContact FromKey(Guid key)
        {
            return new DispensingSystemContact(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of a dispensing system contact.
        /// </summary>
        [Column("DispensingSystemContactKey")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of a dispensing system.
        /// </summary>
        public Guid DispensingSystemKey { get; internal set; }

        /// <summary>
        /// Gets or sets the value that gives a rank for a contact.
        /// </summary>
        [Column("RankValue")]
        public int Rank { get; internal set; }

        /// <summary>
        /// Gets or sets the name of a contact at a customer.
        /// </summary>
        [Column("CustomerContactName")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the text that describes a contact, such as the contact's title or role.
        /// </summary>
        [Column("CustomerContactDescriptionText")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the phone number for a contact at a customer.
        /// </summary>
        [Column("CustomerContactPhoneNumberText")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the fax number for a contact at a customer.
        /// </summary>
        [Column("CustomerContactFaxNumberText")]
        public string FaxNumber { get; set; }

        /// <summary>
        /// Gets or sets the email address for a contact at a customer.
        /// </summary>
        [Column("CustomerContactEmailAddressValue")]
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the internal code that identifies a preferred contact method
        /// for a customer contact.
        /// </summary>
        public string CustomerContactPreferredMethodInternalCode { get; set; }

        /// <summary>
        /// Gets or sets the binary value that is unique within a database and that is assigned a new
        /// value on insert and on update.
        /// </summary>
        [Column("LastModifiedBinaryValue")]
        public byte[] LastModified { get; set; }

        #endregion

        #region Public Members

        public bool IsTransient()
        {
            return Key == default(Guid);
        }

        #endregion
    }
}
