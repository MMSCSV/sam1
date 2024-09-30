using CareFusion.Dispensing.Contracts;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class EncounterPhysicianRoleEntity : IContractConvertible<EncounterPhysicianRole>
    {
        #region IContractConvertible<PhysicianRole> Members

        public EncounterPhysicianRole ToContract()
        {
            return new EncounterPhysicianRole(InternalCode.FromInternalCode<EncounterPhysicianRoleInternalCode>(), DescriptionText)
            {
            };
        }

        #endregion
    }
}
