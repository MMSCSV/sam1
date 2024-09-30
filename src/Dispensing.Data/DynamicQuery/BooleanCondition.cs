using System;
using System.Globalization;
using System.Linq.Expressions;
using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Resources;

namespace CareFusion.Dispensing.Data.DynamicQuery
{
    public class BooleanCondition<T> : Condition<T>
    {
        public BooleanCondition(Expression<Func<T, object>> property, SearchOperator op, bool value)
            : base(property, op, value)
        {
        }

        public override Expression<Func<T, bool>> Predicate
        {
            get { return BuildExpression(); }
        }

        private Expression<Func<T, bool>> BuildExpression()
        {
            Expression<Func<T, bool>> expression;

            var body = Expression.Convert(Property.Body, typeof(bool));
            switch (Operator)
            {
                case SearchOperator.Equals:
                    expression = Expression.Lambda<Func<T, bool>>(
                        Expression.Equal(body, Expression.Constant(bool.Parse(Value.ToString()))),
                            Property.Parameters);
                    break;
                case SearchOperator.NotEquals:
                    expression = Expression.Lambda<Func<T, bool>>(
                        Expression.NotEqual(body, Expression.Constant(bool.Parse(Value.ToString()))),
                            Property.Parameters);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        String.Format(CultureInfo.CurrentCulture, DataResources.Condition_InvalidBooleanOperatorFormat, Operator));
            }

            return expression;
        }
    }
}
