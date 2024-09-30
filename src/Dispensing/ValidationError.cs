using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using CareFusion.Dispensing.Resources;

namespace CareFusion.Dispensing
{
    public class ValidationError
    {
        public static ValidationError CreateValidationError(string message)
        {
            return new ValidationError(default(string), Guid.NewGuid().ToString(), message);
        }

        public static ValidationError CreateValidationError<T>(string message)
        {
            return new ValidationError(typeof(T), Guid.NewGuid().ToString(), message);
        }

        public static ValidationError CreateValidationError<T>(Expression<Func<T, object>> propertySelector, string message)
        {
            MemberInfo memberInfo = Reflect.GetMemberInfo(propertySelector);
            return new ValidationError(typeof(T), memberInfo.Name, message);
        }

        #region Constructors

        public ValidationError()
        {
        }

        public ValidationError(string message)
            : this(default(string), default(string), message)
        {
        }

        public ValidationError(Type target, string key, string message)
            : this (target, key, message, null)
        {
        }

        public ValidationError(string target, string key, string message)
            : this(target, key, message, null)
        {
        }

        public ValidationError(Type target, string key, string message, string tag)
        {
            string name = null;
            if (target != null)
                name = target.FullName;

            Target = name;
            Key = key;
            Message = message;
            Tag = tag;
        }

        public ValidationError(string target, string key, string message, string tag)
        {
            Target = target;
            Key = key;
            Message = message;
            Tag = tag;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the name of the contract that failed validation.
        /// </summary>
        /// <value>The name of the contract.</value>
        public string Target { get; set; }

        /// <summary>
        /// Gets or sets the contract property that failed validation.
        /// </summary>
        /// <value>The contract property.</value>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the tag that represents the failed validation.
        /// </summary>
        /// <value>The tag.</value>
        public string Tag { get; set; }

        /// <summary>
        /// Gets or sets the validation error message.
        /// </summary>
        /// <value>The error message.</value>
        public string Message { get; set; }

        #endregion

        #region Overrides

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture,
                                 DispensingResources.ValidationErrorToString,
                                 Target ?? string.Empty, 
                                 Key ?? string.Empty, 
                                 Message ?? string.Empty);
        }

        #endregion
    }
}
