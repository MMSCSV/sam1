using System.Collections.Generic;
using Pyxis.Core.Data.Schema.Core.Models;

namespace CareFusion.Dispensing.Data.Repositories
{
    public interface ISystemConfigurationRepository
    {
        SystemConfigType GetSystemConfigType(string systemConfigTypeInternalCode);
        IReadOnlyCollection<SystemConfigType> GetSystemConfigTypes();
        IReadOnlyCollection<SystemConfig> GetSystemConfigurations();
        SystemConfig GetSystemConfiguration(string systemConfigTypeInternalCode);
    }
}
