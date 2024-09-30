using System;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a record of a distributor for a given period of time.
    /// </summary>
    [Serializable]
    public class Distributor : Entity<Guid>
    {
        #region Constructors

        public Distributor()
        {
        }

        public Distributor(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator Distributor(Guid key)
        {
            return FromKey(key); 
        }

        public static Distributor FromKey(Guid key)
        {
            return new Distributor(key);
        }

        #endregion

        #region Contract Properties

        /// <summary>
        /// Gets or sets the ID of a distributor.
        /// </summary>
        public string DistributorId { get; set; }

        /// <summary>
        /// Gets the value that indicates whether a distributor is logically deleted.
        /// </summary>
        public bool IsDeleted { get; internal set; }

        public string DistributorName { get; set; }

        public string StreetAddress { get; set; }

        public string CityName { get; set; }

        public string SubdivisionName { get; set; }

        public string PostalCode { get; set; }

        public string CountryName { get; set; }

        public string ContactPhoneNumber { get; set; }

        public string ContactName { get; set; }

        public long TotalRows { get; set; }


        #endregion
    } 
}
