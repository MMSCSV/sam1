using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Pyxis.Core.Data;
using Pyxis.Core.Data.InternalCodes;
using Pyxis.Core.Data.Schema;
using Mms.Logging;
using Pyxis.Dispensing.Notification.PublishedNotices.Data;
using Pyxis.Dispensing.Notification.PublishedNotices.Models;
using ItemDAL = Pyxis.Core.Data.Schema.Item;
using HcOrderDAL = Pyxis.Core.Data.Schema.HcOrder;
using NoticeDAL = Pyxis.Core.Data.Schema.Notice;

namespace Pyxis.Dispensing.Notification.PublishedNotices
{
    public class OMNLNoticeGenerator : IOMNLNoticeGenerator
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly ItemDAL.IFacilityItemRepository _facilityItemRepository;
        private readonly ItemDAL.IItemRepository _itemRepository;
        private readonly HcOrderDAL.IPharmacyOrderRepository _pharmacyOrderRepository;
        private readonly NoticeDAL.IPublishedNoticeRepository _publishedNoticeRepository;
        private readonly INoticeRepository _noticeRepository;

        #region Private Classes

        private class PharmacyOrderNotice
        {
            public Guid PharmacyOrderKey { get; set; }

            public Guid FacilityKey { get; set; }

            public bool GiveItemNotLoaded { get; set; }

            public string PrinterName { get; set; }

            public IEnumerable<Guid> DispensingDeviceKeys { get; set; } 
        }

        private class PharmacyOrderComponentNotice
        {
            public Guid? PharmacyOrderComponentKey { get; set; }

            public Guid? MedItemKey { get; set; }

            public IEnumerable<Guid> DispensingDeviceKeys { get; set; } 
        }

        #endregion

        #region Constructors

        public OMNLNoticeGenerator()
        {
            _facilityItemRepository = new ItemDAL.FacilityItemRepository();
            _itemRepository = new ItemDAL.ItemRepository();
            _pharmacyOrderRepository = new HcOrderDAL.PharmacyOrderRepository();
            _publishedNoticeRepository = new NoticeDAL.PublishedNoticeRepository();
            _noticeRepository = new NoticeRepository();       
        }

        #endregion

        #region Public Members

        public Task GenerateAsync(Guid pharmacyOrderKey)
        {
            return Task.Run(() => Generate(pharmacyOrderKey));
        }

        public void Generate(Guid pharmacyOrderKey)
        {
            try
            {
                Log.Debug($"OMNL Notice generator triggered for order '{pharmacyOrderKey}'.");

                // Get the pharmacy order
                var pharmacyOrder = _pharmacyOrderRepository.GetPharmacyOrder(pharmacyOrderKey);
                if (pharmacyOrder == null)
                {
                    Log.Warn($"The order was not found! Notice cannot be generated for order '{pharmacyOrderKey}'.");
                    return;
                }

                // get the encounter's unit and associated printer name for OMNL
                var encounterUnit = _noticeRepository.GetEncounterUnit(pharmacyOrder.EncounterKey);

                // if there's no unit, or no printer for OMNL, do not generate any notice
                if (encounterUnit == null ||
                    string.IsNullOrEmpty(encounterUnit.OMNLNoticePrinterName))
                {
                    Log.Warn($"Either unit is null or no printer specified for OMNL. Not generating notice for order '{pharmacyOrderKey}'.");
                    return;
                }

                // Get a pharmacy order notice
                bool isOrderedItemVariableDoseGroup;
                Guid? comboMedFacilityItemKey;

                var pharmacyOrderNotice = GetPharmacyOrderNotice(
                    pharmacyOrder, 
                    encounterUnit, 
                    out isOrderedItemVariableDoseGroup,
                    out comboMedFacilityItemKey);

                // Get pharmacy order component notices
                int pharmacyOrderComponentCount;
                var pharmacyOrderComponentNotices = GetPharmacyOrderComponentNotices(
                    pharmacyOrder, 
                    encounterUnit,
                    out pharmacyOrderComponentCount);
                
                // If the order does not have any components only then add VDG members or Combo components
                if(!pharmacyOrderComponentNotices.Any())
                {
                    if(isOrderedItemVariableDoseGroup)
                    {
                        var vdgMemberNotices = GetVariableDosageGroupNotices(pharmacyOrder, encounterUnit);
                        if (vdgMemberNotices == null || !vdgMemberNotices.Any())
                        {
                            pharmacyOrderNotice = null;
                        }
                        else
                        {
                            pharmacyOrderComponentNotices.AddRange(vdgMemberNotices);
                        }
                    }
                    else if (comboMedFacilityItemKey != null)
                    {
                        var comboComponentNotices = GetComboComponentNotices(pharmacyOrder, encounterUnit, comboMedFacilityItemKey);
                        if (comboComponentNotices == null || !comboComponentNotices.Any())
                        {
                            pharmacyOrderNotice = null;
                        }
                        else
                        {
                            pharmacyOrderComponentNotices.AddRange(comboComponentNotices);
                        }
                    }
                }

                // If we have component notices, but no notice for the order, 
                // Create a published notice for the order
                if (pharmacyOrderNotice == null &&
                    pharmacyOrderComponentNotices.Any())
                {
                    pharmacyOrderNotice = new PharmacyOrderNotice
                    {
                        PharmacyOrderKey = pharmacyOrderKey,
                        PrinterName = encounterUnit.OMNLNoticePrinterName,
                        FacilityKey = encounterUnit.FacilityKey,
                    };
                }
                
                // Notice should not be created for an order which has multiple components/is variable dose group/combo or dispensenotusingdispensingdevice
                // and all the associated items / child items are loaded.
                if (pharmacyOrder.DispenseNotUsingDispensingDeviceFlag ||
                    (!pharmacyOrderComponentNotices.Any() && (pharmacyOrderComponentCount > 0 || isOrderedItemVariableDoseGroup || comboMedFacilityItemKey != null)))
                {
                    Log.Warn($"Either DispenseNotUsingDispenseDevice is set to true or all ordered items are loaded and order has components or ordered item is variable dose group. Not generating notice for order '{pharmacyOrder.PharmacyOrderKey}:{pharmacyOrder.PharmacyOrderId}'.");
                    return;
                }

                if (pharmacyOrderNotice == null)
                    Log.Debug($"OMNL notice was not required for order '{pharmacyOrder.PharmacyOrderKey}:{pharmacyOrder.PharmacyOrderId}'.");
                else
                    GenerateOMNLNotice(pharmacyOrderNotice, pharmacyOrderComponentNotices);
            }
            catch (Exception ex)
            {
                Log.Error(string.Format(CultureInfo.InvariantCulture, "An unexpected error occurred while generating OMNL notice for order '{0}'.", pharmacyOrderKey), ex);
            }
        }

        #endregion

        #region Private Members

        #region GetPharmacyOrderNotice

        private PharmacyOrderNotice GetPharmacyOrderNotice(
            HcOrderDAL.Models.PharmacyOrder pharmacyOrder, 
            EncounterUnit encounterUnit,
            out bool isOrderedItemVariableDoseGroup,
            out Guid? comboMedFacilityItemKey)
        {
            Log.Debug($"Checking if pharmacy order '{pharmacyOrder.PharmacyOrderKey}:{pharmacyOrder.PharmacyOrderId}' needs an OMNL notice.");

            // Initialize out parameters
            isOrderedItemVariableDoseGroup = false;
            comboMedFacilityItemKey = null;
            
            // Create our pharmacy order notice
            var pharmacyOrderNotice = new PharmacyOrderNotice
            {
                PharmacyOrderKey = pharmacyOrder.PharmacyOrderKey,
                FacilityKey = encounterUnit.FacilityKey,
                GiveItemNotLoaded = true,
                PrinterName = encounterUnit.OMNLNoticePrinterName
            }; 

            // Try to get a facility item based on the pharmacy order GiveItemKey
            ItemDAL.Models.FacilityItem facilityItem = null;
            if (pharmacyOrder.GiveItemKey.HasValue)
            {
                facilityItem = _facilityItemRepository.GetFacilityItem(encounterUnit.FacilityKey,
                    pharmacyOrder.GiveItemKey.Value);
            }

            // Let's check if we need a notice
            if (!pharmacyOrder.GiveItemKey.HasValue)
            {
                Log.Debug("GiveItemKey not specified therefore requires an OMNL notice.");
            }
            else if (facilityItem == null)
            {
                Log.Debug($"GiveItemKey '{pharmacyOrder.GiveItemKey}' is not approved within the facility therefore requires an OMNL notice.");
            }
            else if (facilityItem.OMNLNoticeFlag)
            {
                var devicesNotLoaded = Enumerable.Empty<Guid>();
                var medItem = _itemRepository.GetMedItem(pharmacyOrder.GiveItemKey.Value);

                // Check if the GiveItem is a VDG
                if (medItem != null && medItem.MedItemTypeInternalCode == MedItemTypeInternalCodes.VDG)
                {
                    Log.Debug($"GiveItemKey '{pharmacyOrder.GiveItemKey}' is a variable dose group, getting group items.");

                    isOrderedItemVariableDoseGroup = true;
                }
                // Check if the GiveItem is a Combo
                else if (facilityItem.ComboFlag)
                {
                    Log.Debug($"GiveItemKey '{pharmacyOrder.GiveItemKey}' is a combo med, getting combo components.");

                    comboMedFacilityItemKey = facilityItem.FacilityItemKey;
                }
                else if (!IsItemLoaded(pharmacyOrder.GiveItemKey.Value, encounterUnit, out devicesNotLoaded))
                {
                    Log.Debug($"GiveItemKey '{pharmacyOrder.GiveItemKey}' is not loaded therefore requires an OMNL notice.");

                    // if the order's give item is not loaded, generate a notice
                    pharmacyOrderNotice.DispensingDeviceKeys = devicesNotLoaded;
                }
                else
                {
                    Log.Debug($"GiveItemKey '{pharmacyOrder.GiveItemKey}' does not require an OMNL notice since it is loaded.");

                    pharmacyOrderNotice = null;
                }
            }
            else
            {
                Log.Warn($"GiveItemKey '{pharmacyOrder.GiveItemKey}' for order '{pharmacyOrder.PharmacyOrderKey}:{pharmacyOrder.PharmacyOrderId}' does not require an OMNL notice since it does not qualify for an OMNL notice.");

                pharmacyOrderNotice = null;
            }

            return pharmacyOrderNotice;
        }

        #endregion

        #region GetPharmacyOrderComponentNotices

        private List<PharmacyOrderComponentNotice> GetPharmacyOrderComponentNotices(
            HcOrderDAL.Models.PharmacyOrder pharmacyOrder, 
            EncounterUnit encounterUnit,
            out int pharmacyOrderComponentCount)
        {
            Log.Debug($"Checking if order '{pharmacyOrder.PharmacyOrderKey}:{pharmacyOrder.PharmacyOrderId}' components need an OMNL notice.");

            // Initialize out parameters
            pharmacyOrderComponentCount = 0;

            var componentNotices = new List<PharmacyOrderComponentNotice>();

            // Get the pharmacy order component set, if it has one.
            var pharmacyOrderComponentSet = _pharmacyOrderRepository.GetPharmacyOrderComponentSetByPharmacyOrder(
                pharmacyOrder.PharmacyOrderKey);

            // The pharmacy order does not have a components
            if (pharmacyOrderComponentSet == null)
                return componentNotices;

            pharmacyOrderComponentCount = pharmacyOrderComponentSet.Count;
            foreach (var pharmacyOrderComponent in pharmacyOrderComponentSet)
            {
                // Try to get a facility item based on the pharmacy order GiveItemKey
                ItemDAL.Models.FacilityItem facilityItem = null;
                if (pharmacyOrderComponent.MedItemKey.HasValue)
                {
                    facilityItem = _facilityItemRepository.GetFacilityItem(encounterUnit.FacilityKey,
                        pharmacyOrderComponent.MedItemKey.Value);
                }

                // if the component doesnt have a valid item, generate a notice
                if (!pharmacyOrderComponent.MedItemKey.HasValue)
                {
                    Log.Debug($"MedItemKey not specified for component '{pharmacyOrderComponent.PharmacyOrderComponentKey}', therefore requires an OMNL notice.");

                    componentNotices.Add(new PharmacyOrderComponentNotice { PharmacyOrderComponentKey = pharmacyOrderComponent.PharmacyOrderComponentKey });
                }
                else if (facilityItem == null)
                {
                    Log.Debug($"Component MedItemKey {pharmacyOrderComponent.MedItemKey} is not approved within the facility therefore requires an OMNL notice.");
                    componentNotices.Add(new PharmacyOrderComponentNotice { PharmacyOrderComponentKey = pharmacyOrderComponent.PharmacyOrderComponentKey });
                }
                else 
                {
                    IEnumerable<Guid> devicesNotLoaded;
                    if (facilityItem.OMNLNoticeFlag && 
                        !IsItemLoaded(pharmacyOrderComponent.MedItemKey.Value, encounterUnit, out devicesNotLoaded))
                    {
                        Log.Debug($"Component Item '{pharmacyOrderComponent.MedItemKey}' is not loaded at '{devicesNotLoaded.Count()}' stations, therefore requires and OMNL notice.");

                        componentNotices.Add(
                            new PharmacyOrderComponentNotice
                            {
                                PharmacyOrderComponentKey = pharmacyOrderComponent.PharmacyOrderComponentKey,
                                DispensingDeviceKeys = devicesNotLoaded.ToArray()
                            });
                    }
                }
            }

            return componentNotices;
        }

        #endregion

        #region GetVariableDosageGroupNotices

        private IReadOnlyCollection<PharmacyOrderComponentNotice> GetVariableDosageGroupNotices(
            HcOrderDAL.Models.PharmacyOrder pharmacyOrder,
            EncounterUnit encounterUnit)
        {
            Log.Debug($"Checking if order '{pharmacyOrder.PharmacyOrderKey}:{pharmacyOrder.PharmacyOrderId}' variable dose group members need an OMNL notice.");

            var vdgNotices = new List<PharmacyOrderComponentNotice>();
            if (!pharmacyOrder.GiveItemKey.HasValue)
                return vdgNotices;

            var vdgMedItems = _noticeRepository.GetVariableDoseGroupMedItems(pharmacyOrder.GiveItemKey.Value);
            foreach (var medItemKey in vdgMedItems)
            {
                IEnumerable<Guid> devicesNotLoaded;
                if (!IsItemLoaded(medItemKey, encounterUnit, out devicesNotLoaded, false))
                {
                    Log.Debug($"VDG Item {medItemKey} is not loaded at {devicesNotLoaded.Count()} stations, therefore requires an OMNL notice.");

                    vdgNotices.Add(
                        new PharmacyOrderComponentNotice
                        {
                            MedItemKey = medItemKey,
                            DispensingDeviceKeys = devicesNotLoaded
                        });
                }
            }

            return vdgNotices;
        }

        private IReadOnlyCollection<PharmacyOrderComponentNotice> GetComboComponentNotices(HcOrderDAL.Models.PharmacyOrder pharmacyOrder,
            EncounterUnit encounterUnit, Guid? comboMedFacilityItemKey)
        {
            Log.Debug($"Checking if order '{pharmacyOrder.PharmacyOrderKey}:{pharmacyOrder.PharmacyOrderId}' combo components need an OMNL notice.");

            var comboComponentNotices = new List<PharmacyOrderComponentNotice>();
            if (comboMedFacilityItemKey == null)
                return comboComponentNotices;

            var comboComponents = _noticeRepository.GetComboComponentMedItems(comboMedFacilityItemKey.Value);
            foreach (var medItemKey in comboComponents)
            {
                IEnumerable<Guid> devicesNotLoaded;
                if (!IsItemLoaded(medItemKey, encounterUnit, out devicesNotLoaded, false))
                {
                    Log.Debug($"Combo Item {medItemKey} is not loaded at {devicesNotLoaded.Count()} stations, therefore requires an OMNL notice.");

                    comboComponentNotices.Add(
                        new PharmacyOrderComponentNotice
                        {
                            MedItemKey = medItemKey,
                            DispensingDeviceKeys = devicesNotLoaded
                        });
                }
            }

            return comboComponentNotices;
        }

        #endregion

        #region Is Item Loaded...

        private bool IsItemLoaded(Guid itemKey, EncounterUnit encounterUnit, out IEnumerable<Guid> devicesNotLoaded, bool checkForEquivalencies = true)
        {
            // devices where ordered item is not loaded
            devicesNotLoaded = _noticeRepository.GetDevicesWhereItemNotLoaded(itemKey, encounterUnit.UnitKey);

            // Check if there are any devices where this item is not loaded. If yes then check for 
            // OMNL to print equivalencies flag. 
            // If its ON then no extra processing required.
            // If its OFF then we need to check if any of the equivalent items are loaded on the device 
            // and exclude that device from the devicesNotLoaded list.
            if (devicesNotLoaded.Any() && checkForEquivalencies)
            {
                // If notice needs to be generated
                if (!encounterUnit.OMNLToPrintEquivalenciesFlag)
                {
                    // Check if equivalencies are actually loaded
                    // If yes, no need to trigger OMNL
                    // If no, then trigger OMNL
                    var equivalentItemKeys = _noticeRepository.GetEquivalentItems(itemKey);
                    foreach (var equivalentItemKey in equivalentItemKeys)
                    {
                        var devicesWhereEquivalentItemIsNotLoaded =
                            _noticeRepository.GetDevicesWhereItemNotLoaded(equivalentItemKey, encounterUnit.UnitKey);

                        devicesNotLoaded = devicesNotLoaded.Intersect(devicesWhereEquivalentItemIsNotLoaded);
                    }
                }
            }

            // Check the count of the devices where neither the ordered item nor any of it's equivalent item(s) 
            // are loaded
            return !devicesNotLoaded.Any();
        }

        #endregion

        #region GenerateOMNLNotice

        private void GenerateOMNLNotice(PharmacyOrderNotice pharmacyOrderNotice, IReadOnlyCollection<PharmacyOrderComponentNotice> pharmacyOrderComponentNotices)
        {
            if (pharmacyOrderNotice == null) return;

            using (ConnectionScopeFactory.Create())
            using (ITransactionScope tx = TransactionScopeFactory.Create())
            {
                var publishedNotice = new NoticeDAL.Models.PublishedNotice
                    {
                        NoticeTypeInternalCode = NoticeTypeInternalCodes.OMNL,
                        PharmacyOrderKey = pharmacyOrderNotice.PharmacyOrderKey,
                        FacilityKey = pharmacyOrderNotice.FacilityKey,
                        GiveItemNotLoadedFlag = pharmacyOrderNotice.GiveItemNotLoaded,
                        PrinterName = pharmacyOrderNotice.PrinterName
                    };
                var publishedNoticeKey = _publishedNoticeRepository.InsertPublishedNotice(ActionContext.Now(), publishedNotice);

                // insert any OMNLDispensingDevice links
                if (pharmacyOrderNotice.DispensingDeviceKeys != null)
                {
                    foreach (Guid deviceKey in pharmacyOrderNotice.DispensingDeviceKeys)
                    {
                        _publishedNoticeRepository.InsertOMNLNoticeDispensingDevice(
                            new NoticeDAL.Models.OMNLNoticeDispensingDevice
                            {
                                PublishedNoticeKey = publishedNoticeKey,
                                DispensingDeviceKey = deviceKey
                            });
                    }
                }

                foreach (var componentNotice in pharmacyOrderComponentNotices)
                {
                    var noticePharmacyOrderComponent = new NoticeDAL.Models.NoticePharmacyOrderComponent
                    {
                        PublishedNoticeKey = publishedNoticeKey,
                        PharmacyOrderComponentKey = componentNotice.PharmacyOrderComponentKey,
                        MedItemKey = componentNotice.MedItemKey
                    };

                    var noticePharmacyOrderComponentKey = _publishedNoticeRepository.InsertNoticePharmacyOrderComponent(
                        noticePharmacyOrderComponent);

                    // insert any OMNLDispensingDevice links
                    if (componentNotice.DispensingDeviceKeys != null)
                    {
                        foreach (Guid deviceKey in componentNotice.DispensingDeviceKeys)
                        {
                            _publishedNoticeRepository.InsertOMNLNoticeDispensingDevice(
                                new NoticeDAL.Models.OMNLNoticeDispensingDevice
                                {
                                    NoticePharmacyOrderComponentKey = noticePharmacyOrderComponentKey,
                                    DispensingDeviceKey = deviceKey
                                });
                        }
                    }
                }

                tx.Complete();
            }
        }

        #endregion

        #endregion
    }
}
