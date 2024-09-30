using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using CareFusion.Dispensing.Resources;

namespace CareFusion.Dispensing
{
    /// <summary>
    /// This exception is thrown when a validation check fails when inserting 
    /// or updating an entity.
    /// </summary>
    public class ValidationException : ApplicationException
    {
        private readonly IList<ValidationError> _errors;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class.
        /// </summary>
        public ValidationException()
            : this(default(string), new ValidationError[0])
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ValidationException(string message)
            : this(message, new ValidationError[0])
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="validationErrors"></param>
        public ValidationException(string message, IEnumerable<ValidationError> validationErrors)
            : base(message)
        {
            _errors = new List<ValidationError>(validationErrors);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        protected ValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {}

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the validation errors.
        /// </summary>
        public IEnumerable<ValidationError> Errors
        {
            get { return _errors; }
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (ValidationError validationError in _errors)
            {
                sb.AppendLine(validationError.Message);
            }

            string validationErrors = sb.ToString();
            validationErrors = validationErrors.TrimEnd(Environment.NewLine.ToCharArray());

            return string.Format(CultureInfo.CurrentCulture,
                DispensingResources.ValidationFailedFaultToString,
                validationErrors);
        }

        #endregion
    }
}
