using System;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents the designation of a physician for a given combination
    /// of encounter and role performed, for a given period of time.
    /// </summary>
    [Serializable]
    public class Physician : Entity<Guid>
    {
        #region Constructors

        public Physician()
        {
        }

        public Physician(Guid key)
        {
            Key = key;    
        }

        #endregion

        #region Operator Overloads

        public static implicit operator Physician(Guid key)
        {
            return FromKey(key);
        }

        public static Physician FromKey(Guid key)
        {
            return new Physician(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets an integer that identifies a member within an ordered set.
        /// </summary>
        public int MemberNumber { get; set; }

        /// <summary>
        /// Gets or sets the ID that identifies a physician.
        /// </summary>
        public string PhysicianId { get; set; }

        /// <summary>
        /// Gets or sets the prefix for a physician.
        /// </summary>
        /// <value>The name.</value>
        public string Prefix { get; set; }

        /// <summary>
        /// Gets or sets the first name for a physician.
        /// </summary>
        /// <value>The name.</value>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the middle name for a physician.
        /// </summary>
        /// <value>The name.</value>
        public string MiddleName { get; set; }

        /// <summary>
        /// Gets or sets the last name for a physician.
        /// </summary>
        /// <value>The name.</value>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the suffix for a physician name.
        /// </summary>
        /// <value>The name.</value>
        public string Suffix { get; set; }

        #endregion
    }
}
