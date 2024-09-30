using System;
using Mms.Logging;

namespace CareFusion.Dispensing.Services
{
    public abstract class DispensingServiceBase : IDisposable
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        #region Dispose
        bool _isDisposed;
        public void Dispose()
        {
            if (_isDisposed)
                return;

            try
            {
                FreeManagedObjects();
            }
            catch (Exception e)
            {
                Log.Error("Failed to dispose", e);
            }

            _isDisposed = true;
        }
        protected virtual void FreeManagedObjects()
        {

        }
        #endregion
    }
}
