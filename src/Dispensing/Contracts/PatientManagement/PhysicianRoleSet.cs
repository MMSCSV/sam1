using System;
using System.Collections.Generic;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{
    public class PhysicianRoleSet : EntitySet<Guid, Physician>
    {
        #region Constructors

        public PhysicianRoleSet()
        {
        }

        public PhysicianRoleSet(Guid key)
            : base(key)
        {
            
        }

        public PhysicianRoleSet(EncounterPhysicianRoleInternalCode physicianRole)
        {
            RoleInternalCode = physicianRole;
        }

        public PhysicianRoleSet(Guid key, EncounterPhysicianRoleInternalCode physicianRole)
            : base(key)
        {
            RoleInternalCode = physicianRole;
        }

        public PhysicianRoleSet(Guid key, EncounterPhysicianRoleInternalCode physicianRole, IEnumerable<Physician> items)
            : base(key, items)
        {
            RoleInternalCode = physicianRole;
        }

        public PhysicianRoleSet(EncounterPhysicianRoleInternalCode physicianRole, IEnumerable<Physician> items)
            : base(items)
        {
            RoleInternalCode = physicianRole;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator PhysicianRoleSet(Guid key)
        {
            return FromKey(key);
        }

        public static PhysicianRoleSet FromKey(Guid key)
        {
            return new PhysicianRoleSet(key);
        }

        #endregion

        #region Public Properties

        public EncounterPhysicianRoleInternalCode RoleInternalCode { get; set; }

        #endregion
    }
}
