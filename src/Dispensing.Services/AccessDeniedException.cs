using System;
using System.Runtime.Serialization;

namespace CareFusion.Dispensing.Services
{
    public class AccessDeniedException : ServiceException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccessDeniedException"/> class.
        /// </summary>
        public AccessDeniedException()
            : base(ServiceExceptionCode.AccessDenied)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessDeniedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public AccessDeniedException(string message)
            : base(ServiceExceptionCode.AccessDenied, message)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessDeniedException"/> class.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        protected AccessDeniedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessDeniedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public AccessDeniedException(string message, Exception innerException)
            : base(ServiceExceptionCode.AccessDenied, message, innerException)
        {}
    }
}
