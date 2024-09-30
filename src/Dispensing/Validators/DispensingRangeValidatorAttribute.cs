using System;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace CareFusion.Dispensing.Validators
{
	/// <summary>
	/// Describes a <see cref="DispensingRangeValidator"/>.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property
		| AttributeTargets.Field
		| AttributeTargets.Method
		| AttributeTargets.Parameter,
		AllowMultiple = true,
		Inherited = false)]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1019",
		Justification = "Fields are used internally")]
	public sealed class DispensingRangeValidatorAttribute : ValueValidatorAttribute
	{
		private readonly IComparable _lowerBound;
		private readonly RangeBoundaryType _lowerBoundType;
		private readonly IComparable _upperBound;
		private readonly RangeBoundaryType _upperBoundType;

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="RangeValidatorAttribute"/> class with fully specified
        /// int bound constraints.</para>
        /// </summary>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <seealso cref="RangeBoundaryType"/>
        public DispensingRangeValidatorAttribute(int lowerBound, int upperBound)
            : this((IComparable)lowerBound, RangeBoundaryType.Inclusive, (IComparable)upperBound, RangeBoundaryType.Inclusive)
        { }

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="RangeValidatorAttribute"/> class with fully specified
		/// int bound constraints.</para>
		/// </summary>
		/// <param name="lowerBound">The lower bound.</param>
		/// <param name="lowerBoundType">The indication of how to perform the lower bound check.</param>
		/// <param name="upperBound">The upper bound.</param>
		/// <param name="upperBoundType">The indication of how to perform the upper bound check.</param>
		/// <seealso cref="RangeBoundaryType"/>
		public DispensingRangeValidatorAttribute(int lowerBound, RangeBoundaryType lowerBoundType,
				int upperBound, RangeBoundaryType upperBoundType)
			: this((IComparable)lowerBound, lowerBoundType, (IComparable)upperBound, upperBoundType)
		{ }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="RangeValidatorAttribute"/> class with fully specified
        /// double bound constraints.</para>
        /// </summary>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <seealso cref="RangeBoundaryType"/>
        public DispensingRangeValidatorAttribute(double lowerBound, double upperBound)
            : this((IComparable)lowerBound, RangeBoundaryType.Inclusive, (IComparable)upperBound, RangeBoundaryType.Inclusive)
        { }

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="RangeValidatorAttribute"/> class with fully specified
		/// double bound constraints.</para>
		/// </summary>
		/// <param name="lowerBound">The lower bound.</param>
		/// <param name="lowerBoundType">The indication of how to perform the lower bound check.</param>
		/// <param name="upperBound">The upper bound.</param>
		/// <param name="upperBoundType">The indication of how to perform the upper bound check.</param>
		/// <seealso cref="RangeBoundaryType"/>
		public DispensingRangeValidatorAttribute(double lowerBound, RangeBoundaryType lowerBoundType,
				double upperBound, RangeBoundaryType upperBoundType)
			: this((IComparable)lowerBound, lowerBoundType, (IComparable)upperBound, upperBoundType)
		{ }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="RangeValidatorAttribute"/> class with fully specified
        /// DateTime bound constraints.</para>
        /// </summary>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <seealso cref="RangeBoundaryType"/>
        public DispensingRangeValidatorAttribute(DateTime lowerBound, DateTime upperBound)
            : this((IComparable)lowerBound, RangeBoundaryType.Inclusive, (IComparable)upperBound, RangeBoundaryType.Inclusive)
        { }

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="RangeValidatorAttribute"/> class with fully specified
		/// DateTime bound constraints.</para>
		/// </summary>
		/// <param name="lowerBound">The lower bound.</param>
		/// <param name="lowerBoundType">The indication of how to perform the lower bound check.</param>
		/// <param name="upperBound">The upper bound.</param>
		/// <param name="upperBoundType">The indication of how to perform the upper bound check.</param>
		/// <seealso cref="RangeBoundaryType"/>
		public DispensingRangeValidatorAttribute(DateTime lowerBound, RangeBoundaryType lowerBoundType,
				DateTime upperBound, RangeBoundaryType upperBoundType)
			: this((IComparable)lowerBound, lowerBoundType, (IComparable)upperBound, upperBoundType)
		{ }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="RangeValidatorAttribute"/> class with fully specified
        /// long bound constraints.</para>
        /// </summary>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <seealso cref="RangeBoundaryType"/>
        public DispensingRangeValidatorAttribute(long lowerBound, long upperBound)
            : this((IComparable)lowerBound, RangeBoundaryType.Inclusive, (IComparable)upperBound, RangeBoundaryType.Inclusive)
        { }

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="RangeValidatorAttribute"/> class with fully specified
		/// long bound constraints.</para>
		/// </summary>
		/// <param name="lowerBound">The lower bound.</param>
		/// <param name="lowerBoundType">The indication of how to perform the lower bound check.</param>
		/// <param name="upperBound">The upper bound.</param>
		/// <param name="upperBoundType">The indication of how to perform the upper bound check.</param>
		/// <seealso cref="RangeBoundaryType"/>
		public DispensingRangeValidatorAttribute(long lowerBound, RangeBoundaryType lowerBoundType,
				long upperBound, RangeBoundaryType upperBoundType)
			: this((IComparable)lowerBound, lowerBoundType, (IComparable)upperBound, upperBoundType)
		{ }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="RangeValidatorAttribute"/> class with fully specified
        /// string bound constraints.</para>
        /// </summary>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <seealso cref="RangeBoundaryType"/>
        public DispensingRangeValidatorAttribute(string lowerBound, string upperBound)
            : this((IComparable)lowerBound, RangeBoundaryType.Inclusive, (IComparable)upperBound, RangeBoundaryType.Inclusive)
        { }

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="RangeValidatorAttribute"/> class with fully specified
		/// string bound constraints.</para>
		/// </summary>
		/// <param name="lowerBound">The lower bound.</param>
		/// <param name="lowerBoundType">The indication of how to perform the lower bound check.</param>
		/// <param name="upperBound">The upper bound.</param>
		/// <param name="upperBoundType">The indication of how to perform the upper bound check.</param>
		/// <seealso cref="RangeBoundaryType"/>
		public DispensingRangeValidatorAttribute(string lowerBound, RangeBoundaryType lowerBoundType,
				string upperBound, RangeBoundaryType upperBoundType)
			: this((IComparable)lowerBound, lowerBoundType, (IComparable)upperBound, upperBoundType)
		{ }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="RangeValidatorAttribute"/> class with fully specified
        /// float bound constraints.</para>
        /// </summary>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <seealso cref="RangeBoundaryType"/>
        public DispensingRangeValidatorAttribute(float lowerBound, float upperBound)
            : this((IComparable)lowerBound, RangeBoundaryType.Inclusive, (IComparable)upperBound, RangeBoundaryType.Inclusive)
        { }

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="RangeValidatorAttribute"/> class with fully specified
		/// float bound constraints.</para>
		/// </summary>
		/// <param name="lowerBound">The lower bound.</param>
		/// <param name="lowerBoundType">The indication of how to perform the lower bound check.</param>
		/// <param name="upperBound">The upper bound.</param>
		/// <param name="upperBoundType">The indication of how to perform the upper bound check.</param>
		/// <seealso cref="RangeBoundaryType"/>
		public DispensingRangeValidatorAttribute(float lowerBound, RangeBoundaryType lowerBoundType,
				float upperBound, RangeBoundaryType upperBoundType)
			: this((IComparable)lowerBound, lowerBoundType, (IComparable)upperBound, upperBoundType)
		{ }

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="RangeValidatorAttribute"/> class with fully specified
        /// short bound constraints.</para>
        /// </summary>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <seealso cref="RangeBoundaryType"/>
        public DispensingRangeValidatorAttribute(short lowerBound, short upperBound)
            : this(typeof(short), (IComparable)lowerBound, RangeBoundaryType.Inclusive, (IComparable)upperBound, RangeBoundaryType.Inclusive)
        { }

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="RangeValidatorAttribute"/> class with fully specified
		/// short bound constraints.</para>
		/// </summary>
		/// <param name="lowerBound">The lower bound.</param>
		/// <param name="lowerBoundType">The indication of how to perform the lower bound check.</param>
		/// <param name="upperBound">The upper bound.</param>
		/// <param name="upperBoundType">The indication of how to perform the upper bound check.</param>
		/// <seealso cref="RangeBoundaryType"/>
		public DispensingRangeValidatorAttribute(short lowerBound, RangeBoundaryType lowerBoundType,
				short upperBound, RangeBoundaryType upperBoundType)
			: this(typeof(short), (IComparable)lowerBound, lowerBoundType, (IComparable)upperBound, upperBoundType)
		{}

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="RangeValidatorAttribute"/> class with fully specified
        /// byte bound constraints.</para>
        /// </summary>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <seealso cref="RangeBoundaryType"/>
        public DispensingRangeValidatorAttribute(byte lowerBound, byte upperBound)
            : this(typeof(byte), (IComparable)lowerBound, RangeBoundaryType.Inclusive, (IComparable)upperBound, RangeBoundaryType.Inclusive)
        { }

		/// <summary>
		/// <para>Initializes a new instance of the <see cref="RangeValidatorAttribute"/> class with fully specified
		/// byte bound constraints.</para>
		/// </summary>
		/// <param name="lowerBound">The lower bound.</param>
		/// <param name="lowerBoundType">The indication of how to perform the lower bound check.</param>
		/// <param name="upperBound">The upper bound.</param>
		/// <param name="upperBoundType">The indication of how to perform the upper bound check.</param>
		/// <seealso cref="RangeBoundaryType"/>
		public DispensingRangeValidatorAttribute(byte lowerBound, RangeBoundaryType lowerBoundType,
				byte upperBound, RangeBoundaryType upperBoundType)
			: this(typeof(byte), (IComparable)lowerBound, lowerBoundType, (IComparable)upperBound, upperBoundType)
		{}

		private DispensingRangeValidatorAttribute(Type type, IComparable lowerBound, RangeBoundaryType lowerBoundType,
			IComparable upperBound, RangeBoundaryType upperBoundType)
		{
			// sanity check against the validation application block attribute.
			new RangeValidatorAttribute(type, lowerBound.ToString(), lowerBoundType, upperBound.ToString(), upperBoundType);

			_lowerBound = lowerBound;
			_lowerBoundType = lowerBoundType;
			_upperBound = upperBound;
			_upperBoundType = upperBoundType;
		}

        private DispensingRangeValidatorAttribute(IComparable lowerBound, RangeBoundaryType lowerBoundType,
			IComparable upperBound, RangeBoundaryType upperBoundType)
		{
			// sanity check against the validation application block attribute.
			new RangeValidatorAttribute(lowerBound.ToString(), lowerBoundType, upperBound.ToString(), upperBoundType);

			_lowerBound = lowerBound;
			_lowerBoundType = lowerBoundType;
			_upperBound = upperBound;
			_upperBoundType = upperBoundType;
		}

		/// <summary>
		/// Creates the <see cref="RangeValidator"/> described by the attribute object.
		/// </summary>
		/// <param name="targetType">The type of object that will be validated by the validator.</param>
		/// <remarks>This operation must be overriden by subclasses.</remarks>
		/// <returns>The created <see cref="RangeValidator"/>.</returns>
		protected override Validator DoCreateValidator(Type targetType)
		{
			return new DispensingRangeValidator(_lowerBound, 
                _lowerBoundType, 
                _upperBound, 
                _upperBoundType, 
                Negated);
		}
	}
}
