using System;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace CareFusion.Dispensing.Validators
{
    /// <summary>
    /// Describes a <see cref="DispensingStringLengthValidator"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property
        | AttributeTargets.Field
        | AttributeTargets.Method
        | AttributeTargets.Parameter,
        AllowMultiple = true,
        Inherited = false)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1019",
        Justification = "Fields are used internally")]
    public sealed class DispensingStringLengthValidatorAttribute : ValueValidatorAttribute
    {
        private readonly int _lowerBound;
        private readonly RangeBoundaryType _lowerBoundType;
        private readonly int _upperBound;
        private readonly RangeBoundaryType _upperBoundType;

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="DispensingStringLengthValidatorAttribute"/> class with an upper bound constraint.</para>
        /// </summary>
        /// <param name="upperBound">The upper bound.</param>
        public DispensingStringLengthValidatorAttribute(int upperBound)
            : this(0, RangeBoundaryType.Ignore, upperBound, RangeBoundaryType.Inclusive)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="DispensingStringLengthValidatorAttribute"/> class with lower and 
        /// upper bound constraints.</para>
        /// </summary>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="upperBound">The upper bound.</param>
        public DispensingStringLengthValidatorAttribute(int lowerBound, int upperBound)
            : this(lowerBound, RangeBoundaryType.Inclusive, upperBound, RangeBoundaryType.Inclusive)
        { }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="DispensingStringLengthValidatorAttribute"/> class with fully specified
        /// bound constraints.</para>
        /// </summary>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="lowerBoundType">The indication of how to perform the lower bound check.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <param name="upperBoundType">The indication of how to perform the upper bound check.</param>
        /// <seealso cref="RangeBoundaryType"/>
        public DispensingStringLengthValidatorAttribute(int lowerBound,
            RangeBoundaryType lowerBoundType,
            int upperBound,
            RangeBoundaryType upperBoundType)
        {
            _lowerBound = lowerBound;
            _lowerBoundType = lowerBoundType;
            _upperBound = upperBound;
            _upperBoundType = upperBoundType;
        }

        /// <summary>
        /// Creates the <see cref="DispensingStringLengthValidator"/> described by the configuration object.
        /// </summary>
        /// <param name="targetType">The type of object that will be validated by the validator.</param>
        /// <returns>The created <see cref="Validator"/>.</returns>
        protected override Validator DoCreateValidator(Type targetType)
        {
            return new DispensingStringLengthValidator(_lowerBound,
                _lowerBoundType,
                _upperBound,
                _upperBoundType,
                Negated);
        }
    }
}
