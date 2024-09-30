using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using CareFusion.Dispensing.Resources;

namespace CareFusion.Dispensing
{
    /// <summary>
    /// Represents a utility class with common guard clauses
    /// </summary>
    public static class Guard
    {
        [AttributeUsage( AttributeTargets.Parameter )]
        class ValidatedNotNullAttribute: Attribute
        {
        }

        /// <summary>
        /// Checks a System.Guid argument to ensure it isn't empty
        /// </summary>
        /// <param name="argumentValue">The argument value to check.</param>
        /// <param name="argumentName">The name of the argument.</param>
        public static void ArgumentNotEmpty( [ValidatedNotNull]Guid argumentValue, string argumentName ) {
            if ( Guid.Empty == argumentValue ) {
                throw new ArgumentException( string.Format( CultureInfo.CurrentCulture, DispensingResources.Argument_Must_Not_Be_Empty, argumentName ) );
            }
        }

        /// <summary>
        /// Checks a string argument to ensure it isn't null or empty
        /// </summary>
        /// <param name="argumentValue">The argument value to check.</param>
        /// <param name="argumentName">The name of the argument.</param>
        public static void ArgumentNotNullOrEmptyString( [ValidatedNotNull]string argumentValue, string argumentName ) {
            ArgumentNotNull( argumentValue, argumentName );

            if ( string.IsNullOrWhiteSpace( argumentValue ) )
                throw new ArgumentException( string.Format( CultureInfo.CurrentCulture, DispensingResources.Argument_Must_Not_Be_Empty, argumentName ) );
        }

        public static void ArgumentNotNullOrEmpty( Expression<Func<string>> parameter ) {
            if ( string.IsNullOrEmpty( GetValue( parameter ) ) ) {
                throw new ArgumentNullException( GetName( parameter ) );
            }
        }

        /// <summary>
        /// Checks an argument to ensure it isn't null
        /// </summary>
        /// <param name="argumentValue">The argument value to check.</param>
        /// <param name="argumentName">The name of the argument.</param>
        public static void ArgumentNotNull( [ValidatedNotNull]object argumentValue, string argumentName ) {
            if ( argumentValue == null )
                throw new ArgumentNullException( argumentName );
        }

        public static void ArgumentNotNull<TParameter>( Expression<Func<TParameter>> parameter ) {
            if ( GetValue( parameter ) == null ) {
                throw new ArgumentNullException( GetName( parameter ) );
            }
        }

        static TParameter GetValue<TParameter>( Expression<Func<TParameter>> parameter ) {
            return parameter.Compile().Invoke();
        }
        static string GetName<TParameter>( Expression<Func<TParameter>> parameter ) {
            var memberExpression = parameter.Body as MemberExpression;
            return memberExpression.Member.Name;
        }

        /// <summary>
        /// Checks an Enum argument to ensure that its value is defined by the specified Enum type.
        /// </summary>
        /// <param name="enumType">The Enum type the value should correspond to.</param>
        /// <param name="value">The value to check for.</param>
        /// <param name="argumentName">The name of the argument holding the value.</param>
        public static void EnumValueIsDefined( Type enumType, object value, string argumentName ) {
            if ( Enum.IsDefined( enumType, value ) == false )
                throw new ArgumentException( String.Format( CultureInfo.CurrentCulture,
                    DispensingResources.Argument_Enumeration_Value_Invalid,
                    argumentName, enumType.ToString() ) );
        }

        #region out of bounds, min, and max checks
        //************************************************
        // BUG:       (Milestone) 89586
        // DEV:       J. K. Johnson
        // DATE:      [date completed/checked in]
        // CHANGESET: 245324
        // COMMENTS:
        //
        //************************************************

        /// <summary>
        /// Checks an argument to ensure it isn't null (if nullable) and falls within the specified bounds. 
        /// </summary>
        /// <typeparam name="T">The parameter's type.</typeparam>
        /// <param name="parameter">The parameter expression(?) to be checked.</param>
        /// <param name="min">The minimum permissible value.</param>
        /// <param name="max">The maximum permissible value.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"/>
        public static void ArgumentIsWithinBounds<T>( Expression<Func<T>> parameter, T min, T max ) where T: IComparable, new() {
            var paramInfo = TestForNull<T>( parameter );

            if ( min.CompareTo( max ) == 1 ) {
                var tmp = min;
                min = max;
                max = tmp;
            }

            if ( paramInfo.Value.CompareTo( min ) == -1 || paramInfo.Value.CompareTo( max ) == 1 ) {
                throw new ArgumentOutOfRangeException(
                    paramInfo.Key,
                    string.Format( "{0} must be within the bounds of {1} and {2}",
                    paramInfo.Key,
                    min,
                    max ) );
            }
        }

        /*************************************************
         * CREATED BY:      J. K. JOHNSON
         * DATE CREATED:    2013-03-14 
         * ***********************************************/
        /// <summary>
        /// Checks an argument to ensure it isn't null (if nullable) and no less than the minimum value specified. 
        /// </summary>
        /// <typeparam name="T">The parameter's type.</typeparam>
        /// <param name="parameter">The parameter expression(?) to be checked.</param>
        /// <param name="min">The minimum permissible value.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"/>
        public static void ArgumentIsGreaterThan<T>( Expression<Func<T>> parameter, T min ) where T: IComparable, new() {
            var paramInfo = TestForNull<T>( parameter );

            if ( paramInfo.Value.CompareTo( min ) < 0 ) {
                throw new ArgumentOutOfRangeException(
                    paramInfo.Key,
                    string.Format( "{0} must be greater than or equal to {1}",
                    paramInfo.Key,
                    min ) );
            }
        }

        /*************************************************
         * CREATED BY:      J. K. JOHNSON
         * DATE CREATED:    2013-03-14 
         * ***********************************************/
        /// <summary>
        /// Checks an argument to ensure it isn't null (if nullable) and no greater than the maximum value specified. 
        /// </summary>
        /// <typeparam name="T">The parameter's type.</typeparam>
        /// <param name="parameter">The parameter expression(?) to be checked.</param>
        /// <param name="max">The maximum permissible value.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"/>
        public static void ArgumentNotGreaterThan<T>( Expression<Func<T>> parameter, T max ) where T: IComparable, new() {
            var paramInfo = TestForNull<T>( parameter );

            if ( paramInfo.Value.CompareTo( max ) == 1 ) {
                throw new ArgumentOutOfRangeException(
                    paramInfo.Key,
                    string.Format( "{0} must be greater than or equal to {1}",
                    paramInfo.Key,
                    max ) );
            }
        }

        public static void ArgumentNotNullOrEmpty<T>( Expression<Func<T>> parameter ) where T: IComparable, new() {
            var paramInfo = TestForNull<T>( parameter );

            if (paramInfo.Value is Guid) {
                var uid = new Guid(paramInfo.Value.ToString());

                if (uid == Guid.Empty) {
                    throw new ArgumentOutOfRangeException(
                        paramInfo.Key,
                        string.Format("{0} cannot be an empty {1}.",
                                      paramInfo.Key,
                                      paramInfo.Value.GetType().Name));
                }
            }

            if ( paramInfo.Value is string ) {
                var value = paramInfo.Value.ToString();

                if ( value == string.Empty ) {
                    throw new ArgumentOutOfRangeException(
                        paramInfo.Key,
                        string.Format( "{0} cannot be an empty {1}.",
                                      paramInfo.Key,
                                      paramInfo.Value.GetType().Name ) );
                }
            }
        }

        /*************************************************
         * CREATED BY:      J. K. JOHNSON
         * DATE CREATED:    2013-03-14 
         * ***********************************************/
        /// <summary>
        /// Ensures the argument isn't null and returns the argument's name and value within a KeyValuePair.
        /// </summary>
        /// <typeparam name="T">The parameter's type.</typeparam>
        /// <param name="parameter">The parameter expression(?) to be checked.</param>
        /// <returns>
        /// A <c> KeyValuePair&lt;string, IComparable&gt;</c> containg the name and value of the argument.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"/>
        private static KeyValuePair<string, T> TestForNull<T>( Expression<Func<T>> parameter ) where T: IComparable, new() {
            var pValue = parameter.Compile().Invoke();

            var memExpr = parameter.Body as MemberExpression;
            var pName = memExpr != null ?
                memExpr.Member.Name :
                null;

            if ( !typeof( T ).IsValueType && pValue == null ) {
                throw new ArgumentNullException( pName,
                                                string.Format( "The parameter {0} cannot be null.",
                                                              pName ) );
            }

            return new KeyValuePair<string, T>( pName, pValue );
        }
        #endregion out of bounds, min, and max checks
    }
}
