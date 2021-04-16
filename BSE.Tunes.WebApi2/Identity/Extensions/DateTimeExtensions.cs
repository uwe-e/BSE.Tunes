using System;

namespace BSE.Tunes.WebApi.Identity.Extensions
{
    public static class DateTimeExtensions
    {
        public static bool HasExceeded(this DateTime creationTime, int seconds, DateTime now)
        {
            return (now > creationTime.AddSeconds(seconds));
        }
    }
}