using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Data.Models;
using CareFusion.Dispensing.Models;
using Dapper;
using Pyxis.Core.Data;
using Pyxis.Core.Data.InternalCodes;
using Pyxis.Core.Data.Schema.CDCat;
using Pyxis.Core.Data.Schema.TableTypes;
using ItemDAL = Pyxis.Core.Data.Schema.Item;

namespace CareFusion.Dispensing.Data.Repositories
{
    /// <summary>
    /// Represents an <see cref="IItemRepository"/> managed by Linq.
    /// </summary>
    internal class ItemRepository : LinqBaseRepository, IItemRepository
    {
        private readonly ItemDAL.FacilityItemRepository _facilityItemRepo;
        private readonly ItemDAL.DosageFormRepository _dosageFormRepo;
        private readonly ItemDAL.EquivalencyDosageFormGroupRepository _equivalencyDosageFormRepo;
        private readonly ItemDAL.HazardousWasteClassRepository _hazardousWasteClassRepo;
        private readonly ItemDAL.SecurityGroupRepository _securityGroupRepo;
        private readonly ItemDAL.OverrideGroupRepository _overrideGroupRepo;
        private readonly ItemDAL.TherapeuticClassRepository _therapeuticClassRepo;
        private readonly ItemDAL.PickAreaRepository _pickAreaRepo;
        private readonly ItemDAL.ItemRepository _itemRepo;
        private readonly ItemDAL.FormularyTemplateRepository _formularyTemplateRepo;
        private readonly ClinicalDataSubjectRepository _clinicalDataSubjectRepo;

        public ItemRepository()
        {
            _itemRepo = new ItemDAL.ItemRepository();
            _facilityItemRepo = new ItemDAL.FacilityItemRepository();
            _dosageFormRepo = new ItemDAL.DosageFormRepository();
            _equivalencyDosageFormRepo = new ItemDAL.EquivalencyDosageFormGroupRepository();
            _hazardousWasteClassRepo = new ItemDAL.HazardousWasteClassRepository();
            _securityGroupRepo = new ItemDAL.SecurityGroupRepository();
            _overrideGroupRepo = new ItemDAL.OverrideGroupRepository();
            _therapeuticClassRepo = new ItemDAL.TherapeuticClassRepository();
            _pickAreaRepo = new ItemDAL.PickAreaRepository();
            _formularyTemplateRepo = new ItemDAL.FormularyTemplateRepository();
            _clinicalDataSubjectRepo = new ClinicalDataSubjectRepository();
        }

        #region Item Members
       
        IEnumerable<Item> IItemRepository.GetItems(IEnumerable<Guid> itemKeys, bool? deleteFlag, Guid? externalSystemKey, string itemId)
        {
            List<Item> items = new List<Item>();
            if (itemKeys != null && !itemKeys.Any())
                return items; // Empty results

            try
            {
                GuidKeyTable selectedKeys = new GuidKeyTable();
                if (itemKeys != null)
                    selectedKeys = new GuidKeyTable(itemKeys.Distinct());

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var multi = connectionScope.QueryMultiple(
                    "Item.bsp_GetItems",
                    new
                    {
                        SelectedKeys = selectedKeys.AsTableValuedParameter(),
                        ExternalSystemKey = externalSystemKey,
                        ItemID = itemId,
                        DeleteFlag = deleteFlag
                    },
                    commandTimeout: connectionScope.DefaultCommandTimeout,
                    commandType: CommandType.StoredProcedure);
                    var itemResults = multi.Read<ItemsResult>();
                    var equivalencySetResults = multi.Read<Models.ItemEquivalencySetResult>();
                    var equivalencyResults = multi.Read<Models.ItemEquivalencyResult>();
                    var therapeuticClassResults = multi.Read<Models.ItemTherapeuticClassResult>();
                    var variableDoseGroupMemberResults = multi.Read<Models.ItemVariableDoseGroupMemberResult>();

                    // Create item equivalency sets.
                    IEnumerable<ItemEquivalencySet> itemEquivalencySets = CreateItemEquivalencySets(
                        equivalencySetResults, equivalencyResults);

                    foreach (var itemResult in itemResults)
                    {
                        Item item;
                        if (itemResult.BusinessDomainInternalCode == BusinessDomainInternalCode.MED.ToInternalCode())
                        {
                            MedItem medItem = new MedItem(itemResult.MedItemKey, itemResult.ItemSnapshotKey,
                                               itemResult.MedItemSnapshotKey, itemResult.MedDisplayName);

                            medItem.GenericName = itemResult.GenericName;
                            medItem.SearchGenericName = itemResult.SearchGenericName;
                            medItem.PureGenericName = itemResult.PureGenericName;
                            medItem.BrandName = itemResult.BrandName;
                            medItem.SearchBrandName = itemResult.SearchBrandName;
                            medItem.Strength = itemResult.StrengthText;
                            medItem.StrengthAmount = itemResult.StrengthAmount;
                            medItem.StrengthUnitOfMeasure = itemResult.StrengthUOMKey != null
                                ? new UnitOfMeasure(itemResult.StrengthUOMKey.Value)
                                    {
                                        BaseUnitOfMeasureKey = itemResult.StrengthBaseUOMKey,
                                        DisplayCode = itemResult.StrengthUOMDisplayCode,
                                        Description = itemResult.StrengthUOMDescriptionText,
                                        UseDosageForm = itemResult.StrengthUOMUseDosageForm.GetValueOrDefault(),
                                        Conversion = itemResult.StrengthUOMConversionAmount
                                    } : default(UnitOfMeasure);
                            medItem.StrengthExternalUnitOfMeasureKey = itemResult.StrengthExternalUOMKey;
                            medItem.ConcentrationVolumeAmount = itemResult.ConcentrationVolumeAmount;
                            medItem.ConcentrationVolumeUnitOfMeasure = itemResult.ConcentrationVolumeUOMKey != null
                                ? new UnitOfMeasure(itemResult.ConcentrationVolumeUOMKey.Value)
                                    {
                                        BaseUnitOfMeasureKey = itemResult.ConcentrationVolumeBaseUOMKey,
                                        DisplayCode = itemResult.ConcentrationVolumeUOMDisplayCode,
                                        Description = itemResult.ConcentrationVolumeUOMDescriptionText,
                                        UseDosageForm = itemResult.ConcentrationVolumeUOMUseDosageForm.GetValueOrDefault(),
                                        Conversion = itemResult.ConcentrationVolumeUOMConversionAmount
                                    } : default(UnitOfMeasure);
                            medItem.ConcentrationVolumeExternalUnitOfMeasureKey = itemResult.ConcentrationVolumeExternalUOMKey;
                            medItem.TotalVolumeAmount = itemResult.TotalVolumeAmount;
                            medItem.TotalVolumeUnitOfMeasure = itemResult.TotalVolumeUOMKey != null
                                ? new UnitOfMeasure(itemResult.TotalVolumeUOMKey.Value)
                                    {
                                        BaseUnitOfMeasureKey = itemResult.TotalVolumeBaseUOMKey,
                                        DisplayCode = itemResult.TotalVolumeUOMDisplayCode,
                                        Description = itemResult.TotalVolumeUOMDescriptionText,
                                        UseDosageForm = itemResult.TotalVolumeUOMUseDosageForm.GetValueOrDefault(),
                                        Conversion = itemResult.TotalVolumeUOMConversionAmount
                                    } : default(UnitOfMeasure);
                            medItem.TotalVolumeExternalUnitOfMeasureKey = itemResult.TotalVolumeExternalUOMKey;
                            medItem.DosageForm = itemResult.DosageFormKey != null
                                ? new DosageForm(itemResult.DosageFormKey.Value)
                                    {
                                        Code = itemResult.DosageFormCode,
                                        Description = itemResult.DosageFormDescriptionText
                                    } : default(DosageForm);
                            medItem.MedItemType = itemResult.MedItemTypeInternalCode.FromNullableInternalCode<MedItemTypeInternalCode>();
                            medItem.MinimumDoseAmount = itemResult.MinimumDoseAmount;
                            medItem.MaximumDoseAmount = itemResult.MaximumDoseAmount;
                            medItem.DoseUnitOfMeasure = itemResult.DoseUOMKey;
                            medItem.MedicationClass = itemResult.MedClassKey != null
                                ? new MedicationClass(itemResult.MedClassKey.Value)
                                      {
                                          Code = itemResult.MedClassCode,
                                          IsControlled = itemResult.MedClassControlledFlag.GetValueOrDefault()
                                      } : default(MedicationClass);

                            medItem.TherapeuticClasses = therapeuticClassResults
                                .Where(tc => tc.MedItemKey == itemResult.MedItemKey)
                                .Select(tc => new TherapeuticClass(tc.TherapeuticClassKey)
                                    {
                                        Code = tc.TherapeuticClassCode,
                                        Description = tc.TherapeuticClassDescriptionText
                                    })
                                .ToArray();

                            medItem.VariableDoseGroupMembers = variableDoseGroupMemberResults
                                .Where(vdg => vdg.VariableDoseGroupKey == itemResult.MedItemKey)
                                .Select(vdg => new VariableDoseGroupMember(vdg.VariableDoseGroupMemberKey)
                                                   {
                                                       MedItemKey = vdg.MedItemKey,
                                                       ItemId = vdg.ItemID,
                                                       DisplayName = vdg.MedDisplayName,
                                                       PureGenericName = vdg.PureGenericName,
                                                       BrandName = vdg.BrandName,
                                                       DosageFormKey = vdg.DosageFormKey,
                                                       DosageFormCode = vdg.DosageFormCode,
                                                       EquivalencyDosageFormGroupKey = vdg.EquivalencyDosageFormGroupKey,
                                                       EquivalencyDosageFormGroupDisplayCode = vdg.EquivalencyDosageFormGroupDisplayCode,
                                                       Rank = vdg.RankValue,
                                                       LastModified = vdg.LastModifiedBinaryValue.ToArray()
                                                   })
                                .ToArray();

                            item = medItem;
                        }
                        else
                        {
                            item = new Item(itemResult.ItemKey, itemResult.ItemSnapshotKey);
                        }

                        item.BusinessDomain = itemResult.BusinessDomainInternalCode.FromInternalCode<BusinessDomainInternalCode>();
                        item.PharmacyInformationSystemKey = itemResult.ExternalSystemKey;
                        item.PharmacyInformationSystemName = itemResult.ExternalSystemName;
                        item.PharmacyInformationSystemProvidesMedicationClass = itemResult.PISProvidesMedClassFlag.GetValueOrDefault();
                        item.PharmacyInformationSystemProvidesPureGenericName = itemResult.PISProvidesPureGenericNameFlag.GetValueOrDefault();
                        item.PharmacyInformationSystemProvidesTherapeuticClass = itemResult.PISProvidesTherapeuticClassFlag.GetValueOrDefault();
                        item.FacilityKey = itemResult.FacilityKey;
                        item.FacilityCode = itemResult.FacilityCode;
                        item.FacilityName = itemResult.FacilityName;
                        item.ItemId = itemResult.ItemID;
                        item.AlternateItemId = itemResult.AlternateItemID;
                        item.ItemName = itemResult.ItemName;
                        item.DescriptionText = itemResult.DescriptionText;
                        item.ItemTypeInternalCode = itemResult.ItemTypeInternalCode;
                        item.ItemSubTypeInternalCode = itemResult.ItemSubTypeInternalCode;
                        item.IsMedItem = itemResult.MedItemFlag;
                        item.ExternalSystemDeleteUtcDateTime = itemResult.ExternalSystemDeleteUTCDateTime;
                        item.ExternalSystemDeleteDateTime = itemResult.ExternalSystemDeleteLocalDateTime;
                        item.CustomField1 = itemResult.CustomField1Text;
                        item.CustomField2 = itemResult.CustomField2Text;
                        item.CustomField3 = itemResult.CustomField3Text;
                        item.EnterpriseItemId = itemResult.EnterpriseItemID;
                        item.IsDeleted = itemResult.DeleteFlag;
                        item.LastModified = itemResult.LastModifiedBinaryValue.ToArray();
                        item.ItemEquivalencySets = itemEquivalencySets
                            .Where(ies => ies.ItemKey == itemResult.ItemKey)
                            .ToArray();

                        items.Add(item);
                    }
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return items;
        }
        
        Item IItemRepository.GetItem(Guid itemKey)
        {
            IEnumerable<Item> items = ((IItemRepository) this).GetItems(new [] {itemKey});

            return items.FirstOrDefault();
        }

        Item IItemRepository.GetItem(Guid externalSystemKey, string itemId)
        {
            IEnumerable<Item> items = ((IItemRepository)this).GetItems(null, false, externalSystemKey, itemId);

            return items.FirstOrDefault();
        }

        Guid? IItemRepository.GetItemKey(Guid externalSystemKey, string itemId)
        {
            Guid? itemKey = null;

            try
            {
                var sql = new SqlBuilder();
                sql.SELECT("ic.ItemKey")
                    .FROM("Item.vw_ItemCurrent ic")
                    .WHERE("ic.ExternalSystemKey = @ExternalSystemKey")
                    ._("ic.ItemId = @ItemId");

                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    itemKey = connectionScope.ExecuteScalar<Guid?>(
                         sql.ToString(),
                         new
                         {
                             ExternalSystemKey = externalSystemKey,
                             ItemId = itemId
                         },
                         commandTimeout: connectionScope.DefaultCommandTimeout,
                         commandType: CommandType.Text);
                }
            }

            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return itemKey;
        }

        Guid? IItemRepository.GetMedItemKey(Guid externalSystemKey, string itemId)
        {
            Guid? medItemKey = null;

            try
            {
                var sql = new SqlBuilder();
                sql.SELECT("mic.MedItemKey")
                    .FROM("Item.vw_ItemCurrent ic")
                    .INNER_JOIN("Item.vw_MedItemCurrent mic ON ic.ItemKey = mic.MedItemKey")
                    .WHERE("ic.ExternalSystemKey = @ExternalSystemKey")
                    ._("ic.ItemId = @ItemId");

                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    medItemKey = connectionScope.ExecuteScalar<Guid?>(
                         sql.ToString(),
                         new
                         {
                             ExternalSystemKey = externalSystemKey,
                             ItemId = itemId
                         },
                         commandTimeout: connectionScope.DefaultCommandTimeout,
                         commandType: CommandType.Text);
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return medItemKey;
        }

        Guid IItemRepository.InsertItem(Context context, Item item)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(item, "item");
            Guid itemKey = Guid.Empty;

            try
            {
                using (var tx = TransactionScopeFactory.Create())
                using (ConnectionScopeFactory.Create())
                {
                    itemKey = _itemRepo.InsertItem(context.ToActionContext(),
                        new ItemDAL.Models.Item
                        {
                            ItemKey = item.Key,
                            BusinessDomainInternalCode = item.BusinessDomain.ToInternalCode(),
                            ExternalSystemKey = item.PharmacyInformationSystemKey,
                            FacilityKey = item.FacilityKey,
                            ItemId = item.ItemId,
                            AlternateItemId = item.AlternateItemId,
                            ItemName = item.ItemName,
                            DescriptionText = item.DescriptionText,
                            ItemTypeInternalCode = item.ItemTypeInternalCode,
                            ItemSubTypeInternalCode = item.ItemSubTypeInternalCode,
                            MedItemFlag = item.IsMedItem,
                            ExternalSystemDeleteUtcDateTime = item.ExternalSystemDeleteUtcDateTime,
                            ExternalSystemDeleteLocalDateTime = item.ExternalSystemDeleteDateTime,
                            CustomField1Text = item.CustomField1,
                            CustomField2Text = item.CustomField2,
                            CustomField3Text = item.CustomField3,
                            EnterpriseItemId = item.EnterpriseItemId,
                            LastModifiedBinaryValue = item.LastModified
                        });

                    // insert equivalencies
                    if (item.ItemEquivalencySets != null)
                    {
                        foreach (var itemEquivalencySet in item.ItemEquivalencySets)
                        {
                            if (itemEquivalencySet.Count > 0)
                            {
                                InsertItemEquivalencySet(_itemRepo, context, itemKey, itemEquivalencySet);
                            }
                        }
                    }

                    MedItem medItem = item as MedItem;
                    if (medItem != null)
                        InsertMedItem(_itemRepo, context, itemKey, medItem);

                    tx.Complete();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return itemKey;
        }

        void IItemRepository.UpdateItem(Context context, Item item)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(item, "item");

            try
            {
                using (var tx = TransactionScopeFactory.Create())
                using (ConnectionScopeFactory.Create())
                {
                    _itemRepo.UpdateItem(context.ToActionContext(),
                        new ItemDAL.Models.Item
                        {
                            ItemKey = item.Key,
                            BusinessDomainInternalCode = item.BusinessDomain.ToInternalCode(),
                            ExternalSystemKey = item.PharmacyInformationSystemKey,
                            FacilityKey = item.FacilityKey,
                            ItemId = item.ItemId,
                            AlternateItemId = item.AlternateItemId,
                            ItemName = item.ItemName,
                            DescriptionText = item.DescriptionText,
                            ItemTypeInternalCode = item.ItemTypeInternalCode,
                            ItemSubTypeInternalCode = item.ItemSubTypeInternalCode,
                            MedItemFlag = item.IsMedItem,
                            ExternalSystemDeleteUtcDateTime = item.ExternalSystemDeleteUtcDateTime,
                            ExternalSystemDeleteLocalDateTime = item.ExternalSystemDeleteDateTime,
                            CustomField1Text = item.CustomField1,
                            CustomField2Text = item.CustomField2,
                            CustomField3Text = item.CustomField3,
                            EnterpriseItemId = item.EnterpriseItemId,
                            LastModifiedBinaryValue = item.LastModified
                        });

                    // delete equivalencies
                    if (item.ItemEquivalencySets != null)
                    {
                        // This set works a little bit different than all other sets in the system. Its
                        // possible to update the set itself, not the members of the set.
                        foreach (var itemEquivalencySet in item.ItemEquivalencySets)
                        {
                            if (itemEquivalencySet.IsTransient() &&
                                itemEquivalencySet.Count > 0)
                            {
                                InsertItemEquivalencySet(_itemRepo, context, item.Key, itemEquivalencySet);
                            }
                            else if (!itemEquivalencySet.IsTransient())
                            {
                                if (!itemEquivalencySet.Any())
                                    DeleteItemEquivalencySet(_itemRepo, context, itemEquivalencySet.Key);
                                else
                                    UpdateItemEquivalencySet(_itemRepo, context, itemEquivalencySet);
                            }
                        }
                    }

                    MedItem medItem = item as MedItem;
                    if (medItem != null)
                        UpdateMedItem(_itemRepo, context, medItem);
                    
                    tx.Complete();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void IItemRepository.DeleteItem(Context context, Guid itemKey)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                _itemRepo.DeleteItem(context.ToActionContext(), itemKey);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        #endregion

        #region Station Query Members
        IEnumerable<Guid> IItemRepository.ListPermissibleItemKeys(Guid userKey, Guid deviceKey, Guid facilityKey, PermissionInternalCode permission)
        {
            IEnumerable<Guid> itemKeys = null;
            try
            {
                var parameters = new DynamicParameters(new Dictionary<string, object>{
                        { "@FacilityKey", facilityKey },
                        { "@DispensingDeviceKey", deviceKey },
                        { "@UserAccountKey", userKey },
                        { "@PermissionInternalCode", permission.ToInternalCode() },
                });

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    itemKeys = connectionScope.Query<Guid>(
                      "Item.bsp_ListPermissibleItemKeys",
                      parameters,
                      commandTimeout: connectionScope.DefaultCommandTimeout,
                      commandType: CommandType.StoredProcedure).ToList();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
            return itemKeys;
        }

        IEnumerable<Guid> IItemRepository.ListOverrideItemKeys(Guid userKey, Guid deviceKey, Guid facilityKey)
        {
            IEnumerable<Guid> itemKeys = null;
            try
            {
                var parameters = new DynamicParameters(new Dictionary<string, object>()
                    {
                        { "@FacilityKey", facilityKey },
                        { "@DispensingDeviceKey", deviceKey },
                        { "@UserAccountKey", userKey },
                        { "@PermissionInternalCode", PermissionInternalCode.RemoveMed.ToInternalCode() }
                    });

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    itemKeys = connectionScope.Query<Guid>(
                       "Item.bsp_ListOverrideItemKeys",
                       parameters,
                       commandTimeout: connectionScope.DefaultCommandTimeout,
                       commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
            return itemKeys;
        }

        #endregion

        #region FacilityItem Members
        IEnumerable<FacilityItem> IItemRepository.GetFacilityItems(IEnumerable<Guid> facilityItemKeys, Guid? facilityKey, Guid? itemKey)
        {
            List<FacilityItem> facilityItems = new List<FacilityItem>();
            if (facilityItemKeys != null && !facilityItemKeys.Any())
                return facilityItems; // Empty results

            try
            {
                GuidKeyTable selectedKeys = new GuidKeyTable();
                if (facilityItemKeys != null)
                    selectedKeys = new GuidKeyTable(facilityItemKeys.Distinct());
                using (var connectionSctope = ConnectionScopeFactory.Create())
                {
                    var multi = connectionSctope.QueryMultiple(
                    "Item.bsp_GetFacilityItems",
                    new
                    {
                        SelectedKeys = selectedKeys.AsTableValuedParameter(),
                        FacilityKey = facilityKey,
                        ItemKey = itemKey
                    },
                    commandTimeout: connectionSctope.DefaultCommandTimeout,
                    commandType: CommandType.StoredProcedure);
                    var facilityItemResults = multi.Read<FacilityItemsResult>();
                    var comboResults = multi.Read<Models.FacilityItemComboComponentResult>();
                    var adminRouteResults = multi.Read<Models.ComboComponentAdminRouteResult>();
                    var overrideGroupResults = multi.Read<Models.FacilityItemOverrideGroupResult>();
                    var pickAreaResults = multi.Read<Models.FacilityItemPickAreaResult>();
                    var physicalCapacityResults = multi.Read<Models.FacilityItemPhysicalCapacityResult>();
                    var clinicalDataSubjectResults = multi.Read<Models.FacilityItemClinicalDataSubjectResult>();
                    var blockedItemResults = multi.Read<Models.FacilityItemBlockedDispensingDeviceResult>();

                    // Get our item objects
                    IEnumerable<Guid> itemKeys = facilityItemResults.Select(fi => fi.ItemKey).Distinct();
                    IDictionary<Guid, Item> items = ((IItemRepository) this).GetItems(itemKeys)
                        .ToDictionary(i => i.Key);

                    foreach (var facilityItemResult in facilityItemResults)
                    {
                        FacilityItem facilityItem = new FacilityItem(facilityItemResult.FacilityItemKey,
                                                                     facilityItemResult.FacilityItemSnapshotKey,
                                                                     facilityItemResult.MedDisplayName)
                            {
                                FacilityKey = facilityItemResult.FacilityKey,
                                Item = items.ContainsKey(facilityItemResult.ItemKey) 
                                    ? items[facilityItemResult.ItemKey] : new Item(facilityItemResult.ItemKey),
                                DisplayName = facilityItemResult.ItemDisplayName,
                                AlternateId = facilityItemResult.AlternateItemID,
                                MedicationClass = facilityItemResult.MedClassKey != null
                                ? new MedicationClass(facilityItemResult.MedClassKey.Value)
                                      {
                                          Code = facilityItemResult.MedClassCode,
                                          IsControlled = facilityItemResult.MedClassControlledFlag.GetValueOrDefault()
                                      } : default(MedicationClass),
                                OutdateTracking = facilityItemResult.OutdateTrackingFlag,
                                VerifyCountMode = facilityItemResult.VerifyCountModeInternalCode.FromNullableInternalCode<VerifyCountModeInternalCode>(),
                                GCSMMedReturnModeInternalCode = facilityItemResult.GCSMMedReturnModeInternalCode.FromNullableInternalCode<MedReturnModeInternalCode>(),
                                GCSMVerifyCountModeInternalCode = facilityItemResult.GCSMVerifyCountModeInternalCode.FromNullableInternalCode<VerifyCountModeInternalCode>(),
                                GCSMCountCUBIEEjectModeInternalCode = facilityItemResult.GCSMCountCUBIEEjectModeInternalCode.FromNullableInternalCode<CountCUBIEEjectModeInternalCode>(),
                                VerifyCountAnesthesiaDispensing = facilityItemResult.VerifyCountAnesthesiaDispensingFlag,
                                WitnessOnDispense = facilityItemResult.WitnessOnDispenseFlag,
                                WitnessOnReturn = facilityItemResult.WitnessOnReturnFlag,
                                WitnessOnDispose = facilityItemResult.WitnessOnDisposeFlag,
                                WitnessOnLoadRefill = facilityItemResult.WitnessOnLoadRefillFlag,
                                WitnessOnUnload = facilityItemResult.WitnessOnUnloadFlag,
                                WitnessOnOverride = facilityItemResult.WitnessOnOverrideFlag,
                                WitnessOnOutdate = facilityItemResult.WitnessOnOutdateFlag,
                                WitnessOnInventory = facilityItemResult.WitnessOnInventoryFlag,
                                WitnessOnEmptyReturnBin = facilityItemResult.WitnessOnEmptyReturnBinFlag,
                                WitnessOnDestock = facilityItemResult.WitnessOnDestockFlag,
                                WitnessOnRxCheck = facilityItemResult.WitnessOnRxCheckFlag,
                                ScanOnLoadRefill = facilityItemResult.ScanOnLoadRefillFlag,
                                ScanOnRemove = facilityItemResult.ScanOnRemoveFlag,
                                DoOnceOnRemove = facilityItemResult.DoOnceOnRemoveFlag,
                                ScanOnReturn = facilityItemResult.ScanOnReturnFlag,
                                ScanOnCheck = facilityItemResult.ScanOnCheckFlag,
                                RequireLotIdOnRemove = facilityItemResult.RequireLotIDOnRemoveFlag,
                                RequireLotIdOnReturn = facilityItemResult.RequireLotIDOnReturnFlag,
                                RequireSerialIdOnRemove = facilityItemResult.RequireSerialIDOnRemoveFlag,
                                RequireSerialIdOnReturn = facilityItemResult.RequireSerialIDOnReturnFlag,
                                RequireExpirationDateOnRemove = facilityItemResult.RequireExpirationDateOnRemoveFlag,
                                RequireExpirationDateOnReturn = facilityItemResult.RequireExpirationDateOnReturnFlag,
                                RequireInventoryReferenceId = facilityItemResult.RequireInventoryReferenceIDFlag,
                                Reverification = facilityItemResult.ReverificationFlag,
                                GCSMOutdateTrackingFlag = facilityItemResult.GCSMOutdateTrackingFlag,
                                GCSMRequireLotNumberWhenRecallFlag = facilityItemResult.GCSMRequireLotNumberWhenRecallFlag,
                                GCSMRequireInventoryReferenceNumberFlag = facilityItemResult.GCSMRequireInventoryReferenceNumberFlag,
                                GCSMWitnessOnAccessToDestructionBinFlag = facilityItemResult.GCSMWitnessOnAccessToDestructionBinFlag,
                                GCSMWitnessOnAddToDestructionBinFlag = facilityItemResult.GCSMWitnessOnAddToDestructionBinFlag,
                                GCSMWitnessOnOutdateFlag = facilityItemResult.GCSMWitnessOnOutdateFlag,
                                GCSMWitnessOnReturnFlag = facilityItemResult.GCSMWitnessOnReturnFlag,
                                GCSMWitnessOnAutorestockFlag = facilityItemResult.GCSMWitnessOnAutorestockFlag,
                                GCSMWitnessOnCompoundingFlag = facilityItemResult.GCSMWitnessOnCompoundingFlag,
                                GCSMWitnessOnDestructionBinFlag = facilityItemResult.GCSMWitnessOnDestructionBinFlag,
                                GCSMWitnessOnDiscrepancyResolutionFlag = facilityItemResult.GCSMWitnessOnDiscrepancyResolutionFlag,
                                GCSMWitnessOnInventoryCountFlag = facilityItemResult.GCSMWitnessOnInventoryCountFlag,
                                GCSMWitnessOnIssueFlag = facilityItemResult.GCSMWitnessOnIssueFlag,
                                GCSMWitnessOnPrescriptionFlag = facilityItemResult.GCSMWitnessOnPrescriptionFlag,
                                GCSMWitnessOnRecallFlag = facilityItemResult.GCSMWitnessOnRecallFlag,
                                GCSMWitnessOnReceiveFlag = facilityItemResult.GCSMWitnessOnReceiveFlag,
                                GCSMWitnessOnReverseCompoundingFlag = facilityItemResult.GCSMWitnessOnReverseCompoundingFlag,
                                GCSMWitnessOnSellFlag = facilityItemResult.GCSMWitnessOnSellFlag,
                                GCSMWitnessOnStockTransferFlag = facilityItemResult.GCSMWitnessOnStockTransferFlag,
                                GCSMWitnessOnWasteFlag = facilityItemResult.GCSMWitnessOnWasteFlag,
                                GCSMWitnessOnUnloadFlag =facilityItemResult.GCSMWitnessOnUnloadFlag,
                                GCSMScanOnReturnFlag = facilityItemResult.GCSMScanOnReturnFlag,
                                GCSMScanOnAutorestockFlag = facilityItemResult.GCSMScanOnAutorestockFlag,
                                GCSMScanOnIssueFlag = facilityItemResult.GCSMScanOnIssueFlag,
                                GCSMScanOnPrescriptionFlag = facilityItemResult.GCSMScanOnPrescriptionFlag,
                                GCSMScanOnReceiveFlag = facilityItemResult.GCSMScanOnReceiveFlag,
                                GCSMScanOnSellFlag = facilityItemResult.GCSMScanOnSellFlag,
                                GCSMScanOnStockTransferFlag = facilityItemResult.GCSMScanOnStockTransferFlag,
                                GCSMScanOnCompoundingFlag = facilityItemResult.GCSMScanOnCompoundingFlag,
                                GCSMCriticalLowPercentage = facilityItemResult.GCSMCriticalLowPercentage,
                                GCSMPrintOnReceiveFlag = facilityItemResult.GCSMPrintOnReceiveFlag,
                                GCSMPrintSingleMedSheetFlag=facilityItemResult.GCSMPrintSingleMedSheetFlag,
                                GCSMPrintDripSheetFlag=facilityItemResult.GCSMPrintDripSheetFlag,
                                GCSMPrintMaximumRowsFlag=facilityItemResult.GCSMPrintMaximumRowsFlag,
                                GCSMAdditionalLabelsPrintedQuantity=facilityItemResult.GCSMAdditionalLabelsPrintedQuantity,
                                GCSMStockOutNoticeFlag = facilityItemResult.GCSMStockOutNoticeFlag,
                                GCSMADMDispenseQuantity = facilityItemResult.GCSMADMDispenseQuantity,
                                GCSMDistributorPackageSizeQuantity=facilityItemResult.GCSMDistributorPackageSizeQuantity,
                                GCSMTotalDeviceParDurationAmount = facilityItemResult.GCSMTotalDeviceParDurationAmount,
                                GCSMTotalParDurationAmount = facilityItemResult.GCSMTotalParDurationAmount,
                                GCSMDistributorKey = facilityItemResult.GCSMDistributorKey,
                                GCSMPreferredProductIDKey = facilityItemResult.GCSMPreferredProductIDKey,
                                GCSMVendorItemCode = facilityItemResult.GCSMVendorItemCode,
                                GCSMRequireOriginDestinationFlag = facilityItemResult.GCSMRequireOriginDestinationFlag,
                                TooCloseRemoveDuration = facilityItemResult.TooCloseRemoveDurationAmount,
                                SecurityGroupKey = facilityItemResult.SecurityGroupKey,
                                IsActive = facilityItemResult.ActiveFlag,
                                AutoResolveDiscrepancy = facilityItemResult.AutoResolveDiscrepancyFlag,
                                IsChargeable = facilityItemResult.ChargeableFlag,
                                IsHighCost = facilityItemResult.HighCostFlag,
                                AllowSplitting = facilityItemResult.AllowSplittingFlag,
                                IsHighRisk = facilityItemResult.HighRiskFlag,
                                HazardousWasteClassKey = facilityItemResult.HazardousWasteClassKey,
                                TrackInventoryQuantity = facilityItemResult.TrackInventoryQuantityFlag,
                                IsMultiDose = facilityItemResult.MultiDoseFlag,
                                IsBackOrdered = facilityItemResult.BackorderedFlag,
                                RefillUnitOfMeasure = facilityItemResult.RefillUOMKey != null
                                    ? new UnitOfMeasure(facilityItemResult.RefillUOMKey.Value)
                                        {
                                            BaseUnitOfMeasureKey = facilityItemResult.RefillBaseUOMKey,
                                            DisplayCode = facilityItemResult.RefillUOMDisplayCode,
                                            Description = facilityItemResult.RefillUOMDescriptionText,
                                            UseDosageForm = facilityItemResult.RefillUOMUseDosageForm.GetValueOrDefault(),
                                            Conversion = facilityItemResult.RefillUOMConversionAmount
                                        } : default(UnitOfMeasure),
                                IssueUnitOfMeasure = facilityItemResult.IssueUOMKey != null
                                    ? new UnitOfMeasure(facilityItemResult.IssueUOMKey.Value)
                                    {
                                        BaseUnitOfMeasureKey = facilityItemResult.IssueBaseUOMKey,
                                        DisplayCode = facilityItemResult.IssueUOMDisplayCode,
                                        Description = facilityItemResult.IssueUOMDescriptionText,
                                        UseDosageForm = facilityItemResult.IssueUOMUseDosageForm.GetValueOrDefault(),
                                        Conversion = facilityItemResult.IssueUOMConversionAmount
                                    } : default(UnitOfMeasure),
                                UnitsOfIssuePerUnitOfRefill = facilityItemResult.UOIPerUORAmount,
                                IsStandardStockWithinDispensingDevice = facilityItemResult.StandardStockFlag,
                                MedReturnMode = facilityItemResult.MedReturnModeInternalCode.FromInternalCode<MedReturnModeInternalCode>(),
                                MedFailoverReturnMode = facilityItemResult.MedFailoverReturnModeInternalCode.FromNullableInternalCode<MedFailoverReturnModeInternalCode>(),
                                ReplenishmentScanMode = facilityItemResult.ReplenishmentScanModeInternalCode.FromNullableInternalCode<ReplenishmentScanModeInternalCode>(),
                                ItemTypeInternalCode = facilityItemResult.ItemTypeInternalCode.FromNullableInternalCode<ItemTypeInternalCode>(),
                                ItemSubTypeInternalCode = facilityItemResult.ItemSubTypeInternalCode.FromNullableInternalCode<ItemSubTypeInternalCode>(),
                                FractionalUnitOfMeasureType = facilityItemResult.FractionalUOMTypeInternalCode.FromNullableInternalCode<FractionalUOMTypeInternalCode>(),
                                AutoMedLabelMode = facilityItemResult.AutoMedLabelModeInternalCode.FromNullableInternalCode<AutoMedLabelModeInternalCode>(),
                                PharmacyNotes = facilityItemResult.PharmacyNotesText,
                                NursingNotes = facilityItemResult.NursingNotesText,
                                CriticalLowPercentage = facilityItemResult.CriticalLowPercentage,
                                StockOutNotice = facilityItemResult.StockOutNoticeFlag,
                                OmnlNotice = facilityItemResult.OMNLNoticeFlag,
                                AnesthesiaMyItems = facilityItemResult.AnesthesiaMyItemsFlag,
                                ResolveUndocumentedWaste = facilityItemResult.ResolveUndocumentedWasteFlag,
                                IsCombo = facilityItemResult.ComboFlag,
                                DispenseComponentsOnly = facilityItemResult.DispenseComponentsOnlyFlag,
                                ChargeCombo = facilityItemResult.ChargeComboFlag,
                                DisplayCalculationOnDispense = facilityItemResult.DisplayCalculationOnDispenseFlag,
                                ReplenishmentPickAreaKey = facilityItemResult.ReplenishmentPickAreaKey,
                                DistributorKey = facilityItemResult.DistributorKey,
                                RxCheck = facilityItemResult.RxCheckFlag,
                                PrintOnRemove = facilityItemResult.PrintOnRemoveFlag,
                                PrintOnReturn = facilityItemResult.PrintOnReturnFlag,
                                PrintOnDispose = facilityItemResult.PrintOnDisposeFlag,
                                PrintOnLoadRefill = facilityItemResult.PrintOnLoadRefillFlag,
                                ShowConversionOnRemove = facilityItemResult.ShowConversionOnRemoveFlag,
                                ScanAllOnPick = facilityItemResult.ScanAllOnPickFlag,
                                CountCubieEjectMode = facilityItemResult.CountCUBIEEjectModeInternalCode.FromNullableInternalCode<CountCUBIEEjectModeInternalCode>(),
                                PharmacyOrderDispenseQuantityFlag = facilityItemResult.PharmacyOrderDispenseQuantityFlag,
                                InjectableFlag = facilityItemResult.InjectableFlag,
                                LastModified = facilityItemResult.LastModifiedBinaryValue.ToArray()
                            };

                        // Combo Meds
                        facilityItem.ComboComponents = comboResults
                            .Where(cc => cc.ComboFacilityItemKey == facilityItemResult.FacilityItemKey)
                            .Select(cc => new ComboComponent(cc.ComboComponentKey)
                                {
                                    FacilityItemKey = cc.ComponentFacilityItemKey,
                                    DisplayName = cc.ComponentMedDisplayName,
                                    Quantity = cc.ComponentQuantity,
                                    Charge = cc.ChargeComponentFlag,
                                    Multiplier = cc.MultiplierFlag,
                                    AdministrationRoutes = adminRouteResults
                                        .Where(ar => ar.ComboComponentKey == cc.ComboComponentKey)
                                        .Select(ar => ar.AdminRouteKey)
                                        .ToArray(),
                                    LastModified = cc.LastModifiedBinaryValue.ToArray()
                                })
                            .ToArray();

                        // Override Groups
                        facilityItem.OverrideGroups = overrideGroupResults
                            .Where(og => og.FacilityItemKey == facilityItemResult.FacilityItemKey)
                            .Select(og => new OverrideGroup(og.OverrideGroupKey)
                                {
                                    DisplayCode = og.DisplayCode,
                                    Description = og.DescriptionText,
                                    IsActive = og.ActiveFlag
                                })
                            .ToArray();

                        // Pick Areas
                        facilityItem.PickAreas = pickAreaResults
                            .Where(pa => pa.FacilityItemKey == facilityItemResult.FacilityItemKey)
                            .Select(pa => new PickArea(pa.PickAreaKey)
                                {
                                    Name = pa.PickAreaName
                                })
                            .ToArray();

                        // Physical Capacities
                        facilityItem.PhysicalCapacities = physicalCapacityResults
                            .Where(pc => pc.FacilityItemKey == facilityItemResult.FacilityItemKey)
                            .Select(pc => new PhysicalCapacity(pc.FacilityItemPhysicalCapacityKey)
                                {
                                    StorageSpaceSize = new StorageSpaceSize(pc.StorageSpaceSizeInternalCode, pc.StorageSpaceSizeDescriptionText)
                                        {
                                            DisplayCode = pc.StorageSpaceSizeDisplayCode
                                        },
                                    MaximumQuantity = pc.MaximumQuantity,
                                    PhysicalMaximumQuantity = pc.PhysicalMaximumQuantity,
                                    ParQuantity = pc.ParQuantity,
                                    RefillPointQuantity = pc.RefillPointQuantity,
                                    LastModified = pc.LastModifiedBinaryValue.ToArray()
                                })
                            .ToArray();

                        // Override clinical data subjects
                        facilityItem.OverrideClinicalDataSubjects = clinicalDataSubjectResults
                            .Where(cdsfif => cdsfif.FacilityItemKey == facilityItemResult.FacilityItemKey &&
                                             cdsfif.PatientCareFunctionInternalCode == PatientCareFunctionInternalCode.OVERRIDE.ToInternalCode())
                            .Select(cdsfif => cdsfif.ClinicalDataSubjectKey)
                            .ToArray();

                        // Remove clinical data subjects
                        facilityItem.RemoveClinicalDataSubjects = clinicalDataSubjectResults
                            .Where(cdsfif => cdsfif.FacilityItemKey == facilityItemResult.FacilityItemKey &&
                                             cdsfif.PatientCareFunctionInternalCode == PatientCareFunctionInternalCode.REMOVE.ToInternalCode())
                            .Select(cdsfif => cdsfif.ClinicalDataSubjectKey)
                            .ToArray();

                        // Return clinical data subjects
                        facilityItem.ReturnClinicalDataSubjects = clinicalDataSubjectResults
                            .Where(cdsfif => cdsfif.FacilityItemKey == facilityItemResult.FacilityItemKey &&
                                             cdsfif.PatientCareFunctionInternalCode == PatientCareFunctionInternalCode.RETURN.ToInternalCode())
                            .Select(cdsfif => cdsfif.ClinicalDataSubjectKey)
                            .ToArray();

                        // Waste clinical data subjects
                        facilityItem.WasteClinicalDataSubjects = clinicalDataSubjectResults
                            .Where(cdsfif => cdsfif.FacilityItemKey == facilityItemResult.FacilityItemKey &&
                                             cdsfif.PatientCareFunctionInternalCode == PatientCareFunctionInternalCode.WASTE.ToInternalCode())
                            .Select(cdsfif => cdsfif.ClinicalDataSubjectKey)
                            .ToArray();

                        // Blocked dispensing devices
                        facilityItem.BlockedDispensingDevices = blockedItemResults
                            .Where(bdd => bdd.FacilityItemKey == facilityItemResult.FacilityItemKey)
                            .Select(bdd => bdd.DispensingDeviceKey)
                            .ToArray();

                        facilityItems.Add(facilityItem);
                    }
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return facilityItems;
        }

        FacilityItem IItemRepository.GetFacilityItem(Guid facilityItemKey)
        {
            IEnumerable<FacilityItem> facilityItems =
                ((IItemRepository) this).GetFacilityItems(new [] {facilityItemKey});

            return facilityItems.FirstOrDefault();
        }

        FacilityItem IItemRepository.GetFacilityItem(Guid facilityKey, Guid itemKey)
        {
            IEnumerable<FacilityItem> facilityItems =
                ((IItemRepository)this).GetFacilityItems(null, facilityKey, itemKey);

            return facilityItems.FirstOrDefault();
        }

        Guid IItemRepository.InsertFacilityItem(Context context, FacilityItem facilityItem)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(facilityItem, "facilityItem");
            Guid? facilityItemKey = null;

            try
            {
                using (ITransactionScope tx = TransactionScopeFactory.Create())
                {
                    var model = new ItemDAL.Models.FacilityItem()
                    {
                        FacilityKey = facilityItem.FacilityKey,
                        ItemKey = facilityItem.Item != null ? facilityItem.Item.Key : default(Guid),
                        ItemDisplayName = facilityItem.DisplayName,
                        AlternateItemId = facilityItem.AlternateId,
                        MedClassKey = facilityItem.MedicationClass != null ? facilityItem.MedicationClass.Key : default(Guid?),
                        OutdateTrackingFlag = facilityItem.OutdateTracking,
                        VerifyCountModeInternalCode = facilityItem.VerifyCountMode.ToInternalCode(),
                        VerifyCountAnesthesiaDispensingFlag = facilityItem.VerifyCountAnesthesiaDispensing,
                        WitnessOnDispenseFlag = facilityItem.WitnessOnDispense,
                        WitnessOnReturnFlag = facilityItem.WitnessOnReturn,
                        WitnessOnDisposeFlag = facilityItem.WitnessOnDispose,
                        WitnessOnLoadRefillFlag = facilityItem.WitnessOnLoadRefill,
                        WitnessOnUnloadFlag = facilityItem.WitnessOnUnload,
                        WitnessOnOverrideFlag = facilityItem.WitnessOnOverride,
                        WitnessOnOutdateFlag = facilityItem.WitnessOnOutdate,
                        WitnessOnInventoryFlag = facilityItem.WitnessOnInventory,
                        WitnessOnEmptyReturnBinFlag = facilityItem.WitnessOnEmptyReturnBin,
                        WitnessOnDestockFlag = facilityItem.WitnessOnDestock,
                        WitnessOnRxCheckFlag = facilityItem.WitnessOnRxCheck,
                        ScanOnLoadRefillFlag = facilityItem.ScanOnLoadRefill,
                        ScanOnRemoveFlag = facilityItem.ScanOnRemove,
                        ScanOnReturnFlag = facilityItem.ScanOnReturn,
                        ScanOnCheckFlag = facilityItem.ScanOnCheck,
                        DoOnceOnRemoveFlag = facilityItem.DoOnceOnRemove,
                        RequireLotIdOnRemoveFlag = facilityItem.RequireLotIdOnRemove,
                        RequireLotIdOnReturnFlag = facilityItem.RequireLotIdOnReturn,
                        RequireSerialIdOnRemoveFlag = facilityItem.RequireSerialIdOnRemove,
                        RequireSerialIdOnReturnFlag = facilityItem.RequireSerialIdOnReturn,
                        RequireExpirationDateOnRemoveFlag = facilityItem.RequireExpirationDateOnRemove,
                        RequireExpirationDateOnReturnFlag = facilityItem.RequireExpirationDateOnReturn,
                        RequireInventoryReferenceIdFlag = facilityItem.RequireInventoryReferenceId,
                        ReverificationFlag = facilityItem.Reverification,
                        TooCloseRemoveDurationAmount = facilityItem.TooCloseRemoveDuration,
                        SecurityGroupKey = facilityItem.SecurityGroupKey,
                        ActiveFlag = facilityItem.IsActive,
                        AutoResolveDiscrepancyFlag = facilityItem.AutoResolveDiscrepancy,
                        ChargeableFlag = facilityItem.IsChargeable,
                        HighCostFlag = facilityItem.IsHighCost,
                        AllowSplittingFlag = facilityItem.AllowSplitting,
                        HighRiskFlag = facilityItem.IsHighRisk,
                        HazardousWasteClassKey = facilityItem.HazardousWasteClassKey,
                        TrackInventoryQuantityFlag = facilityItem.TrackInventoryQuantity,
                        MultiDoseFlag = facilityItem.IsMultiDose,
                        BackorderedFlag = facilityItem.IsBackOrdered,
                        RefillUOMKey = facilityItem.RefillUnitOfMeasure != null ? facilityItem.RefillUnitOfMeasure.Key : default(Guid?),
                        IssueUOMKey = facilityItem.IssueUnitOfMeasure != null ? facilityItem.IssueUnitOfMeasure.Key : default(Guid?),
                        UOIPerUORAmount = facilityItem.UnitsOfIssuePerUnitOfRefill,
                        StandardStockFlag = facilityItem.IsStandardStockWithinDispensingDevice,
                        MedReturnModeInternalCode = facilityItem.MedReturnMode.ToInternalCode(),
                        MedFailoverReturnModeInternalCode = facilityItem.MedFailoverReturnMode.ToInternalCode(),
                        ReplenishmentScanModeInternalCode = facilityItem.ReplenishmentScanMode.ToInternalCode(),
                        FractionalUOMTypeInternalCode = facilityItem.FractionalUnitOfMeasureType.ToInternalCode(),
                        AutoMedLabelModeInternalCode = facilityItem.AutoMedLabelMode.ToInternalCode(),
                        PharmacyNotesText = facilityItem.PharmacyNotes,
                        NursingNotesText = facilityItem.NursingNotes,
                        CriticalLowPercentage = facilityItem.CriticalLowPercentage,
                        StockOutNoticeFlag = facilityItem.StockOutNotice,
                        OMNLNoticeFlag = facilityItem.OmnlNotice,
                        AnesthesiaMyItemsFlag = facilityItem.AnesthesiaMyItems,
                        ResolveUndocumentedWasteFlag = facilityItem.ResolveUndocumentedWaste,
                        ComboFlag = facilityItem.IsCombo,
                        DispenseComponentsOnlyFlag = facilityItem.DispenseComponentsOnly,
                        ChargeComboFlag = facilityItem.ChargeCombo,
                        DisplayCalculationOnDispenseFlag = facilityItem.DisplayCalculationOnDispense,
                        ReplenishmentPickAreaKey = facilityItem.ReplenishmentPickAreaKey,
                        DistributorKey = facilityItem.DistributorKey,
                        RxCheckFlag = facilityItem.RxCheck,
                        PrintOnRemoveFlag = facilityItem.PrintOnRemove,
                        PrintOnReturnFlag = facilityItem.PrintOnReturn,
                        PrintOnDisposeFlag = facilityItem.PrintOnDispose,
                        PrintOnLoadRefillFlag = facilityItem.PrintOnLoadRefill,
                        ShowConversionOnRemoveFlag = facilityItem.ShowConversionOnRemove,
                        ScanAllOnPickFlag = facilityItem.ScanAllOnPick,
                        CountCubieEjectModeInternalCode = facilityItem.CountCubieEjectMode.ToInternalCode(),
                        PharmacyOrderDispenseQuantityFlag = facilityItem.PharmacyOrderDispenseQuantityFlag,
                        InjectableFlag = facilityItem.InjectableFlag,
                        GCSMMedReturnModeInternalCode = facilityItem.GCSMMedReturnModeInternalCode.ToInternalCode(),
                        GCSMVerifyCountModeInternalCode = facilityItem.GCSMVerifyCountModeInternalCode.ToInternalCode(),
                        GCSMCountCUBIEEjectModeInternalCode = facilityItem.GCSMCountCUBIEEjectModeInternalCode.ToInternalCode(),
                        GCSMOutdateTrackingFlag = facilityItem.GCSMOutdateTrackingFlag,
                        GCSMRequireLotNumberWhenRecallFlag = facilityItem.GCSMRequireLotNumberWhenRecallFlag,
                        GCSMRequireInventoryReferenceNumberFlag = facilityItem.GCSMRequireInventoryReferenceNumberFlag,
                        GCSMWitnessOnAccessToDestructionBinFlag = facilityItem.GCSMWitnessOnAccessToDestructionBinFlag,
                        GCSMWitnessOnAddToDestructionBinFlag = facilityItem.GCSMWitnessOnAddToDestructionBinFlag,
                        GCSMWitnessOnOutdateFlag = facilityItem.GCSMWitnessOnOutdateFlag,
                        GCSMWitnessOnReturnFlag = facilityItem.GCSMWitnessOnReturnFlag,
                        GCSMWitnessOnAutorestockFlag = facilityItem.GCSMWitnessOnAutorestockFlag,
                        GCSMWitnessOnCompoundingFlag = facilityItem.GCSMWitnessOnCompoundingFlag,
                        GCSMWitnessOnDestructionBinFlag = facilityItem.GCSMWitnessOnDestructionBinFlag,
                        GCSMWitnessOnDiscrepancyResolutionFlag = facilityItem.GCSMWitnessOnDiscrepancyResolutionFlag,
                        GCSMWitnessOnInventoryCountFlag = facilityItem.GCSMWitnessOnInventoryCountFlag,
                        GCSMWitnessOnIssueFlag = facilityItem.GCSMWitnessOnIssueFlag,
                        GCSMWitnessOnPrescriptionFlag = facilityItem.GCSMWitnessOnPrescriptionFlag,
                        GCSMWitnessOnRecallFlag = facilityItem.GCSMWitnessOnRecallFlag,
                        GCSMWitnessOnReceiveFlag = facilityItem.GCSMWitnessOnReceiveFlag,
                        GCSMWitnessOnReverseCompoundingFlag = facilityItem.GCSMWitnessOnReverseCompoundingFlag,
                        GCSMWitnessOnSellFlag = facilityItem.GCSMWitnessOnSellFlag,
                        GCSMWitnessOnStockTransferFlag = facilityItem.GCSMWitnessOnStockTransferFlag,
                        GCSMWitnessOnUnloadFlag = facilityItem.GCSMWitnessOnUnloadFlag,
                        GCSMWitnessOnWasteFlag = facilityItem.GCSMWitnessOnWasteFlag,
                        GCSMScanOnCompoundingFlag = facilityItem.GCSMScanOnCompoundingFlag,
                        GCSMScanOnReturnFlag = facilityItem.GCSMScanOnReturnFlag,
                        GCSMScanOnAutorestockFlag = facilityItem.GCSMScanOnAutorestockFlag,
                        GCSMScanOnIssueFlag = facilityItem.GCSMScanOnIssueFlag,
                        GCSMScanOnPrescriptionFlag = facilityItem.GCSMScanOnPrescriptionFlag,
                        GCSMScanOnReceiveFlag = facilityItem.GCSMScanOnReceiveFlag,
                        GCSMScanOnSellFlag = facilityItem.GCSMScanOnSellFlag,
                        GCSMScanOnStockTransferFlag = facilityItem.GCSMScanOnStockTransferFlag,
                        GCSMCriticalLowPercentage = facilityItem.GCSMCriticalLowPercentage,
                        GCSMPrintOnReceiveFlag = facilityItem.GCSMPrintOnReceiveFlag,
                        GCSMPrintSingleMedSheetFlag = facilityItem.GCSMPrintSingleMedSheetFlag,
                        GCSMPrintDripSheetFlag = facilityItem.GCSMPrintDripSheetFlag,
                        GCSMPrintMaximumRowsFlag = facilityItem.GCSMPrintMaximumRowsFlag,
                        GCSMAdditionalLabelsPrintedQuantity = (byte) facilityItem.GCSMAdditionalLabelsPrintedQuantity,
                        GCSMStockOutNoticeFlag = facilityItem.GCSMStockOutNoticeFlag,
                        GCSMADMDispenseQuantity = facilityItem.GCSMADMDispenseQuantity,
                        GCSMTotalDeviceParDurationAmount = facilityItem.GCSMTotalDeviceParDurationAmount,
                        GCSMTotalParDurationAmount = facilityItem.GCSMTotalParDurationAmount,
                        GCSMDistributorPackageSizeQuantity = facilityItem.GCSMDistributorPackageSizeQuantity,
                        GCSMDistributorKey = facilityItem.GCSMDistributorKey,
                        GCSMPreferredProductIDKey = facilityItem.GCSMPreferredProductIDKey,
                        GCSMVendorItemCode = facilityItem.GCSMVendorItemCode,
                        GCSMRequireOriginDestinationFlag = facilityItem.GCSMRequireOriginDestinationFlag
                    };

                    facilityItemKey = _facilityItemRepo.InsertFacilityItem(context.ToActionContext(), model);

                    if (facilityItemKey.HasValue)
                    {
                        if (facilityItem.ComboComponents != null && facilityItem.ComboComponents.Any())
                        {
                            if (!facilityItem.DispenseComponentsOnly)
                            {
                                facilityItem.ComboComponents.Single(c => c.FacilityItemKey == Guid.Empty).FacilityItemKey = facilityItemKey.Value;
                            }

                            InsertFacilityItemComboComponents(
                                context,
                                facilityItemKey.Value,
                                facilityItem.ComboComponents);
                        }

                        if (facilityItem.OverrideGroups != null)
                        {
                            InsertFacilityItemOverrideGroups(
                                context,
                                facilityItemKey.Value,
                                facilityItem.OverrideGroups.Select(og => og.Key));
                        }

                        if (facilityItem.PickAreas != null)
                        {
                            InsertFacilityItemPickAreas(
                                context,
                                facilityItemKey.Value,
                                facilityItem.PickAreas.Select(pa => pa.Key));
                        }

                        if (facilityItem.BlockedDispensingDevices != null)
                        {
                            InsertFacilityItemBlockedDispensingDeviceEntities(
                                context,
                                facilityItemKey.Value,
                                facilityItem.BlockedDispensingDevices);
                        }

                        if (facilityItem.PhysicalCapacities != null)
                        {
                            InsertFacilityItemPhysicalCapacities(
                                context,
                                facilityItemKey.Value,
                                facilityItem.PhysicalCapacities);
                        }

                        if (facilityItem.OverrideClinicalDataSubjects != null)
                        {
                            InsertFacilityItemClinicalDataSubjects(
                                context,
                                facilityItemKey.Value,
                                PatientCareFunctionInternalCode.OVERRIDE,
                                facilityItem.OverrideClinicalDataSubjects);
                        }

                        if (facilityItem.RemoveClinicalDataSubjects != null)
                        {
                            InsertFacilityItemClinicalDataSubjects(
                                context,
                                facilityItemKey.Value,
                                PatientCareFunctionInternalCode.REMOVE,
                                facilityItem.RemoveClinicalDataSubjects);
                        }

                        if (facilityItem.ReturnClinicalDataSubjects != null)
                        {
                            InsertFacilityItemClinicalDataSubjects(
                                context,
                                facilityItemKey.Value,
                                PatientCareFunctionInternalCode.RETURN,
                                facilityItem.ReturnClinicalDataSubjects);
                        }

                        if (facilityItem.WasteClinicalDataSubjects != null)
                        {
                            InsertFacilityItemClinicalDataSubjects(
                                context,
                                facilityItemKey.Value,
                                PatientCareFunctionInternalCode.WASTE,
                                facilityItem.WasteClinicalDataSubjects);
                        }

                        if (facilityItem.FacilityEquivalencies != null)
                        {
                            InsertorUpdateFacilityItemEquivalencies(
                                context,
                                facilityItem);
                        }

                        tx.Complete();
                    }
                   
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return facilityItemKey.GetValueOrDefault();
        }

        void IItemRepository.UpdateFacilityItem(Context context, FacilityItem facilityItem)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(facilityItem, "facilityItem");

            try
            {
                var model = new ItemDAL.Models.FacilityItem()
                {
                    ItemDisplayName = facilityItem.DisplayName,
                    AlternateItemId = facilityItem.AlternateId,
                    MedClassKey = facilityItem.MedicationClass != null ? facilityItem.MedicationClass.Key : default(Guid?),
                    OutdateTrackingFlag = facilityItem.OutdateTracking,
                    VerifyCountModeInternalCode = facilityItem.VerifyCountMode != null ? facilityItem.VerifyCountMode.ToInternalCode() : default(string),
                    VerifyCountAnesthesiaDispensingFlag = facilityItem.VerifyCountAnesthesiaDispensing,
                    WitnessOnDispenseFlag = facilityItem.WitnessOnDispense,
                    WitnessOnReturnFlag = facilityItem.WitnessOnReturn,
                    WitnessOnDisposeFlag = facilityItem.WitnessOnDispose,
                    WitnessOnLoadRefillFlag = facilityItem.WitnessOnLoadRefill,
                    WitnessOnUnloadFlag = facilityItem.WitnessOnUnload,
                    WitnessOnOverrideFlag = facilityItem.WitnessOnOverride,
                    WitnessOnOutdateFlag = facilityItem.WitnessOnOutdate,
                    WitnessOnInventoryFlag = facilityItem.WitnessOnInventory,
                    WitnessOnEmptyReturnBinFlag = facilityItem.WitnessOnEmptyReturnBin,
                    WitnessOnDestockFlag = facilityItem.WitnessOnDestock,
                    WitnessOnRxCheckFlag = facilityItem.WitnessOnRxCheck,
                    ScanOnLoadRefillFlag = facilityItem.ScanOnLoadRefill,
                    ScanOnRemoveFlag = facilityItem.ScanOnRemove,
                    ScanOnReturnFlag = facilityItem.ScanOnReturn,
                    ScanOnCheckFlag = facilityItem.ScanOnCheck,
                    DoOnceOnRemoveFlag = facilityItem.DoOnceOnRemove,
                    RequireLotIdOnRemoveFlag = facilityItem.RequireLotIdOnRemove,
                    RequireLotIdOnReturnFlag = facilityItem.RequireLotIdOnReturn,
                    RequireSerialIdOnRemoveFlag = facilityItem.RequireSerialIdOnRemove,
                    RequireSerialIdOnReturnFlag = facilityItem.RequireSerialIdOnReturn,
                    RequireExpirationDateOnRemoveFlag = facilityItem.RequireExpirationDateOnRemove,
                    RequireExpirationDateOnReturnFlag = facilityItem.RequireExpirationDateOnReturn,
                    RequireInventoryReferenceIdFlag = facilityItem.RequireInventoryReferenceId,
                    ReverificationFlag = facilityItem.Reverification,
                    TooCloseRemoveDurationAmount = facilityItem.TooCloseRemoveDuration,
                    SecurityGroupKey = facilityItem.SecurityGroupKey,
                    ActiveFlag = facilityItem.IsActive,
                    AutoResolveDiscrepancyFlag = facilityItem.AutoResolveDiscrepancy,
                    ChargeableFlag = facilityItem.IsChargeable,
                    HighCostFlag = facilityItem.IsHighCost,
                    AllowSplittingFlag = facilityItem.AllowSplitting,
                    HighRiskFlag = facilityItem.IsHighRisk,
                    HazardousWasteClassKey = facilityItem.HazardousWasteClassKey,
                    TrackInventoryQuantityFlag = facilityItem.TrackInventoryQuantity,
                    MultiDoseFlag = facilityItem.IsMultiDose,
                    BackorderedFlag = facilityItem.IsBackOrdered,
                    RefillUOMKey = facilityItem.RefillUnitOfMeasure != null ? facilityItem.RefillUnitOfMeasure.Key : default(Guid?),
                    IssueUOMKey = facilityItem.IssueUnitOfMeasure != null ? facilityItem.IssueUnitOfMeasure.Key : default(Guid?),
                    UOIPerUORAmount = facilityItem.UnitsOfIssuePerUnitOfRefill,
                    StandardStockFlag = facilityItem.IsStandardStockWithinDispensingDevice,
                    MedReturnModeInternalCode = facilityItem.MedReturnMode.ToInternalCode(),
                    MedFailoverReturnModeInternalCode = facilityItem.MedFailoverReturnMode.ToInternalCode(),
                    ReplenishmentScanModeInternalCode = facilityItem.ReplenishmentScanMode.ToInternalCode(),
                    FractionalUOMTypeInternalCode = facilityItem.FractionalUnitOfMeasureType.ToInternalCode(),
                    AutoMedLabelModeInternalCode = facilityItem.AutoMedLabelMode.ToInternalCode(),
                    PharmacyNotesText = facilityItem.PharmacyNotes,
                    NursingNotesText = facilityItem.NursingNotes,
                    CriticalLowPercentage = facilityItem.CriticalLowPercentage,
                    StockOutNoticeFlag = facilityItem.StockOutNotice,
                    OMNLNoticeFlag = facilityItem.OmnlNotice,
                    AnesthesiaMyItemsFlag = facilityItem.AnesthesiaMyItems,
                    ResolveUndocumentedWasteFlag = facilityItem.ResolveUndocumentedWaste,
                    ComboFlag = facilityItem.IsCombo,
                    DispenseComponentsOnlyFlag = facilityItem.DispenseComponentsOnly,
                    ChargeComboFlag = facilityItem.ChargeCombo,
                    DisplayCalculationOnDispenseFlag = facilityItem.DisplayCalculationOnDispense,
                    ReplenishmentPickAreaKey = facilityItem.ReplenishmentPickAreaKey,
                    DistributorKey = facilityItem.DistributorKey,
                    RxCheckFlag = facilityItem.RxCheck,
                    PrintOnRemoveFlag = facilityItem.PrintOnRemove,
                    PrintOnReturnFlag = facilityItem.PrintOnReturn,
                    PrintOnDisposeFlag = facilityItem.PrintOnDispose,
                    PrintOnLoadRefillFlag = facilityItem.PrintOnLoadRefill,
                    ShowConversionOnRemoveFlag = facilityItem.ShowConversionOnRemove,
                    ScanAllOnPickFlag = facilityItem.ScanAllOnPick,
                    CountCubieEjectModeInternalCode = facilityItem.CountCubieEjectMode.ToInternalCode(),
                    PharmacyOrderDispenseQuantityFlag = facilityItem.PharmacyOrderDispenseQuantityFlag,
                    InjectableFlag = facilityItem.InjectableFlag,
                    GCSMMedReturnModeInternalCode = facilityItem.GCSMMedReturnModeInternalCode.ToInternalCode(),
                    GCSMVerifyCountModeInternalCode = facilityItem.GCSMVerifyCountModeInternalCode.ToInternalCode(),
                    GCSMCountCUBIEEjectModeInternalCode = facilityItem.GCSMCountCUBIEEjectModeInternalCode.ToInternalCode(),
                    GCSMOutdateTrackingFlag = facilityItem.GCSMOutdateTrackingFlag,
                    GCSMRequireLotNumberWhenRecallFlag = facilityItem.GCSMRequireLotNumberWhenRecallFlag,
                    GCSMRequireInventoryReferenceNumberFlag = facilityItem.GCSMRequireInventoryReferenceNumberFlag,
                    GCSMWitnessOnAccessToDestructionBinFlag = facilityItem.GCSMWitnessOnAccessToDestructionBinFlag,
                    GCSMWitnessOnAddToDestructionBinFlag = facilityItem.GCSMWitnessOnAddToDestructionBinFlag,
                    GCSMWitnessOnOutdateFlag = facilityItem.GCSMWitnessOnOutdateFlag,
                    GCSMWitnessOnReturnFlag = facilityItem.GCSMWitnessOnReturnFlag,
                    GCSMWitnessOnAutorestockFlag = facilityItem.GCSMWitnessOnAutorestockFlag,
                    GCSMWitnessOnCompoundingFlag = facilityItem.GCSMWitnessOnCompoundingFlag,
                    GCSMWitnessOnDestructionBinFlag = facilityItem.GCSMWitnessOnDestructionBinFlag,
                    GCSMWitnessOnDiscrepancyResolutionFlag = facilityItem.GCSMWitnessOnDiscrepancyResolutionFlag,
                    GCSMWitnessOnInventoryCountFlag = facilityItem.GCSMWitnessOnInventoryCountFlag,
                    GCSMWitnessOnIssueFlag = facilityItem.GCSMWitnessOnIssueFlag,
                    GCSMWitnessOnPrescriptionFlag = facilityItem.GCSMWitnessOnPrescriptionFlag,
                    GCSMWitnessOnRecallFlag = facilityItem.GCSMWitnessOnRecallFlag,
                    GCSMWitnessOnReceiveFlag = facilityItem.GCSMWitnessOnReceiveFlag,
                    GCSMWitnessOnReverseCompoundingFlag = facilityItem.GCSMWitnessOnReverseCompoundingFlag,
                    GCSMWitnessOnSellFlag = facilityItem.GCSMWitnessOnSellFlag,
                    GCSMWitnessOnStockTransferFlag = facilityItem.GCSMWitnessOnStockTransferFlag,
                    GCSMWitnessOnUnloadFlag = facilityItem.GCSMWitnessOnUnloadFlag,
                    GCSMWitnessOnWasteFlag = facilityItem.GCSMWitnessOnWasteFlag,
                    GCSMScanOnCompoundingFlag = facilityItem.GCSMScanOnCompoundingFlag,
                    GCSMScanOnReturnFlag = facilityItem.GCSMScanOnReturnFlag,
                    GCSMScanOnAutorestockFlag = facilityItem.GCSMScanOnAutorestockFlag,
                    GCSMScanOnIssueFlag = facilityItem.GCSMScanOnIssueFlag,
                    GCSMScanOnPrescriptionFlag = facilityItem.GCSMScanOnPrescriptionFlag,
                    GCSMScanOnReceiveFlag = facilityItem.GCSMScanOnReceiveFlag,
                    GCSMScanOnSellFlag = facilityItem.GCSMScanOnSellFlag,
                    GCSMScanOnStockTransferFlag = facilityItem.GCSMScanOnStockTransferFlag,
                    GCSMCriticalLowPercentage = facilityItem.GCSMCriticalLowPercentage,
                    GCSMPrintOnReceiveFlag = facilityItem.GCSMPrintOnReceiveFlag,
                    GCSMPrintSingleMedSheetFlag = facilityItem.GCSMPrintSingleMedSheetFlag,
                    GCSMPrintDripSheetFlag = facilityItem.GCSMPrintDripSheetFlag,
                    GCSMPrintMaximumRowsFlag = facilityItem.GCSMPrintMaximumRowsFlag,
                    GCSMAdditionalLabelsPrintedQuantity = (byte) facilityItem.GCSMAdditionalLabelsPrintedQuantity,
                    GCSMStockOutNoticeFlag = facilityItem.GCSMStockOutNoticeFlag,
                    GCSMADMDispenseQuantity = facilityItem.GCSMADMDispenseQuantity,
                    GCSMTotalDeviceParDurationAmount = facilityItem.GCSMTotalDeviceParDurationAmount,
                    GCSMTotalParDurationAmount = facilityItem.GCSMTotalParDurationAmount,
                    GCSMDistributorPackageSizeQuantity = facilityItem.GCSMDistributorPackageSizeQuantity,
                    GCSMDistributorKey = facilityItem.GCSMDistributorKey,
                    GCSMPreferredProductIDKey = facilityItem.GCSMPreferredProductIDKey,
                    GCSMVendorItemCode = facilityItem.GCSMVendorItemCode,
                    GCSMRequireOriginDestinationFlag = facilityItem.GCSMRequireOriginDestinationFlag,
                    LastModifiedBinaryValue = facilityItem.LastModified ?? new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 },
                    FacilityItemKey = facilityItem.Key,
                };

                using (ITransactionScope tx = TransactionScopeFactory.Create())
                {
                    _facilityItemRepo.UpdateFacilityItem(context.ToActionContext(), model);

                    UpdateFacilityItemComboComponents(
                        context,
                        facilityItem.Key,
                        facilityItem.ComboComponents ?? new ComboComponent[0]);

                    UpdateFacilityItemOverrideGroups(
                        context,
                        facilityItem.Key,
                        (facilityItem.OverrideGroups != null)
                            ? facilityItem.OverrideGroups.Select(og => og.Key)
                            : new Guid[0]);

                    UpdateFacilityItemPickAreas(
                        context,
                        facilityItem.Key,
                        (facilityItem.PickAreas != null)
                            ? facilityItem.PickAreas.Select(pa => pa.Key)
                            : new Guid[0]);

                    UpdateFacilityItemBlockedDispensingDeviceEntities(
                        context,
                        facilityItem.Key,
                        facilityItem.BlockedDispensingDevices ?? new Guid[0]);

                    UpdateFacilityItemPhysicalCapacities(
                        context,
                        facilityItem.Key,
                        facilityItem.PhysicalCapacities ?? new PhysicalCapacity[0]);

                    UpdateFacilityItemClinicalDataSubjects(
                        context,
                        facilityItem.Key,
                        PatientCareFunctionInternalCode.OVERRIDE,
                        facilityItem.OverrideClinicalDataSubjects ?? new Guid[0]);

                    UpdateFacilityItemClinicalDataSubjects(
                        context,
                        facilityItem.Key,
                        PatientCareFunctionInternalCode.REMOVE,
                        facilityItem.RemoveClinicalDataSubjects ?? new Guid[0]);

                    UpdateFacilityItemClinicalDataSubjects(
                        context,
                        facilityItem.Key,
                        PatientCareFunctionInternalCode.RETURN,
                        facilityItem.ReturnClinicalDataSubjects ?? new Guid[0]);

                    UpdateFacilityItemClinicalDataSubjects(
                        context,
                        facilityItem.Key,
                        PatientCareFunctionInternalCode.WASTE,
                        facilityItem.WasteClinicalDataSubjects ?? new Guid[0]);

                    if (facilityItem.FacilityEquivalencies != null)
                    {
                        InsertorUpdateFacilityItemEquivalencies(
                            context,
                            facilityItem);
                    }

                    tx.Complete();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }
        
        void IItemRepository.DeleteFacilityItem(Context context, Guid facilityKey, Guid itemKey)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                _facilityItemRepo.DeleteFacilityItem(context.ToActionContext(), facilityKey, itemKey);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        #endregion

        #region FormularyTemplate Members

        IEnumerable<FormularyTemplate> IItemRepository.GetFormularyTemplates(IEnumerable<Guid> formularyTemplateKeys, bool? deleted,
                                                          Guid? externalSystemKey)
        {
            List<FormularyTemplate> formularyTemplates = new List<FormularyTemplate>();
            if (formularyTemplateKeys != null && !formularyTemplateKeys.Any())
                return formularyTemplates; // Empty results

            try
            {
                GuidKeyTable selectedKeys = new GuidKeyTable();
                if (formularyTemplateKeys != null)
                    selectedKeys = new GuidKeyTable(formularyTemplateKeys.Distinct());

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var multi = connectionScope.QueryMultiple(
                    "Item.bsp_GetFormularyTemplates",
                    new
                    {
                        selectedKeys = selectedKeys.AsTableValuedParameter(),
                        ExternalSystemKey = externalSystemKey,
                        DeleteFlag = deleted
                    },
                    commandTimeout: connectionScope.DefaultCommandTimeout,
                    commandType: CommandType.StoredProcedure);
                    var formularyTemplateResults = multi.Read<FormularyTemplatesResult>();
                    var overrideGroupResults = multi.Read<Models.FormularyTemplateOverrideGroupResult>();

                    foreach (var formularyTemplateResult in formularyTemplateResults)
                    {
                        FormularyTemplate formularyTemplate =
                            new FormularyTemplate(formularyTemplateResult.FormularyTemplateKey)
                                {
                                    ExternalSystemKey = formularyTemplateResult.ExternalSystemKey,
                                    ExternalSystemName = formularyTemplateResult.ExternalSystemName,
                                    Name = formularyTemplateResult.FormularyTemplateName,
                                    Description = formularyTemplateResult.DescriptionText,
                                    OutdateTracking = formularyTemplateResult.OutdateTrackingFlag,
                                    VerifyCountMode = formularyTemplateResult.VerifyCountModeInternalCode.FromNullableInternalCode<VerifyCountModeInternalCode>(),
                                    VerifyCountAnesthesiaDispensing = formularyTemplateResult.VerifyCountAnesthesiaDispensingFlag,
                                    WitnessOnDispense = formularyTemplateResult.WitnessOnDispenseFlag,
                                    WitnessOnReturn = formularyTemplateResult.WitnessOnReturnFlag,
                                    WitnessOnDispose = formularyTemplateResult.WitnessOnDisposeFlag,
                                    WitnessOnLoadRefill = formularyTemplateResult.WitnessOnLoadRefillFlag,
                                    WitnessOnUnload = formularyTemplateResult.WitnessOnUnloadFlag,
                                    WitnessOnOverride = formularyTemplateResult.WitnessOnOverrideFlag,
                                    WitnessOnOutdate = formularyTemplateResult.WitnessOnOutdateFlag,
                                    WitnessOnInventory = formularyTemplateResult.WitnessOnInventoryFlag,
                                    WitnessOnEmptyReturnBin = formularyTemplateResult.WitnessOnEmptyReturnBinFlag,
                                    WitnessOnDestock = formularyTemplateResult.WitnessOnDestockFlag,
                                    WitnessOnRxCheck = formularyTemplateResult.WitnessOnRxCheckFlag,
                                    ScanOnLoadRefill = formularyTemplateResult.ScanOnLoadRefillFlag,
                                    ScanOnRemove = formularyTemplateResult.ScanOnRemoveFlag,
                                    DoOnceOnRemove = formularyTemplateResult.DoOnceOnRemoveFlag,
                                    ScanOnReturn = formularyTemplateResult.ScanOnReturnFlag,
                                    ScanOnCheck = formularyTemplateResult.ScanOnCheckFlag,
                                    RequireLotIdOnRemove = formularyTemplateResult.RequireLotIDOnRemoveFlag,
                                    RequireLotIdOnReturn = formularyTemplateResult.RequireLotIDOnReturnFlag,
                                    RequireSerialIdOnRemove = formularyTemplateResult.RequireSerialIDOnRemoveFlag,
                                    RequireSerialIdOnReturn = formularyTemplateResult.RequireSerialIDOnReturnFlag,
                                    RequireExpirationDateOnRemove = formularyTemplateResult.RequireExpirationDateOnRemoveFlag,
                                    RequireExpirationDateOnReturn = formularyTemplateResult.RequireExpirationDateOnReturnFlag,
                                    RequireInventoryReferenceId = formularyTemplateResult.RequireInventoryReferenceIDFlag,
                                    Reverification = formularyTemplateResult.ReverificationFlag,
                                    TooCloseRemoveDuration = formularyTemplateResult.TooCloseRemoveDurationAmount,
                                    SecurityGroupKey = formularyTemplateResult.SecurityGroupKey,
                                    IsActive = formularyTemplateResult.ActiveFlag,
                                    AutoResolveDiscrepancy = formularyTemplateResult.AutoResolveDiscrepancyFlag,
                                    IsChargeable = formularyTemplateResult.ChargeableFlag,
                                    IsHighCost = formularyTemplateResult.HighCostFlag,
                                    AllowSplitting = formularyTemplateResult.AllowSplittingFlag,
                                    IsHighRisk = formularyTemplateResult.HighRiskFlag,
                                    TrackInventoryQuantity = formularyTemplateResult.TrackInventoryQuantityFlag,
                                    IsMultiDose = formularyTemplateResult.MultiDoseFlag,
                                    IsBackOrdered = formularyTemplateResult.BackorderedFlag,
                                    MedReturnMode = formularyTemplateResult.MedReturnModeInternalCode.FromNullableInternalCode<MedReturnModeInternalCode>(),
                                    MedFailoverReturnMode = formularyTemplateResult.MedFailoverReturnModeInternalCode.FromNullableInternalCode<MedFailoverReturnModeInternalCode>(),
                                    ReplenishmentScanMode = formularyTemplateResult.ReplenishmentScanModeInternalCode.FromNullableInternalCode<ReplenishmentScanModeInternalCode>(),
                                    FractionalUnitOfMeasureType = formularyTemplateResult.FractionalUOMTypeInternalCode.FromNullableInternalCode<FractionalUOMTypeInternalCode>(),
                                    AutoMedLabelMode = formularyTemplateResult.AutoMedLabelModeInternalCode.FromNullableInternalCode<AutoMedLabelModeInternalCode>(),
                                    PharmacyNotes = formularyTemplateResult.PharmacyNotesText,
                                    NursingNotes = formularyTemplateResult.NursingNotesText,
                                    CriticalLowPercentage = formularyTemplateResult.CriticalLowPercentage,
                                    StockOutNotice = formularyTemplateResult.StockOutNoticeFlag,
                                    OmnlNotice = formularyTemplateResult.OMNLNoticeFlag,
                                    AnesthesiaMyItems = formularyTemplateResult.AnesthesiaMyItemsFlag,
                                    ResolveUndocumentedWaste = formularyTemplateResult.ResolveUndocumentedWasteFlag,
                                    DistributorKey = formularyTemplateResult.DistributorKey,
                                    RxCheck = formularyTemplateResult.RxCheckFlag,
                                    PrintOnRemove = formularyTemplateResult.PrintOnRemoveFlag,
                                    PrintOnReturn = formularyTemplateResult.PrintOnReturnFlag,
                                    PrintOnDispose = formularyTemplateResult.PrintOnDisposeFlag,
                                    PrintOnLoadRefill = formularyTemplateResult.PrintOnLoadRefillFlag,
                                    ShowConversionOnRemove = formularyTemplateResult.ShowConversionOnRemoveFlag,
                                    ScanAllOnPick = formularyTemplateResult.ScanAllOnPickFlag,
                                    CountCubieEjectMode = formularyTemplateResult.CountCUBIEEjectModeInternalCode.FromNullableInternalCode<CountCUBIEEjectModeInternalCode>(),
                                    PharmacyOrderDispenseQuantityFlag = formularyTemplateResult.PharmacyOrderDispenseQuantityFlag,
                                    InjectableFlag = formularyTemplateResult.InjectableFlag,
                                    GCSMAdditionalLabelsPrintedQuantity = formularyTemplateResult.GCSMAdditionalLabelsPrintedQuantity,
                                    GCSMCriticalLowPercentage = formularyTemplateResult.GCSMCriticalLowPercentage,
                                    GCSMDistributorKey = formularyTemplateResult.GCSMDistributorKey,
                                    GCSMOutdateTrackingFlag = formularyTemplateResult.GCSMOutdateTrackingFlag,
                                    GCSMPrintDripSheetFlag = formularyTemplateResult.GCSMPrintDripSheetFlag,
                                    GCSMPrintMaximumRowsFlag = formularyTemplateResult.GCSMPrintMaximumRowsFlag,
                                    GCSMPrintOnReceiveFlag = formularyTemplateResult.GCSMPrintOnReceiveFlag,
                                    GCSMPrintSingleMedSheetFlag = formularyTemplateResult.GCSMPrintSingleMedSheetFlag,
                                    GCSMScanOnAutorestockFlag = formularyTemplateResult.GCSMScanOnAutorestockFlag,
                                    GCSMScanOnPrescriptionFlag = formularyTemplateResult.GCSMScanOnPrescriptionFlag,
                                    GCSMScanOnIssueFlag = formularyTemplateResult.GCSMScanOnIssueFlag,
                                    GCSMScanOnReceiveFlag = formularyTemplateResult.GCSMScanOnReceiveFlag,
                                    GCSMScanOnReturnFlag = formularyTemplateResult.GCSMScanOnReturnFlag,
                                    GCSMScanOnSellFlag = formularyTemplateResult.GCSMScanOnSellFlag,
                                    GCSMScanOnStockTransferFlag = formularyTemplateResult.GCSMScanOnStockTransferFlag,
                                    GCSMWitnessOnAutorestockFlag = formularyTemplateResult.GCSMWitnessOnAutorestockFlag,
                                    GCSMWitnessOnCompoundingFlag = formularyTemplateResult.GCSMWitnessOnCompoundingFlag,
                                    GCSMWitnessOnDestructionBinFlag = formularyTemplateResult.GCSMWitnessOnDestructionBinFlag,
                                    GCSMWitnessOnDiscrepancyResolutionFlag = formularyTemplateResult.GCSMWitnessOnDiscrepancyResolutionFlag,
                                    GCSMWitnessOnIssueFlag = formularyTemplateResult.GCSMWitnessOnIssueFlag,
                                    GCSMWitnessOnPrescriptionFlag = formularyTemplateResult.GCSMWitnessOnPrescriptionFlag,
                                    GCSMWitnessOnOutdateFlag = formularyTemplateResult.GCSMWitnessOnOutdateFlag,
                                    GCSMWitnessOnReceiveFlag = formularyTemplateResult.GCSMWitnessOnReceiveFlag,
                                    GCSMWitnessOnReturnFlag = formularyTemplateResult.GCSMWitnessOnReturnFlag,
                                    GCSMWitnessOnSellFlag = formularyTemplateResult.GCSMWitnessOnSellFlag,
                                    GCSMWitnessOnStockTransferFlag = formularyTemplateResult.GCSMWitnessOnStockTransferFlag,
                                    GCSMWitnessOnWasteFlag = formularyTemplateResult.GCSMWitnessOnWasteFlag,
                                    GCSMMedReturnModeInternalCode = formularyTemplateResult.GCSMMedReturnModeInternalCode.FromNullableInternalCode<MedReturnModeInternalCode>(),
                                    GCSMVerifyCountModeInternalCode = formularyTemplateResult.GCSMVerifyCountModeInternalCode.FromNullableInternalCode<VerifyCountModeInternalCode>(),
                                    GCSMCountCUBIEEjectModeInternalCode = formularyTemplateResult.GCSMCountCUBIEEjectModeInternalCode.FromNullableInternalCode<CountCUBIEEjectModeInternalCode>(),
                                    GCSMRequireLotNumberWhenRecallFlag = formularyTemplateResult.GCSMRequireLotNumberWhenRecallFlag,
                                    GCSMRequireInventoryReferenceNumberFlag = formularyTemplateResult.GCSMRequireInventoryReferenceNumberFlag,
                                    GCSMWitnessOnAccessToDestructionBinFlag = formularyTemplateResult.GCSMWitnessOnAccessToDestructionBinFlag,
                                    GCSMWitnessOnAddToDestructionBinFlag = formularyTemplateResult.GCSMWitnessOnAddToDestructionBinFlag,
                                    GCSMWitnessOnInventoryCountFlag = formularyTemplateResult.GCSMWitnessOnInventoryCountFlag,
                                    GCSMWitnessOnRecallFlag = formularyTemplateResult.GCSMWitnessOnRecallFlag,
                                    GCSMWitnessOnReverseCompoundingFlag = formularyTemplateResult.GCSMWitnessOnReverseCompoundingFlag,
                                    GCSMWitnessOnUnloadFlag = formularyTemplateResult.GCSMWitnessOnUnloadFlag,
                                    GCSMScanOnCompoundingFlag = formularyTemplateResult.GCSMScanOnCompoundingFlag,
                                    GCSMStockOutNoticeFlag = formularyTemplateResult.GCSMStockOutNoticeFlag,
                                    GCSMADMDispenseQuantity = formularyTemplateResult.GCSMADMDispenseQuantity,
                                    GCSMDistributorPackageSizeQuantity=formularyTemplateResult.GCSMDistributorPackageSizeQuantity,
                                    GCSMTotalDeviceParDurationAmount = formularyTemplateResult.GCSMTotalDeviceParDurationAmount,
                                    GCSMTotalParDurationAmount = formularyTemplateResult.GCSMTotalParDurationAmount,
                                    GCSMRequireOriginDestinationFlag = formularyTemplateResult.GCSMRequireOriginDestinationFlag,
                                    LastModifiedActorKey = formularyTemplateResult.LastModifiedActorKey,
                                    LastModifiedUserAccountDisplayName = formularyTemplateResult.LastModifiedUserAccountDisplayName,
                                    LastModifiedUtcDateTime = formularyTemplateResult.LastModifiedUTCDateTime,
                                    LastModified = formularyTemplateResult.LastModifiedBinaryValue ?? new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }
                                };

                        // Override Groups
                        formularyTemplate.OverrideGroups = overrideGroupResults
                            .Where(og => og.FormularyTemplateKey == formularyTemplateResult.FormularyTemplateKey)
                            .Select(og => og.OverrideGroupKey)
                            .ToArray();

                        formularyTemplates.Add(formularyTemplate);
                    }
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return formularyTemplates;
        }

        FormularyTemplate IItemRepository.GetFormularyTemplate(Guid formularyTemplateKey)
        {
            IEnumerable<FormularyTemplate> formularyTemplates =
                ((IItemRepository)this).GetFormularyTemplates(new [] { formularyTemplateKey });

            return formularyTemplates.FirstOrDefault();
        }

        Guid IItemRepository.InsertFormularyTemplate(Context context, FormularyTemplate formularyTemplate)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(formularyTemplate, "formularyTemplate");
            Guid? formularyTemplateKey = null;

            try
            {
                var model = new ItemDAL.Models.FormularyTemplate()
                {
                    ExternalSystemKey = formularyTemplate.ExternalSystemKey,
                    FormularyTemplateName = formularyTemplate.Name,
                    DescriptionText = formularyTemplate.Description,
                    OutdateTrackingFlag = formularyTemplate.OutdateTracking,
                    VerifyCountModeInternalCode = formularyTemplate.VerifyCountMode.ToInternalCode(),
                    VerifyCountAnesthesiaDispensingFlag = formularyTemplate.VerifyCountAnesthesiaDispensing,
                    WitnessOnDispenseFlag = formularyTemplate.WitnessOnDispense,
                    WitnessOnReturnFlag = formularyTemplate.WitnessOnReturn,
                    WitnessOnDisposeFlag = formularyTemplate.WitnessOnDispose,
                    WitnessOnLoadRefillFlag = formularyTemplate.WitnessOnLoadRefill,
                    WitnessOnUnloadFlag = formularyTemplate.WitnessOnUnload,
                    WitnessOnOverrideFlag = formularyTemplate.WitnessOnOverride,
                    WitnessOnOutdateFlag = formularyTemplate.WitnessOnOutdate,
                    WitnessOnInventoryFlag = formularyTemplate.WitnessOnInventory,
                    WitnessOnEmptyReturnBinFlag = formularyTemplate.WitnessOnEmptyReturnBin,
                    WitnessOnDestockFlag = formularyTemplate.WitnessOnDestock,
                    WitnessOnRxCheckFlag = formularyTemplate.WitnessOnRxCheck,
                    ScanOnLoadRefillFlag = formularyTemplate.ScanOnLoadRefill,
                    ScanOnRemoveFlag = formularyTemplate.ScanOnRemove,
                    ScanOnReturnFlag = formularyTemplate.ScanOnReturn,
                    ScanOnCheckFlag = formularyTemplate.ScanOnCheck,
                    DoOnceOnRemoveFlag = formularyTemplate.DoOnceOnRemove,
                    RequireLotIdOnRemoveFlag = formularyTemplate.RequireLotIdOnRemove,
                    RequireLotIdOnReturnFlag = formularyTemplate.RequireLotIdOnReturn,
                    RequireSerialIdOnRemoveFlag = formularyTemplate.RequireSerialIdOnRemove,
                    RequireSerialIdOnReturnFlag = formularyTemplate.RequireSerialIdOnReturn,
                    RequireExpirationDateOnRemoveFlag = formularyTemplate.RequireExpirationDateOnRemove,
                    RequireExpirationDateOnReturnFlag = formularyTemplate.RequireExpirationDateOnReturn,
                    RequireInventoryReferenceIdFlag = formularyTemplate.RequireInventoryReferenceId,
                    ReverificationFlag = formularyTemplate.Reverification,
                    TooCloseRemoveDurationAmount = formularyTemplate.TooCloseRemoveDuration,
                    SecurityGroupKey = formularyTemplate.SecurityGroupKey,
                    ActiveFlag = formularyTemplate.IsActive,
                    AutoResolveDiscrepancyFlag = formularyTemplate.AutoResolveDiscrepancy,
                    ChargeableFlag = formularyTemplate.IsChargeable,
                    HighCostFlag = formularyTemplate.IsHighCost,
                    AllowSplittingFlag = formularyTemplate.AllowSplitting,
                    HighRiskFlag = formularyTemplate.IsHighRisk,
                    TrackInventoryQuantityFlag = formularyTemplate.TrackInventoryQuantity = true,
                    MultiDoseFlag = formularyTemplate.IsMultiDose,
                    BackorderedFlag = formularyTemplate.IsBackOrdered,
                    MedReturnModeInternalCode = formularyTemplate.MedReturnMode.ToInternalCode(),
                    MedFailoverReturnModeInternalCode = formularyTemplate.MedFailoverReturnMode.ToInternalCode(),
                    ReplenishmentScanModeInternalCode = formularyTemplate.ReplenishmentScanMode.ToInternalCode(),
                    FractionalUOMTypeInternalCode = formularyTemplate.FractionalUnitOfMeasureType.ToInternalCode(),
                    AutoMedLabelModeInternalCode = formularyTemplate.AutoMedLabelMode.ToInternalCode(),
                    PharmacyNotesText = formularyTemplate.PharmacyNotes,
                    NursingNotesText = formularyTemplate.NursingNotes,
                    CriticalLowPercentage = formularyTemplate.CriticalLowPercentage,
                    StockOutNoticeFlag = formularyTemplate.StockOutNotice,
                    OMNLNoticeFlag = formularyTemplate.OmnlNotice,
                    AnesthesiaMyItemsFlag = formularyTemplate.AnesthesiaMyItems,
                    ResolveUndocumentedWasteFlag = formularyTemplate.ResolveUndocumentedWaste,
                    DistributorKey = formularyTemplate.DistributorKey,
                    RxCheckFlag = formularyTemplate.RxCheck,
                    PrintOnRemoveFlag = formularyTemplate.PrintOnRemove,
                    PrintOnReturnFlag = formularyTemplate.PrintOnReturn,
                    PrintOnDisposeFlag = formularyTemplate.PrintOnDispose,
                    PrintOnLoadRefillFlag = formularyTemplate.PrintOnLoadRefill,
                    ShowConversionOnRemoveFlag = formularyTemplate.ShowConversionOnRemove,
                    ScanAllOnPickFlag = formularyTemplate.ScanAllOnPick,
                    CountCubieEjectModeInternalCode = formularyTemplate.CountCubieEjectMode.ToInternalCode(),
                    PharmacyOrderDispenseQuantityFlag = formularyTemplate.PharmacyOrderDispenseQuantityFlag,
                    InjectableFlag = formularyTemplate.InjectableFlag,
                    GCSMMedReturnModeInternalCode = formularyTemplate.GCSMMedReturnModeInternalCode.ToInternalCode(),
                    GCSMVerifyCountModeInternalCode = formularyTemplate.GCSMVerifyCountModeInternalCode.ToInternalCode(),
                    GCSMCountCUBIEEjectModeInternalCode = formularyTemplate.GCSMCountCUBIEEjectModeInternalCode.ToInternalCode(),
                    GCSMOutdateTrackingFlag = formularyTemplate.GCSMOutdateTrackingFlag,
                    GCSMRequireLotNumberWhenRecallFlag = formularyTemplate.GCSMRequireLotNumberWhenRecallFlag,
                    GCSMRequireInventoryReferenceNumberFlag = formularyTemplate.GCSMRequireInventoryReferenceNumberFlag,
                    GCSMWitnessOnAccessToDestructionBinFlag = formularyTemplate.GCSMWitnessOnAccessToDestructionBinFlag,
                    GCSMWitnessOnAddToDestructionBinFlag = formularyTemplate.GCSMWitnessOnAddToDestructionBinFlag,
                    GCSMWitnessOnOutdateFlag = formularyTemplate.GCSMWitnessOnOutdateFlag,
                    GCSMWitnessOnReturnFlag = formularyTemplate.GCSMWitnessOnReturnFlag,
                    GCSMWitnessOnAutorestockFlag = formularyTemplate.GCSMWitnessOnAutorestockFlag,
                    GCSMWitnessOnCompoundingFlag = formularyTemplate.GCSMWitnessOnCompoundingFlag,
                    GCSMWitnessOnDestructionBinFlag = formularyTemplate.GCSMWitnessOnDestructionBinFlag,
                    GCSMWitnessOnDiscrepancyResolutionFlag = formularyTemplate.GCSMWitnessOnDiscrepancyResolutionFlag,
                    GCSMWitnessOnInventoryCountFlag = formularyTemplate.GCSMWitnessOnInventoryCountFlag,
                    GCSMWitnessOnIssueFlag = formularyTemplate.GCSMWitnessOnIssueFlag,
                    GCSMWitnessOnPrescriptionFlag = formularyTemplate.GCSMWitnessOnPrescriptionFlag,
                    GCSMWitnessOnRecallFlag = formularyTemplate.GCSMWitnessOnRecallFlag,
                    GCSMWitnessOnReceiveFlag = formularyTemplate.GCSMWitnessOnReceiveFlag,
                    GCSMWitnessOnReverseCompoundingFlag = formularyTemplate.GCSMWitnessOnReverseCompoundingFlag,
                    GCSMWitnessOnSellFlag = formularyTemplate.GCSMWitnessOnSellFlag,
                    GCSMWitnessOnStockTransferFlag = formularyTemplate.GCSMWitnessOnStockTransferFlag,
                    GCSMWitnessOnUnloadFlag = formularyTemplate.GCSMWitnessOnUnloadFlag,
                    GCSMWitnessOnWasteFlag = formularyTemplate.GCSMWitnessOnWasteFlag,
                    GCSMScanOnCompoundingFlag = formularyTemplate.GCSMScanOnCompoundingFlag,
                    GCSMScanOnReturnFlag = formularyTemplate.GCSMScanOnReturnFlag,
                    GCSMScanOnAutorestockFlag = formularyTemplate.GCSMScanOnAutorestockFlag,
                    GCSMScanOnIssueFlag = formularyTemplate.GCSMScanOnIssueFlag,
                    GCSMScanOnPrescriptionFlag = formularyTemplate.GCSMScanOnPrescriptionFlag,
                    GCSMScanOnReceiveFlag = formularyTemplate.GCSMScanOnReceiveFlag,
                    GCSMScanOnSellFlag = formularyTemplate.GCSMScanOnSellFlag,
                    GCSMScanOnStockTransferFlag = formularyTemplate.GCSMScanOnStockTransferFlag,
                    GCSMCriticalLowPercentage = formularyTemplate.GCSMCriticalLowPercentage,
                    GCSMPrintOnReceiveFlag = formularyTemplate.GCSMPrintOnReceiveFlag,
                    GCSMPrintSingleMedSheetFlag = formularyTemplate.GCSMPrintSingleMedSheetFlag,
                    GCSMPrintDripSheetFlag = formularyTemplate.GCSMPrintDripSheetFlag,
                    GCSMPrintMaximumRowsFlag = formularyTemplate.GCSMPrintMaximumRowsFlag,
                    GCSMAdditionalLabelsPrintedQuantity = formularyTemplate.GCSMAdditionalLabelsPrintedQuantity,
                    GCSMStockOutNoticeFlag = formularyTemplate.GCSMStockOutNoticeFlag,
                    GCSMADMDispenseQuantity = formularyTemplate.GCSMADMDispenseQuantity,
                    GCSMTotalDeviceParDurationAmount = formularyTemplate.GCSMTotalDeviceParDurationAmount,
                    GCSMTotalParDurationAmount = formularyTemplate.GCSMTotalParDurationAmount,
                    GCSMDistributorPackageSizeQuantity = formularyTemplate.GCSMDistributorPackageSizeQuantity,
                    GCSMDistributorKey = formularyTemplate.GCSMDistributorKey,
                    GCSMRequireOriginDestinationFlag = formularyTemplate.GCSMRequireOriginDestinationFlag,
                    MedClassKey = formularyTemplate.MedicationClassKey
                };

                using (ITransactionScope tx = TransactionScopeFactory.Create())
                { 
                    formularyTemplateKey = _formularyTemplateRepo.InsertFormularyTemplate(context.ToActionContext(), model);

                    if (formularyTemplateKey.HasValue)
                    {
                        if (formularyTemplate.OverrideGroups != null)
                        {
                            InsertFormularyTemplateOverrideGroups(
                                context,
                                formularyTemplateKey.Value,
                                formularyTemplate.OverrideGroups);
                        }
                    }

                    tx.Complete();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return formularyTemplateKey.GetValueOrDefault();
        }

        void IItemRepository.UpdateFormularyTemplate(Context context, FormularyTemplate formularyTemplate)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(formularyTemplate, "formularyTemplate");

            try
            {
                var model = new ItemDAL.Models.FormularyTemplate()
                {
                    ExternalSystemKey = formularyTemplate.ExternalSystemKey,
                    FormularyTemplateName = formularyTemplate.Name,
                    DescriptionText = formularyTemplate.Description,
                    OutdateTrackingFlag = formularyTemplate.OutdateTracking,
                    VerifyCountModeInternalCode = formularyTemplate.VerifyCountMode.ToInternalCode(),
                    VerifyCountAnesthesiaDispensingFlag = formularyTemplate.VerifyCountAnesthesiaDispensing,
                    WitnessOnDispenseFlag = formularyTemplate.WitnessOnDispense,
                    WitnessOnReturnFlag = formularyTemplate.WitnessOnReturn,
                    WitnessOnDisposeFlag = formularyTemplate.WitnessOnDispose,
                    WitnessOnLoadRefillFlag = formularyTemplate.WitnessOnLoadRefill,
                    WitnessOnUnloadFlag = formularyTemplate.WitnessOnUnload,
                    WitnessOnOverrideFlag = formularyTemplate.WitnessOnOverride,
                    WitnessOnOutdateFlag = formularyTemplate.WitnessOnOutdate,
                    WitnessOnInventoryFlag = formularyTemplate.WitnessOnInventory,
                    WitnessOnEmptyReturnBinFlag = formularyTemplate.WitnessOnEmptyReturnBin,
                    WitnessOnDestockFlag = formularyTemplate.WitnessOnDestock,
                    WitnessOnRxCheckFlag = formularyTemplate.WitnessOnRxCheck,
                    ScanOnLoadRefillFlag = formularyTemplate.ScanOnLoadRefill,
                    ScanOnRemoveFlag = formularyTemplate.ScanOnRemove,
                    ScanOnReturnFlag = formularyTemplate.ScanOnReturn,
                    ScanOnCheckFlag = formularyTemplate.ScanOnCheck,
                    DoOnceOnRemoveFlag = formularyTemplate.DoOnceOnRemove,
                    RequireLotIdOnRemoveFlag = formularyTemplate.RequireLotIdOnRemove,
                    RequireLotIdOnReturnFlag = formularyTemplate.RequireLotIdOnReturn,
                    RequireSerialIdOnRemoveFlag = formularyTemplate.RequireSerialIdOnRemove,
                    RequireSerialIdOnReturnFlag = formularyTemplate.RequireSerialIdOnReturn,
                    RequireExpirationDateOnRemoveFlag = formularyTemplate.RequireExpirationDateOnRemove,
                    RequireExpirationDateOnReturnFlag = formularyTemplate.RequireExpirationDateOnReturn,
                    RequireInventoryReferenceIdFlag = formularyTemplate.RequireInventoryReferenceId,
                    ReverificationFlag = formularyTemplate.Reverification,
                    TooCloseRemoveDurationAmount = formularyTemplate.TooCloseRemoveDuration,
                    SecurityGroupKey = formularyTemplate.SecurityGroupKey,
                    ActiveFlag = formularyTemplate.IsActive,
                    AutoResolveDiscrepancyFlag = formularyTemplate.AutoResolveDiscrepancy,
                    ChargeableFlag = formularyTemplate.IsChargeable,
                    HighCostFlag = formularyTemplate.IsHighCost,
                    AllowSplittingFlag = formularyTemplate.AllowSplitting,
                    HighRiskFlag = formularyTemplate.IsHighRisk,
                    TrackInventoryQuantityFlag = formularyTemplate.TrackInventoryQuantity = true,
                    MultiDoseFlag = formularyTemplate.IsMultiDose,
                    BackorderedFlag = formularyTemplate.IsBackOrdered,
                    MedReturnModeInternalCode = formularyTemplate.MedReturnMode.ToInternalCode(),
                    MedFailoverReturnModeInternalCode = formularyTemplate.MedFailoverReturnMode.ToInternalCode(),
                    ReplenishmentScanModeInternalCode = formularyTemplate.ReplenishmentScanMode.ToInternalCode(),
                    FractionalUOMTypeInternalCode = formularyTemplate.FractionalUnitOfMeasureType.ToInternalCode(),
                    AutoMedLabelModeInternalCode = formularyTemplate.AutoMedLabelMode.ToInternalCode(),
                    PharmacyNotesText = formularyTemplate.PharmacyNotes,
                    NursingNotesText = formularyTemplate.NursingNotes,
                    CriticalLowPercentage = formularyTemplate.CriticalLowPercentage,
                    StockOutNoticeFlag = formularyTemplate.StockOutNotice,
                    OMNLNoticeFlag = formularyTemplate.OmnlNotice,
                    AnesthesiaMyItemsFlag = formularyTemplate.AnesthesiaMyItems,
                    ResolveUndocumentedWasteFlag = formularyTemplate.ResolveUndocumentedWaste,
                    DistributorKey = formularyTemplate.DistributorKey,
                    RxCheckFlag = formularyTemplate.RxCheck,
                    PrintOnRemoveFlag = formularyTemplate.PrintOnRemove,
                    PrintOnReturnFlag = formularyTemplate.PrintOnReturn,
                    PrintOnDisposeFlag = formularyTemplate.PrintOnDispose,
                    PrintOnLoadRefillFlag = formularyTemplate.PrintOnLoadRefill,
                    ShowConversionOnRemoveFlag = formularyTemplate.ShowConversionOnRemove,
                    ScanAllOnPickFlag = formularyTemplate.ScanAllOnPick,
                    CountCubieEjectModeInternalCode = formularyTemplate.CountCubieEjectMode.ToInternalCode(),
                    PharmacyOrderDispenseQuantityFlag = formularyTemplate.PharmacyOrderDispenseQuantityFlag,
                    InjectableFlag = formularyTemplate.InjectableFlag,
                    GCSMMedReturnModeInternalCode = formularyTemplate.GCSMMedReturnModeInternalCode.ToInternalCode(),
                    GCSMVerifyCountModeInternalCode = formularyTemplate.GCSMVerifyCountModeInternalCode.ToInternalCode(),
                    GCSMCountCUBIEEjectModeInternalCode = formularyTemplate.GCSMCountCUBIEEjectModeInternalCode.ToInternalCode(),
                    GCSMOutdateTrackingFlag = formularyTemplate.GCSMOutdateTrackingFlag,
                    GCSMRequireLotNumberWhenRecallFlag = formularyTemplate.GCSMRequireLotNumberWhenRecallFlag,
                    GCSMRequireInventoryReferenceNumberFlag = formularyTemplate.GCSMRequireInventoryReferenceNumberFlag,
                    GCSMWitnessOnAccessToDestructionBinFlag = formularyTemplate.GCSMWitnessOnAccessToDestructionBinFlag,
                    GCSMWitnessOnAddToDestructionBinFlag = formularyTemplate.GCSMWitnessOnAddToDestructionBinFlag,
                    GCSMWitnessOnOutdateFlag = formularyTemplate.GCSMWitnessOnOutdateFlag,
                    GCSMWitnessOnReturnFlag = formularyTemplate.GCSMWitnessOnReturnFlag,
                    GCSMWitnessOnAutorestockFlag = formularyTemplate.GCSMWitnessOnAutorestockFlag,
                    GCSMWitnessOnCompoundingFlag = formularyTemplate.GCSMWitnessOnCompoundingFlag,
                    GCSMWitnessOnDestructionBinFlag = formularyTemplate.GCSMWitnessOnDestructionBinFlag,
                    GCSMWitnessOnDiscrepancyResolutionFlag = formularyTemplate.GCSMWitnessOnDiscrepancyResolutionFlag,
                    GCSMWitnessOnInventoryCountFlag = formularyTemplate.GCSMWitnessOnInventoryCountFlag,
                    GCSMWitnessOnIssueFlag = formularyTemplate.GCSMWitnessOnIssueFlag,
                    GCSMWitnessOnPrescriptionFlag = formularyTemplate.GCSMWitnessOnPrescriptionFlag,
                    GCSMWitnessOnRecallFlag = formularyTemplate.GCSMWitnessOnRecallFlag,
                    GCSMWitnessOnReceiveFlag = formularyTemplate.GCSMWitnessOnReceiveFlag,
                    GCSMWitnessOnReverseCompoundingFlag = formularyTemplate.GCSMWitnessOnReverseCompoundingFlag,
                    GCSMWitnessOnSellFlag = formularyTemplate.GCSMWitnessOnSellFlag,
                    GCSMWitnessOnStockTransferFlag = formularyTemplate.GCSMWitnessOnStockTransferFlag,
                    GCSMWitnessOnUnloadFlag = formularyTemplate.GCSMWitnessOnUnloadFlag,
                    GCSMWitnessOnWasteFlag = formularyTemplate.GCSMWitnessOnWasteFlag,
                    GCSMScanOnCompoundingFlag = formularyTemplate.GCSMScanOnCompoundingFlag,
                    GCSMScanOnReturnFlag = formularyTemplate.GCSMScanOnReturnFlag,
                    GCSMScanOnAutorestockFlag = formularyTemplate.GCSMScanOnAutorestockFlag,
                    GCSMScanOnIssueFlag = formularyTemplate.GCSMScanOnIssueFlag,
                    GCSMScanOnPrescriptionFlag = formularyTemplate.GCSMScanOnPrescriptionFlag,
                    GCSMScanOnReceiveFlag = formularyTemplate.GCSMScanOnReceiveFlag,
                    GCSMScanOnSellFlag = formularyTemplate.GCSMScanOnSellFlag,
                    GCSMScanOnStockTransferFlag = formularyTemplate.GCSMScanOnStockTransferFlag,
                    GCSMCriticalLowPercentage = formularyTemplate.GCSMCriticalLowPercentage,
                    GCSMPrintOnReceiveFlag = formularyTemplate.GCSMPrintOnReceiveFlag,
                    GCSMPrintSingleMedSheetFlag = formularyTemplate.GCSMPrintSingleMedSheetFlag,
                    GCSMPrintDripSheetFlag = formularyTemplate.GCSMPrintDripSheetFlag,
                    GCSMPrintMaximumRowsFlag = formularyTemplate.GCSMPrintMaximumRowsFlag,
                    GCSMAdditionalLabelsPrintedQuantity = formularyTemplate.GCSMAdditionalLabelsPrintedQuantity,
                    GCSMStockOutNoticeFlag = formularyTemplate.GCSMStockOutNoticeFlag,
                    GCSMADMDispenseQuantity = formularyTemplate.GCSMADMDispenseQuantity,
                    GCSMTotalDeviceParDurationAmount = formularyTemplate.GCSMTotalDeviceParDurationAmount,
                    GCSMTotalParDurationAmount = formularyTemplate.GCSMTotalParDurationAmount,
                    GCSMDistributorPackageSizeQuantity = formularyTemplate.GCSMDistributorPackageSizeQuantity,
                    GCSMDistributorKey = formularyTemplate.GCSMDistributorKey,
                    GCSMRequireOriginDestinationFlag = formularyTemplate.GCSMRequireOriginDestinationFlag,
                    LastModifiedBinaryValue = formularyTemplate.LastModified ?? new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 },
                    FormularyTemplateKey = formularyTemplate.Key,
                    MedClassKey = formularyTemplate.MedicationClassKey
                };

                using (ITransactionScope tx = TransactionScopeFactory.Create())
                {
                    _formularyTemplateRepo.UpdateFormularyTemplate(context.ToActionContext(), model);

                    UpdateFormularyTemplateOverrideGroups(
                        context,
                        formularyTemplate.Key,
                        formularyTemplate.OverrideGroups ?? new Guid[0]);

                    tx.Complete();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void IItemRepository.DeleteFormularyTemplate(Context context, Guid formularyTemplateKey)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                _formularyTemplateRepo.DeleteFormularyTemplate(context.ToActionContext(), formularyTemplateKey);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        #endregion

        #region ItemScanCode Members

        IEnumerable<ItemScanCode> IItemRepository.GetItemScanCodes(IEnumerable<Guid> itemScanCodeKeys, bool? deleted, string scanCodeValue)
        {
            IEnumerable<ItemScanCode> itemScanCodes = new List<ItemScanCode>();
            if (itemScanCodeKeys != null && !itemScanCodeKeys.Any())
                return itemScanCodes; // Empty results

            try
            {
                GuidKeyTable selectedKeys = new GuidKeyTable();
                if (itemScanCodeKeys != null)
                    selectedKeys = new GuidKeyTable(itemScanCodeKeys.Distinct());

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var itemScanCodeResults = connectionScope.Query<ItemScanCodesResult>(
                    "Item.bsp_GetItemScanCodes",
                    new
                    {
                        SelectedKeys = selectedKeys.AsTableValuedParameter(),
                        DeleteFlag = deleted,
                        ScanCodeValue = scanCodeValue
                    },
                    commandTimeout: connectionScope.DefaultCommandTimeout,
                    commandType: CommandType.StoredProcedure);

                    // Get our item objects
                    IEnumerable<Guid> itemKeys = itemScanCodeResults.Select(isc => isc.ItemKey).Distinct();
                    IDictionary<Guid, Item> items = ((IItemRepository)this).GetItems(itemKeys)
                        .ToDictionary(i => i.Key);

                    itemScanCodes = itemScanCodeResults
                        .Select(result => new ItemScanCode(result.ItemScanCodeKey)
                        {
                            ScanCode = result.ScanCodeValue,
                            Item = items.ContainsKey(result.ItemKey)
                                ? items[result.ItemKey] : new Item(result.ItemKey),
                            LinkedByUserAccountKey = result.LinkedByUserAccountKey,
                            LinkedByUserAccountFullName = result.LinkedByUserAccountFullName,
                            LinkedByUserAccountUserId = result.LinkedByUserAccountUserID,
                            LinkedDateTime = result.LinkedLocalDateTime,
                            LinkedUtcDateTime = result.LinkedUTCDateTime,
                            VerifiedByUserAccountKey = result.VerifiedByUserAccountKey,
                            VerifiedByUserAccountFullName = result.VerifiedByUserAccountFullName,
                            VerifiedByUserAccountUserId = result.VerifiedByUserAccountUserID,
                            VerifiedDateTime = result.VerifiedLocalDateTime,
                            VerifiedUtcDateTime = result.VerifiedUTCDateTime,
                            FromExternalSystem = result.FromExternalSystemFlag,
                            ScanProductDeleteReason = result.ScanProductDeleteReasonInternalCode.FromNullableInternalCode<ScanProductDeleteReasonInternalCode>(),
                            OtherItemId = result.OtherItemID,
                            CreatedByExternalSystemName = result.CreatedByExternalSystemName,
                            DeletedByExternalSystemName = result.DeletedByExternalSystemName,
                            LastModified = result.LastModifiedBinaryValue
                        })
                        .ToArray();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return itemScanCodes;
        }

        ItemScanCode IItemRepository.GetItemScanCode(Guid itemScanCodeKey)
        {
            IEnumerable<ItemScanCode> itemScanCodes = 
                ((IItemRepository) this).GetItemScanCodes(new [] { itemScanCodeKey });

            return itemScanCodes.FirstOrDefault();
        }

        Guid IItemRepository.InsertItemScanCode(Context context, ItemScanCode itemScanCode)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(itemScanCode, "itemScanCode");
            Guid? itemScanCodeKey = null;

            try
            {
                var model = new ItemDAL.Models.ItemScanCode()
                {
                    ItemKey = itemScanCode.Item != null ? itemScanCode.Item.Key : default(Guid),
                    ScanCodeValue = itemScanCode.ScanCode,
                    LinkedByUserAccountKey = itemScanCode.LinkedByUserAccountKey,
                    LinkedUtcDateTime = itemScanCode.LinkedUtcDateTime,
                    LinkedLocalDateTime = itemScanCode.LinkedDateTime,
                    VerifiedByUserAccountKey = itemScanCode.VerifiedByUserAccountKey,
                    VerifiedUtcDateTime = itemScanCode.VerifiedUtcDateTime,
                    VerifiedLocalDateTime = itemScanCode.VerifiedDateTime,
                    FromExternalSystemFlag = itemScanCode.FromExternalSystem,
                    ScanProductDeleteReasonInternalCode = itemScanCode.ScanProductDeleteReason.ToInternalCode(),
                    OtherItemId = itemScanCode.OtherItemId,
                    CreatedByExternalSystemName = itemScanCode.CreatedByExternalSystemName,
                    DeleteFlag = itemScanCode.Deleted,
                };

                itemScanCodeKey = _itemRepo.InsertItemScanCode(context.ToActionContext(), model);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return itemScanCodeKey.GetValueOrDefault();
        }

        void IItemRepository.UpdateItemScanCode(Context context, ItemScanCode itemScanCode)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(itemScanCode, "itemScanCode");

            try
            {
                var model = new ItemDAL.Models.ItemScanCode()
                {
                    ItemKey = itemScanCode.Item != null ? itemScanCode.Item.Key : default(Guid),
                    ScanCodeValue = itemScanCode.ScanCode,
                    LinkedByUserAccountKey = itemScanCode.LinkedByUserAccountKey,
                    LinkedUtcDateTime = itemScanCode.LinkedUtcDateTime,
                    LinkedLocalDateTime = itemScanCode.LinkedDateTime,
                    VerifiedByUserAccountKey = itemScanCode.VerifiedByUserAccountKey,
                    VerifiedUtcDateTime = itemScanCode.VerifiedUtcDateTime,
                    VerifiedLocalDateTime = itemScanCode.VerifiedDateTime,
                    FromExternalSystemFlag = itemScanCode.FromExternalSystem,
                    ScanProductDeleteReasonInternalCode = itemScanCode.ScanProductDeleteReason.ToInternalCode(),
                    OtherItemId = itemScanCode.OtherItemId,
                    CreatedByExternalSystemName = itemScanCode.CreatedByExternalSystemName,
                    LastModifiedBinaryValue = itemScanCode.LastModified ?? new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 },
                    ItemScanCodeKey = itemScanCode.Key
                };

                _itemRepo.UpdateItemScanCode(context.ToActionContext(), model);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void IItemRepository.DeleteItemScanCode(Context context, Guid itemScanCodeKey, ScanProductDeleteReasonInternalCode? scanProductDeleteReason,
            string otherItemId, string deletedByExternalSystemName)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                _itemRepo.DeleteItemScanCode(context.ToActionContext(), itemScanCodeKey, scanProductDeleteReason?.ToInternalCode(), deletedByExternalSystemName, otherItemId);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        IEnumerable<ItemScanCodeLog> IItemRepository.GetScanCodeLog(string scanCode, Guid facilityKey)
        {
            Guard.ArgumentNotNullOrEmptyString(scanCode, "scanCodeValue");

            IEnumerable<ItemScanCodeLog> log = null;

            try
            {
                var parameters = new DynamicParameters(new Dictionary<string, object>{
                        { "@FacilityKey", facilityKey },
                        { "@ScanCodeValue", scanCode },
                });

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var result = connectionScope.Query<ListRejectedDeletedItemScanCodesResult>(
                     "Item.bsp_ListRejectedDeletedItemScanCodes",
                     parameters,
                     commandTimeout: connectionScope.DefaultCommandTimeout,
                     commandType: CommandType.StoredProcedure);

                    log = result.Select(l => new ItemScanCodeLog
                    {
                        FromExternalSystemFlag = l.FromExternalSystemFlag,
                        ItemDisplayName = l.MedDisplayName,
                        ItemScanCodeKey = l.ItemScanCodeKey,
                        LinkedByUserAccountKey = l.LinkedByUserAccountKey,
                        LinkedUtcDateTime = l.LinkedUTCDateTime,
                        OtherItemId = l.OtherItemID,
                        ScanCode = l.ScanCodeValue,
                        ScanProductDeleteReasonInternalCode = l.ScanProductDeleteReasonInternalCode.FromInternalCode<ScanProductDeleteReasonInternalCode>(),
                        StartUtcDateTime = l.StartUTCDateTime,
                        VerifiedByUserAccountKey = l.VerifiedByUserAccountKey,
                        VerifiedUtcDateTime = l.VerifiedUTCDateTime,
                        ItemId = l.ItemID,
                        ScanCodeDeleteReasonDescription = l.ScanProductDeleteReasonDescription

                    }).ToList();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return log;
        }

        IEnumerable<ItemScanCodeLog> IItemRepository.GetProductIdLog(string productId, Guid facilityKey)
        {
            Guard.ArgumentNotNullOrEmptyString(productId, "productId");

            IEnumerable<ItemScanCodeLog> log = null;

            try
            {
                var parameters = new DynamicParameters(new Dictionary<string, object>{
                        { "@FacilityKey", facilityKey },
                        { "@ProductID", productId },
                });

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var results = connectionScope.Query<ListRejectedDeletedItemProductIDsResult>(
                     "Item.bsp_ListRejectedDeletedItemProductIDs",
                     parameters,
                     commandTimeout: connectionScope.DefaultCommandTimeout,
                     commandType: CommandType.StoredProcedure);

                    log = results.Select(l => new ItemScanCodeLog
                    {
                        FromExternalSystemFlag = l.FromExternalSystemFlag,
                        ItemDisplayName = l.MedDisplayName,
                        ItemScanCodeKey = l.ProductIDKey,
                        LinkedByUserAccountKey = l.LinkedByUserAccountKey,
                        LinkedUtcDateTime = l.LinkedUTCDateTime,
                        OtherItemId = l.OtherItemID,
                        ScanCode = l.ProductID,
                        ScanProductDeleteReasonInternalCode = l.ScanProductDeleteReasonInternalCode.FromInternalCode<ScanProductDeleteReasonInternalCode>(),
                        StartUtcDateTime = l.StartUTCDateTime,
                        VerifiedByUserAccountKey = l.VerifiedByUserAccountKey,
                        VerifiedUtcDateTime = l.VerifiedUTCDateTime,
                        ItemId = l.ItemID,
                        ScanCodeDeleteReasonDescription = l.ScanProductDeleteReasonDescription
                    }).ToList();

                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return log;
        }

        #endregion

        #region ItemProductId Members

        ItemProductId IItemRepository.GetItemProductId(Guid itemProductIdKey)
        {
            ItemProductId itemProductId = null;

            try
            {
                ItemDAL.Models.ProductIdentification product = _itemRepo.GetProductIdentification(itemProductIdKey);
                itemProductId = new ItemProductId()
                {
                    Key = product.ProductIdKey,
                    ItemKey = product.ItemKey,
                    ProductId = product.ProductId,
                    LinkedByUserAccountKey = product.LinkedByUserAccountKey,
                    LinkedUtcDateTime = product.LinkedUtcDateTime,
                    LinkedDateTime = product.LinkedLocalDateTime,
                    VerifiedByUserAccountKey = product.VerifiedByUserAccountKey,
                    VerifiedUtcDateTime = product.VerifiedUtcDateTime,
                    VerifiedDateTime = product.VerifiedLocalDateTime,
                    FromExternalSystem = product.FromExternalSystemFlag,
                    ScanProductDeleteReason = product.ScanProductDeleteReasonInternalCode?.FromInternalCode<ScanProductDeleteReasonInternalCode>(),
                    OtherItemId = product.OtherItemId,
                    CreatedByExternalSystemName = product.CreatedByExternalSystemName,
                    DeletedByExternalSystemName = product.DeletedByExternalSystemName,
                    Deleted = product.DeleteFlag,
                    LastModified = product.LastModifiedBinaryValue,
                };
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return itemProductId;
        }

        Guid IItemRepository.InsertItemProductId(Context context, ItemProductId itemProductId)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(itemProductId, "itemProductId");
            Guid? itemProductIdKey = null;

            try
            {
                var productIdentification = new ItemDAL.Models.ProductIdentification()
                {
                    ProductIdKey = itemProductId.Key,
                    ItemKey = itemProductId.ItemKey,
                    ProductId = itemProductId.ProductId,
                    LinkedByUserAccountKey = itemProductId.LinkedByUserAccountKey,
                    LinkedUtcDateTime = itemProductId.LinkedUtcDateTime,
                    LinkedLocalDateTime = itemProductId.LinkedDateTime,
                    VerifiedByUserAccountKey = itemProductId.VerifiedByUserAccountKey,
                    VerifiedUtcDateTime = itemProductId.VerifiedUtcDateTime,
                    VerifiedLocalDateTime = itemProductId.VerifiedDateTime,
                    FromExternalSystemFlag = itemProductId.FromExternalSystem,
                    ScanProductDeleteReasonInternalCode = itemProductId.ScanProductDeleteReason?.ToInternalCode(),
                    OtherItemId = itemProductId.OtherItemId,
                    CreatedByExternalSystemName = itemProductId.CreatedByExternalSystemName,
                    DeletedByExternalSystemName = itemProductId.DeletedByExternalSystemName,
                    DeleteFlag = itemProductId.Deleted,
                    LastModifiedBinaryValue = itemProductId.LastModified,
                };
                itemProductIdKey = _itemRepo.InsertProductIdentification(context.ToActionContext(), productIdentification);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return itemProductIdKey.GetValueOrDefault();
        }

        void IItemRepository.UpdateItemProductId(Context context, ItemProductId itemProductId)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(itemProductId, "itemProductId");

            try
            {
                var productIdentification = new ItemDAL.Models.ProductIdentification()
                {
                    ProductIdKey = itemProductId.Key,
                    ItemKey = itemProductId.ItemKey,
                    ProductId = itemProductId.ProductId,
                    LinkedByUserAccountKey = itemProductId.LinkedByUserAccountKey,
                    LinkedUtcDateTime = itemProductId.LinkedUtcDateTime,
                    LinkedLocalDateTime = itemProductId.LinkedDateTime,
                    VerifiedByUserAccountKey = itemProductId.VerifiedByUserAccountKey,
                    VerifiedUtcDateTime = itemProductId.VerifiedUtcDateTime,
                    VerifiedLocalDateTime = itemProductId.VerifiedDateTime,
                    FromExternalSystemFlag = itemProductId.FromExternalSystem,
                    ScanProductDeleteReasonInternalCode = itemProductId.ScanProductDeleteReason?.ToInternalCode(),
                    OtherItemId = itemProductId.OtherItemId,
                    CreatedByExternalSystemName = itemProductId.CreatedByExternalSystemName,
                    DeletedByExternalSystemName = itemProductId.DeletedByExternalSystemName,
                    DeleteFlag = itemProductId.Deleted,
                    LastModifiedBinaryValue = itemProductId.LastModified,
                };
                _itemRepo.UpdateProductIdentification(context.ToActionContext(), productIdentification);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void IItemRepository.DeleteItemProductId(Context context, Guid itemProductIdKey, ScanProductDeleteReasonInternalCode? scanProductDeleteReason,
            string otherItemId, string deletedByExternalSystemName)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                _itemRepo.DeleteProductIdentification(context.ToActionContext(), itemProductIdKey, scanProductDeleteReason?.ToInternalCode(), deletedByExternalSystemName, otherItemId);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }
        #endregion

        #region DosageForm Members

        DosageForm IItemRepository.GetDosageForm(Guid dosageFormKey)
        {
            DosageForm dosageForm = null;

            try
            {
                var sql = new SqlBuilder();
                sql.SELECT("df.ExternalSystemKey")
                    ._("df.DosageFormCode AS Code")
                    ._("df.DescriptionText AS Description")
                    ._("df.EquivalencyDosageFormGroupKey")
                    ._("df.DosageFormKey AS [Key]")
                    ._("df.SortValue AS SortOrder")
                    ._("df.LastModifiedBinaryValue AS LastModified")
                    ._("esc.ExternalSystemName AS ExternalSystemName")
                    .FROM("Item.DosageForm df")
                    .LEFT_JOIN("Core.vw_ExternalSystemCurrent esc ON esc.ExternalSystemKey = df.ExternalSystemKey")
                    .WHERE("df.DeleteFlag = 0")
                    .WHERE("df.DosageFormKey = @DosageFormKey");

                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    dosageForm = connectionScope.Query<DosageForm>(
                         sql.ToString(),
                         new { DosageFormKey = dosageFormKey },
                         commandTimeout: connectionScope.DefaultCommandTimeout,
                         commandType: CommandType.Text)
                         .FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return dosageForm;
        }

        DosageForm IItemRepository.GetDosageForm(Guid externalSystemKey, string dosageFormCode, LoadOptions loadOptions)
        {
            DosageForm dosageForm = null;

            try
            {
                var sql = new SqlBuilder();
                sql.SELECT("df.ExternalSystemKey")
                    ._("df.DosageFormCode AS Code")
                    ._("df.DescriptionText AS Description")
                    ._("df.EquivalencyDosageFormGroupKey")
                    ._("df.SortValue AS SortOrder")
                    ._("df.LastModifiedBinaryValue AS LastModified")
                    ._("df.DosageFormKey AS [Key]")
                    ._("esc.ExternalSystemName AS ExternalSystemName")
                    .FROM("Item.DosageForm df")
                    .LEFT_JOIN("Core.vw_ExternalSystemCurrent esc ON esc.ExternalSystemKey = df.ExternalSystemKey")
                    .WHERE("df.DeleteFlag = 0")
                    .WHERE("df.ExternalSystemKey = @ExternalSystemKey")
                    .WHERE("df.DosageFormCode = @DosageFormCode");

                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    dosageForm = connectionScope.Query<DosageForm>(
                         sql.ToString(),
                         new {
                             ExternalSystemKey = externalSystemKey,
                             DosageFormCode = dosageFormCode,
                            },
                         commandTimeout: connectionScope.DefaultCommandTimeout,
                         commandType: CommandType.Text)
                         .FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return dosageForm;
        }

        Guid IItemRepository.InsertDosageForm(Context context, DosageForm dosageForm)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(dosageForm, "dosageForm");
            Guid? dosageFormKey = null;

            try
            {
                dosageFormKey = _dosageFormRepo.InsertDosageForm(context.ToActionContext(),
                    new ItemDAL.Models.DosageForm
                    {
                        DosageFormKey = dosageForm.Key,
                        ExternalSystemKey = dosageForm.ExternalSystemKey,
                        DosageFormCode = dosageForm.Code,
                        DescriptionText = dosageForm.Description,
                        EquivalencyDosageFormGroupKey = dosageForm.EquivalencyDosageFormGroupKey,
                        SortValue = dosageForm.SortOrder,
                        LastModifiedBinaryValue = dosageForm.LastModified
                    });
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return dosageFormKey.GetValueOrDefault();
        }

        void IItemRepository.UpdateDosageForm(Context context, DosageForm dosageForm)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(dosageForm, "dosageForm");

            try
            {
                _dosageFormRepo.UpdateDosageForm(context.ToActionContext(),
                    new ItemDAL.Models.DosageForm
                    {
                        DosageFormKey = dosageForm.Key,
                        ExternalSystemKey = dosageForm.ExternalSystemKey,
                        DosageFormCode = dosageForm.Code,
                        DescriptionText = dosageForm.Description,
                        EquivalencyDosageFormGroupKey = dosageForm.EquivalencyDosageFormGroupKey,
                        SortValue = dosageForm.SortOrder,
                        LastModifiedBinaryValue = dosageForm.LastModified
                    });
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void IItemRepository.DeleteDosageForm(Context context, Guid dosageFormKey)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                _dosageFormRepo.DeleteDosageForm(context.ToActionContext(), dosageFormKey);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        EquivalencyDosageFormGroup IItemRepository.GeEquivalencyDosageFormGroup(Guid equivalencyDosageFormGroupKey)
        {
            EquivalencyDosageFormGroup equivalencyDosageFormGroup = null;

            try
            {
                var equivalencyDosageFormGroupModel = _equivalencyDosageFormRepo.GetEquivalencyDosageFormGroup(equivalencyDosageFormGroupKey);
                if(equivalencyDosageFormGroupModel != null)
                {
                    equivalencyDosageFormGroup = new EquivalencyDosageFormGroup()
                    {
                        Key = equivalencyDosageFormGroupModel.EquivalencyDosageFormGroupKey,
                        DisplayCode = equivalencyDosageFormGroupModel.DisplayCode,
                        InternalCode = equivalencyDosageFormGroupModel.InternalCode.FromNullableInternalCode<EquivalencyDosageFormGroupInternalCode>(),
                        Description = equivalencyDosageFormGroupModel.DescriptionText,
                        SortOrder = equivalencyDosageFormGroupModel.SortValue,
                        LastModified = equivalencyDosageFormGroupModel.LastModifiedBinaryValue,
                    };
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return equivalencyDosageFormGroup;
        }

        Guid IItemRepository.InsertEquivalencyDosageFormGroup(Context context, EquivalencyDosageFormGroup equivalencyDosageFormGroup)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(equivalencyDosageFormGroup, "equivalencyDosageFormGroup");
            Guid? equivalencyDosageFormGroupKey = null;

            try
            {
                var edfgModel = new ItemDAL.Models.EquivalencyDosageFormGroup()
                {
                    DisplayCode = equivalencyDosageFormGroup.DisplayCode,
                    DescriptionText = equivalencyDosageFormGroup.Description,
                    SortValue = equivalencyDosageFormGroup.SortOrder,
                };
                equivalencyDosageFormGroupKey = _equivalencyDosageFormRepo.InsertEquivalencyDosageFormGroup(context.ToActionContext(), edfgModel);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return equivalencyDosageFormGroupKey.GetValueOrDefault();
        }

        void IItemRepository.UpdateEquivalencyDosageFormGroup(Context context, EquivalencyDosageFormGroup equivalencyDosageFormGroup)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(equivalencyDosageFormGroup, "equivalencyDosageFormGroup");

            try
            {
                var edfgModel = new ItemDAL.Models.EquivalencyDosageFormGroup()
                {
                    DisplayCode = equivalencyDosageFormGroup.DisplayCode,
                    DescriptionText = equivalencyDosageFormGroup.Description,
                    SortValue = equivalencyDosageFormGroup.SortOrder,
                    LastModifiedBinaryValue = equivalencyDosageFormGroup.LastModified,
                    EquivalencyDosageFormGroupKey = equivalencyDosageFormGroup.Key
                };

                _equivalencyDosageFormRepo.UpdateEquivalencyDosageFormGroup(context.ToActionContext(), edfgModel);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void IItemRepository.DeleteEquivalencyDosageFormGroup(Context context, Guid equivalencyDosageFormGroupKey)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                _equivalencyDosageFormRepo.DeleteEquivalencyDosageFormGroup(context.ToActionContext(), equivalencyDosageFormGroupKey);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        #endregion

        #region HazardousWasteClass Members
        HazardousWasteClass IItemRepository.GetHazardousWasteClass(Guid hazardousWasteClassKey)
        {
            HazardousWasteClass hazardousWasteClass = null;

            try
            {
                ItemDAL.Models.HazardousWasteClass model = _hazardousWasteClassRepo.GetHazardousWasteClass(hazardousWasteClassKey);
                if(model != null)
                {
                    hazardousWasteClass = new HazardousWasteClass()
                    {
                        Key = model.HazardousWasteClassKey,
                        DisplayCode = model.DisplayCode,
                        Description = model.DescriptionText,
                        DisposalInstructions = model.DisposalInstructionsText,
                        FacilityKey = model.FacilityKey,
                        SortOrder = model.SortValue,
                        LastModified = model.LastModifiedBinaryValue
                    };
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return hazardousWasteClass;
        }

        Guid IItemRepository.InsertHazardousWasteClass(Context context, HazardousWasteClass hazardousWasteClass)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(hazardousWasteClass, "hazardousWasteClass");
            Guid? hazardousWasteClassKey = null;

            try
            {
                ItemDAL.Models.HazardousWasteClass model = new ItemDAL.Models.HazardousWasteClass()
                {
                    FacilityKey = hazardousWasteClass.FacilityKey,
                    DisplayCode = hazardousWasteClass.DisplayCode,
                    DescriptionText = hazardousWasteClass.Description,
                    DisposalInstructionsText = hazardousWasteClass.DisposalInstructions,
                    SortValue = hazardousWasteClass.SortOrder,
                };

                hazardousWasteClassKey = _hazardousWasteClassRepo.InsertHazardousWasteClass(context.ToActionContext(), model);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return hazardousWasteClassKey.GetValueOrDefault();
        }

        void IItemRepository.UpdateHazardousWasteClass(Context context, HazardousWasteClass hazardousWasteClass)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(hazardousWasteClass, "hazardousWasteClass");

            try
            {
                ItemDAL.Models.HazardousWasteClass model = new ItemDAL.Models.HazardousWasteClass()
                {
                    HazardousWasteClassKey = hazardousWasteClass.Key,
                    DisplayCode = hazardousWasteClass.DisplayCode,
                    DescriptionText = hazardousWasteClass.Description,
                    DisposalInstructionsText = hazardousWasteClass.DisposalInstructions,
                    SortValue = hazardousWasteClass.SortOrder,
                    LastModifiedBinaryValue = hazardousWasteClass.LastModified
                };

                _hazardousWasteClassRepo.UpdateHazardousWasteClass(context.ToActionContext(), model);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void IItemRepository.DeleteHazardousWasteClass(Context context, Guid hazardousWasteClassKey)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                _hazardousWasteClassRepo.DeleteHazardousWasteClass(context.ToActionContext(), hazardousWasteClassKey);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }
        #endregion

        #region MedicationClass Members

        IEnumerable<MedicationClass> IItemRepository.GetMedicationClasses(IEnumerable<Guid> medicationClassKeys, bool? deleted, 
            Guid? facilityKey, Guid? externalSystemKey, string medClassCode)
        {
            IEnumerable<MedicationClass> medicationClasses = new List<MedicationClass>();
            if (medicationClassKeys != null && !medicationClassKeys.Any())
                return medicationClasses; // Empty results

            try
            {
                GuidKeyTable selectedKeys = new GuidKeyTable();
                if (medicationClassKeys != null)
                    selectedKeys = new GuidKeyTable(medicationClassKeys.Distinct());
                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var medClassResults = connectionScope.Query<MedClassesResult>(
                    "Item.bsp_GetMedClasses",
                    new
                    {
                        SelectedKeys = selectedKeys.AsTableValuedParameter(),
                        DeleteFlag = deleted,
                        FacilityKey = facilityKey,
                        ExternalSystemKey = externalSystemKey,
                        MedClassCode = medClassCode
                    },
                    commandTimeout: connectionScope.DefaultCommandTimeout,
                    commandType: CommandType.StoredProcedure);

                    medicationClasses = medClassResults
                        .Select(result => new MedicationClass(result.MedClassKey)
                        {
                            Code = result.MedClassCode,
                            Description = result.DescriptionText,
                            ExternalSystemKey = result.ExternalSystemKey,
                            ExternalSystemName = result.ExternalSystemName,
                            SortOrder = result.SortValue,
                            IsControlled = result.ControlledFlag,
                            FormularyTemplateKey = result.FormularyTemplateKey,
                            IsDeleted = result.DeleteFlag,
                            LastModified = result.LastModifiedBinaryValue.ToArray()
                        })
                        .ToArray();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return medicationClasses;
        }

        MedicationClass IItemRepository.GetMedicationClass(Guid medicationClassKey)
        {
            IEnumerable<MedicationClass> medicationClasses =
                ((IItemRepository) this).GetMedicationClasses(new [] {medicationClassKey}, false);

            return medicationClasses.FirstOrDefault();
        }

        MedicationClass IItemRepository.GetMedicationClass(Guid externalSystemKey, string medClassCode)
        {
            IEnumerable<MedicationClass> medicationClasses =
                ((IItemRepository)this).GetMedicationClasses(null, false, null, externalSystemKey, medClassCode);

            return medicationClasses.FirstOrDefault();
        }

        Guid IItemRepository.InsertMedicationClass(Context context, MedicationClass medicationClass)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(medicationClass, "medicationClass");
            Guid? medicationClassKey = null;

            try
            {
                ItemDAL.IMedClassRepository medClassRepository = new ItemDAL.MedClassRepository();

                medicationClassKey = medClassRepository.InsertMedClass(context.ToActionContext(),
                    new ItemDAL.Models.MedClass
                    {
                        MedClassKey = medicationClass.Key,
                        ExternalSystemKey = medicationClass.ExternalSystemKey,
                        MedClassCode = medicationClass.Code,
                        DescriptionText = medicationClass.Description,
                        ControlledFlag = medicationClass.IsControlled,
                        FormularyTemplateKey = medicationClass.FormularyTemplateKey,
                        SortValue = medicationClass.SortOrder,
                        LastModifiedBinaryValue = medicationClass.LastModified
                    });
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return medicationClassKey.GetValueOrDefault();
        }

        void IItemRepository.UpdateMedicationClass(Context context, MedicationClass medicationClass)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(medicationClass, "medicationClass");

            try
            {
                ItemDAL.IMedClassRepository medClassRepository = new ItemDAL.MedClassRepository();

                medClassRepository.UpdateMedClass(context.ToActionContext(),
                    new ItemDAL.Models.MedClass
                    {
                        MedClassKey = medicationClass.Key,
                        ExternalSystemKey = medicationClass.ExternalSystemKey,
                        MedClassCode = medicationClass.Code,
                        DescriptionText = medicationClass.Description,
                        ControlledFlag = medicationClass.IsControlled,
                        FormularyTemplateKey = medicationClass.FormularyTemplateKey,
                        SortValue = medicationClass.SortOrder,
                        LastModifiedBinaryValue = medicationClass.LastModified
                    });
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void IItemRepository.DeleteMedicationClass(Context context, Guid medicationClassKey)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                ItemDAL.IMedClassRepository medClassRepository = new ItemDAL.MedClassRepository();

                medClassRepository.DeleteMedClass(context.ToActionContext(), medicationClassKey);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }
        #endregion

        #region SecurityGroup Members

        IEnumerable<SecurityGroup> IItemRepository.GetSecurityGroups()
        {
            IEnumerable<SecurityGroup> securityGroups = null;

            try
            {
                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var query = new SqlBuilder();
                    query.SELECT("*")
                        .FROM("Item.SecurityGroup");
                    var results = connectionScope.Query<ItemDAL.Models.SecurityGroup>(
                       query.ToString(),
                       commandTimeout: connectionScope.DefaultCommandTimeout,
                       commandType: CommandType.Text);

                    securityGroups = results.Select(sg =>
                        new SecurityGroup()
                        {
                            Key = sg.SecurityGroupKey,
                            BusinessDomain = sg.BusinessDomainInternalCode.FromInternalCode<BusinessDomainInternalCode>(),
                            DisplayCode = sg.DisplayCode,
                            Description = sg.DescriptionText,
                            SortOrder = sg.SortValue,
                            IsActive = sg.ActiveFlag,
                            LastModified = sg.LastModifiedBinaryValue
                        }).ToArray();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return securityGroups;
        }

        SecurityGroup IItemRepository.GetSecurityGroup(Guid securityGroupKey)
        {
            SecurityGroup securityGroup = null;

            try
            {
                ItemDAL.Models.SecurityGroup model = _securityGroupRepo.GetSecurityGroup(securityGroupKey);
                if (model != null)
                {
                    securityGroup = new SecurityGroup()
                    {
                        Key = model.SecurityGroupKey,
                        BusinessDomain = model.BusinessDomainInternalCode.FromInternalCode<BusinessDomainInternalCode>(),
                        DisplayCode = model.DisplayCode,
                        Description = model.DescriptionText,
                        SortOrder = model.SortValue,
                        IsActive = model.ActiveFlag,
                        LastModified = model.LastModifiedBinaryValue
                    };
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return securityGroup;
        }

        Guid IItemRepository.InsertSecurityGroup(Context context, SecurityGroup securityGroup)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(securityGroup, "securityGroup");
            Guid? securityGroupKey = null;

            try
            {
                var model = new ItemDAL.Models.SecurityGroup()
                {
                    BusinessDomainInternalCode = securityGroup.BusinessDomain.ToInternalCode(),
                    DisplayCode = securityGroup.DisplayCode,
                    DescriptionText = securityGroup.Description,
                    SortValue = securityGroup.SortOrder,
                    ActiveFlag = securityGroup.IsActive,
                };

                securityGroupKey = _securityGroupRepo.InsertSecurityGroup(context.ToActionContext(), model);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return securityGroupKey.GetValueOrDefault();
        }

        void IItemRepository.UpdateSecurityGroup(Context context, SecurityGroup securityGroup)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(securityGroup, "securityGroup");

            try
            {
                var model = new ItemDAL.Models.SecurityGroup()
                {
                    SecurityGroupKey = securityGroup.Key,
                    DisplayCode = securityGroup.DisplayCode,
                    DescriptionText = securityGroup.Description,
                    SortValue = securityGroup.SortOrder,
                    ActiveFlag = securityGroup.IsActive,
                    LastModifiedBinaryValue = securityGroup.LastModified,
                };

                _securityGroupRepo.UpdateSecurityGroup(context.ToActionContext(), model);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        #endregion

        #region OverrideGroup Members

        IEnumerable<OverrideGroup> IItemRepository.GetOverrideGroups()
        {
            IEnumerable<OverrideGroup> overrideGroups = null;

            try
            {
                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var query = new SqlBuilder();
                    query.SELECT("*")
                        .FROM("Item.OverrideGroup");
                    var results = connectionScope.Query<ItemDAL.Models.OverrideGroup>(
                       query.ToString(),
                       commandTimeout: connectionScope.DefaultCommandTimeout,
                       commandType: CommandType.Text);

                    overrideGroups = results.Select(og =>
                        new OverrideGroup()
                        {
                            Key = og.OverrideGroupKey,
                            DisplayCode = og.DisplayCode,
                            Description = og.DescriptionText,
                            SortOrder = og.SortValue,
                            IsActive = og.ActiveFlag,
                            LastModified = og.LastModifiedBinaryValue
                        }).ToArray();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return overrideGroups;
        }

        OverrideGroup IItemRepository.GetOverrideGroup(Guid overrideGroupKey)
        {
            OverrideGroup overrideGroup = null;

            try
            {
                ItemDAL.Models.OverrideGroup model = _overrideGroupRepo.GetOverrideGroup(overrideGroupKey);
                if(model!= null)
                {
                    overrideGroup = new OverrideGroup()
                    {
                        Key = model.OverrideGroupKey,
                        DisplayCode = model.DisplayCode,
                        Description = model.DescriptionText,
                        IsActive = model.ActiveFlag,
                        SortOrder = model.SortValue,
                        LastModified = model.LastModifiedBinaryValue
                    };
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return overrideGroup;
        }

        Guid IItemRepository.InsertOverrideGroup(Context context, OverrideGroup overrideGroup)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(overrideGroup, "overrideGroup");
            Guid? overrideGroupKey = null;

            try
            {
                ItemDAL.Models.OverrideGroup model = new ItemDAL.Models.OverrideGroup()
                {
                    DisplayCode = overrideGroup.DisplayCode,
                    DescriptionText = overrideGroup.Description,
                    SortValue = overrideGroup.SortOrder,
                    ActiveFlag = overrideGroup.IsActive,
                };
                overrideGroupKey = _overrideGroupRepo.InsertOverrideGroup(context.ToActionContext(), model);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return overrideGroupKey.GetValueOrDefault();
        }

        void IItemRepository.UpdateOverrideGroup(Context context, OverrideGroup overrideGroup)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(overrideGroup, "overrideGroup");

            try
            {
                ItemDAL.Models.OverrideGroup model = new ItemDAL.Models.OverrideGroup()
                {
                    OverrideGroupKey = overrideGroup.Key,
                    DisplayCode = overrideGroup.DisplayCode,
                    DescriptionText = overrideGroup.Description,
                    SortValue = overrideGroup.SortOrder,
                    ActiveFlag = overrideGroup.IsActive,
                    LastModifiedBinaryValue = overrideGroup.LastModified
                };

                _overrideGroupRepo.UpdateOverrideGroup(context.ToActionContext(), model);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }
        #endregion

        #region TherapeuticClass Members

        TherapeuticClass IItemRepository.GetTherapeuticClass(Guid therapeuticClassKey)
        {
            TherapeuticClass therapeuticClass = null;

            try
            {
                var sql = new SqlBuilder();
                sql.SELECT("tcc.ExternalSystemKey")
                    ._("tcc.TherapeuticClassKey AS [Key]")
                    ._("tcc.TherapeuticClassCode AS Code")
                    ._("tcc.DescriptionText AS Description")
                    ._("tcc.SortValue AS SortOrder")
                    ._("tcc.LastModifiedBinaryValue AS LastModified")
                    ._("esc.ExternalSystemName AS ExternalSystemName")
                    .FROM("Item.vw_TherapeuticClassCurrent tcc")
                    .LEFT_JOIN("Core.vw_ExternalSystemCurrent esc ON esc.ExternalSystemKey = tcc.ExternalSystemKey")
                    .WHERE("tcc.DeleteFlag = 0")
                    .WHERE("tcc.TherapeuticClassKey = @TherapeuticClassKey");

                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    therapeuticClass = connectionScope.Query<TherapeuticClass>(
                         sql.ToString(),
                         new { TherapeuticClassKey = therapeuticClassKey },
                         commandTimeout: connectionScope.DefaultCommandTimeout,
                         commandType: CommandType.Text)
                         .FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return therapeuticClass;
        }

        TherapeuticClass IItemRepository.GetTherapeuticClass(Guid externalSystemKey, string therapeuticClassCode, LoadOptions loadOptions)
        {
            TherapeuticClass therapeuticClass = null;

            try
            {
                var sql = new SqlBuilder();
                sql.SELECT("tcc.ExternalSystemKey")
                    ._("tcc.TherapeuticClassKey AS [Key]")
                    ._("tcc.TherapeuticClassCode AS Code")
                    ._("tcc.DescriptionText AS Description")
                    ._("tcc.SortValue AS SortOrder")
                    ._("tcc.LastModifiedBinaryValue AS LastModified")
                    ._("esc.ExternalSystemName AS ExternalSystemName")
                    .FROM("Item.vw_TherapeuticClassCurrent tcc")
                    .LEFT_JOIN("Core.vw_ExternalSystemCurrent esc ON esc.ExternalSystemKey = tcc.ExternalSystemKey")
                    .WHERE("tcc.DeleteFlag = 0")
                    .WHERE("tcc.ExternalSystemKey = @ExternalSystemKey")
                    .WHERE("tcc.TherapeuticClassCode = @TherapeuticClassCode");

                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    therapeuticClass = connectionScope.Query<TherapeuticClass>(
                         sql.ToString(),
                         new
                         {
                             TherapeuticClassCode = therapeuticClassCode,
                             ExternalSystemKey = externalSystemKey
                         },
                         commandTimeout: connectionScope.DefaultCommandTimeout,
                         commandType: CommandType.Text)
                         .FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return therapeuticClass;
        }

        Guid IItemRepository.InsertTherapeuticClass(Context context, TherapeuticClass therapeuticClass)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(therapeuticClass, "therapeuticClass");
            Guid? therapeuticClassKey = null;

            try
            {
                therapeuticClassKey = _therapeuticClassRepo.InsertTherapeuticClass(context.ToActionContext(),
                    new ItemDAL.Models.TherapeuticClass
                    {
                        TherapeuticClassKey = therapeuticClass.Key,
                        ExternalSystemKey = therapeuticClass.ExternalSystemKey,
                        TherapeuticClassCode = therapeuticClass.Code,
                        DescriptionText = therapeuticClass.Description,
                        SortValue = therapeuticClass.SortOrder,
                        LastModifiedBinaryValue = therapeuticClass.LastModified
                    });
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return therapeuticClassKey.GetValueOrDefault();
        }

        void IItemRepository.UpdateTherapeuticClass(Context context, TherapeuticClass therapeuticClass)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(therapeuticClass, "therapeuticClass");

            try
            {
                _therapeuticClassRepo.UpdateTherapeuticClass(context.ToActionContext(),
                    new ItemDAL.Models.TherapeuticClass
                    {
                        TherapeuticClassKey = therapeuticClass.Key,
                        ExternalSystemKey = therapeuticClass.ExternalSystemKey,
                        TherapeuticClassCode = therapeuticClass.Code,
                        DescriptionText = therapeuticClass.Description,
                        SortValue = therapeuticClass.SortOrder,
                        LastModifiedBinaryValue = therapeuticClass.LastModified
                    });
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void IItemRepository.DeleteTherapeuticClass(Context context, Guid therapeuticClassKey)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                _therapeuticClassRepo.DeleteTherapeuticClass(context.ToActionContext(), therapeuticClassKey);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        #endregion

        #region PickArea Members

        IEnumerable<PickArea> IItemRepository.GetPickAreas(IEnumerable<Guid> pickAreaKeys, bool? deleted, Guid? facilityKey)
        {
            IEnumerable<PickArea> pickAreas = new List<PickArea>();
            if (pickAreaKeys != null && !pickAreaKeys.Any())
                return pickAreas; // Empty results

            try
            {
                GuidKeyTable selectedKeys = new GuidKeyTable();
                if (pickAreaKeys != null)
                    selectedKeys = new GuidKeyTable(pickAreaKeys.Distinct());

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var pickAreaResults = connectionScope.Query<PickAreasResult>(
                    "Item.bsp_GetPickAreas",
                    new { SelectedKeys = selectedKeys.AsTableValuedParameter(), DeleteFlag = deleted, FacilityKey = facilityKey },
                    commandTimeout: connectionScope.DefaultCommandTimeout,
                    commandType: CommandType.StoredProcedure);

                    pickAreas = pickAreaResults
                        .Select(result => new PickArea(result.PickAreaKey)
                        {
                            FacilityKey = result.FacilityKey,
                            FacilityCode = result.FacilityCode,
                            FacilityName = result.FacilityName,
                            Name = result.PickAreaName,
                            LastModified = result.LastModifiedBinaryValue.ToArray()
                        })
                        .ToArray();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return pickAreas;
        }

        PickArea IItemRepository.GetPickArea(Guid pickAreaKey)
        {
            var pickAreas =
                ((IItemRepository)this).GetPickAreas(new[] { pickAreaKey }, false);

            return pickAreas.FirstOrDefault();
        }

        Guid IItemRepository.InsertPickArea(Context context, PickArea pickArea)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(pickArea, "pickArea");
            Guid? pickAreaKey = null;

            try
            {
                ItemDAL.Models.PickArea model = new ItemDAL.Models.PickArea()
                {
                    FacilityKey = pickArea.FacilityKey,
                    PickAreaName = pickArea.Name,
                };
                pickAreaKey = _pickAreaRepo.InsertPickArea(context.ToActionContext(), model);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return pickAreaKey.GetValueOrDefault();
        }

        void IItemRepository.UpdatePickArea(Context context, PickArea pickArea)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(pickArea, "pickArea");

            try
            {
                ItemDAL.Models.PickArea model = new ItemDAL.Models.PickArea()
                {
                    PickAreaKey = pickArea.Key,
                    PickAreaName = pickArea.Name,
                    LastModifiedBinaryValue = pickArea.LastModified
                };
                _pickAreaRepo.UpdatePickArea(context.ToActionContext(), model);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void IItemRepository.DeletePickArea(Context context, Guid pickAreaKey)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                _pickAreaRepo.DeletePickArea(context.ToActionContext(), pickAreaKey);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        #endregion

        #region Messages
        Guid IItemRepository.InsertItemDisplayMessage(Context context, Guid itemKey, String messageText)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNullOrEmptyString(messageText, "messageText");
            Guid? itemDisplayedMessageKey = null;

            try
            {
                var parameters = new DynamicParameters(new Dictionary<string, object>()
                {
                    {"@ItemKey", itemKey},
                    {"@DisplayUTCDateTime", context.ActionUtcDateTime},
                    {"@DisplayLocalDateTime", context.ActionDateTime},
                    {"@DisplayedMessageInternalCode", "VDGWarning"},
                    {"@MessageText", messageText},
                    {"@UserAccountKey", (Guid?)context.Actor},

                });
                parameters.Add("@ItemDisplayedMessageKey", dbType: DbType.Guid, direction: ParameterDirection.Output);

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    connectionScope.Execute(
                     "Item.usp_ItemDisplayedMessageInsert",
                     parameters,
                     commandTimeout: connectionScope.DefaultCommandTimeout,
                     commandType: CommandType.StoredProcedure);
                };

                itemDisplayedMessageKey = parameters.Get<Guid>("@ItemDisplayedMessageKey");
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return itemDisplayedMessageKey.GetValueOrDefault();    
        }

        #endregion

        #region Item Data

        private IEnumerable<ItemEquivalencySet> CreateItemEquivalencySets(
            IEnumerable<Models.ItemEquivalencySetResult> equivalencySetResults, IEnumerable<Models.ItemEquivalencyResult> equivalencyResults)
        {
            List<ItemEquivalencySet> equivalencySets = new List<ItemEquivalencySet>();
            foreach (var itemEquivalencySetResult in equivalencySetResults)
            {
                ItemEquivalency[] equivalencies = equivalencyResults
                    .Where(eq => eq.ItemEquivalencySetKey == itemEquivalencySetResult.ItemEquivalencySetKey)
                    .Select(eq => new ItemEquivalency(eq.ItemEquivalencyKey)
                    {
                        ItemKey = eq.EquivalentItemKey,
                        DisplayName = eq.EquivalentItemMedDisplayName,
                        PureGenericName = eq.EquivalentItemPureGenericName,
                        DosageFormCode = eq.EquivalentItemDosageFormCode,
                        ItemId = eq.EquivalentItemID,
                        BrandName = eq.EquivalentItemBrandName,
                        Quantity = eq.EquivalentItemQuantity,
                        LastModified = eq.LastModifiedBinaryValue.ToArray()
                    })
                    .ToArray();

                ItemEquivalencySet itemEquivalencySet = new ItemEquivalencySet(itemEquivalencySetResult.ItemEquivalencySetKey, equivalencies)
                {
                    Approved = itemEquivalencySetResult.ApprovedFlag,
                    ItemKey = itemEquivalencySetResult.ItemKey,
                    LastModified = itemEquivalencySetResult.LastModifiedBinaryValue.ToArray()
                };

                equivalencySets.Add(itemEquivalencySet);
            }

            return equivalencySets;
        }

        #endregion

        #region Private Members

        protected static void InsertItemEquivalencySet(ItemDAL.IItemRepository itemRepository, Context context, Guid itemKey, ItemEquivalencySet itemEquivalencySet)
        {
            Guard.ArgumentNotNull(itemRepository, "itemRepository");
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(itemEquivalencySet, "itemEquivalencySet");

            ItemDAL.Models.ItemEquivalencySet set = new ItemDAL.Models.ItemEquivalencySet
            {
                ItemKey = itemKey,
                ApprovedFlag = itemEquivalencySet.Approved,
                LastModifiedBinaryValue = itemEquivalencySet.LastModified
            };

            itemEquivalencySet.ForEach(ie => set.Add(new ItemDAL.Models.ItemEquivalency
            {
                EquivalentItemKey = ie.ItemKey,
                EquivalentItemQuantity = ie.Quantity

            }));

            itemRepository.InsertItemEquivalencySet(context.ToActionContext(), set);
        }

        private static void UpdateItemEquivalencySet(ItemDAL.IItemRepository itemRepository, Context context, ItemEquivalencySet itemEquivalencySet)
        {
            Guard.ArgumentNotNull(itemRepository, "itemRepository");
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(itemEquivalencySet, "itemEquivalencySet");

            itemRepository.UpdateItemEquivalencySet(context.ToActionContext(), 
                itemEquivalencySet.Key,
                itemEquivalencySet.Approved);
        }

        protected static void DeleteItemEquivalencySet(ItemDAL.IItemRepository itemRepository, Context context, Guid itemEquivalencySetKey)
        {
            Guard.ArgumentNotNull(itemRepository, "itemRepository");
            Guard.ArgumentNotNull(context, "context");

            itemRepository.DeleteItemEquivalencySet(context.ToActionContext(),
                itemEquivalencySetKey);
        }

        private void InsertMedItem(ItemDAL.IItemRepository itemRepository, Context context, Guid itemKey, MedItem medItem)
        {
            Guard.ArgumentNotNull(itemRepository, "itemRepository");
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(medItem, "medItem");

            itemRepository.InsertMedItem(context.ToActionContext(),
                new ItemDAL.Models.MedItem
                {
                    MedItemKey = itemKey,
                    GenericName = medItem.GenericName,
                    PureGenericName = medItem.PureGenericName,
                    SearchGenericName = medItem.SearchGenericName,
                    BrandName = medItem.BrandName,
                    SearchBrandName = medItem.SearchBrandName,
                    StrengthText = medItem.Strength,
                    StrengthAmount = medItem.StrengthAmount,
                    StrengthUOMKey = medItem.StrengthUnitOfMeasure != null
                                            ? medItem.StrengthUnitOfMeasure.Key
                                            : default(Guid?),
                    StrengthExternalUOMKey = medItem.StrengthExternalUnitOfMeasureKey,
                    ConcentrationVolumeAmount = medItem.ConcentrationVolumeAmount,
                    ConcentrationVolumeUOMKey = medItem.ConcentrationVolumeUnitOfMeasure != null
                                                    ? medItem.ConcentrationVolumeUnitOfMeasure.Key
                                                    : default(Guid?),
                    ConcentrationVolumeExternalUOMKey = medItem.ConcentrationVolumeExternalUnitOfMeasureKey,
                    TotalVolumeAmount = medItem.TotalVolumeAmount,
                    TotalVolumeUOMKey = medItem.TotalVolumeUnitOfMeasure != null
                                            ? medItem.TotalVolumeUnitOfMeasure.Key
                                            : default(Guid?),
                    TotalVolumeExternalUOMKey = medItem.TotalVolumeExternalUnitOfMeasureKey,
                    DosageFormKey = medItem.DosageForm != null ? medItem.DosageForm.Key : default(Guid?),
                    MedClassKey = medItem.MedicationClass != null ? medItem.MedicationClass.Key : default(Guid?),
                    MedItemTypeInternalCode = medItem.MedItemType.ToInternalCode(),
                    MinimumDoseAmount = medItem.MinimumDoseAmount,
                    MaximumDoseAmount = medItem.MaximumDoseAmount,
                    DoseUOMKey = medItem.DoseUnitOfMeasure,
                    LastModifiedBinaryValue = medItem.LastModified
                });

            if (medItem.TherapeuticClasses != null)
            {
                InsertTherapeuticClasses(
                    context,
                    itemKey,
                    medItem.TherapeuticClasses.Select(tc => tc.Key));
            }

            if(medItem.VariableDoseGroupMembers != null)
            {
                InsertVariableDoseGroupMembers(itemRepository, context, itemKey, medItem);
            }
        }
        
        private void UpdateMedItem(ItemDAL.IItemRepository itemRepository, Context context, MedItem medItem)
        {
            Guard.ArgumentNotNull(itemRepository, "itemRepository");
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(medItem, "medItem");

            itemRepository.UpdateMedItem(context.ToActionContext(),
                new ItemDAL.Models.MedItem
                {
                    MedItemKey = medItem.Key,
                    GenericName = medItem.GenericName,
                    PureGenericName = medItem.PureGenericName,
                    SearchGenericName = medItem.SearchGenericName,
                    BrandName = medItem.BrandName,
                    SearchBrandName = medItem.SearchBrandName,
                    StrengthText = medItem.Strength,
                    StrengthAmount = medItem.StrengthAmount,
                    StrengthUOMKey = medItem.StrengthUnitOfMeasure != null
                                            ? medItem.StrengthUnitOfMeasure.Key
                                            : default(Guid?),
                    StrengthExternalUOMKey = medItem.StrengthExternalUnitOfMeasureKey,
                    ConcentrationVolumeAmount = medItem.ConcentrationVolumeAmount,
                    ConcentrationVolumeUOMKey = medItem.ConcentrationVolumeUnitOfMeasure != null
                                                    ? medItem.ConcentrationVolumeUnitOfMeasure.Key
                                                    : default(Guid?),
                    ConcentrationVolumeExternalUOMKey = medItem.ConcentrationVolumeExternalUnitOfMeasureKey,
                    TotalVolumeAmount = medItem.TotalVolumeAmount,
                    TotalVolumeUOMKey = medItem.TotalVolumeUnitOfMeasure != null
                                            ? medItem.TotalVolumeUnitOfMeasure.Key
                                            : default(Guid?),
                    TotalVolumeExternalUOMKey = medItem.TotalVolumeExternalUnitOfMeasureKey,
                    DosageFormKey = medItem.DosageForm != null ? medItem.DosageForm.Key : default(Guid?),
                    MedClassKey = medItem.MedicationClass != null ? medItem.MedicationClass.Key : default(Guid?),
                    MedItemTypeInternalCode = medItem.MedItemType.ToInternalCode(),
                    MinimumDoseAmount = medItem.MinimumDoseAmount,
                    MaximumDoseAmount = medItem.MaximumDoseAmount,
                    DoseUOMKey = medItem.DoseUnitOfMeasure,
                    LastModifiedBinaryValue = medItem.LastModified
                });
           
            UpdateTherapeuticClasses(
                context,
                medItem.Key,
                (medItem.TherapeuticClasses != null)
                    ? medItem.TherapeuticClasses.Select(tc => tc.Key)
                    : new Guid[0]);

            UpdateVariableDoseGroupMembers(itemRepository, context, medItem);
        }

        private void InsertTherapeuticClasses(Context context, Guid medItemKey, IEnumerable<Guid> therapeuticClassKeys)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(therapeuticClassKeys, "therapeuticClassKeys");

            foreach (Guid therapeuticClassKey in therapeuticClassKeys)
            {
                _therapeuticClassRepo.AssociateTherapeuticClass(context.ToActionContext(),
                    medItemKey,
                    therapeuticClassKey);
            }
        }

        private void UpdateTherapeuticClasses(Context context, Guid medItemKey, IEnumerable<Guid> therapeuticClassKeys)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(therapeuticClassKeys, "therapeuticClassKeys");

            // Get the list of therapeutic class keys associated with this med item
            IReadOnlyCollection<Guid> currentTherapeuticClassKeys =
                _therapeuticClassRepo.GetAssociatedTherapeuticClassKeys(medItemKey);

            //Find the therapeutic class keys that were removed.
            IEnumerable<Guid> removedTherapeuticClassKeys = currentTherapeuticClassKeys.Except(therapeuticClassKeys);

            //Remove therapeutic classes that are no longer associated with this med item.
            foreach (Guid therapeuticClassKey in removedTherapeuticClassKeys)
            {
                _therapeuticClassRepo.DisassociateTherapeuticClass(context.ToActionContext(),
                    medItemKey,
                    therapeuticClassKey);
            }

            //Find the therapeutic classes that were added
            IEnumerable<Guid> addedTherapeuticClassKeys = therapeuticClassKeys.Except(currentTherapeuticClassKeys);
            InsertTherapeuticClasses(context, medItemKey, addedTherapeuticClassKeys);
        }

        private void InsertFacilityItemComboComponents(Context context, Guid facilityItemKey, IEnumerable<ComboComponent> comboComponents)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(comboComponents, "comboComponents");

            foreach (ComboComponent comboComponent in comboComponents)
            {
                Guid? comboComponentKey = null;

                ItemDAL.Models.ComboComponent model = new ItemDAL.Models.ComboComponent()
                {
                    ComboFacilityItemKey = facilityItemKey,
                    ComponentFacilityItemKey = comboComponent.FacilityItemKey == Guid.Empty ? facilityItemKey : comboComponent.FacilityItemKey,
                    ComponentQuantity = comboComponent.Quantity,
                    ChargeComponentFlag = comboComponent.Charge,
                    MultiplierFlag = comboComponent.Multiplier,
                };

                comboComponentKey = _facilityItemRepo.InsertComboComponent(context.ToActionContext(), model);

                if (comboComponentKey != null &&
                    comboComponent.AdministrationRoutes != null)
                {
                    InsertComboComponentAdminRoutes(
                        context,
                        comboComponentKey.Value,
                        comboComponent.AdministrationRoutes);
                }
            }
        }

        private void UpdateFacilityItemComboComponents(Context context, Guid facilityItemKey, IEnumerable<ComboComponent> comboComponents)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(comboComponents, "comboComponents");

            using (var connectionScope = ConnectionScopeFactory.Create())
            {
                // Get the list of combo component keys associated with this facility item.
                var query = new SqlBuilder();
                query.SELECT("*").
                        FROM("Item.vw_ComboComponentCurrent");
                var queryableComboEntity = connectionScope.Query<ComboComponentCurrentResult>(query.ToString()).AsQueryable();
                List<Guid> removedComboComponentKeys = queryableComboEntity.Where(cc => cc.ComboFacilityItemKey == facilityItemKey)
                .Select(cc => cc.ComboComponentKey)
                .ToList();

                // Find the combo components that were added.
                List<ComboComponent> addedComboComponents = new List<ComboComponent>();
                foreach (ComboComponent comboComponent in comboComponents)
                {
                    if (comboComponent.IsTransient())
                    {
                        addedComboComponents.Add(comboComponent);
                        continue;
                    }

                    var comboModel = new ItemDAL.Models.ComboComponent()
                    {
                        ComponentQuantity = comboComponent.Quantity,
                        ChargeComponentFlag = comboComponent.Charge,
                        MultiplierFlag = comboComponent.Multiplier,
                        LastModifiedBinaryValue = comboComponent.LastModified,
                        ComboComponentKey = comboComponent.Key,
                    };
                
                    _facilityItemRepo.UpdateComboComponent(context.ToActionContext(), comboModel, false);

                    UpdateComboComponentAdminRoutes(
                        context,
                        comboComponent.Key, 
                        comboComponent.AdministrationRoutes ?? new Guid[0]);

                    removedComboComponentKeys.Remove(comboComponent.Key);
                }

                // Delete the combo components that were removed.
                foreach (var comboComponentKey in removedComboComponentKeys)
                {
                    _facilityItemRepo.DeleteComboComponent(context.ToActionContext(), comboComponentKey);
                }

                // Add the combo components.
                InsertFacilityItemComboComponents(
                    context,
                    facilityItemKey,
                    addedComboComponents);
            }
        }

        private static void InsertComboComponentAdminRoutes(Context context, Guid comboComponentKey, IEnumerable<Guid> adminRouteKeys)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(adminRouteKeys, "adminRouteKeys");

            foreach (Guid adminRouteKey in adminRouteKeys)
            {
                Guid? key = null;
                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var parameters = new DynamicParameters(new Dictionary<string, object>()
                    {
                        { "@ComboComponentKey", comboComponentKey },
                        { "@AdminRouteKey", adminRouteKey},
                        { "@StartUTCDateTime", context.ActionUtcDateTime},
                        { "@StartLocalDateTime", context.ActionDateTime},
                        { "@LastModifiedActorKey", (Guid?)context.Actor}
                    });
                    parameters.Add("@ComboComponentAdminRouteKey", dbType: DbType.Guid, direction: ParameterDirection.Output);

                    connectionScope.Execute(
                        "Item.usp_ComboComponentAdminRouteInsert",
                        parameters,
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure);
                    key = parameters.Get<Guid?>("@ComboComponentAdminRouteKey");
                }
            }
        }

        private static void UpdateComboComponentAdminRoutes(Context context, Guid comboComponentKey, IEnumerable<Guid> adminRouteKeys)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(adminRouteKeys, "adminRouteKeys");

            // Get the list of admin route keys associated with this combo component
            IEnumerable<Guid> currentAdminRouteKeys = new List<Guid>();

            using (var connectionScope = ConnectionScopeFactory.Create())
            {
                var query = new SqlBuilder();
                query.SELECT("ccar.AdminRouteKey").
                        FROM("Item.vw_ComboComponentAdminRouteCurrent ccar")
                        .WHERE("ccar.ComboComponentKey = @ComboComponentKey");
                currentAdminRouteKeys = connectionScope.Query<Guid>(query.ToString(),
                        new { ComboComponentKey = comboComponentKey}, commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.Text).ToList();
            }

            // Find the admin route keys that were removed.
            IEnumerable<Guid> removedAdminRouteKeys = currentAdminRouteKeys.Except(adminRouteKeys);

            // Remove admin routes that are no longer associated with this combo component.
            foreach (Guid adminRouteKey in removedAdminRouteKeys)
            {
                var parameters = new DynamicParameters(new Dictionary<string, object>()
                {
                    { "@ComboComponentKey", comboComponentKey },
                    { "@AdminRouteKey", adminRouteKey},
                    { "@DisassociationUTCDateTime", context.ActionUtcDateTime},
                    { "@DisassociationLocalDateTime", context.ActionDateTime},
                    { "@DisassociationActorKey", (Guid?)context.Actor}
                });

                using (var connection = ConnectionScopeFactory.Create())
                {
                    connection.Execute(
                    "Item.usp_ComboComponentAdminRouteDelete",
                    parameters,
                    commandTimeout: connection.DefaultCommandTimeout,
                    commandType: CommandType.StoredProcedure);
                }
            }

            // Find the admin routes that were added
            IEnumerable<Guid> addedAdminRouteKeys = adminRouteKeys.Except(currentAdminRouteKeys);
            InsertComboComponentAdminRoutes(context, comboComponentKey, addedAdminRouteKeys);
        }

        private void InsertFacilityItemOverrideGroups(Context context, Guid facilityItemKey, IEnumerable<Guid> overrideGroupKeys)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(overrideGroupKeys, "overrideGroupKeys");

            foreach (Guid overrideGroupKey in overrideGroupKeys)
            {
                _overrideGroupRepo.AssociateOverrideGroupMember(context.ToActionContext(), overrideGroupKey, facilityItemKey);
            }
        }

        private void UpdateFacilityItemOverrideGroups(Context context, Guid facilityItemKey, IEnumerable<Guid> overrideGroupKeys)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(overrideGroupKeys, "overrideGroupKeys");

            //Get the list of override group keys associated with this facility item
            IEnumerable<Guid> currentoverrideGroupKeys = new List<Guid>();
            using (var connectionScope = ConnectionScopeFactory.Create())
            {
                var query = new SqlBuilder();
                query.SELECT("ogmc.OverrideGroupKey").
                        FROM("Item.vw_OverrideGroupMemberCurrent ogmc")
                        .WHERE("ogmc.FacilityItemKey = @FacilityItemKey");
                currentoverrideGroupKeys = connectionScope.Query<Guid>(query.ToString(), new { FacilityItemKey = facilityItemKey }, commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.Text).ToList();
            }

            //Find the override group keys that were removed.
            IEnumerable<Guid> removedOverrideGroupKeys = currentoverrideGroupKeys.Except(overrideGroupKeys);

            //Remove override groups that are no longer associated with this med item.
            foreach (Guid overrideGroupKey in removedOverrideGroupKeys)
            {
                _overrideGroupRepo.DisassociateOverrideGroupMember(context.ToActionContext(), overrideGroupKey, facilityItemKey);
            }

            // Find the override groups that were added
            IEnumerable<Guid> addedOverrideGroupKeys = overrideGroupKeys.Except(currentoverrideGroupKeys);
            InsertFacilityItemOverrideGroups(context, facilityItemKey, addedOverrideGroupKeys);
        }

        private void InsertFacilityItemPickAreas(Context context, Guid facilityItemKey, IEnumerable<Guid> pickAreaKeys)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(pickAreaKeys, "pickAreaKeys");

            foreach (Guid pickAreaKey in pickAreaKeys)
            {
                _facilityItemRepo.AssociateFacilityItemPickArea(context.ToActionContext(), facilityItemKey, pickAreaKey);
            }
        }

        private void UpdateFacilityItemPickAreas(Context context, Guid facilityItemKey, IEnumerable<Guid> pickAreaKeys)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(pickAreaKeys, "pickAreaKeys");

            // Get the list of pick area keys associated with this facility itemx
            IEnumerable<Guid> currentPickAreaKeys = new List<Guid>();
            using (var connectionScope = ConnectionScopeFactory.Create())
            {
                var query = new SqlBuilder();
                query.SELECT("fipa.PickAreaKey").
                        FROM("Item.vw_FacilityItemPickAreaCurrent fipa")
                        .WHERE("fipa.FacilityItemKey = @FacilityItemKey");
                currentPickAreaKeys = connectionScope.Query<Guid>(query.ToString(), new { FacilityItemKey = facilityItemKey }, commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.Text).ToList();
            }

            // Find the pick area keys that were removed.
            IEnumerable<Guid> removedPickAreaKeys = currentPickAreaKeys.Except(pickAreaKeys);

            // Remove pick area that are no longer associated with this med item.
            foreach (Guid pickAreaKey in removedPickAreaKeys)
            {
                _facilityItemRepo.DisassociateFacilityItemPickArea(context.ToActionContext(), facilityItemKey, pickAreaKey);
            }

            // Find the pick areas that were added
            IEnumerable<Guid> addedPickAreaKeys = pickAreaKeys.Except(currentPickAreaKeys);
            InsertFacilityItemPickAreas(context, facilityItemKey, addedPickAreaKeys);
        }

        private void InsertFacilityItemBlockedDispensingDeviceEntities(Context context, Guid facilityItemKey, IEnumerable<Guid> blockedDispensingDeviceKeys)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(blockedDispensingDeviceKeys, "blockedDispensingDeviceKeys");

            foreach (Guid blockedDispensingDeviceKey in blockedDispensingDeviceKeys)
            {
                _facilityItemRepo.AssociateBlockedFacilityItemDispensingDevice(context.ToActionContext(), facilityItemKey, blockedDispensingDeviceKey);
            }
        }

        private void UpdateFacilityItemBlockedDispensingDeviceEntities(Context context, Guid facilityItemKey, IEnumerable<Guid> blockedDispensingDeviceKeys)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(blockedDispensingDeviceKeys, "blockedDispensingDeviceKeys");

            // Get the list of blocked dispensing device keys associated with this facility item
            IEnumerable<Guid> currentBlockedDispensingDeviceKeys = new List<Guid>();
            using (var connectionScope = ConnectionScopeFactory.Create())
            {
                var query = new SqlBuilder();
                query.SELECT("bfidd.DispensingDeviceKey").
                        FROM("Item.vw_BlockedFacilityItemDispensingDeviceCurrent bfidd")
                        .WHERE("bfidd.FacilityItemKey = @FacilityItemKey");
                currentBlockedDispensingDeviceKeys = connectionScope.Query<Guid>(query.ToString(), new { FacilityItemKey = facilityItemKey }, commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.Text).ToList();
            }

            // Find the keys that were removed.
            IEnumerable<Guid> removedBlockedDispensingDeviceKeys = currentBlockedDispensingDeviceKeys.Except(blockedDispensingDeviceKeys);

            // Remove blocked dispensing devices that are no longer associated with this med item.
            foreach (Guid removedKey in removedBlockedDispensingDeviceKeys)
            {
                _facilityItemRepo.DisassociateBlockedFacilityItemDispensingDevice(context.ToActionContext(), facilityItemKey, removedKey);
            }

            // Find the blocked dispensing devices that were added
            IEnumerable<Guid> addedBlockedDispensingDeviceKeys = blockedDispensingDeviceKeys.Except(currentBlockedDispensingDeviceKeys);
            InsertFacilityItemBlockedDispensingDeviceEntities(context, facilityItemKey, addedBlockedDispensingDeviceKeys);
        }

        private void InsertFacilityItemPhysicalCapacities(Context context, Guid facilityItemKey, IEnumerable<PhysicalCapacity> physicalCapacities)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(physicalCapacities, "physicalCapacities");

            foreach (PhysicalCapacity physicalCapacity in physicalCapacities)
            {
                Guid? key = null;
                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var model = new ItemDAL.Models.FacilityItemPhysicalCapacity()
                    {
                        FacilityItemKey = facilityItemKey,
                        StorageSpaceSizeInternalCode = physicalCapacity.StorageSpaceSize != null ? physicalCapacity.StorageSpaceSize.InternalCode.ToInternalCode() : default(string),
                        MaximumQuantity = physicalCapacity.MaximumQuantity,
                        PhysicalMaximumQuantity = physicalCapacity.PhysicalMaximumQuantity,
                        ParQuantity = physicalCapacity.ParQuantity,
                        RefillPointQuantity = physicalCapacity.RefillPointQuantity,
                    };

                    key = _facilityItemRepo.InsertFacilityItemPhysicalCapacity(context.ToActionContext(), model);
                }
            }
        }

        private void UpdateFacilityItemPhysicalCapacities(Context context, Guid facilityItemKey, IEnumerable<PhysicalCapacity> physicalCapacities)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(physicalCapacities, "physicalCapacities");

            //Find the physical capacity that were added.
            List<PhysicalCapacity> addedPhysicalCapacities = new List<PhysicalCapacity>();
            foreach (PhysicalCapacity physicalCapacity in physicalCapacities)
            {
                if (physicalCapacity.IsTransient())
                {
                    addedPhysicalCapacities.Add(physicalCapacity);
                    continue;
                }

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var model = new ItemDAL.Models.FacilityItemPhysicalCapacity()
                    {
                        MaximumQuantity = physicalCapacity.MaximumQuantity,
                        PhysicalMaximumQuantity = physicalCapacity.PhysicalMaximumQuantity,
                        ParQuantity = physicalCapacity.ParQuantity,
                        RefillPointQuantity = physicalCapacity.RefillPointQuantity,
                        LastModifiedBinaryValue = physicalCapacity.LastModified ?? new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 },
                        FacilityItemPhysicalCapacityKey = physicalCapacity.Key,
                    };

                    _facilityItemRepo.UpdateFacilityItemPhysicalCapacity(context.ToActionContext(), model);
                }
            }

            // Add the physical capacities.
            InsertFacilityItemPhysicalCapacities(
                context,
                facilityItemKey,
                addedPhysicalCapacities);
        }

        private void InsertFacilityItemClinicalDataSubjects(Context context, Guid facilityItemKey, PatientCareFunctionInternalCode patientCareFunction, IEnumerable<Guid> clinicalDataSubjectKeys)
        {
            Guard.ArgumentNotNull(clinicalDataSubjectKeys, "clinicalDataSubjectKeys");

            foreach (Guid clinicalDataSubjectKey in clinicalDataSubjectKeys)
            {
                _clinicalDataSubjectRepo.AssociateClinicalDataSubjectFacilityItemFunction(context.ToActionContext(), clinicalDataSubjectKey, facilityItemKey,
                    patientCareFunction.ToInternalCode());
            }
        }

        private void UpdateFacilityItemClinicalDataSubjects(Context context, Guid facilityItemKey, PatientCareFunctionInternalCode patientCareFunction, IEnumerable<Guid> clinicalDataSubjectKeys)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(clinicalDataSubjectKeys, "clinicalDataSubjectKeys");

            // Get the list of clinical data subject keys associated with this facility item.
            IEnumerable<Guid> currentClinicalDataSubjectKeys = new List<Guid>();
            using (var connectionScope = ConnectionScopeFactory.Create())
            {
                var query = new SqlBuilder();
                query.SELECT("cdsfif.ClinicalDataSubjectKey").
                        FROM("CDCat.vw_ClinicalDataSubjectFacilityItemFunctionCurrent cdsfif")
                        .WHERE("cdsfif.FacilityItemKey = @FacilityItemKey")
                        ._("cdsfif.PatientCareFunctionInternalCode = @PatientCareFunctionInternalCode");
                currentClinicalDataSubjectKeys = connectionScope.Query<Guid>(query.ToString(),
                    new
                    {
                        FacilityItemKey = facilityItemKey,
                        PatientCareFunctionInternalCode = patientCareFunction.ToInternalCode(),
                    },
                    commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.Text).ToList();
            }

            // Find the clinical data subject keys that were removed.
            IEnumerable<Guid> removedClinicalDataSubjectKeys = currentClinicalDataSubjectKeys.Except(clinicalDataSubjectKeys);

            // Remove override groups that are no longer associated with this facility item.
            foreach (Guid removedClinicalDataSubjectKey in removedClinicalDataSubjectKeys)
            {
                _clinicalDataSubjectRepo.DisassociateClinicalDataSubjectFacilityItemFunction(context.ToActionContext(), removedClinicalDataSubjectKey, facilityItemKey,
                    patientCareFunction.ToInternalCode());
            }

            // Find the clinical data subject keys that were added
            IEnumerable<Guid> addedClinicalDataSubjectKeys = clinicalDataSubjectKeys.Except(currentClinicalDataSubjectKeys);
            InsertFacilityItemClinicalDataSubjects(context, facilityItemKey, patientCareFunction, addedClinicalDataSubjectKeys);
        }

        private void InsertFormularyTemplateOverrideGroups(Context context, Guid formularyTemplateKey, IEnumerable<Guid> overrideGroupKeys)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(overrideGroupKeys, "overrideGroupKeys");

            foreach (Guid overrideGroupKey in overrideGroupKeys)
            {
                Guid? key = null;
                key = _formularyTemplateRepo.AssociateFormularyTemplateOverrideGroup(context.ToActionContext(), formularyTemplateKey, overrideGroupKey);
            }
        }

        private void UpdateFormularyTemplateOverrideGroups(Context context, Guid formularyTemplateKey, IEnumerable<Guid> overrideGroupKeys)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(overrideGroupKeys, "overrideGroupKeys");

            //Get the list of override group keys associated with this formulary template
            IEnumerable<Guid> currentOverrideGroupKeys = new List<Guid>();
            using (var connectionScope = ConnectionScopeFactory.Create())
            {
                var query = new SqlBuilder();
                query.SELECT("ftog.OverrideGroupKey").
                        FROM("Item.vw_FormularyTemplateOverrideGroupCurrent ftog")
                        .WHERE("ftog.FormularyTemplateKey = @FormularyTemplateKey");

                currentOverrideGroupKeys = connectionScope.Query<Guid>(query.ToString(),
                    new
                    {
                        FormularyTemplateKey = formularyTemplateKey,
                    },
                    commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.Text).ToList();
            }

            //Find the override group keys that were removed.
            IEnumerable<Guid> removedOverrideGroupKeys = currentOverrideGroupKeys.Except(overrideGroupKeys);

            //Remove override groups that are no longer associated with this formulary template.
            foreach (Guid overrideGroupKey in removedOverrideGroupKeys)
            {
                _formularyTemplateRepo.DisassociateFormularyTemplateOverrideGroup(context.ToActionContext(), formularyTemplateKey, overrideGroupKey);
            }

            // Find the override groups that were added
            IEnumerable<Guid> addedOverrideGroupKeys = overrideGroupKeys.Except(currentOverrideGroupKeys);
            InsertFormularyTemplateOverrideGroups(context, formularyTemplateKey, addedOverrideGroupKeys);
        }

        private static void InsertVariableDoseGroupMembers(ItemDAL.IItemRepository itemRepository, Context context, Guid itemKey, MedItem medItem)
        {
            Guard.ArgumentNotNull(itemRepository, "itemRepository");
            Guard.ArgumentNotNull(medItem, "medItem");
            
            foreach (var member in medItem.VariableDoseGroupMembers)
            {
                itemRepository.InsertVariableDoseGroupMember(context.ToActionContext(),
                    new ItemDAL.Models.VariableDoseGroupMember
                    {
                        VariableDoseGroupKey = itemKey,
                        MedItemKey = member.MedItemKey,
                        RankValue = member.Rank.GetValueOrDefault()
                    });
            }
        }

        private static void UpdateVariableDoseGroupMembers(ItemDAL.IItemRepository itemRepository, Context context, MedItem medItem)
        {
            Guard.ArgumentNotNull(itemRepository, "itemRepository");
            Guard.ArgumentNotNull(medItem, "medItem");

            // Get current variable dose group member for the input medItem
            var currentMembers = itemRepository.GetVariableDoseGroupMemberKeys(medItem.Key);

            var membersToBeRemoved = currentMembers.Except(medItem.VariableDoseGroupMembers?.Select(vdgm => vdgm.Key) ?? Enumerable.Empty<Guid>()).ToArray();

            // Delete the members for this medItem
            foreach (var memberKey in membersToBeRemoved)
            {
                itemRepository.DeleteVariableDoseGroupMember(context.ToActionContext(),
                    memberKey);
            }

            if(medItem.VariableDoseGroupMembers != null && medItem.VariableDoseGroupMembers.Length > 0)
            {
                // Traverse each member that is associated (either newly added or updated) to this medItem
                foreach (var member in medItem.VariableDoseGroupMembers)
                {
                    ItemDAL.Models.VariableDoseGroupMember variableDoseGroupMember = 
                        new ItemDAL.Models.VariableDoseGroupMember
                        {
                            VariableDoseGroupMemberKey = member.Key,
                            VariableDoseGroupKey = medItem.Key,
                            MedItemKey = member.MedItemKey,
                            RankValue = member.Rank.GetValueOrDefault(),
                            LastModifiedBinaryValue = member.LastModified
                        };

                    // If this is a new member then add it.
                    if (member.IsTransient())
                    {
                        itemRepository.InsertVariableDoseGroupMember(context.ToActionContext(), variableDoseGroupMember);

                        continue;
                    }

                    itemRepository.UpdateVariableDoseGroupMember(context.ToActionContext(), variableDoseGroupMember);
                }
            }
        }

        private void InsertorUpdateFacilityItemEquivalencies(Context context, FacilityItem facilityItem)
        {
            foreach (var facilityEquivalency in facilityItem.FacilityEquivalencies)
            {
                if (facilityEquivalency.Approved)
                    AssociateFacilityEquivalency(_itemRepo, context, facilityEquivalency.ItemEquivalencySetKey, facilityItem.FacilityKey);
                else
                    DisassociateFacilityEquivalency(_itemRepo, context, facilityEquivalency.ItemEquivalencySetKey, facilityItem.FacilityKey);
            }

        }
        
        private static void AssociateFacilityEquivalency(ItemDAL.IItemRepository itemRepository, Context context, Guid itemEquivalencySetKey, Guid facilityKey)
        {
            Guard.ArgumentNotNull(itemRepository, "itemRepository");
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(itemEquivalencySetKey, "itemEquivalencySetKey");
            Guard.ArgumentNotNull(facilityKey, "facilityKey");

            itemRepository.AssociateFacilityEquivalency(context.ToActionContext(), itemEquivalencySetKey, facilityKey);
        }
        
        private static void DisassociateFacilityEquivalency(ItemDAL.IItemRepository itemRepository, Context context, Guid itemEquivalencySetKey, Guid facilityKey)
        {
            Guard.ArgumentNotNull(itemRepository, "itemRepository");
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(itemEquivalencySetKey, "itemEquivalencySetKey");
            Guard.ArgumentNotNull(facilityKey, "facilityKey");

            itemRepository.DisassociateFacilityEquivalency(context.ToActionContext(), itemEquivalencySetKey, facilityKey);
        }
        #endregion

    }
}
