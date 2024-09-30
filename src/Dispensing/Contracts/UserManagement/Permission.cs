using System.Runtime.Serialization;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Extension to the auto-generated Permission class.
    /// </summary>
    public partial class Permission
    {
        #region Additional Properties

        /// <summary>
        /// Gets or sets the nonlocalizable code that identifies a kind of permission.
        /// </summary>
        [DataMember]
        public PermissionType PermissionType { get; set; }

        #endregion
    }
}
