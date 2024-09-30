using System;
using CareFusion.Dispensing.Contracts;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Services.Business
{
    public interface IItemScanCodeManager
    {
        ItemScanCode GetMedItemScanCode(string scancode, Guid facilityKey);

        ItemScanCode GetMedItemScanCode(Guid itemKey, Guid facilityKey);

        void LinkItemScanCode(Context context, string scancode, Guid itemKey, string createdByExternalSystemName = null);

        void LinkAndVerifyItemScanCode(Context context, string scancode, Guid itemKey, string createdByExternalSystemName = null);

        void RelinkScanCode(Context context, Guid itemScanCodeKey, Guid itemKey);

        void VerifyScanCode(Context context, Guid itemScanCodeKey);

        void DeleteItemScanCode(Context context, Guid itemScanCodeKey, ScanProductDeleteReasonInternalCode? scanProductDeleteReason = null, string deletedByExternalSystemName = null);
    }
}
