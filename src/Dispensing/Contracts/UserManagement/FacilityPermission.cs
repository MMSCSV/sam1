using System;
using System.Text;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a facility permission tuple.
    /// </summary>
    [Serializable]
    public sealed class FacilityPermission
    {
        private readonly Guid _userAccountKey;
        private readonly Guid? _facilityKey;
        private readonly PermissionInternalCode _permission;

        public FacilityPermission(Guid userAccountKey, Guid? facilityKey, PermissionInternalCode permission)
        {
            _userAccountKey = userAccountKey;
            _facilityKey = facilityKey;
            _permission = permission;
        }

        public Guid UserAccountKey
        {
            get { return _userAccountKey; }
        }

        public Guid? FacilityKey
        {
            get { return _facilityKey; }
        }

        public PermissionInternalCode Permission
        {
            get { return _permission; }
        }

        #region Override Members

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            FacilityPermission facilityPermission = obj as FacilityPermission;
            if (facilityPermission == null)
            {
                return false;
            }

            return (_userAccountKey == facilityPermission._userAccountKey &&
                    _facilityKey == facilityPermission._facilityKey &&
                    _permission == facilityPermission._permission);
        }

        public override int GetHashCode()
        {
            return _userAccountKey.GetHashCode() ^ _facilityKey.GetHashCode() ^ _permission.GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            sb.Append(_userAccountKey);
            sb.Append(", ");
            sb.Append(_facilityKey);
            sb.Append(", ");
            sb.Append(Permission);
            sb.Append(")");

            return sb.ToString();

        }

        #endregion
    }
}
