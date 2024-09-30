using System;
using System.Linq.Expressions;
using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Data.DynamicQuery
{
    public abstract class Condition<T>
    {
        private readonly Expression<Func<T, object>> _property;
        private readonly SearchOperator _operator;
        private readonly object _value;

        protected Condition(Expression<Func<T,object>> property, SearchOperator op, object value)
        {
            _property = property;
            _operator = op;
            _value = value;
        }

        public static implicit operator Expression<Func<T, bool>>(Condition<T> condition)
        {
            return condition.Predicate;
        }

        protected Expression<Func<T, object>> Property
        {
            get { return _property; }
        }

        protected SearchOperator Operator
        {
            get { return _operator; }
        }
        
        protected object Value
        {
            get { return _value; }
        }

        public abstract Expression<Func<T, bool>> Predicate { get; }
    }

}