using System;
using CareFusion.Dispensing.Resources;
using CareFusion.Dispensing.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a role that users may perform.
    /// </summary>
    [Serializable]
    public class Role : Entity<Guid>
    {
        #region Constructors

        public Role()
        {

        }

        public Role(Guid key)
        {
            Key = key;
        }
        
        #endregion

        #region Operator Overloads

        public static implicit operator Role(Guid key)
        {
            return FromKey(key);
        }

        public static Role FromKey(Guid key)
        {
            return new Role(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the internal code of a role.
        /// </summary>
        public UserRoleInternalCode? InternalCode { get; internal set; }

        /// <summary>
        /// Gets or sets the surrogate key of facility.
        /// </summary>
        public Facility Facility { get; set; }

        /// <summary>
        /// Gets or sets the name of a role.
        /// </summary>
        [DispensingStringLengthValidator(ValidationConstants.RoleNameUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_NameOutOfBounds")]
        [RequiredStringValidator(MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_NameRequired")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the text that describes a role.
        /// </summary>
        [DispensingStringLengthValidator(ValidationConstants.RoleDescriptionUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_DescriptionOutOfBounds")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a user role is available for temporary users.
        /// </summary>
        public bool AllowTemporaryUsers { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a role is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the number or users associated with this role.
        /// </summary>
        public long UserMemberCount { get; internal set; }

        /// <summary>
        /// Gets or sets the list of role permissions associated with this role.
        /// </summary>
        [ObjectCollectionValidator(typeof(RolePermission))]
        public RolePermission[] Permissions { get; set; }

        /// <summary>
        /// Gets or sets the list of override groups associated with this role.
        /// </summary>
        public Guid[] OverrideGroups { get; set; }

        /// <summary>
        /// Gets or sets the list of associated visiting user roles
        /// </summary>
        public Guid[] VisitingUserRoles { get; set; }

        #endregion
    }
}
