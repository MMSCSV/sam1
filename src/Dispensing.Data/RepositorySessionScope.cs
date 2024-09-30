using System;
using System.Configuration;
using System.Runtime.Remoting.Messaging;
using CareFusion.Dispensing.Data.Entities;
using CareFusion.Dispensing.Data.Logging;
using Pyxis.Core.Data;
using Mms.Logging;

namespace CareFusion.Dispensing.Data
{
    public sealed class RepositorySessionScope : IDisposable
    {
        public const string EnableSqlLoggingAppSettingName = "EnableSqlLogging";
        private const string CurrentContextName = "CURRENT_REPOSITORY_SESSION";
        private readonly bool _disposeConnectionScope;
        private readonly bool _disposeDataContext;

        /// <summary>
        /// Initializes a new <see cref="RepositorySessionScope"/> using the default configuration connection string setting 'DispensingDatabase'.
        /// </summary>
        public RepositorySessionScope()
        {
            if (!bool.TryParse(ConfigurationManager.AppSettings[EnableSqlLoggingAppSettingName], out var enableSqlLogging))
                enableSqlLogging = false;

            if (Current == null)
            {
                ConnectionScope = ConnectionScopeFactory.Create();

                Context = new BaseDataContext(ConnectionScope.Connection)
                {
                    ObjectTrackingEnabled = true,
                    DeferredLoadingEnabled = true,
                };

                if (enableSqlLogging)
                    Context.Log = new DataContextLogger(LogManager.GetLogger(typeof(BaseDataContext)));

                _disposeConnectionScope = true;
                _disposeDataContext = true;

                Current = this;
            }
            else
            {
                ConnectionScope = Current.ConnectionScope;
                Context = Current.Context;
            }
        }

        internal static RepositorySessionScope Current
        {
            get
            {
                var current = CallContext.GetData(CurrentContextName);

                return (RepositorySessionScope) current;
            }

            private set => CallContext.SetData(CurrentContextName, value);
        }

        internal IConnectionScope ConnectionScope { get; private set; }

        internal BaseDataContext Context { get; private set; }

        #region Dispose

        private bool _isDisposed;
        public void Dispose()
        {
            if (_isDisposed)
                return;

            FreeManagedObjects();

            _isDisposed = true;
        }
        private void FreeManagedObjects()
        {
            if (Current == this)
            {
                CallContext.FreeNamedDataSlot(CurrentContextName);
            }

            if (_disposeDataContext && Context != null)
            {
                Context.Dispose();
                Context = null;
            }

            if (!_disposeConnectionScope) return;

            if (ConnectionScope == null) return;

            ConnectionScope.Dispose();
            ConnectionScope = null;
        }
        #endregion
    }
}
