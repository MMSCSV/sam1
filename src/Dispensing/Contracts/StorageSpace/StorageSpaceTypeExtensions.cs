using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{
    public static class StorageSpaceTypeExtensions
    {
        /// <summary>
        /// Determines if the storage space type is a cubie pocket.
        /// </summary>
        public static bool IsCubiePocket(this StorageSpaceTypeInternalCode typeCode)
        {
            switch (typeCode)
            {
                case StorageSpaceTypeInternalCode.CUBIEPKT1:
                case StorageSpaceTypeInternalCode.CUBIEPKT2:
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Determines if the storage space type is a fridge bin pocket.
        /// </summary>
        public static bool IsFridgeBinPocket(this StorageSpaceTypeInternalCode typeCode)
        {
            switch (typeCode)
            {
                case StorageSpaceTypeInternalCode.FRIDGEBINPKT1:
                    return true;
            }
            return false;
        }



        /// <summary>
        /// Determines if the storage space type is a fridge bin pocket.
        /// </summary>
        public static bool IsPremiumBinPocket(this StorageSpaceTypeInternalCode typeCode)
        {
            switch (typeCode)
            {
                case StorageSpaceTypeInternalCode.PSCBINPKT1:
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Determines if the storage space type needs physical max
        /// </summary>
        public static bool HasPhysicalMax(this StorageSpaceTypeInternalCode typeCode)
        {
            return IsCubiePocket(typeCode) || IsFridgeBinPocket(typeCode) || IsPremiumBinPocket(typeCode);
        }

        public static bool IsFridgeBin(this StorageSpaceTypeInternalCode typeCode)
        {
            switch (typeCode)
            {
                case StorageSpaceTypeInternalCode.FRIDGEBIN1:
                    return true;
            }
            return false;
        }

        public static bool IsPremiumBin(this StorageSpaceTypeInternalCode typeCode)
        {
            switch (typeCode)
            {
                case StorageSpaceTypeInternalCode.PSCBIN1:
                    return true;
            }
            return false;
        }

        public static bool IsCubieDrawer(this StorageSpaceTypeInternalCode typeCode)
        {
            switch (typeCode)
            {
                case StorageSpaceTypeInternalCode.CUBIEDRWR1:
                case StorageSpaceTypeInternalCode.CUBIEDRWR2:
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Determines if the storage space type is a drawer.
        /// </summary>
        /// <returns></returns>
        public static bool IsDrawer(this StorageSpaceTypeInternalCode typeCode)
        {
            switch (typeCode)
            {
                case StorageSpaceTypeInternalCode.CUBIEDRWR1:
                case StorageSpaceTypeInternalCode.CUBIEDRWR2:
                case StorageSpaceTypeInternalCode.MTRXDRWR1:
                case StorageSpaceTypeInternalCode.MINIDRWR1:
                case StorageSpaceTypeInternalCode.ACCMTRXDRWR1:
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Determines if the storage space type is a door.
        /// </summary>
        /// <param name="typeCode"></param>
        /// <returns></returns>
        public static bool IsDoor(this StorageSpaceTypeInternalCode typeCode)
        {
            switch (typeCode)
            {
                case StorageSpaceTypeInternalCode.TALLAUXDOOR1:
                case StorageSpaceTypeInternalCode.SMRTREMOTEMGR1:
                case StorageSpaceTypeInternalCode.REMOTEMGR1:
                case StorageSpaceTypeInternalCode.FRIDGEMGR1:
                case StorageSpaceTypeInternalCode.FRIDGEMGR2:
                case StorageSpaceTypeInternalCode.PSCMGR1:
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Determines if the storage space type is an accessible drawer or pocket
        /// </summary>
        /// <param name="typeCode"></param>
        /// <returns></returns>
        public static bool IsAccessible(this StorageSpaceTypeInternalCode typeCode)
        {
            switch (typeCode)
            {
                case StorageSpaceTypeInternalCode.ACCMTRXDRWR1:
                case StorageSpaceTypeInternalCode.ACCMTRXPKT1:
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether multiple pockets are exposed when accessing this storage space type.
        /// </summary>
        public static bool MultiplePocketsExposedOnAccess(this StorageSpaceTypeInternalCode typeCode)
        {
            switch (typeCode)
            {
                case StorageSpaceTypeInternalCode.MINIDRWRPKT1:
                case StorageSpaceTypeInternalCode.MTRXPKT1:
                case StorageSpaceTypeInternalCode.INTMATRIXRETBIN1:
                case StorageSpaceTypeInternalCode.SRMPKT1:
                case StorageSpaceTypeInternalCode.RMPKT1:
                case StorageSpaceTypeInternalCode.TALLAUXPKT1:
                case StorageSpaceTypeInternalCode.ACCMTRXPKT1:
                case StorageSpaceTypeInternalCode.FRIDGEPKT1:
                case StorageSpaceTypeInternalCode.EXCESSPKT1:
                case StorageSpaceTypeInternalCode.REMOTEPKT1:
                case StorageSpaceTypeInternalCode.PSCPKT1:
                case StorageSpaceTypeInternalCode.PSCBINPKT1:
                case StorageSpaceTypeInternalCode.FRIDGEBINPKT1:
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Determines if the storage space type is a return bin (internal or external)
        /// </summary>
        public static bool IsReturnBin(this StorageSpaceTypeInternalCode typeCode)
        {
            switch (typeCode)
            {
                case StorageSpaceTypeInternalCode.INTMATRIXRETBIN1:
                case StorageSpaceTypeInternalCode.GENERALEXTRETBIN1:
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Determines if the storage space is accessible via hardware.
        /// </summary>
        /// <returns></returns>
        public static bool IsHardwareAccessible(this StorageSpaceTypeInternalCode typeCode)
        {
            return typeCode.IsHardwareAccessibleDirectly() || typeCode.IsHardwareAccessibleIndirectly();
        }

        /// <summary>
        /// Determines if the storage space is directly accessible via hardware
        /// (i.e. it is something that the HAL/Bus has knowledge about and forms part of the access path)
        /// </summary>
        /// <example>
        /// Cubie drawers/pockets, Matrix drawers, Mini drawer/tray/pockets, Doors, SRM, Refrigerator Cabinet
        /// </example>
        public static bool IsHardwareAccessibleDirectly(this StorageSpaceTypeInternalCode typeCode)
        {
            switch (typeCode)
            {
                case StorageSpaceTypeInternalCode.CUBIEDRWR1:
                case StorageSpaceTypeInternalCode.CUBIEDRWR2:
                case StorageSpaceTypeInternalCode.CUBIEPKT1:
                case StorageSpaceTypeInternalCode.CUBIEPKT2:
                case StorageSpaceTypeInternalCode.MTRXDRWR1:
                case StorageSpaceTypeInternalCode.MINIDRWR1:
                case StorageSpaceTypeInternalCode.MINIDRWRPKT1:
                case StorageSpaceTypeInternalCode.MINITRAY1:
                case StorageSpaceTypeInternalCode.TALLAUXDOOR1:
                case StorageSpaceTypeInternalCode.MEDTALLAUXCAB1:
                case StorageSpaceTypeInternalCode.SMRTREMOTEMGR1:
                case StorageSpaceTypeInternalCode.ACCMTRXDRWR1:
                case StorageSpaceTypeInternalCode.REMOTEMGR1:
                case StorageSpaceTypeInternalCode.FRIDGEMGR1:
                case StorageSpaceTypeInternalCode.FRIDGEBIN1:
                case StorageSpaceTypeInternalCode.FRIDGEMGR2:
                case StorageSpaceTypeInternalCode.PSCMGR1:
                case StorageSpaceTypeInternalCode.PSCBIN1:
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Determines if the storage space is indirectly accessible via hardware
        /// (i.e. it is something that the HAL/Bus doesnt have knowledge about but is contained in an accessible piece of hardware)
        /// </summary>
        /// <example>
        /// Matrix pockets, SRM pockets, Tall Aux pockets
        /// </example>
        /// <returns></returns>
        public static bool IsHardwareAccessibleIndirectly(this StorageSpaceTypeInternalCode typeCode)
        {
            switch (typeCode)
            {
                case StorageSpaceTypeInternalCode.MTRXPKT1:
                case StorageSpaceTypeInternalCode.ACCMTRXPKT1:
                case StorageSpaceTypeInternalCode.INTMATRIXRETBIN1:
                case StorageSpaceTypeInternalCode.SRMPKT1:
                case StorageSpaceTypeInternalCode.RMPKT1:
                case StorageSpaceTypeInternalCode.TALLAUXPKT1:
                case StorageSpaceTypeInternalCode.FRIDGEPKT1:
                case StorageSpaceTypeInternalCode.FRIDGEBINPKT1:
                case StorageSpaceTypeInternalCode.INTDESTRUCTBIN1:
                case StorageSpaceTypeInternalCode.PSCPKT1:
                case StorageSpaceTypeInternalCode.PSCBINPKT1:
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Determines if the storage space's state can be updated.
        /// Only certain storage spaces have state records.
        /// </summary>
        /// <returns></returns>
        public static bool CanUpdateState(this StorageSpaceTypeInternalCode typeCode)
        {
            switch (typeCode)
            {
                case StorageSpaceTypeInternalCode.CUBIEDRWR1:
                case StorageSpaceTypeInternalCode.CUBIEDRWR2:
                case StorageSpaceTypeInternalCode.CUBIEPKT1:
                case StorageSpaceTypeInternalCode.CUBIEPKT2:
                case StorageSpaceTypeInternalCode.MTRXDRWR1:
                case StorageSpaceTypeInternalCode.MINIDRWR1:
                case StorageSpaceTypeInternalCode.MINITRAY1:
                case StorageSpaceTypeInternalCode.TALLAUXDOOR1:
                case StorageSpaceTypeInternalCode.SMRTREMOTEMGR1:
                case StorageSpaceTypeInternalCode.REMOTEMGR1:
                case StorageSpaceTypeInternalCode.ACCMTRXDRWR1:
                case StorageSpaceTypeInternalCode.FRIDGEMGR1:
                case StorageSpaceTypeInternalCode.FRIDGEBIN1:
                case StorageSpaceTypeInternalCode.FRIDGEMGR2:
                case StorageSpaceTypeInternalCode.PSCMGR1:
                case StorageSpaceTypeInternalCode.PSCBIN1:
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Determines if the storage space's parent state needs to be updated.
        /// </summary>
        /// <returns></returns>
        public static bool NeedToUpdateParentState(this StorageSpaceTypeInternalCode typeCode)
        {
            switch (typeCode)
            {
                case StorageSpaceTypeInternalCode.MTRXPKT1:
                case StorageSpaceTypeInternalCode.INTMATRIXRETBIN1:
                case StorageSpaceTypeInternalCode.SRMPKT1:
                case StorageSpaceTypeInternalCode.RMPKT1:
                case StorageSpaceTypeInternalCode.TALLAUXPKT1:
                case StorageSpaceTypeInternalCode.MINIDRWRPKT1:
                case StorageSpaceTypeInternalCode.ACCMTRXPKT1:
                case StorageSpaceTypeInternalCode.FRIDGEPKT1:
                case StorageSpaceTypeInternalCode.INTDESTRUCTBIN1:
                case StorageSpaceTypeInternalCode.PSCPKT1:
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Determines if the storage space can be failed.
        /// Only certain storage spaces have state records that can be marked as failed.
        /// </summary>
        /// <returns></returns>
        public static bool CanFail(this StorageSpaceTypeInternalCode typeCode)
        {
            return typeCode.CanUpdateState();
        }

        /// <summary>
        /// Determines if the storage space's parent needs to be failed.
        /// </summary>
        /// <returns></returns>
        public static bool NeedToFailParent(this StorageSpaceTypeInternalCode typeCode)
        {
            return typeCode.NeedToUpdateParentState();
        }

        /// <summary>
        /// Determines if a failure condition causes failures to
        /// cascade to a storage space's parent.
        /// </summary>
        /// <param name="failureReason"></param>
        /// <returns></returns>
        public static bool CascadeFailureToParent(this StorageSpaceTypeInternalCode typeCode, 
            StorageSpaceFailureReasonInternalCode failureReason)
        {
            return ((typeCode.IsCubiePocket() || typeCode.IsFridgeBin() || typeCode.IsPremiumBin()) &&
                     failureReason == StorageSpaceFailureReasonInternalCode.UserInitiatedFailure);
        }
    }
}
