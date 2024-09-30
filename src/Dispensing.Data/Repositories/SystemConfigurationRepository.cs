using System.Collections.Generic;
using System.Linq;
using Pyxis.Core.Data.Schema.Core;
using Pyxis.Core.Data.Schema.Core.Models;

namespace CareFusion.Dispensing.Data.Repositories
{
    public class SystemConfigurationRepository : ISystemConfigurationRepository
    {
        private readonly ReferenceRepository _referenceRepository;
        private readonly SystemConfigRepository _systemConfigRepository;

        public SystemConfigurationRepository()
        {
            _referenceRepository = new ReferenceRepository();
            _systemConfigRepository = new SystemConfigRepository();
        }

        public SystemConfigType GetSystemConfigType(string systemConfigTypeInternalCode)
        {
            var systemConfigTypes = GetSystemConfigTypes();

            return systemConfigTypes.ToList()
                .FirstOrDefault(sc => sc.SystemConfigTypeInternalCode == systemConfigTypeInternalCode);
        }

        public IReadOnlyCollection<SystemConfigType> GetSystemConfigTypes()
        {
            return _referenceRepository.GetSystemConfigTypes();
        }

        public IReadOnlyCollection<SystemConfig> GetSystemConfigurations()
        {
            return _systemConfigRepository.GetSystemConfigs();
        }

        public SystemConfig GetSystemConfiguration(string systemConfigTypeInternalCode)
        {
            return _systemConfigRepository.GetSystemConfig(systemConfigTypeInternalCode);
        }
    }
}
