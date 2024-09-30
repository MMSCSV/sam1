using System.Collections.Generic;
using CareFusion.Dispensing.Contracts;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class StorageSpaceEntity : IContractConvertible<StorageSpace>
    {
        #region IContractConvertible<StorageSpace> Members

        public StorageSpace ToContract()
        {
            return new StorageSpace(Key, StorageSpaceSnapshotKey)
            {
                FacilityKey = FacilityKey,
                StorageSpaceTypeInternalCode = StorageSpaceTypeInternalCode.FromInternalCode<StorageSpaceTypeInternalCode>(),
                StorageSpaceFormKey = StorageSpaceFormKey,
                StorageSpaceFormDescriptionText = GetDescriptionText(StorageSpaceFormEntity),
                ParentStorageSpaceKey = ParentStorageSpaceKey,
                StorageSpaceName = StorageSpaceName,
                SerialID = SerialID,
                StackedSerialID = StackedSerialID,
                ProductName = ProductName,
                PositionID = PositionID,
                AnchorStrorageSpaceKey = AnchorStorageSpaceKey,
                SystemBusDeviceKey = SystemBusDeviceKey,
                SecondSystemBusDeviceKey = SecondSystemBusDeviceKey,
                MiniDrawerTrayMode = MiniDrawerTrayModeInternalCode.FromNullableInternalCode<MiniDrawerTrayModeInternalCode>(),
                PendingMiniDrawerTrayMode = PendingMiniDrawerTrayModeInternalCode.FromNullableInternalCode<MiniDrawerTrayModeInternalCode>(),
                StorageSpaceState = GetStorageSpaceState(StorageSpaceStateEntities),
                StorageSpaceFormInternalCode = GetStorageSpaceFormInternalCode(StorageSpaceFormEntity),
                LastModified = LastModifiedBinaryValue.ToArray(),
                ShortName = StorageSpaceShortName,
                AbbreviatedName = StorageSpaceAbbreviatedName,
                SortValue = SortValue,
                Level = LevelNumber,
                DispensingDeviceKey = DispensingDeviceKey,
                ParentMiniDrawerTrayModeInternalCode = ParentMiniDrawerTrayModeInternalCode.FromNullableInternalCode<MiniDrawerTrayModeInternalCode>(),
                ParentPendingMiniDrawerTrayModeInternalCode = ParentPendingMiniDrawerTrayModeInternalCode.FromNullableInternalCode<MiniDrawerTrayModeInternalCode>(),
                MoreThanOneItem = MoreThanOneItemFlag,
                UnavailableForInventory = UnavailableForInventoryFlag,
                FailureInHierarchy = FailureInHierarchyFlag,
                DefrostInHierarchy = DefrostInHierarchyFlag,
                RestrictedAccess = RestrictedAccessFlag
            };
        }

        public override string ToString()
        {
            return StorageSpaceShortName;
        }

        private StorageSpaceState GetStorageSpaceState(IEnumerable<StorageSpaceStateEntity> entitySet)
        {
            StorageSpaceState state = null;

            if (null != entitySet)
            {
                foreach (StorageSpaceStateEntity storageSpaceEntity in entitySet)
                {
                    state = storageSpaceEntity.ToContract();
                }
            }

            return state;
        }

        private string GetDescriptionText(StorageSpaceFormEntity storageSpaceFormEntity)
        {
            string descriptionText = null;

            if (null != storageSpaceFormEntity)
            {
                descriptionText = storageSpaceFormEntity.DescriptionText;
            }

            return descriptionText;
        }

        private StorageSpaceFormInternalCode? GetStorageSpaceFormInternalCode(StorageSpaceFormEntity storageSpaceFormEntity)
        {
            StorageSpaceFormInternalCode? code = null;

            if (null != storageSpaceFormEntity)
            {
                code = storageSpaceFormEntity.StorageSpaceFormInternalCode.FromNullableInternalCode<StorageSpaceFormInternalCode>();   
            }

            return code;
        }
        
        #endregion
    }
}
