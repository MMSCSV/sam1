using System.IO;
using System.Text;
using Mms.Logging;

namespace CareFusion.Dispensing.Data.Logging
{
    /// <summary>
    /// Represents a helper TextWriter class to redirect log messages
    /// from the DataContext to a logger.
    /// </summary>
    public class DataContextLogger : TextWriter
    {
        private Encoding _encoding;
        private readonly ILog _log;

        public DataContextLogger(ILog log)
        {
            _log = log;
        }

        public override Encoding Encoding
        {
            get
            {
                if (_encoding == null)
                {
                    _encoding = new UnicodeEncoding(false, false);
                }

                return _encoding;
            }
        }

        public override void Write(string value)
        {
            // We probably want to write sql output in debug only to avoid
            // clutter in other logging modes.
            _log.Debug(value);
        }

        public override void Write(char[] buffer, int index, int count)
        {
            Write(new string(buffer, index, count));
        }
    }
}
