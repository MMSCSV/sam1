using System;
using CareFusion.Dispensing.Contracts;
using Pyxis.Core.Data.InternalCodes;
using System.Collections.Generic;

namespace CareFusion.Dispensing.Services.Business
{
    public interface IProductIdManager
    {
        ItemProductId GetMedProductId(string productId, Guid facilityKey);
        IEnumerable<ItemProductId> GetItemProductIds(Guid itemKey);
        void LinkProductId(Context context, string productId, Guid itemKey, string createdByExternalSystemName = null);

        void LinkAndVerifyProductId(Context context, string productId, Guid itemKey, string createdByExternalSystemName = null);

        void RelinkProductId(Context context, Guid productIdKey, Guid itemKey);

        void VerifyProductId(Context context, Guid productIdKey);

        void DeleteProductId(Context context, Guid productIdKey,ScanProductDeleteReasonInternalCode? scanProductDeleteReason = null, string deletedByExternalSystemName = null);

        void AddProductIdFromScanCode(Context context, ItemScanCode itemScanCode);
    }
}
