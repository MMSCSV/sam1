using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class UnitRoomEntity : IContractConvertible<Room>
    {
        #region IContractConvertible<UnitRoom> Members

        public Room ToContract()
        {
            string unitName = null;
            if (UnitEntity != null)
            {
                unitName = UnitEntity.UnitName;
            }

            return new Room(Key)
            {
                Name = RoomName,
                Description = RoomDescriptionText,
                UnitKey = UnitKey,
                UnitName = unitName,
                LastModified = LastModifiedBinaryValue.ToArray()
            };
        }

        #endregion
    }
}
