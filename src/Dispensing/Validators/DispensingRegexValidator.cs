using System;
using System.Text.RegularExpressions;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace CareFusion.Dispensing.Validators
{
    /// <summary>
    /// Performs validation on strings by matching them to a <see cref="Regex"/>.
    /// </summary>
    internal class DispensingRegexValidator : RegexValidator
    {
        /// <summary>
        /// <para>Initializes a new instance of the <see cref="RegexValidator"/> class with a regex pattern, 
        /// matching options and a failure message template.</para>
        /// </summary>
        /// <param name="pattern">The pattern to match.</param>
        /// <param name="options">The <see cref="RegexOptions"/> to use when matching.</param>
        /// <param name="messageTemplate">The message template.</param>
        /// <param name="negated">True if the validator must negate the result of the validation.</param>
        /// <param name="patternResourceName">The resource name containing the pattern for the regular expression.</param>
        /// <param name="patternResourceType">The type containing the resource for the regular expression.</param>
        internal DispensingRegexValidator(string pattern, string patternResourceName, Type patternResourceType, RegexOptions options, string messageTemplate, bool negated)
            : base(pattern, patternResourceName, patternResourceType, options, messageTemplate, negated)
        {
        }

        protected override void DoValidate(string objectToValidate, object currentTarget, string key, Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults validationResults)
        {
            if (objectToValidate != null)
                base.DoValidate(objectToValidate, currentTarget, key, validationResults);
        }
    }
}
