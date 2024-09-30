using System;
using CareFusion.Dispensing.Resources;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents an association of a user role and a permission for a 
    /// given period of time such that each user that is a member of
    /// the role is granted the permission for that period of time.
    /// </summary>
    [Serializable]
    public class RolePermission : Entity<Guid>
    {
        #region Constructors

        public RolePermission()
        {

        }

        public RolePermission(Guid key)
        {
            Key = key;
        }
        
        #endregion

        #region Operator Overloads

        public static implicit operator RolePermission(Guid key)
        {
            return FromKey(key);
        }

        public static RolePermission FromKey(Guid key)
        {
            return new RolePermission(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the permission.
        /// </summary>
        [NotNullValidator(MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "RolePermissionRequired")]
        public Permission Permission { get; set; }

        /// <summary>
        /// Gets or sets the security groups. 
        /// </summary>
        public Guid[] SecurityGroups { get; set; }

        #endregion
    }
}
