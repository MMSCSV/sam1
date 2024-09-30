using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace CareFusion.Dispensing.Validators
{
    /// <summary>
    /// Performs validation on strings by comparing their lengths to the specified boundaries. 
    /// </summary>
    /// <remarks>
    /// <see langword="null"/> is logged as a successful.
    /// </remarks>
    internal class DispensingStringLengthValidator : StringLengthValidator
    {
        /// <summary>
        /// <para>Initializes a new instance of the <see cref="DispensingStringLengthValidator"/> class with fully specified
        /// bound constraints.</para>
        /// </summary>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="lowerBoundType">The indication of how to perform the lower bound check.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <param name="upperBoundType">The indication of how to perform the upper bound check.</param>
        /// <param name="negated">True if the validator must negate the result of the validation.</param>
        /// <seealso cref="RangeBoundaryType"/>
        internal DispensingStringLengthValidator(int lowerBound, RangeBoundaryType lowerBoundType,
            int upperBound, RangeBoundaryType upperBoundType, bool negated)
            : base(lowerBound, lowerBoundType, upperBound, upperBoundType, negated)
        { }

        /// <summary>
        /// Validates by comparing the length for <paramref name="objectToValidate"/> with the constraints
        /// specified for the validator.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        /// <param name="currentTarget">The object on the behalf of which the validation is performed.</param>
        /// <param name="key">The key that identifies the source of <paramref name="objectToValidate"/>.</param>
        /// <param name="validationResults">The validation results to which the outcome of the validation should be stored.</param>
        /// <remarks>
        /// <see langword="null"/> is logged as successful.
        /// </remarks>
        protected override void DoValidate(string objectToValidate, object currentTarget, string key, ValidationResults validationResults)
        {
            if (objectToValidate != null)
                base.DoValidate(objectToValidate, currentTarget, key, validationResults);
        }
    }
}
