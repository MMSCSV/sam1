using System;
using System.Data.Linq;

namespace CareFusion.Dispensing.Data
{
    public static class ByteArrayExtensions
    {
        /// <summary>
        /// Returns a <see cref="Binary"/> instance from a byte array safely.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>A <see cref="Binary"/> object, otherwise null if source is null.</returns>
        public static Binary ToBinary(this byte[] source)
        {
            return source ?? (Binary)null;
        }
    }

    /// <summary>
    /// This class is a workaround for the limitation in LINQ to SQL Binary type for rowversion. The Binary
    /// type does not have a lambda expression for less than and greater than operators. 
    /// 
    /// Internally the System.Data.Linq.SqlClient.PreBindDotNetConverter.IsCompareMethod(SQlMethodCall call) says
    /// that if the method call expression being parsed points to a static method named "Compare" which has 
    /// two parameters and returns an int, then its a comparison method. Being recognized as a comparison method
    ///  means that PreBindDotNetConverter class should turn it into a comparison expression which could be GreaterThan,
    /// LessThan, or Equals.
    /// </summary>
    /// <example>
    /// Binary lastTimeStamp = GetLastTimeStamp();
    ///
    /// var list = from c in dataContext.Customers
    ///            c.LastModifiedBinaryValue.Compare(lastTimeStamp) > 0 
    /// </example>
    public static class BinaryComparer
    {
        public static int Compare(this Binary b1, Binary b2)
        {
            throw new NotImplementedException();
        }
    }
}
