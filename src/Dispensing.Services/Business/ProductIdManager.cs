using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Data;
using CareFusion.Dispensing.Data.Entities;
using CareFusion.Dispensing.Resources;
using Pyxis.Core.Data.InternalCodes;
using MsValidation = Microsoft.Practices.EnterpriseLibrary.Validation.Validation;

namespace CareFusion.Dispensing.Services.Business
{
    public class ProductIdManager : IProductIdManager
    {
        #region IProductIdManager Members

        public ItemProductId GetMedProductId(string productId, Guid facilityKey)
        {
            using (IItemRepository repository = RepositoryFactory.Create<IItemRepository>())
            {
                // PIS items
                IQueryable<ItemEntity> pisItemQuery =
                    (from i in repository.GetQueryableEntity<ItemEntity>()
                     join es in repository.GetQueryableEntity<ExternalSystemEntity>()
                        on i.ExternalSystemKey equals es.Key
                     join f in repository.GetQueryableEntity<FacilityEntity>()
                        on es.Key equals f.PharmacyInformationSystemKey
                     where i.BusinessDomainInternalCode == BusinessDomainInternalCode.MED.ToInternalCode() &&
                           f.Key == facilityKey
                     select i);

                // Local items that are part of the specified facility
                IQueryable<ItemEntity> localItemQuery =
                    (from i in repository.GetQueryableEntity<ItemEntity>()
                     where i.FacilityKey == facilityKey &&
                           i.BusinessDomainInternalCode == BusinessDomainInternalCode.MED.ToInternalCode()
                     select i);

                IQueryable<ItemEntity> medItemQuery = pisItemQuery.Union(localItemQuery);

                ProductIDEntity productIdEntity = (from i in medItemQuery
                                                   join pi in repository.GetQueryableEntity<ProductIDEntity>()
                                                          on i.Key equals pi.ItemKey
                                                   where pi.ProductID == productId
                                                   select pi).SingleOrDefault();
                ItemProductId iProductId = null;

                if (productIdEntity != null)
                {
                    iProductId = productIdEntity.ToContract();
                }

                return iProductId;
            }
        }
        public IEnumerable<ItemProductId> GetItemProductIds(Guid itemKey)
        {
            using (IItemRepository repository = RepositoryFactory.Create<IItemRepository>())
            {
                return (from pi in repository.GetQueryableEntity<ProductIDEntity>()
                        where pi.ItemKey == itemKey
                        select pi.ToContract()).ToList();
            }
        }

        public void LinkProductId(Context context, string productId, Guid itemKey, string createdByExternalSystemName = null)
        {
            Guard.ArgumentNotNull(context, "context");

            using (IItemRepository repository = RepositoryFactory.Create<IItemRepository>())
            {
                ItemProductId itemProductId = new ItemProductId
                {
                    ItemKey = itemKey,
                    ProductId = productId,
                    LinkedByUserAccountKey = (context.Actor is UserActor) ? context.Actor.Key : default(Guid?),
                    LinkedDateTime = context.ActionDateTime,
                    LinkedUtcDateTime = context.ActionUtcDateTime,
                    FromExternalSystem = createdByExternalSystemName != null,
                    CreatedByExternalSystemName = createdByExternalSystemName
                };

                // Validate
                ValidateItemProductId(repository, itemProductId);

                // Insert
                repository.InsertItemProductId(context, itemProductId);
            }
        }

        public void LinkAndVerifyProductId(Context context, string productId, Guid itemKey, string createdByExternalSystemName = null)
        {
            Guard.ArgumentNotNull(context, "context");

            using (IItemRepository repository = RepositoryFactory.Create<IItemRepository>())
            {
                ItemProductId itemProductId = new ItemProductId
                {
                    ItemKey = itemKey,
                    ProductId = productId,
                    LinkedByUserAccountKey = (context.Actor is UserActor) ? context.Actor.Key : default(Guid?),
                    LinkedDateTime = context.ActionDateTime,
                    LinkedUtcDateTime = context.ActionUtcDateTime,
                    VerifiedByUserAccountKey = (context.Actor is UserActor) ? context.Actor.Key : default(Guid?),
                    VerifiedDateTime = context.ActionDateTime,
                    VerifiedUtcDateTime = context.ActionUtcDateTime,
                    FromExternalSystem = createdByExternalSystemName != null,
                    CreatedByExternalSystemName = createdByExternalSystemName
                };

                // Validate
                ValidateItemProductId(repository, itemProductId);

                // Insert
                repository.InsertItemProductId(context, itemProductId);
            }
        }

        public void RelinkProductId(Context context, Guid productIdKey, Guid itemKey)
        {
            Guard.ArgumentNotNull(context, "context");

            using (IItemRepository repository = RepositoryFactory.Create<IItemRepository>())
            {
                ItemProductId itemProductId = repository.GetItemProductId(productIdKey);
                if (itemProductId == null)
                    throw new EntityNotFoundException();

                itemProductId.ItemKey = itemKey;
                itemProductId.LinkedByUserAccountKey = (context.Actor is UserActor) ? context.Actor.Key : default(Guid?);
                itemProductId.LinkedDateTime = context.ActionDateTime;
                itemProductId.LinkedUtcDateTime = context.ActionUtcDateTime;
                itemProductId.VerifiedByUserAccountKey = null;
                itemProductId.VerifiedDateTime = null;
                itemProductId.VerifiedUtcDateTime = null;
                itemProductId.FromExternalSystem = false;

                // Validate
                ValidateItemProductId(repository, itemProductId);

                // Update
                repository.DeleteItemProductId(context, productIdKey);
                repository.InsertItemProductId(context, itemProductId);
            }
        }

        public void VerifyProductId(Context context, Guid productIdKey)
        {
            Guard.ArgumentNotNull(context, "context");

            using (IItemRepository repository = RepositoryFactory.Create<IItemRepository>())
            {
                var itemProductId = repository.GetItemProductId(productIdKey);
                if (itemProductId == null)
                    throw new EntityNotFoundException();

                // Set verification
                itemProductId.VerifiedByUserAccountKey = context.Actor.Key;
                itemProductId.VerifiedDateTime = context.ActionDateTime;
                itemProductId.VerifiedUtcDateTime = context.ActionUtcDateTime;

                // Update
                repository.UpdateItemProductId(context, itemProductId);
            }
        }

        public void DeleteProductId(Context context, Guid productIdKey, ScanProductDeleteReasonInternalCode? scanProductDeleteReasonInternalCode = null, string deletedByExternalSystemName = null)
        {
            Guard.ArgumentNotNull(context, "context");

            using (IItemRepository repository = RepositoryFactory.Create<IItemRepository>())
            {
                repository.DeleteItemProductId(context, productIdKey, scanProductDeleteReasonInternalCode, deletedByExternalSystemName: deletedByExternalSystemName);
            }
        }

        public void AddProductIdFromScanCode(Context context, ItemScanCode itemScanCode)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(itemScanCode, "itemScanCode");

            using (IItemRepository repository = RepositoryFactory.Create<IItemRepository>())
            {
                var itemProductId = new ItemProductId
                {
                    ItemKey = itemScanCode.Item.Key,
                    ProductId = itemScanCode.ScanCode,
                    LinkedByUserAccountKey = itemScanCode.LinkedByUserAccountKey,
                    LinkedDateTime = itemScanCode.LinkedDateTime,
                    LinkedUtcDateTime = itemScanCode.LinkedUtcDateTime,
                    VerifiedByUserAccountKey = itemScanCode.VerifiedByUserAccountKey,
                    VerifiedDateTime = itemScanCode.VerifiedDateTime,
                    VerifiedUtcDateTime = itemScanCode.VerifiedUtcDateTime,
                    FromExternalSystem = itemScanCode.FromExternalSystem,
                    ScanProductDeleteReason = itemScanCode.ScanProductDeleteReason,
                    OtherItemId = itemScanCode.OtherItemId,
                };

                // Validate
                ValidateItemProductId(repository, itemProductId);

                // Insert
                repository.InsertItemProductId(context, itemProductId);
            }
        }

        #endregion

        #region Private Members

        private static void ValidateItemProductId(IRepository repository, ItemProductId itemProductId)
        {
            List<ValidationError> validationErrors = new List<ValidationError>();

            // Validate contract
            var validationResults = MsValidation.Validate(itemProductId);
            if (!validationResults.IsValid)
                validationErrors.AddRange(validationResults.ToValidationErrorsArray());

            var itemInfo = repository.GetQueryableEntity<ItemEntity>()
                .Where(i => i.Key == itemProductId.ItemKey)
                .Select(i => new
                {
                    i.ExternalSystemKey,
                    i.FacilityKey
                })
                .SingleOrDefault();

            if (itemInfo != null)
            {
                IQueryable<ItemEntity> itemQuery;
                if (itemInfo.ExternalSystemKey != null)
                {
                    var pisItems = (from i in repository.GetQueryableEntity<ItemEntity>()
                                    where i.BusinessDomainInternalCode == BusinessDomainInternalCode.MED.ToInternalCode() &&
                                          i.ExternalSystemKey == itemInfo.ExternalSystemKey
                                    select i);

                    var localItems = (from i in repository.GetQueryableEntity<ItemEntity>()
                                      join f in repository.GetQueryableEntity<FacilityEntity>()
                                          on i.FacilityKey equals f.Key
                                      where i.BusinessDomainInternalCode == BusinessDomainInternalCode.MED.ToInternalCode() &&
                                            f.PharmacyInformationSystemKey == itemInfo.ExternalSystemKey
                                      select i);

                    itemQuery = pisItems.Union(localItems);
                }
                else
                {
                    var pisItems = (from i in repository.GetQueryableEntity<ItemEntity>()
                                    join es in repository.GetQueryableEntity<ExternalSystemEntity>()
                                        on i.ExternalSystemKey equals es.Key
                                    join f in repository.GetQueryableEntity<FacilityEntity>()
                                        on es.Key equals f.PharmacyInformationSystemKey
                                    where i.BusinessDomainInternalCode == BusinessDomainInternalCode.MED.ToInternalCode() &&
                                          f.Key == itemInfo.FacilityKey
                                    select i);

                    // Local items that are part of the item Facility.
                    var localItems = (from i in repository.GetQueryableEntity<ItemEntity>()
                                      where i.FacilityKey == itemInfo.FacilityKey &&
                                            i.BusinessDomainInternalCode == BusinessDomainInternalCode.MED.ToInternalCode()
                                      select i);

                    itemQuery = pisItems.Union(localItems);
                }

                var productIdQuery = (from pi in repository.GetQueryableEntity<ProductIDEntity>()
                                      join i in itemQuery
                                          on pi.ItemKey equals i.Key
                                      where pi.ProductID == itemProductId.ProductId
                                      select pi);

                if (!itemProductId.IsTransient())
                {
                    // If product id is not transient then don't include itself.
                    productIdQuery = productIdQuery.Where(pi => pi.Key != itemProductId.Key);
                }

                // If already exists then its an error.
                if (productIdQuery.Any())
                {
                    validationErrors.Add(ValidationError.CreateValidationError<ItemProductId>(
                        pi => pi.ProductId, ValidationStrings.ProductIdProductIdNotUnique));
                }
            }
            else
            {
                validationErrors.Add(ValidationError.CreateValidationError<ItemProductId>(
                        pi => pi.ProductId, ValidationStrings.ProductIdProductIdNotFound));
            }

            if (validationErrors.Count > 0)
            {
                throw new ValidationException(
                    string.Format(CultureInfo.CurrentCulture,
                        ServiceResources.Exception_Validation,
                        typeof(ItemProductId).Name),
                    validationErrors);
            }
        }

        #endregion
    }
}
