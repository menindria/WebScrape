using System;

namespace WebScrape.Common
{
    public static class DateTimeExtensions
    {
        public static string ToStandardDateString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }
    }
}
