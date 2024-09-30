using System.Data.SqlClient;
using System.Linq;

namespace CareFusion.Dispensing.Data.Repositories
{
    internal static class SqlErrorNumbers
    {
        internal const int RecordAlreadyModified = 50505;
        internal const int ConstraintViolation = 50507;
        internal const int ConstraintEmptySetViolation = 50508;
        internal const int SqlConstraintViolation = 2627;
        internal const int SqlCheckConstraintViolation = 547;
        internal const int SqlUniqueIndexViolation = 2601;
        internal const int SqlXmlValidation = 6908;
        internal const int SqlTimeoutError = -2;

        /// <summary>
        /// Determines whether the specified SQL error number(s) is equal to the
        /// error number returned by the <see cref="SqlException"/> message.
        /// </summary>
        /// <param name="e">The SqlException.</param>
        /// <param name="errorNumbers">The SQL error numbers to compare.</param>
        /// <returns>True if any of the specified SQL error numbers is equal to <see cref="SqlException"/> message, otherwise false.</returns>
        /// <remarks>
        /// In order to parse the error number from the error message, it is
        /// expected that the error message is is formatted as the following:
        /// [ErrorNumber]: [ErrorMessage]
        /// </remarks>
        internal static bool EqualsSqlErrorNumber(this SqlException e, params int[] errorNumbers)
        {
            Guard.ArgumentNotNull(e, "e");

            // Check the errors collection first
            for (int i = 0; i < e.Errors.Count; i++)
            {
                if (errorNumbers.Contains(e.Errors[i].Number))
                    return true;
            }

            // Check the error message itself.
            string errorMessage = e.Message;
            if (!string.IsNullOrEmpty(errorMessage))
            {
                int index = errorMessage.IndexOf(':');
                if (index >= 0)
                {
                    string errorNumberText = errorMessage.Substring(0, index);
                    int errorNumber;
                    if (int.TryParse(errorNumberText, out errorNumber))
                    {
                        return errorNumbers.Contains(errorNumber);
                    }
                    
                }
            }

            return false;
        }
    }
}
