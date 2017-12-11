using System;

namespace XwMaxLib.Extensions
{
    public static class MyExtensionDateTime
    {
        public static DateTime Tomorrow(this DateTime date)
        {
            return date.AddDays(1);
        }

        public static string ToStringISO(this DateTime date)
        {
            return date.ToString("yyyy-MM-ddTHH:mm:ss");
        }

        public static Int64 ToUnixTimestamp(this DateTime value)
        {
            TimeSpan span = (value - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());
            return (Int64)span.TotalSeconds;
        }
    }
}
