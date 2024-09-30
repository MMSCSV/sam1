using System.Collections.Generic;
using System.Linq;
using Pyxis.Core.Data.Schema.Core.Models;

namespace Dispensing.Services.tests.Stubs.SystemConfiguration
{
    public class SystemConfigurationStub: SystemConfigurationStubBase
    {
        private readonly IReadOnlyCollection<SystemConfig> _systemConfigs;

        public SystemConfigurationStub(IReadOnlyCollection<SystemConfig> systemConfigs)
        {
            _systemConfigs = systemConfigs;
        }

        public override SystemConfig GetSystemConfiguration(string systemConfigTypeInternalCode)
        {
            return
                _systemConfigs.FirstOrDefault(sc => sc.SystemConfigTypeInternalCode.Equals(systemConfigTypeInternalCode));
        }
    }
}
