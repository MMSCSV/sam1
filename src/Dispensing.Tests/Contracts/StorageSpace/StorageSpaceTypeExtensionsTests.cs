using CareFusion.Dispensing.Contracts;
using NUnit.Framework;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Tests.Contracts.StorageSpace
{
    public class StorageSpaceTypeExtensionsTests
    {
        [TestCase(StorageSpaceTypeInternalCode.CUBIEPKT1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.CUBIEPKT2, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.CUBIEDRWR1, ExpectedResult = false)]
        [TestCase(StorageSpaceTypeInternalCode.FRIDGEBIN1, ExpectedResult = false)]
        [TestCase(StorageSpaceTypeInternalCode.ACCMTRXDRWR1, ExpectedResult = false)]
        public bool IsCubiePocketTest(StorageSpaceTypeInternalCode storageSpaceType)
        {
            return storageSpaceType.IsCubiePocket();
        }

        [TestCase(StorageSpaceTypeInternalCode.FRIDGEBINPKT1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.FRIDGEBIN1, ExpectedResult = false)]
        [TestCase(StorageSpaceTypeInternalCode.FRIDGEMGR1, ExpectedResult = false)]
        [TestCase(StorageSpaceTypeInternalCode.PSCMGR1, ExpectedResult = false)]
        public bool IsFridgeBinPocketTest(StorageSpaceTypeInternalCode storageSpaceType)
        {
            return storageSpaceType.IsFridgeBinPocket();
        }

        [TestCase(StorageSpaceTypeInternalCode.PSCBINPKT1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.PSCBIN1, ExpectedResult = false)]
        [TestCase(StorageSpaceTypeInternalCode.PSCMGR1, ExpectedResult = false)]
        public bool IsPremiumBinPocketTest(StorageSpaceTypeInternalCode storageSpaceType)
        {
            return storageSpaceType.IsPremiumBinPocket();
        }

        [TestCase(StorageSpaceTypeInternalCode.CUBIEPKT1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.CUBIEPKT2, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.FRIDGEBINPKT1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.PSCBINPKT1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.PSCBIN1, ExpectedResult = false)]
        [TestCase(StorageSpaceTypeInternalCode.FRIDGEBIN1, ExpectedResult = false)]
        [TestCase(StorageSpaceTypeInternalCode.REMOTEPKT1, ExpectedResult = false)]
        public bool HasPhysicalMaxTest(StorageSpaceTypeInternalCode storageSpaceType)
        {
            return storageSpaceType.HasPhysicalMax();
        }

        [TestCase(StorageSpaceTypeInternalCode.FRIDGEBIN1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.PSCBIN1, ExpectedResult = false)]
        [TestCase(StorageSpaceTypeInternalCode.GENERALEXTRETBIN1, ExpectedResult = false)]
        public bool IsFridgeBinTest(StorageSpaceTypeInternalCode storageSpaceType)
        {
            return storageSpaceType.IsFridgeBin();
        }

        [TestCase(StorageSpaceTypeInternalCode.PSCBIN1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.FRIDGEBIN1, ExpectedResult = false)]
        [TestCase(StorageSpaceTypeInternalCode.GENERALEXTRETBIN1, ExpectedResult = false)]
        public bool IsPremiumBinTest(StorageSpaceTypeInternalCode storageSpaceType)
        {
            return storageSpaceType.IsPremiumBin();
        }

        [TestCase(StorageSpaceTypeInternalCode.CUBIEDRWR1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.CUBIEDRWR2, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.ACCMTRXDRWR1, ExpectedResult = false)]
        [TestCase(StorageSpaceTypeInternalCode.MINIDRWR1, ExpectedResult = false)]
        public bool IsCubieDrawerTest(StorageSpaceTypeInternalCode storageSpaceType)
        {
            return storageSpaceType.IsCubieDrawer();
        }

        [TestCase(StorageSpaceTypeInternalCode.CUBIEDRWR1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.CUBIEDRWR2, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.MTRXDRWR1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.MINIDRWR1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.ACCMTRXDRWR1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.MINIDRWRPKT1, ExpectedResult = false)]
        public bool IsDrawerTest(StorageSpaceTypeInternalCode storageSpaceType)
        {
            return storageSpaceType.IsDrawer();
        }

        [TestCase(StorageSpaceTypeInternalCode.TALLAUXDOOR1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.SMRTREMOTEMGR1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.REMOTEMGR1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.FRIDGEMGR1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.FRIDGEMGR2, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.PSCMGR1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.EXCESSPKT1, ExpectedResult = false)]
        public bool IsDoorTest(StorageSpaceTypeInternalCode storageSpaceType)
        {
            return storageSpaceType.IsDoor();
        }

        [TestCase(StorageSpaceTypeInternalCode.ACCMTRXDRWR1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.ACCMTRXPKT1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.INTMATRIXRETBIN1, ExpectedResult = false)]
        public bool IsAccessibleTest(StorageSpaceTypeInternalCode storageSpaceType)
        {
            return storageSpaceType.IsAccessible();
        }

        [TestCase(StorageSpaceTypeInternalCode.MINIDRWRPKT1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.MTRXPKT1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.INTMATRIXRETBIN1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.SRMPKT1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.RMPKT1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.TALLAUXPKT1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.ACCMTRXPKT1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.FRIDGEPKT1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.EXCESSPKT1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.REMOTEPKT1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.PSCBINPKT1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.FRIDGEBINPKT1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.PSCPKT1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.CUBIEPKT1, ExpectedResult = false)]
        [TestCase(StorageSpaceTypeInternalCode.CUBIEPKT2, ExpectedResult = false)]
        public bool MultiplePocketsExposedOnAccessTest(StorageSpaceTypeInternalCode storageSpaceType)
        {
            return storageSpaceType.MultiplePocketsExposedOnAccess();
        }

        [TestCase(StorageSpaceTypeInternalCode.INTMATRIXRETBIN1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.GENERALEXTRETBIN1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.EXTDESTRUCTBIN1, ExpectedResult = false)]
        [TestCase(StorageSpaceTypeInternalCode.FRIDGEBIN1, ExpectedResult = false)]
        public bool IsReturnBinTest(StorageSpaceTypeInternalCode storageSpaceType)
        {
            return storageSpaceType.IsReturnBin();
        }

        [TestCase(StorageSpaceTypeInternalCode.CUBIEDRWR1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.CUBIEDRWR2, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.CUBIEPKT1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.CUBIEPKT2, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.MTRXDRWR1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.MINIDRWR1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.MINIDRWRPKT1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.MINITRAY1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.TALLAUXDOOR1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.MEDTALLAUXCAB1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.SMRTREMOTEMGR1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.ACCMTRXDRWR1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.REMOTEMGR1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.FRIDGEMGR1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.FRIDGEBIN1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.FRIDGEMGR2, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.PSCMGR1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.PSCBIN1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.PSCBINPKT1, ExpectedResult = false)]
        [TestCase(StorageSpaceTypeInternalCode.ACCMTRXPKT1, ExpectedResult = false)]
        [TestCase(StorageSpaceTypeInternalCode.FRIDGEBINPKT1, ExpectedResult = false)]
        public bool IsHardwareAccessibleDirectlyTest(StorageSpaceTypeInternalCode storageSpaceType)
        {
            return storageSpaceType.IsHardwareAccessibleDirectly();
        }

        [TestCase(StorageSpaceTypeInternalCode.MTRXPKT1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.ACCMTRXPKT1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.INTMATRIXRETBIN1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.SRMPKT1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.RMPKT1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.TALLAUXPKT1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.FRIDGEPKT1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.FRIDGEBINPKT1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.INTDESTRUCTBIN1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.PSCPKT1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.PSCBINPKT1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.FRIDGEBIN1, ExpectedResult = false)]
        [TestCase(StorageSpaceTypeInternalCode.PSCBIN1, ExpectedResult = false)]
        public bool IsHardwareAccessibleIndirectlyTest(StorageSpaceTypeInternalCode storageSpaceType)
        {
            return storageSpaceType.IsHardwareAccessibleIndirectly();
        }

        [TestCase(StorageSpaceTypeInternalCode.CUBIEDRWR1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.CUBIEDRWR2, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.CUBIEPKT1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.CUBIEPKT2, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.MTRXDRWR1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.MINIDRWR1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.MINITRAY1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.TALLAUXDOOR1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.SMRTREMOTEMGR1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.REMOTEMGR1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.ACCMTRXDRWR1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.FRIDGEMGR1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.FRIDGEBIN1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.FRIDGEMGR2, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.PSCMGR1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.PSCBIN1, ExpectedResult = true)]
        [TestCase(StorageSpaceTypeInternalCode.FRIDGEBINPKT1, ExpectedResult = false)]
        [TestCase(StorageSpaceTypeInternalCode.PSCBINPKT1, ExpectedResult = false)]
        public bool CanUpdateStateTest(StorageSpaceTypeInternalCode storageSpaceType)
        {
            return storageSpaceType.CanUpdateState();
        }
    }
}
