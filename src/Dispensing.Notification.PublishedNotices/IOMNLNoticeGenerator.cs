using System;
using System.Threading.Tasks;

namespace Pyxis.Dispensing.Notification.PublishedNotices
{
    public interface IOMNLNoticeGenerator
    {
        void Generate(Guid pharmacyOrderKey);

        Task GenerateAsync(Guid pharmacyOrderKey);
    }
}
