using System;
using System.Globalization;
using CareFusion.Dispensing.Resources.Common;

namespace CareFusion.Dispensing
{
    public enum TimePart
    {
        Hour,
        Minute,
        Second,
        Millisecond
    }

    public static class DateTimeExtensions
    {
        public static DateTime TruncateTime(this DateTime source, TimePart timePart)
        {
            DateTime truncated;

            switch (timePart)
            {
                case TimePart.Hour:
                    truncated = new DateTime(source.Year, source.Month, source.Day, 0, 0, 0, 0, source.Kind);
                    break;
                case TimePart.Minute:
                    truncated = new DateTime(source.Year, source.Month, source.Day, source.Hour, 0, 0, 0, source.Kind);
                    break;
                case TimePart.Second:
                    truncated = new DateTime(source.Year, source.Month, source.Day, source.Hour, source.Minute, 0, 0, source.Kind);
                    break;
                case TimePart.Millisecond:
                    truncated = new DateTime(source.Year, source.Month, source.Day, source.Hour, source.Minute, source.Second, 0, source.Kind);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("timePart");
            }

            return truncated;
        }

        public static string ToShortDateStringEx(this DateTime dt)
        {
           
            return dt.ToString(CommonResources.ShortDateFormat);
        }

        public static string ToShortDateTimeStringEx(this DateTime dt)
        {
            return dt.ToString(CommonResources.ShortDateTimeFormat);
        }

        public static string ToShortDateTimeAMPMStringEx(this DateTime dt)
        {
            return dt.ToString(CommonResources.ShortDateTimeAMPMFormat);
        }

        public static string ToShortDateTimeSecondsString(this DateTime dt)
        {
            return dt.ToString(CommonResources.ShortDateTimeSecondsFormat);
        }

        public static string ToShortMonthString(this DateTime dt)
        {
            return dt.ToString(CommonResources.ShortMonthDateFormat);
        }

        public static string ToShortTimeStringEx(this DateTime dt)
        {
            return dt.ToString(CommonResources.ShortTimeFormat);
        }

        public static string ToShortMonthTimeString(this DateTime dt)
        {
            return dt.ToShortMonthString() + " " + dt.ToShortTimeStringEx();
        }

        public static string ToDateOfBirthMonthString(this DateTime dt)
        {
            return dt.ToString(CommonResources.DateOfBirthMonthPrecisionFormat);
        }

        public static string ToDateOfBirthYearString(this DateTime dt)
        {
            return dt.ToString(CommonResources.DateOfBirthYearPrecisionFormat);
        }

        public static string ToShortOSDateTimeString(this DateTime dt)
        {
            return dt.ToShortDateString() + " " + dt.ToShortTimeStringEx();
        }
        public static string ToShortOSDateString(this DateTime dt)
        {
            return dt.ToShortDateString();
        }

        public static bool TryParseEx(string dateString, out DateTime date)
        {
            bool parsed = DateTime.TryParse(dateString, out date);

            if (!parsed)
            {
                parsed = DateTime.TryParseExact(dateString,
                    new string[] { CommonResources.DateControlLongParseFormat, CommonResources.DateControlShortParseFormat },
                    CultureInfo.CurrentUICulture,
                    DateTimeStyles.None,
                    out date);
            }

            return parsed;
        }
    }
}
