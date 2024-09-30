using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CareFusion.Dispensing.Contracts
{
    #region Search Field

    public struct SearchField
    {
        public SearchField(string displayName, SearchFieldType fieldType, string field) : this()
        {
            DisplayName = displayName;
            FieldType = fieldType;
            Field = field;
        }

        public string DisplayName { get; private set; }

        public SearchFieldType FieldType { get; private set; }

        public string Field { get; private set; }
    }

    #endregion

    #region Search Field Operator

    public struct SearchFieldOperator
    {
        private readonly string _displayName;
        private readonly SearchOperator _operator;

        public SearchFieldOperator(string displayName, SearchOperator op)
        {
            _displayName = displayName;
            _operator = op;
        }

        public string DisplayName 
        { 
            get { return _displayName; }
        }

        public SearchOperator Operator
        {

            get { return _operator; }
        }
    }

    #endregion

    [Serializable]
    public abstract class SearchCriteria
    {
        private static Dictionary<SearchOperator, string> _operatorDisplayNames;

        public static IEnumerable<SearchField> GetSearchableFields<T>()
        {
            Type type = typeof(T);
            List<SearchField> searchFields = new List<SearchField>();

            var fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (var fieldInfo in fieldInfos)
            {
                var searchableAttribute = fieldInfo.GetCustomAttributes<SearchableAttribute>(false).FirstOrDefault();

                if (searchableAttribute == null)
                    continue;

                var displayAttribute = fieldInfo.GetCustomAttributes<LocalizableDisplayNameAttribute>(false).FirstOrDefault();

                var fieldType = searchableAttribute.FieldType;
                var displayName = displayAttribute == null ? fieldInfo.Name : displayAttribute.DisplayName;

                searchFields.Add(new SearchField(displayName, fieldType, fieldInfo.Name));
            }

            return searchFields.OrderBy(sf => sf.DisplayName);
        }

        public static IEnumerable<SearchOperator> GetOperatorsForType(SearchFieldType fieldType)
        {
            SearchOperator[] operators;
            switch (fieldType)
            {
                case SearchFieldType.String:
                case SearchFieldType.Text:
                    operators = new[] { SearchOperator.Equals, SearchOperator.NotEquals, SearchOperator.Contains, SearchOperator.NotContains, SearchOperator.StartsWith, SearchOperator.EndsWith };
                    break;
                case SearchFieldType.Number:
                case SearchFieldType.Date:
                case SearchFieldType.Time:
                case SearchFieldType.DateTime:
                case SearchFieldType.UTCDate:
                case SearchFieldType.Binary:
                    operators = new[] { SearchOperator.Equals, SearchOperator.NotEquals, SearchOperator.LessThan, SearchOperator.GreaterThan };
                    break;
                case SearchFieldType.Boolean:
                    operators = new[] { SearchOperator.Equals };
                    break;
                case SearchFieldType.Hidden:
                    operators = new[] { SearchOperator.Equals };
                    break;
                case SearchFieldType.List:
                    operators = new[] { SearchOperator.Equals, SearchOperator.NotEquals };
                    break;
                default:
                    throw new ArgumentOutOfRangeException("fieldType");
            }
            return operators;
        }

        public static IEnumerable<SearchFieldOperator> GetFieldOperators(SearchFieldType fieldType)
        {
            var operatorDisplayNames = GetSearchOperatorDisplayNames();
            var operators = GetOperatorsForType(fieldType);

            return operators.Select(x => new SearchFieldOperator(operatorDisplayNames[x], x)).OrderBy(x => x.DisplayName);
        }

        public static SearchFieldType GetFieldType<T>(string field)
        {
            var fieldInfo = typeof(T).GetField(field);
            var searchableAttribute = fieldInfo.GetCustomAttributes<SearchableAttribute>(false).FirstOrDefault();
            return searchableAttribute.FieldType;
        }

        private static Dictionary<SearchOperator, string> GetSearchOperatorDisplayNames()
        {
            if (_operatorDisplayNames == null)
            {
                _operatorDisplayNames = new Dictionary<SearchOperator, string>();
                Type type = typeof(SearchOperator);

                FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static);
                for (int i = 0; i < fieldInfos.Length; i++)
                {
                    var fieldInfo = fieldInfos[i];
                    LocalizableDisplayNameAttribute displayAttribute =
                        fieldInfo.GetCustomAttributes<LocalizableDisplayNameAttribute>(false)
                            .FirstOrDefault();

                    SearchOperator op = (SearchOperator)fieldInfo.GetValue(null);
                    _operatorDisplayNames[op] = displayAttribute == null ? fieldInfo.Name : displayAttribute.DisplayName;
                }
            }

            return _operatorDisplayNames;
        }
    }
}
