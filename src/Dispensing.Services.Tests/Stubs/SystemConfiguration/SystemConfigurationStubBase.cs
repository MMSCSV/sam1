using System;
using System.Collections.Generic;
using CareFusion.Dispensing.Data.Repositories;
using Pyxis.Core.Data.Schema.Core.Models;

namespace Dispensing.Services.tests.Stubs.SystemConfiguration
{
    public class SystemConfigurationStubBase : ISystemConfigurationRepository
    {
        public virtual SystemConfigType GetSystemConfigType(string systemConfigTypeInternalCode)
        {
            throw new NotImplementedException();
        }

        public virtual IReadOnlyCollection<SystemConfigType> GetSystemConfigTypes()
        {
            throw new NotImplementedException();
        }

        public virtual IReadOnlyCollection<SystemConfig> GetSystemConfigurations()
        {
            throw new NotImplementedException();
        }

        public virtual SystemConfig GetSystemConfiguration(string systemConfigTypeInternalCode)
        {
            throw new NotImplementedException();
        }
    }
}
