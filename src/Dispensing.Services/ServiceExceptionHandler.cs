using System;
using System.Data.SqlClient;
using System.Globalization;
using CareFusion.Dispensing.Data;
using CareFusion.Dispensing.Resources;
using Pyxis.Core.Data;
using Mms.Logging;

namespace CareFusion.Dispensing.Services
{
    /// <summary>
    /// Handles exceptions for the service layer.
    /// </summary>
    public static class ServiceExceptionHandler
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Handles the specified <see cref="Exception"/>.
        /// </summary>
        /// <param name="exceptionToHandle">An <see cref="Exception"/> object.</param>
        /// <returns>
        /// Whether or not a rethrow is recommended.
        /// </returns>
        /// <example>
        /// The following code shows the usage of this mehtod.
        /// <code>
        /// try
        ///	{
        ///		Foo();
        ///	}
        ///	catch (Exception e)
        ///	{
        ///		if (ServiceExceptionHandler.HandleException(e)) throw;
        ///	}
        /// </code>
        /// </example>
        public static bool HandleException(Exception exceptionToHandle)
        {
            if (exceptionToHandle == null) throw new ArgumentNullException("exceptionToHandle");

            // Rethrow if this is a ServiceException or ValidationException
            if (typeof(ServiceException).IsInstanceOfType(exceptionToHandle) ||
                typeof(ValidationException).IsInstanceOfType(exceptionToHandle))
                return true;

            // Delegate SqlException's to the DataExceptionHandler.
            if (typeof(SqlException).IsInstanceOfType(exceptionToHandle))
            {
                try
                {
                    DataExceptionHandler.HandleException(exceptionToHandle);
                }
                catch(Exception e)
                {
                    if (typeof(ConcurrencyException).IsInstanceOfType(e))
                    {
                        throw new ServiceException(ServiceExceptionCode.DataConcurrency, e.Message, e);
                    }

                    if (typeof(EntityNotFoundException).IsInstanceOfType(e))
                    {
                        throw new ServiceException(ServiceExceptionCode.DataEntityNotFound, e.Message, e);
                    }

                    throw new ServiceException(ServiceExceptionCode.Data, e.Message, e);
                }
            }
            
            // Wrap the DataException into a ServiceException, do not log.
            if (typeof(DataException).IsInstanceOfType(exceptionToHandle))
            {
                if (typeof(ConcurrencyException).IsInstanceOfType(exceptionToHandle))
                {
                    throw new ServiceException(ServiceExceptionCode.DataConcurrency, exceptionToHandle.Message, exceptionToHandle);
                }

                if (typeof(EntityNotFoundException).IsInstanceOfType(exceptionToHandle))
                {
                    throw new ServiceException(ServiceExceptionCode.DataEntityNotFound, exceptionToHandle.Message, exceptionToHandle);
                }

                throw new ServiceException(ServiceExceptionCode.Data, exceptionToHandle.Message, exceptionToHandle);
            }

            // Unhandled exception.
            string message = string.Format(CultureInfo.CurrentCulture, ServiceResources.UnhandledServiceExceptionFormat,
                                           exceptionToHandle.Message);
            Log.Error(EventId.UnexpectedError, message, exceptionToHandle);
            throw new ServiceException(ServiceExceptionCode.Unhandled, message, exceptionToHandle);
        }
    }
}
