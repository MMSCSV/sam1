using System;

namespace CareFusion.Dispensing.DI
{
    public class DIException : Exception
    {
        public DIException()
            : base()
        {
        }

        public DIException(string message)
            : base(message)
        {
        }

        public DIException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
