using System;
using System.Linq;

namespace CareFusion.Dispensing.Contracts
{
    [Serializable]
    public class SearchCondition<TField> : SearchCriteria
    {
        internal static readonly Type[] ConvertTypes = new []
            {
                typeof (object),
                typeof (string),
                typeof (bool),
                typeof (char),
                typeof (sbyte),
                typeof (byte),
                typeof (short),
                typeof (ushort),
                typeof (int),
                typeof (uint),
                typeof (long),
                typeof (ulong),
                typeof (float),
                typeof (double),
                typeof (decimal),
                typeof (DateTime),
                typeof (Guid),
                typeof (Guid?)
            };

        #region Contructors

        protected SearchCondition()
        {
        }

        public SearchCondition(TField field, SearchOperator op, object value)
        {
            Field = field;
            Operator = op;
            Value = value;

            if (!IsValidOperator())
            {
                throw new NotSupportedException(string.Format("You cannot use the {0} with the {1} field", op, field));
            }
        }

        public SearchCondition(string field, SearchOperator op, object value)
            : this(field.EnumParse<TField>(true), op, value)
        {
        }


        #endregion

        #region Public Properties

        public TField Field { get; set; }

        public SearchOperator Operator { get; set; }

        public object Value { get; set; }

        #endregion

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", Field, Operator, Value);
        }

        public string ValueToString()
        {
            if (Value == null)
                return default(string);

            return Value.ToString();
        }

        public bool ValueToBoolean()
        {
            if (Value == null)
                return default(bool);

            string v = Value.ToString();

            return bool.Parse(v);
        }

        public Guid ValueToGuid()
        {
            if (Value == null)
                return default(Guid);

            string v = Value.ToString();

            return Guid.Parse(v);
        }

        public Guid? ValueToNullableGuid()
        {
            if (Value == null)
                return default(Guid?);

            string v = Value.ToString();

            return Guid.Parse(v);
        }

        public int ValueToInt32()
        {
            if (Value == null)
                return default(int);
            
            string v = Value.ToString();

            return int.Parse(v);
        }

        public DateTime ValueToDateTime()
        {
            if (Value == null)
                return default(DateTime);

            string v = Value.ToString();

            return DateTime.Parse(v);
        }

        private bool IsValidOperator()
        {
            var fieldType = SearchCriteria.GetFieldType<TField>(Field.ToString());
            var operators = GetOperatorsForType(fieldType);

            return operators.Contains(Operator);
        }
    }
}
