using System;
using System.Globalization;
using System.Linq.Expressions;
using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Resources;
using LinqKit;

namespace CareFusion.Dispensing.Data.DynamicQuery
{
    public class GuidCondition<T> : Condition<T>
    {
        public GuidCondition(Expression<Func<T, object>> property, SearchOperator op, Guid? value)
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

            var body = Expression.Convert(Property.Body, typeof(Guid?));
            switch (Operator)
            {
                case SearchOperator.Equals:
                    expression = Expression.Lambda<Func<T, bool>>(
                        Expression.Equal(body, Expression.Constant(Value, typeof(Guid?))),
                            Property.Parameters);
                    break;
                case SearchOperator.NotEquals:
                    {
                        var notEqualsExpression = Expression.Lambda<Func<T, bool>>(
                            Expression.NotEqual(body, Expression.Constant(Value, typeof (Guid?))),
                            Property.Parameters);

                        // Search for NULLs
                        var nullExpression = Expression.Lambda<Func<T, bool>>(
                            Expression.Equal(body, Expression.Constant(null)),
                            Property.Parameters);

                        // Include NULLs within results.
                        expression = notEqualsExpression.Or(nullExpression);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        String.Format(CultureInfo.CurrentCulture, DataResources.Condition_InvalidGuidOperatorFormat, Operator));
            }

            return expression;
        }
    }
}
