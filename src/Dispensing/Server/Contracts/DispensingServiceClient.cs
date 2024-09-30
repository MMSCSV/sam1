using System;
using System.ServiceModel;

namespace CareFusion.Dispensing.Server.Contracts
{
    public class DispensingServiceClient<TChannel> : ClientBase<TChannel>, IDisposable
        where TChannel : class
    {
        #region Constructors

        public DispensingServiceClient()
        {
        }

        public DispensingServiceClient(string endpointConfigurationName)
            : base(endpointConfigurationName)
        {
        }

        public DispensingServiceClient(string endpointConfigurationName, string remoteHostname)
            : base(endpointConfigurationName)
        {
            if (!string.IsNullOrEmpty(remoteHostname))
            {
                string address = Endpoint.Address.Uri.ToString();
                address = address.Replace("localhost", remoteHostname);
                Endpoint.Address = new EndpointAddress(address);
            }
        }     
        #endregion

        #region Public Properties

        public TChannel Proxy
        {
            get { return Channel; }
        }

        #endregion        

        #region Dispose
        bool _isDisposed;
        public void Dispose()
        {
            if (_isDisposed)
                return;

            FreeManagedObjects();

            _isDisposed = true;
        }
        protected virtual void FreeManagedObjects()
        {
            // If its not open then there is no reason to call close.
            if (State != CommunicationState.Opened)
                return;

            try
            {
                Close();
            }
            catch (CommunicationException)
            {
                Abort();
            }
            catch (TimeoutException)
            {
                Abort();
            }
            catch (Exception)
            {
                Abort();
                throw;
            }
        }
        #endregion 
    }
}
