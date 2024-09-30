using System;
using System.Diagnostics;
using System.Runtime.Caching;
using CareFusion.Dispensing.Caching;
using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Data.Repositories;
using CareFusion.Dispensing.Models;
using Mms.Logging;

namespace CareFusion.Dispensing.Services
{
    public class DispensingSystemService :  IDispensingSystemService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly ObjectCache _cache = MemoryCache.Default;
        private readonly IDispensingSystemRepository _dispensingSystemRepository;

        public DispensingSystemService()
        {
            _dispensingSystemRepository = new DispensingSystemRepository();
        }
          

        public DispensingSystem GetDispensingSystemSettings(bool refreshCache = false)
        {
            DispensingSystem dispensingSystem = _cache.Get(CacheKeys.CommonDispensingSystem) as DispensingSystem;

            if (dispensingSystem == null || refreshCache)
            {
                Stopwatch sw = Stopwatch.StartNew();
                dispensingSystem = _dispensingSystemRepository.GetDispensingSystem();
                Log.Debug("GetDispensingSystem() query took {0} milliseconds.", sw.ElapsedMilliseconds);
                if (dispensingSystem != null)
                {
                    //  Bug 493335
                    var absoluteExpiration = DateTimeOffset.UtcNow.Add(TimeSpan.FromMinutes(5));
                    _cache.Set(CacheKeys.CommonDispensingSystem, dispensingSystem, absoluteExpiration);
                }
            }

            return dispensingSystem;
        }
    }
}
