using System;

namespace CareFusion.Dispensing.Contracts.LocationManagement
{
    /// <summary>
    /// Represents a record of Controlled Substance ID.
    /// </summary>
    [Serializable]
    public class ControlledSubstanceLicense : Entity<Guid>
    {
        #region Constructors

        public ControlledSubstanceLicense() { }

        public ControlledSubstanceLicense(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator ControlledSubstanceLicense(Guid key)
        {
            return FromKey(key);
        }

        public static ControlledSubstanceLicense FromKey(Guid key)
        {
            return new ControlledSubstanceLicense(key);
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets or sets the LicenseID of a Controlled Substance License.
        /// </summary>
        public string LicenseID { get; set; }

        /// <summary>
        /// Gets or sets the License Display Name of a Controlled Substance License.
        /// </summary>
        public string LicenseName { get; set; }

        /// <summary>
        /// Gets ort sets the ExternalFlag of a Controlled Substance License.
        /// </summary>
        public string DescriptionText { get; set; }

        /// <summary>
        /// Gets ort sets the Street Address of a Controlled Substance License.
        /// </summary>
        public string StreetAddressText { get; set; }

        /// <summary>
        /// Gets ort sets the City name of a Controlled Substance License.
        /// </summary>
        public string CityName { get; set; }

        /// <summary>
        /// Gets ort sets the Subdivision name of a Controlled Substance License.
        /// </summary>
        public string SubdivisionName { get; set; }

        /// <summary>
        /// Gets ort sets the Postal code text of a Controlled Substance License.
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets ort sets the Country name of a Controlled Substance License.
        /// </summary>
        public string CountryName { get; set; }

        #endregion
    }
}
