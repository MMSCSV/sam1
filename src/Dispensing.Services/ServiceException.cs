using System;
using System.Runtime.Serialization;

namespace CareFusion.Dispensing.Services
{
    public enum ServiceExceptionCode
    {
        General,
        Unhandled,
        Data,
        DataConcurrency,
        DataEntityNotFound,
        AccessDenied
    }

    public class ServiceException : ApplicationException
    {
        private readonly ServiceExceptionCode _code;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceException"/> class.
        /// </summary>
        public ServiceException()
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceException"/> class.
        /// </summary>
        /// <param name="code"></param>
        public ServiceException(ServiceExceptionCode code)
        {
            _code = code;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ServiceException(string message)
            : base(message)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceException"/> class.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message">The message.</param>
        public ServiceException(ServiceExceptionCode code, string message)
            : base(message)
        {
            _code = code;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceException"/> class.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        protected ServiceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ServiceException(string message, Exception innerException)
            : base (message, innerException)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceException"/> class.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ServiceException(ServiceExceptionCode code, string message, Exception innerException)
            : base(message, innerException)
        {
            _code = code;
        }

        public ServiceExceptionCode Code
        {
            get { return _code; }
        }
    }
}
