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
    public class ItemScanCodeManager : IItemScanCodeManager
    {
        #region Constructors

        public ItemScanCodeManager()
        {
            
        }

        #endregion

        #region IItemManager Members

        public ItemScanCode GetMedItemScanCode(string scancode, Guid facilityKey)
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

                Guid? itemScanCodeKey = (
                    from i in medItemQuery
                    join isc in repository.GetQueryableEntity<ItemScanCodeEntity>()
                        on i.Key equals isc.ItemKey
                    where isc.ScanCodeValue == scancode
                    select (Guid?)isc.Key).SingleOrDefault();

                ItemScanCode iScanCode = null;
                if (itemScanCodeKey != null)
                {
                    iScanCode = repository.GetItemScanCode(itemScanCodeKey.Value);
                }
                 
                return iScanCode;
            }
        }
        public ItemScanCode GetMedItemScanCode(Guid itemKey, Guid facilityKey)
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

                Guid? itemScanCodeKey = (
                    from i in medItemQuery
                    join isc in repository.GetQueryableEntity<ItemScanCodeEntity>()
                        on i.Key equals isc.ItemKey
                    where isc.ItemKey == itemKey
                    select (Guid?)isc.Key).SingleOrDefault();

                ItemScanCode iScanCode = null;
                if (itemScanCodeKey != null)
                {
                    iScanCode = repository.GetItemScanCode(itemScanCodeKey.Value);
                }

                return iScanCode;
            }
        }

        public void LinkItemScanCode(Context context, string scancode, Guid itemKey, string createdByExternalSystemName = null)
        {
            Guard.ArgumentNotNull(context, "context");

            using (IItemRepository repository = RepositoryFactory.Create<IItemRepository>())
            {
                ItemScanCode itemScanCode = new ItemScanCode
                {
                    Item = itemKey,
                    ScanCode = scancode,
                    LinkedByUserAccountKey = (context.Actor is UserActor) ? context.Actor.Key : default(Guid?),
                    LinkedDateTime = context.ActionDateTime,
                    LinkedUtcDateTime = context.ActionUtcDateTime,
                    FromExternalSystem = createdByExternalSystemName != null,
                    CreatedByExternalSystemName = createdByExternalSystemName
                };

                // Validate
                ValidateItemScanCode(repository, itemScanCode);

                // Insert
                repository.InsertItemScanCode(context, itemScanCode);
            }
        }

        public void LinkAndVerifyItemScanCode(Context context, string scancode, Guid itemKey, string createdByExternalSystemName = null)
        {
            Guard.ArgumentNotNull(context, "context");

            using (IItemRepository repository = RepositoryFactory.Create<IItemRepository>())
            {
                ItemScanCode itemScanCode = new ItemScanCode
                {
                    Item = itemKey,
                    ScanCode = scancode,
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
                ValidateItemScanCode(repository, itemScanCode);

                // Insert
                repository.InsertItemScanCode(context, itemScanCode);
            }
        }

        public void RelinkScanCode(Context context, Guid itemScanCodeKey, Guid itemKey)
        {
            Guard.ArgumentNotNull(context, "context");

            using (IItemRepository repository = RepositoryFactory.Create<IItemRepository>())
            {
                ItemScanCode itemScanCode = repository.GetItemScanCode(itemScanCodeKey);
                if (itemScanCode == null)
                    throw new EntityNotFoundException();

                itemScanCode.Item = itemKey;
                itemScanCode.LinkedByUserAccountKey = (context.Actor is UserActor) ? context.Actor.Key : default(Guid?);
                itemScanCode.LinkedDateTime = context.ActionDateTime;
                itemScanCode.LinkedUtcDateTime = context.ActionUtcDateTime;
                itemScanCode.VerifiedByUserAccountKey = null;
                itemScanCode.VerifiedDateTime = null;
                itemScanCode.VerifiedUtcDateTime = null;
                itemScanCode.FromExternalSystem = false;

                // Validate
                ValidateItemScanCode(repository, itemScanCode);

                // Update
                repository.DeleteItemScanCode(context, itemScanCodeKey);
                repository.InsertItemScanCode(context, itemScanCode);
            }
        }

        public void VerifyScanCode(Context context, Guid itemScanCodeKey)
        {
            Guard.ArgumentNotNull(context, "context");

            using (IItemRepository repository = RepositoryFactory.Create<IItemRepository>())
            {
                ItemScanCode itemScanCode = repository.GetItemScanCode(itemScanCodeKey);
                if (itemScanCode == null)
                    throw new EntityNotFoundException();

                // Set verification
                itemScanCode.VerifiedByUserAccountKey = (Guid?)context.Actor;
                itemScanCode.VerifiedDateTime = context.ActionDateTime;
                itemScanCode.VerifiedUtcDateTime = context.ActionUtcDateTime;

                // Update
                repository.UpdateItemScanCode(context, itemScanCode);
            }
        }

        public void DeleteItemScanCode(Context context, Guid itemScanCodeKey, ScanProductDeleteReasonInternalCode? scanProductDeleteReason, string deletedByExternalSystemName = null)
        {
            Guard.ArgumentNotNull(context, "context");

            using (IItemRepository repository = RepositoryFactory.Create<IItemRepository>())
            {
                repository.DeleteItemScanCode(context, itemScanCodeKey, scanProductDeleteReason, deletedByExternalSystemName: deletedByExternalSystemName);
            }
        }

        #endregion

        #region Private Members

        private static void ValidateItemScanCode(IRepository repository, ItemScanCode itemScanCode)
        {
            List<ValidationError> validationErrors = new List<ValidationError>();

            // Validate contract
            var validationResults = MsValidation.Validate(itemScanCode);
            if (!validationResults.IsValid)
                validationErrors.AddRange(validationResults.ToValidationErrorsArray());

            var itemInfo = repository.GetQueryableEntity<ItemEntity>()
                .Where(i => i.Key == itemScanCode.Item.Key)
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

                var itemScanCodeQuery = (from isc in repository.GetQueryableEntity<ItemScanCodeEntity>()
                                         join i in itemQuery
                                             on isc.ItemKey equals i.Key
                                         where isc.ScanCodeValue == itemScanCode.ScanCode
                                         select isc);

                if (!itemScanCode.IsTransient())
                {
                    // If item scan code is not transient then don't include itself.
                    itemScanCodeQuery = itemScanCodeQuery.Where(isc => isc.Key != itemScanCode.Key);
                }

                // If already exists then its an error.
                if (itemScanCodeQuery.Count() > 0)
                {
                    validationErrors.Add(ValidationError.CreateValidationError<ItemScanCode>(
                        isc => isc.ScanCode, ValidationStrings.ItemScanCodeScanCodeNotUnique));
                }
            }
            else
            {
                // Move to resource file.
                validationErrors.Add(ValidationError.CreateValidationError<ItemScanCode>(
                        isc => isc.Item, ValidationStrings.ItemScanCodeScanCodeItemNotFound));
            }

            if (validationErrors.Count > 0)
            {
                throw new ValidationException(
                    string.Format(CultureInfo.CurrentCulture,
                        ServiceResources.Exception_Validation,
                        typeof(ItemScanCode).Name),
                    validationErrors);
            }
        }
       
        #endregion
    }
}
