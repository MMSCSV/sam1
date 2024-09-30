using System;

namespace CareFusion.Dispensing
{
    public static class DecimalExtensions
    {
        public static decimal ToNumeric14_4(this decimal source)
        {
            return Decimal.Round(source, 4);
        }

        public static decimal? ToNullableNumeric14_4(this decimal? source)
        {
            return (source.HasValue) ? Decimal.Round(source.Value, 4) : default(decimal?);
        }

        public static decimal ToNumeric28_18(this decimal source)
        {
            return Decimal.Round(source, 18);
        }

        public static decimal? ToNullableNumeric28_18(this decimal? source)
        {
            return (source.HasValue) ? Decimal.Round(source.Value, 18) : default(decimal?);
        }

        public static decimal ToNumeric28_14(this decimal source)
        {
            return Decimal.Round(source, 14);
        }

        public static decimal? ToNullableNumeric28_14(this decimal? source)
        {
            return (source.HasValue) ? Decimal.Round(source.Value, 14) : default(decimal?);
        }
    }
}
