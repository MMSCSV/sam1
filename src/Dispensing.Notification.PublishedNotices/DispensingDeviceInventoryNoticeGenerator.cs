using System;
using System.Threading.Tasks;
using Pyxis.Core.Data.InternalCodes;
using Pyxis.Core.Data.Schema;
using Mms.Logging;
using Pyxis.Dispensing.Notification.PublishedNotices.Data;
using Pyxis.Dispensing.Notification.PublishedNotices.Models;
using NoticeDAL = Pyxis.Core.Data.Schema.Notice;

namespace Pyxis.Dispensing.Notification.PublishedNotices
{
    public class DispensingDeviceInventoryNoticeGenerator : IDispensingDeviceInventoryNoticeGenerator
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly NoticeDAL.IPublishedNoticeRepository _publishedNoticeRepository;
        private readonly INoticeRepository _noticeRepository;

        #region Constructors

        public DispensingDeviceInventoryNoticeGenerator()
        {
            _publishedNoticeRepository = new NoticeDAL.PublishedNoticeRepository();
            _noticeRepository = new NoticeRepository();
        }

        #endregion

        #region Public Members

        public Task GenerateAsync()
        {
            return Task.Run(() => Generate());
        }

        public void Generate()
        {
            try
            {
                int updatedRecordCount;
                _noticeRepository.UpdateDispensingDeviceInventoryStates(out updatedRecordCount);

                if (updatedRecordCount > 0)
                {
                    Log.Debug($"Found {updatedRecordCount} inventory state changes. Generating critical low and stock out published notices.");

                    // Generate bulletins that are dependent on inventory state changes.
                    GenerateCriticalLowPublishedNotices();
                    GenerateStockoutPublishedNotices();
                }
            }
            catch (Exception e)
            {
                Log.Error(EventId.NotificationServiceError, "An unexpected error occurred generating dispensing device inventory states.", e);
            }
        }

        #endregion

        #region Private Members

        private void GenerateCriticalLowPublishedNotices()
        {
            try
            {
                var items = _noticeRepository.GetCriticalLowItems();
                Log.Debug($"Found {items.Count} critical low items.");
                foreach (var item in items)
                {
                    CreatePublishedNotice(NoticeTypeInternalCode.CRITLOW, item);
                }
            }
            catch (Exception e)
            {
                Log.Error(EventId.NotificationServiceError, "An unexpected error occurred generating critical low published notices.", e);
            }
            
        }

        private void GenerateStockoutPublishedNotices()
        {
            try
            {
                var items = _noticeRepository.GetStockedOutItems();
                Log.Debug($"Found {items.Count} stocked out items.");
                foreach (var item in items)
                {
                    CreatePublishedNotice(NoticeTypeInternalCode.STOCKOUT, item);
                }
            }
            catch (Exception e)
            {
                Log.Error(EventId.NotificationServiceError, "An unexpected error occurred generating stock out published notices.", e);
            }
        }

        private void CreatePublishedNotice(
            NoticeTypeInternalCode noticeType,
            InventoryItem inventoryItem)
        {
            var publishedNotice = new NoticeDAL.Models.PublishedNotice
            {
                NoticeTypeInternalCode = noticeType.ToInternalCode(),
                FacilityKey = inventoryItem.FacilityKey,
                ItemKey = inventoryItem.ItemKey,
                DispensingDeviceKey = inventoryItem.DispensingDeviceKey,
                PrinterName = inventoryItem.NoticePrinterName
            };

            _publishedNoticeRepository.InsertPublishedNotice(ActionContext.Now(), publishedNotice);
        }

        #endregion
    }
}
