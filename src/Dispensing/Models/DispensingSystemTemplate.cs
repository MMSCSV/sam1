using System;
using System.ComponentModel.DataAnnotations.Schema;
using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Models
{
    [Serializable]
    public class DispensingSystemTemplate : IEntity<Guid>
    {
        #region Constructors

        public DispensingSystemTemplate()
        {
        }

        public DispensingSystemTemplate(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator DispensingSystemTemplate(Guid key)
        {
            return FromKey(key);
        }

        public static DispensingSystemTemplate FromKey(Guid key)
        {
            return new DispensingSystemTemplate(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of a dispensing system template.
        /// </summary>
        [Column("DispensingSystemTemplateKey")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether the configurable warning banner is displayed at dispensing devices and server.
        /// </summary>
        [Column("WarningBannerFlag")]
        public bool WarningBanner { get; set; }

        /// <summary>
        /// Gets or sets the header for the warning that displays at dispensing devices and server when the Activate Warning Banner is turned on.
        /// </summary>
        [Column("WarningBannerHeaderText")]
        public string WarningBannerHeader { get; set; }

        /// <summary>
        /// Gets or sets the title of the warning that displays at dispensing devices and server when the Activate Warning Banner is turned on.
        /// </summary>
        [Column("WarningBannerTitleText")]
        public string WarningBannerTitle { get; set; }

        /// <summary>
        /// Gets or sets the description that displays at dispensing devices and server when the Activate Warning Banner is turned on.
        /// </summary>
        [Column("WarningBannerDescriptionText")]
        public string WarningBannerDescription { get; set; }

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
