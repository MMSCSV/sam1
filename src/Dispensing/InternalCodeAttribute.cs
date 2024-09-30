using System;

namespace CareFusion.Dispensing
{
     /// <summary>
    /// This attribute is used to represent an internal code
    /// for a value in an enum.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class InternalCodeAttribute : Attribute
    {
        #region Constructors

        public InternalCodeAttribute(string internalCode)
        {
            Guard.ArgumentNotNullOrEmptyString(internalCode, "internalCode");

            InternalCode = internalCode;
        }

        #endregion

        #region Public Properties

        public string InternalCode { get; private set; }

        #endregion 
    }
}
