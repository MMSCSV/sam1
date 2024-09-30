using System;
using System.Globalization;
using System.Linq.Expressions;
using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Resources;
using LinqKit;

namespace CareFusion.Dispensing.Data.DynamicQuery
{
    public class StringCondition<T> : Condition<T>
    {
        public StringCondition(Expression<Func<T, object>> property, SearchOperator op, string value)
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

            switch(Operator)
            {
                case SearchOperator.StartsWith:
                    var startsWith = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
                    expression = Expression.Lambda<Func<T, bool>>(
                        Expression.Call(Property.Body, startsWith, Expression.Constant(Value)),
                            Property.Parameters);
                    break;
                case SearchOperator.Contains:
                    expression = Expression.Lambda<Func<T, bool>>(
                        Expression.Call(Property.Body, "Contains", null, Expression.Constant(Value)),
                            Property.Parameters);
                    break;
                case SearchOperator.EndsWith:
                    var endsWith = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
                    expression = Expression.Lambda<Func<T, bool>>(
                        Expression.Call(Property.Body, endsWith, Expression.Constant(Value)),
                            Property.Parameters);
                    break;
                case SearchOperator.NotContains:
                    {
                        var notContainsExpression = Expression.Lambda<Func<T, bool>>(Expression.Not(
                            Expression.Call(Property.Body, "Contains", null, Expression.Constant(Value))),
                                                                      Property.Parameters);

                        // Search for NULLs
                        var nullExpression = Expression.Lambda<Func<T, bool>>(
                            Expression.Equal(Property.Body, Expression.Constant(null)),
                            Property.Parameters);

                        // Include NULLs within results.
                        expression = notContainsExpression.Or(nullExpression);
                    }
                    break;
                case SearchOperator.Equals:
                    if (object.Equals(Value, "=<empty>"))
                    {
                        expression = Expression.Lambda<Func<T, bool>>(
                            Expression.Equal(Property.Body, Expression.Constant(string.Empty)),
                                Property.Parameters);
                    }
                    else if (object.Equals(Value, "=<null>"))
                    {
                        expression = Expression.Lambda<Func<T, bool>>(
                            Expression.Equal(Property.Body, Expression.Constant(null)),
                                Property.Parameters);
                    }
                    else
                    {
                        // Search for specified string.
                        expression = Expression.Lambda<Func<T, bool>>(
                            Expression.Equal(Property.Body, Expression.Constant(Value)),
                                Property.Parameters);
                    }
                    break;
                case SearchOperator.NotEquals:
                    {
                        var notEqualsExpression = Expression.Lambda<Func<T, bool>>(
                            Expression.NotEqual(Property.Body, Expression.Constant(Value)),
                            Property.Parameters);

                        // Search for NULLs
                        var nullExpression = Expression.Lambda<Func<T, bool>>(
                            Expression.Equal(Property.Body, Expression.Constant(null)),
                            Property.Parameters);

                        // Include NULLs within results.
                        expression = notEqualsExpression.Or(nullExpression);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        String.Format(CultureInfo.CurrentCulture, DataResources.Condition_InvalidStringOperatorFormat, Operator));
            }

            return expression;
        }
    }
}