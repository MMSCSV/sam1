using System.Threading.Tasks;

namespace Pyxis.Dispensing.Notification.PublishedNotices
{
    public interface IDispensingDeviceInventoryNoticeGenerator
    {
        void Generate();

        Task GenerateAsync();
    }
}