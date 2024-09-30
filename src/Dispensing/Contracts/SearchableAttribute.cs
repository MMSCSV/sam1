using System;

namespace CareFusion.Dispensing.Contracts
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class SearchableAttribute : Attribute
    {
        private readonly SearchFieldType _fieldType;

        public SearchableAttribute(SearchFieldType fieldType)
        {
            _fieldType = fieldType;
        }

        #region Public Properties

        public SearchFieldType FieldType
        {
            get { return _fieldType; }
        }

        #endregion 
    }
}
