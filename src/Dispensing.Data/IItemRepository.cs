using System;
using System.Collections.Generic;
using CareFusion.Dispensing.Contracts;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Data
{
    public interface IItemRepository : IRepository
    {
        #region Item Members
        
        /// <summary>
        /// Retrieves a collection of <see cref="Item"/> by key.
        /// </summary>
        /// <param name="itemKeys">The collection of item keys or NULL for all.</param>
        /// <param name="deleteFlag"></param>
        /// <param name="externalSystemKey"></param>
        /// <param name="itemId"></param>
        /// <returns>An IEnumerable(T) object, where the generic parameter T is <see cref="Item"/>.</returns>
        IEnumerable<Item> GetItems(IEnumerable<Guid> itemKeys = null, bool? deleteFlag = null, Guid? externalSystemKey = null, string itemId = null);
        
        /// <summary>
        /// Gets a <see cref="Item"/> by its surrogate key.
        /// </summary>
        /// <param name="itemKey">The surrogate key of the <see cref="Item"/>.</param>
        /// <returns>The <see cref="Item"/> if found, otherwise null.</returns>
        Item GetItem(Guid itemKey);

        /// <summary>
        /// Gets an <see cref="Item"/> by external system and item ID.
        /// </summary>
        /// <param name="externalSystemKey">The external system key.</param>
        /// <param name="itemId">The item ID.</param>
        /// The <see cref="Item"/> if found, otherwise null.
        Item GetItem(Guid externalSystemKey, string itemId);

        /// <summary>
        /// Gets an <see cref="Item"/> by external system and item ID.
        /// </summary>
        /// <param name="externalSystemKey">The external system key.</param>
        /// <param name="itemId">The item ID.</param>
        /// The <see cref="Item"/> if found, otherwise null.
        Guid? GetItemKey(Guid externalSystemKey, string itemId);

        /// <summary>
        /// Gets an <see cref="Item"/> by external system and item ID.
        /// </summary>
        /// <param name="externalSystemKey">The external system key.</param>
        /// <param name="itemId">The item ID.</param>
        /// The <see cref="Item"/> if found, otherwise null.
        Guid? GetMedItemKey(Guid externalSystemKey, string itemId);

        /// <summary>
        /// Inserts an <see cref="Item"/> or <see cref="MedItem"/> into the database.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="item">The item.</param>
        /// <returns>A item key, which uniquely identifies the item in the database.</returns>
        Guid InsertItem(Context context, Item item);

        /// <summary>
        /// Updates an existing <see cref="Item"/> or <see cref="MedItem"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="item">The item.</param>
        void UpdateItem(Context context, Item item);

        /// <summary>
        /// Logically deletes an existing <see cref="Item"/> or <see cref="MedItem"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="itemKey">The item key.</param>
        void DeleteItem(Context context, Guid itemKey);

        #endregion

        #region Station Query Members
        /// <summary>
        /// Lists the item keys that are permissible to the user at the given device for the given permission
        /// </summary>
        IEnumerable<Guid> ListPermissibleItemKeys(Guid userKey, Guid deviceKey, Guid facilityKey, PermissionInternalCode permission);

        /// <summary>
        /// Lists the item keys that can be removed on override by the user at the given device and facility
        /// </summary>
        IEnumerable<Guid> ListOverrideItemKeys(Guid userKey, Guid deviceKey, Guid facilityKey);

        #endregion

        #region FacilityItem Members

        /// <summary>
        /// Retrieves a collection of <see cref="FacilityItem"/> by key.
        /// </summary>
        /// <param name="facilityItemKeys">The collection of facility item keys or NULL for all.</param>
        /// <param name="facilityKey"></param>
        /// <param name="itemKey"></param>
        /// <returns>An IEnumerable(T) object, where the generic parameter T is <see cref="FacilityItem"/>.</returns>
        IEnumerable<FacilityItem> GetFacilityItems(IEnumerable<Guid> facilityItemKeys = null, Guid? facilityKey = null, Guid? itemKey = null);

        /// <summary>
        /// Gets a <see cref="FacilityItem"/> by its surrogate key.
        /// </summary>
        /// <param name="facilityItemKey">The surrogate key of the <see cref="Item"/>.</param>
        /// <returns>The <see cref="FacilityItem"/> if found, otherwise null.</returns>
        FacilityItem GetFacilityItem(Guid facilityItemKey);

        /// <summary>
        /// Gets a <see cref="FacilityItem"/> by its surrogate key.
        /// </summary>
        /// <param name="facilityKey">The surrogate key of a <see cref="Facility"/>.</param>
        /// <param name="itemKey">The surrogate key of a <see cref="Item"/>.</param>
        /// <returns>The <see cref="FacilityItem"/> if found, otherwise null.</returns>
        FacilityItem GetFacilityItem(Guid facilityKey, Guid itemKey);

        /// <summary>
        /// Inserts an <see cref="FacilityItem"/> into the database.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="facilityItem">The facility item.</param>
        /// <returns>A facility item key, which uniquely identifies the facility item in the database.</returns>
        Guid InsertFacilityItem(Context context, FacilityItem facilityItem);

        /// <summary>
        /// Updates an existing <see cref="FacilityItem"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="facilityItem">The facility item.</param>
        void UpdateFacilityItem(Context context, FacilityItem facilityItem);

        /// <summary>
        /// Logically disassociates an <see cref="FacilityItem"/> from an existing <see cref="Item"/> or <see cref="MedItem"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="facilityKey">The facility key.</param>
        /// <param name="itemKey">The item key.</param>
        void DeleteFacilityItem(Context context, Guid facilityKey, Guid itemKey);

        #endregion

        #region FormularyTemplate Members

        /// <summary>
        /// Retrieves a collection of <see cref="FormularyTemplate"/> by key.
        /// </summary>
        /// <param name="formularyTemplateKeys">The collection of formulary template keys or NULL for all.</param>
        /// <param name="deleted"></param>
        /// <param name="externalSystemKey"></param>
        /// <returns>An IEnumerable(T) object, where the generic parameter T is <see cref="FormularyTemplate"/>.</returns>
        IEnumerable<FormularyTemplate> GetFormularyTemplates(IEnumerable<Guid> formularyTemplateKeys = null,
                                                             bool? deleted = null, Guid? externalSystemKey = null);

        /// <summary>
        /// Gets a <see cref="FormularyTemplate"/> by its surrogate key.
        /// </summary>
        /// <param name="formularyTemplateKey">The surrogate key of the <see cref="FormularyTemplate"/>.</param>
        /// <returns>The <see cref="FormularyTemplate"/> if found, otherwise null.</returns>
        FormularyTemplate GetFormularyTemplate(Guid formularyTemplateKey);

        /// <summary>
        /// Inserts an <see cref="FormularyTemplate"/> into the database.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="formularyTemplate">The formulary template.</param>
        /// <returns>A formulary template key, which uniquely identifies the formulary template in the database.</returns>
        Guid InsertFormularyTemplate(Context context, FormularyTemplate formularyTemplate);

        /// <summary>
        /// Updates an existing <see cref="FormularyTemplate"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="formularyTemplate">The formulary template.</param>
        void UpdateFormularyTemplate(Context context, FormularyTemplate formularyTemplate);

        /// <summary>
        /// Logically deletes a <see cref="FormularyTemplate"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="formularyTemplateKey">The formulary template key.</param>
        void DeleteFormularyTemplate(Context context, Guid formularyTemplateKey);

        #endregion

        #region ItemScanCode Members

        /// <summary>
        /// Retrieves a collection of <see cref="ItemScanCode"/> by key.
        /// </summary>
        /// <param name="itemScanCodeKeys">The collection of item scan code keys or NULL for all.</param>
        /// <param name="deleted"></param>
        /// <param name="scanCodeValue"></param>
        /// <returns>An IEnumerable(T) object, where the generic parameter T is <see cref="ItemScanCode"/>.</returns>
        IEnumerable<ItemScanCode> GetItemScanCodes(IEnumerable<Guid> itemScanCodeKeys = null, bool? deleted = null, string scanCodeValue = null);

        /// <summary>
        /// Gets a <see cref="ItemScanCode"/> by its surrogate key.
        /// </summary>
        /// <param name="itemScanCodeKey">The surrogate key of the <see cref="ItemScanCode"/>.</param>
        /// <returns>The <see cref="ItemScanCode"/> if found, otherwise null.</returns>
        ItemScanCode GetItemScanCode(Guid itemScanCodeKey);

        /// <summary>
        /// Inserts an <see cref="ItemScanCode"/> into the database.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="itemScanCode">The item scan code.</param>
        /// <returns>A item scan code key, which uniquely identifies the item in the database.</returns>
        Guid InsertItemScanCode(Context context, ItemScanCode itemScanCode);

        /// <summary>
        /// Updates an existing <see cref="ItemScanCode"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="itemScanCode">The item scan code.</param>
        void UpdateItemScanCode(Context context, ItemScanCode itemScanCode);

        /// <summary>
        /// Logically deletes an existing <see cref="ItemScanCode"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="itemScanCodeKey">The item scan code key.</param>
        /// <param name="scanProductDeleteReason">The reason for the delete.</param>
        /// <param name="otherItemId">The other item ID the caused the delete.</param>
        /// <param name="deletedByExternalSystemName">The name of the external system that caused a scan code to be deleted.</param>
        void DeleteItemScanCode(Context context, Guid itemScanCodeKey, ScanProductDeleteReasonInternalCode? scanProductDeleteReason = null,
            string otherItemId = null, string deletedByExternalSystemName = null);

        /// <summary>
        /// Gets the rejected and unlinked log for the scan codes.
        /// </summary>
        /// <param name="scanCode">The item scan code</param>
        /// <param name="facilityKey">The facility key</param>
        /// <returns></returns>
        IEnumerable<ItemScanCodeLog> GetScanCodeLog(string scanCode, Guid facilityKey);

        /// <summary>
        /// Gets the rejected and unlinked log for the product ids.
        /// </summary>
        /// <param name="productId">The product id</param>
        /// <param name="facilityKey">The facility key</param>
        /// <returns></returns>
        IEnumerable<ItemScanCodeLog> GetProductIdLog(string productId, Guid facilityKey);
        
        #endregion

        #region ItemProductId Members

        /// <summary>
        /// Gets a <see cref="ItemProductId"/> by its surrogate key.
        /// </summary>
        /// <param name="itemProductIdKey">The surrogate key of the <see cref="ItemProductId"/>.</param>
        /// <returns>The <see cref="ItemProductId"/> if found, otherwise null.</returns>
        ItemProductId GetItemProductId(Guid itemProductIdKey);

        /// <summary>
        /// Inserts an <see cref="ItemProductId"/> into the database.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="itemProductId">The item product ID.</param>
        /// <returns>A item product ID key, which uniquely identifies the item in the database.</returns>
        Guid InsertItemProductId(Context context, ItemProductId itemProductId);

        /// <summary>
        /// Updates an existing <see cref="ItemProductId"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="itemProductId">The item product ID.</param>
        void UpdateItemProductId(Context context, ItemProductId itemProductId);

        /// <summary>
        /// Logically deletes an existing <see cref="ItemProductId"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="itemProductIdKey">The item product ID key.</param>
        /// /// <param name="scanProductDeleteReason">The reason for the delete.</param>
        /// <param name="otherItemId">The other item ID the caused the delete.</param>
        /// <param name="deletedByExternalSystemName">The name of the external system that caused a product ID to be deleted.</param>
        void DeleteItemProductId(Context context, Guid itemProductIdKey, ScanProductDeleteReasonInternalCode? scanProductDeleteReason = null,
            string otherItemId = null, string deletedByExternalSystemName = null);

        #endregion

        #region DosageForm Members

        /// <summary>
        /// Gets a <see cref="DosageForm"/> by its surrogate key.
        /// </summary>
        /// <param name="dosageFormKey">The surrogate key of the <see cref="DosageForm"/>.</param>
        /// <returns>The <see cref="DosageForm"/> if found, otherwise null.</returns>
        DosageForm GetDosageForm(Guid dosageFormKey);

        /// <summary>
        /// Gets a <see cref="DosageForm"/> by external system key and dosage form code.
        /// </summary>
        /// <param name="externalSystemKey">The surrogate key of the <see cref="ExternalSystem"/>.</param>
        /// <param name="dosageFormCode"></param>
        /// <param name="loadOptions"></param>
        /// <returns>The <see cref="DosageForm"/> if found, otherwise null.</returns>
        DosageForm GetDosageForm(Guid externalSystemKey, string dosageFormCode, LoadOptions loadOptions = LoadOptions.Full);

        /// <summary>
        /// Inserts a <see cref="DosageForm"/> into the database.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="dosageForm">The dosage form.</param>
        /// <returns>A dosage form key, which uniquely identifies the dosage form in the database.</returns>
        Guid InsertDosageForm(Context context, DosageForm dosageForm);

        /// <summary>
        /// Updates an existing <see cref="DosageForm"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="dosageForm">The dosage form.</param>
        void UpdateDosageForm(Context context, DosageForm dosageForm);

        /// <summary>
        /// Deletes an existing <see cref="DosageForm"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="dosageFormKey">The dosage form surrogate key.</param>
        void DeleteDosageForm(Context context, Guid dosageFormKey);

        #endregion

        #region EquivalencyDosageFormGroup Members

        /// <summary>
        /// Gets a <see cref="EquivalencyDosageFormGroup"/> by its surrogate key.
        /// </summary>
        /// <param name="equivalencyDosageFormGroupKey">The surrogate key of the <see cref="DosageForm"/>.</param>
        /// <returns>The <see cref="EquivalencyDosageFormGroup"/> if found, otherwise null.</returns>
        EquivalencyDosageFormGroup GeEquivalencyDosageFormGroup(Guid equivalencyDosageFormGroupKey);

        /// <summary>
        /// Inserts a <see cref="EquivalencyDosageFormGroup"/> into the database.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="equivalencyDosageFormGroup">The dosage form.</param>
        /// <returns>An equivalency dosage form group key, which uniquely identifies the dosage form in the database.</returns>
        Guid InsertEquivalencyDosageFormGroup(Context context, EquivalencyDosageFormGroup equivalencyDosageFormGroup);

        /// <summary>
        /// Updates an existing <see cref="EquivalencyDosageFormGroup"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="equivalencyDosageFormGroup">The equivalency dosage form group.</param>
        void UpdateEquivalencyDosageFormGroup(Context context, EquivalencyDosageFormGroup equivalencyDosageFormGroup);

        /// <summary>
        /// Deletes an existing <see cref="EquivalencyDosageFormGroup"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="equivalencyDosageFormGroupKey">The dosage form surrogate key.</param>
        void DeleteEquivalencyDosageFormGroup(Context context, Guid equivalencyDosageFormGroupKey);

        #endregion

        #region HazardousWasteClass Members

        /// <summary>
        /// Gets a <see cref="HazardousWasteClass"/> by its surrogate key.
        /// </summary>
        /// <param name="hazardousWasteClassKey">The surrogate key of the <see cref="HazardousWasteClass"/>.</param>
        /// <returns>The <see cref="HazardousWasteClass"/> if found, otherwise null.</returns>
        HazardousWasteClass GetHazardousWasteClass(Guid hazardousWasteClassKey);

        /// <summary>
        /// Inserts a <see cref="HazardousWasteClass"/> into the database.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="hazardousWasteClass">The hazardous waste class.</param>
        /// <returns>A hazardous waste class key, which uniquely identifies the hazardous waste class in the database.</returns>
        Guid InsertHazardousWasteClass(Context context, HazardousWasteClass hazardousWasteClass);

        /// <summary>
        /// Updates an existing <see cref="HazardousWasteClass"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="hazardousWasteClass">The hazardous waste class.</param>
        void UpdateHazardousWasteClass(Context context, HazardousWasteClass hazardousWasteClass);

        /// <summary>
        /// Deletes an existing <see cref="HazardousWasteClass"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="hazardousWasteClassKey">The hazardous waste class surrogate key.</param>
        void DeleteHazardousWasteClass(Context context, Guid hazardousWasteClassKey);

        #endregion

        #region MedicationClass Members

        /// <summary>
        /// Retrieves a collection of <see cref="MedicationClass"/> by key.
        /// </summary>
        /// <param name="medicationClassKeys">The collection of medication class keys or NULL for all.</param>
        /// <param name="deleted"></param>
        /// <param name="facilityKey"></param>
        /// <param name="externalSystemKey"></param>
        /// <param name="medClassCode"></param>
        /// <returns>An IEnumerable(T) object, where the generic parameter T is <see cref="MedicationClass"/>.</returns>
        IEnumerable<MedicationClass> GetMedicationClasses(IEnumerable<Guid> medicationClassKeys = null,
                                                          bool? deleted = null, Guid? facilityKey = null, Guid? externalSystemKey = null,
                                                          string medClassCode = null);

        /// <summary>
        /// Gets a <see cref="MedicationClass"/> by its surrogate key.
        /// </summary>
        /// <param name="medicationClassKey">The surrogate key of the <see cref="MedicationClass"/>.</param>
        /// <returns>The <see cref="MedicationClass"/> if found, otherwise null.</returns>
        MedicationClass GetMedicationClass(Guid medicationClassKey);

        /// <summary>
        /// Gets a <see cref="MedicationClass"/> by external system key and med class code.
        /// </summary>
        /// <param name="externalSystemKey">The surrogate key of the <see cref="ExternalSystem"/>.</param>
        /// <param name="medClassCode"></param>
        /// <returns>The <see cref="MedicationClass"/> if found, otherwise null.</returns>
        MedicationClass GetMedicationClass(Guid externalSystemKey, string medClassCode);

        /// <summary>
        /// Inserts a <see cref="MedicationClass"/> into the database.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="medicationClass">The medication class.</param>
        /// <returns>A medication class key, which uniquely identifies the medication class in the database.</returns>
        Guid InsertMedicationClass(Context context, MedicationClass medicationClass);

        /// <summary>
        /// Updates an existing <see cref="MedicationClass"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="medicationClass">The medication class.</param>
        void UpdateMedicationClass(Context context, MedicationClass medicationClass);

        /// <summary>
        /// Deletes an existing <see cref="MedicationClass"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="medicationClassKey">The medication class surrogate key.</param>
        void DeleteMedicationClass(Context context, Guid medicationClassKey);

        #endregion

        #region SecurityGroup Members

        /// <summary>
        /// Retrieves a collection of <see cref="SecurityGroup"/>.
        /// </summary>
        /// <returns>An IEnumerable(T) object, where the generic parameter T is <see cref="SecurityGroup"/>.</returns>
        IEnumerable<SecurityGroup> GetSecurityGroups();

        /// <summary>
        /// Gets a <see cref="SecurityGroup"/> by its surrogate key.
        /// </summary>
        /// <param name="securityGroupKey">The surrogate key of the <see cref="SecurityGroup"/>.</param>
        /// <returns>The <see cref="SecurityGroup"/> if found, otherwise null.</returns>
        SecurityGroup GetSecurityGroup(Guid securityGroupKey);

        /// <summary>
        /// Inserts a <see cref="SecurityGroup"/> into the database.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="securityGroup">The security group.</param>
        /// <returns>A security group key, which uniquely identifies the security group in the database.</returns>
        Guid InsertSecurityGroup(Context context, SecurityGroup securityGroup);

        /// <summary>
        /// Updates an existing <see cref="SecurityGroup"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="securityGroup">The security group.</param>
        void UpdateSecurityGroup(Context context, SecurityGroup securityGroup);

        #endregion

        #region OverrideGroup Members

        /// <summary>
        /// Retrieves a collection of <see cref="OverrideGroup"/>.
        /// </summary>
        /// <returns>An IEnumerable(T) object, where the generic parameter T is <see cref="OverrideGroup"/>.</returns>
        IEnumerable<OverrideGroup> GetOverrideGroups();

        /// <summary>
        /// Gets a <see cref="OverrideGroup"/> by its surrogate key.
        /// </summary>
        /// <param name="overrideGroupKey">The surrogate key of the <see cref="OverrideGroup"/>.</param>
        /// <returns>The <see cref="OverrideGroup"/> if found, otherwise null.</returns>
        OverrideGroup GetOverrideGroup(Guid overrideGroupKey);

        /// <summary>
        /// Inserts a <see cref="OverrideGroup"/> into the database.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="overrideGroup">The override group.</param>
        /// <returns>A override group key, which uniquely identifies the override group in the database.</returns>
        Guid InsertOverrideGroup(Context context, OverrideGroup overrideGroup);

        /// <summary>
        /// Updates an existing <see cref="OverrideGroup"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="overrideGroup">The override group.</param>
        void UpdateOverrideGroup(Context context, OverrideGroup overrideGroup);

        #endregion

        #region TherapeuticClass Members

        /// <summary>
        /// Gets a <see cref="TherapeuticClass"/> by its surrogate key.
        /// </summary>
        /// <param name="therapeuticClassKey">The surrogate key of the <see cref="TherapeuticClass"/>.</param>
        /// <returns>The <see cref="TherapeuticClass"/> if found, otherwise null.</returns>
        TherapeuticClass GetTherapeuticClass(Guid therapeuticClassKey);

        /// <summary>
        /// Gets a <see cref="TherapeuticClass"/> by external system key and therapeautic class code.
        /// </summary>
        /// <param name="externalSystemKey">The surrogate key of the <see cref="ExternalSystem"/>.</param>
        /// <param name="therapeuticClassCode"></param>
        /// <param name="loadOptions"></param>
        /// <returns>The <see cref="TherapeuticClass"/> if found, otherwise null.</returns>
        TherapeuticClass GetTherapeuticClass(Guid externalSystemKey, string therapeuticClassCode, LoadOptions loadOptions = LoadOptions.Full);

        /// <summary>
        /// Inserts a <see cref="TherapeuticClass"/> into the database.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="therapeuticClass">The therapeutic class.</param>
        /// <returns>A therapeutic class key, which uniquely identifies the therapeutic class in the database.</returns>
        Guid InsertTherapeuticClass(Context context, TherapeuticClass therapeuticClass);

        /// <summary>
        /// Updates an existing <see cref="TherapeuticClass"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="therapeuticClass">The therapeutic class.</param>
        void UpdateTherapeuticClass(Context context, TherapeuticClass therapeuticClass);

        /// <summary>
        /// Deletes an existing <see cref="TherapeuticClass"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="therapeuticClassKey">The therapeutic class surrogate key.</param>
        void DeleteTherapeuticClass(Context context, Guid therapeuticClassKey);

        #endregion

        #region PickArea Members

        /// <summary>
        /// Retrieves a collection of <see cref="PickArea"/> by key.
        /// </summary>
        /// <param name="pickAreaKeys">The collection of pick area keys or NULL for all.</param>
        /// <param name="deleted"></param>
        /// <param name="facilityKey"></param>
        /// <returns>An IEnumerable(T) object, where the generic parameter T is <see cref="PickArea"/>.</returns>
        IEnumerable<PickArea> GetPickAreas(IEnumerable<Guid> pickAreaKeys = null, bool? deleted = null, Guid? facilityKey = null);

        /// <summary>
        /// Gets a <see cref="PickArea"/> by its surrogate key.
        /// </summary>
        /// <param name="pickAreaKey">The surrogate key of the <see cref="PickArea"/>.</param>
        /// <returns>The <see cref="PickArea"/> if found, otherwise null.</returns>
        PickArea GetPickArea(Guid pickAreaKey);

        /// <summary>
        /// Inserts a <see cref="PickArea"/> into the database.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="pickArea">The pick area.</param>
        /// <returns>A pick area key, which uniquely identifies the pick area in the database.</returns>
        Guid InsertPickArea(Context context, PickArea pickArea);

        /// <summary>
        /// Updates an existing <see cref="PickArea"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="pickArea">The pick area.</param>
        void UpdatePickArea(Context context, PickArea pickArea);

        /// <summary>
        /// Logically deletes an existing <see cref="PickArea"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="pickAreaKey">The pick area key.</param>
        void DeletePickArea(Context context, Guid pickAreaKey);

        #endregion

        #region Messages

        /// <summary>
        /// Insert message that was displayed and accepted by the user
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="itemKey">The item for which message was displayed</param>
        /// <param name="messageText">Actual message displayed to the user</param>
        Guid InsertItemDisplayMessage(Context context, Guid itemKey, String messageText);

        #endregion

    }
}
