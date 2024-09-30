using System;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a kind of user.
    /// </summary>
    [Serializable]
    public class UserType : Entity<Guid>
    {
        #region Constructors

        public UserType()
        {

        }

        public UserType(Guid key)
        {
            Key = key;
        }
        
        #endregion

        #region Operator Overloads

        public static implicit operator UserType(Guid key)
        {
            return FromKey(key);
        }

        public static UserType FromKey(Guid key)
        {
            return new UserType(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the localizable code that identifies a user type.
        /// </summary>
        public string DisplayCode { get; set; }

        /// <summary>
        /// Gets the internal code of a user type.
        /// </summary>
        public UserTypeInternalCode? InternalCode { get; internal set; }

        /// <summary>
        /// Gets or sets the text that describes a user type.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the value that is used to control the sort order.
        /// </summary>
        public int? SortOrder { get; set; }

        /// <summary>
        /// Gets or sets the internal code that identifies a standard user type.
        /// </summary>
        public string StandardUserTypeInternalCode { get; set; }

        /// <summary>
        /// Gets internal code that identifies a standard user type.
        /// </summary>
        public StandardUserTypeInternalCode? StandardUserType
        {
            get { return StandardUserTypeInternalCode.FromNullableInternalCode<StandardUserTypeInternalCode>(); }
        }

        public string StandardUserTypeDescription { get; set; }

        #endregion
    }
}
