using System;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents an association of a user role and a user account for a given period
    /// of time such that the user account may perform the user roles for that period
    /// of time.
    /// </summary>
    [Serializable]
    public class UserRole : Entity<Guid>
    {
        #region Constructors

        public UserRole()
        {

        }

        public UserRole(Guid key)
        {
            Key = key;
        }
        
        #endregion

        #region Operator Overloads

        public static implicit operator UserRole(Guid key)
        {
            return FromKey(key);
        }

        public static UserRole FromKey(Guid key)
        {
            return new UserRole(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of a role.
        /// </summary>
        public Guid RoleKey { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether the user-role-member
        /// association is assigned temporarily for med visitor access.
        /// </summary>
        public bool MedicationTemporaryAccess { get; internal set; }

        /// <summary>
        /// Gets or sets the surrogate keys of areas associated with this user role.
        /// </summary>
        public Guid[] Areas { get; set; }

        #endregion
    }
}
