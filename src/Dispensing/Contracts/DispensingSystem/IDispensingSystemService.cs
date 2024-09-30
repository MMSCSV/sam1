using CareFusion.Dispensing.Models;

namespace CareFusion.Dispensing.Contracts
{
    public interface IDispensingSystemService
    {
        DispensingSystem GetDispensingSystemSettings(bool refreshCache = false);
    }
}

