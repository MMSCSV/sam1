using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a facility contact.
    /// </summary>
    [Serializable]
    public class FacilityContact : IEntity<Guid>
    {
        public const int PrimaryContactRankValue = 10;
        public const int SecondaryContactRankValue = 20;

        #region Constructors

        public FacilityContact()
        {
        }

        public FacilityContact(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator FacilityContact(Guid key)
        {
            return FromKey(key);
        }

        public static FacilityContact FromKey(Guid key)
        {
            return new FacilityContact(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of a facility contact.
        /// </summary>
        [Column("FacilityContactKey")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of a facility.
        /// </summary>
        public Guid FacilityKey { get; set; }

        /// <summary>
        /// Gets or sets the non-localizable code that identifies a business domain.
        /// </summary>
        public string BusinessDomainInternalCode { get; set; }
        
        /// <summary>
        /// Gets or sets the rank of a facility contact for a given facility contact type.
        /// </summary>
        [Column("RankValue")]
        public int Rank { get; set; }

        /// <summary>
        /// Gets or sets the name of a contact at a facility.
        /// </summary>
        [Column("CustomerContactName")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the text that describes a contact at a facility, such as the
        /// contact's title or role.
        /// </summary>
        [Column("CustomerContactDescriptionText")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the phone number for a contact at a facility.
        /// </summary>
        [Column("CustomerContactPhoneNumberText")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the fax number for a contact at a facility.
        /// </summary>
        [Column("CustomerContactFaxNumberText")]
        public string FaxNumber { get; set; }

        /// <summary>
        /// Gets or sets the email address for a contact at a facility.
        /// </summary>
        [Column("CustomerContactEmailAddressValue")]
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the internal code that identifies a preferred contact method
        /// for a contact at a facility.
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
