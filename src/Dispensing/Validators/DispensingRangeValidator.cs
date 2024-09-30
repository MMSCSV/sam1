using System;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace CareFusion.Dispensing.Validators
{
    internal class DispensingRangeValidator : RangeValidator
    {
		/// <summary>
		/// <para>Initializes a new instance of the <see cref="RangeValidator"/> class with fully specified
		/// bound constraints.</para>
		/// </summary>
		/// <param name="lowerBound">The lower bound.</param>
		/// <param name="lowerBoundType">The indication of how to perform the lower bound check.</param>
		/// <param name="upperBound">The upper bound.</param>
		/// <param name="upperBoundType">The indication of how to perform the upper bound check.</param>
		/// <param name="negated">True if the validator must negate the result of the validation.</param>
		/// <seealso cref="RangeBoundaryType"/>
		internal DispensingRangeValidator(IComparable lowerBound, RangeBoundaryType lowerBoundType,
			IComparable upperBound, RangeBoundaryType upperBoundType, bool negated)
			: base(lowerBound, lowerBoundType, upperBound, upperBoundType, null, negated)
		{ }
		
        /// <summary>
        /// Validates by comparing <paramref name="objectToValidate"/> with the constraints
        /// specified for the validator.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        /// <param name="currentTarget">The object on the behalf of which the validation is performed.</param>
        /// <param name="key">The key that identifies the source of <paramref name="objectToValidate"/>.</param>
        /// <param name="validationResults">The validation results to which the outcome of the validation should be stored.</param>
        /// <remarks>
        /// <see langword="null"/> is considered a successful validation.
        /// </remarks>
        public override void DoValidate(object objectToValidate, object currentTarget, string key, ValidationResults validationResults)
        {
            if (objectToValidate != null)
                base.DoValidate(objectToValidate, currentTarget, key, validationResults);
        }
    }
}
