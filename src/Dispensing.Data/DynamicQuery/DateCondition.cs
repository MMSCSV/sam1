using System;
using System.Globalization;
using System.Linq.Expressions;
using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Resources;

namespace CareFusion.Dispensing.Data.DynamicQuery
{
    public class DateCondition<T> : Condition<T>
    {
        public DateCondition(Expression<Func<T, object>> prop, SearchOperator op, DateTime value)
            : base(prop, op, value)
        {
        }

        public override Expression<Func<T, bool>> Predicate
        {
            get { return BuildExpression(); }
        }

        private Expression<Func<T, bool>> BuildExpression()
        {
            Expression<Func<T, bool>> expression;

            var body = Expression.Convert(Property.Body, typeof(DateTime));
            var dateProperty = typeof(DateTime).GetProperty("Date");
            var left = Expression.Property(body, dateProperty);
            var right = Expression.Constant(((DateTime) Value).Date);

            switch(Operator)
            {
                case SearchOperator.Equals:
                    expression = Expression.Lambda<Func<T, bool>>(
                        Expression.Equal(left, right),
                        Property.Parameters);
                    break;
                case SearchOperator.NotEquals:
                    expression = Expression.Lambda<Func<T, bool>>(
                        Expression.NotEqual(left, right),
                        Property.Parameters);
                    break;
                case SearchOperator.LessThan:
                    expression = Expression.Lambda<Func<T, bool>>(
                        Expression.LessThan(left, right),
                        Property.Parameters);
                    break;
                case SearchOperator.GreaterThan:
                    expression = Expression.Lambda<Func<T, bool>>(
                        Expression.GreaterThan(left, right),
                        Property.Parameters);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        String.Format(CultureInfo.CurrentCulture, DataResources.Condition_InvalidStringOperatorFormat, Operator));
            }

            return expression;
        }
    }
}