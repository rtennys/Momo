using System;

namespace Momo.Common
{
    public static class DateExtensionMethods
    {
        public static DateTime January(this int day, int year)
        {
            return new DateTime(year, 1, day);
        }

        public static DateTime February(this int day, int year)
        {
            return new DateTime(year, 2, day);
        }

        public static DateTime March(this int day, int year)
        {
            return new DateTime(year, 3, day);
        }

        public static DateTime April(this int day, int year)
        {
            return new DateTime(year, 4, day);
        }

        public static DateTime May(this int day, int year)
        {
            return new DateTime(year, 5, day);
        }

        public static DateTime June(this int day, int year)
        {
            return new DateTime(year, 6, day);
        }

        public static DateTime July(this int day, int year)
        {
            return new DateTime(year, 7, day);
        }

        public static DateTime August(this int day, int year)
        {
            return new DateTime(year, 8, day);
        }

        public static DateTime September(this int day, int year)
        {
            return new DateTime(year, 9, day);
        }

        public static DateTime October(this int day, int year)
        {
            return new DateTime(year, 10, day);
        }

        public static DateTime November(this int day, int year)
        {
            return new DateTime(year, 11, day);
        }

        public static DateTime December(this int day, int year)
        {
            return new DateTime(year, 12, day);
        }


        public static DateTime At(this DateTime date, TimeSpan time)
        {
            return new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, time.Seconds, date.Kind);
        }

        public static DateTime At(this DateTime date, int hours, int minutes, int seconds = 0)
        {
            return new DateTime(date.Year, date.Month, date.Day, hours, minutes, seconds, date.Kind);
        }


        /// <summary>Raw switching of the kind... no time conversion</summary>
        public static DateTime Utc(this DateTime date)
        {
            return date.ToKind(DateTimeKind.Utc);
        }

        /// <summary>Raw switching of the kind... no time conversion</summary>
        public static DateTime Local(this DateTime date, DateTimeKind kind = DateTimeKind.Utc)
        {
            return date.ToKind(DateTimeKind.Local);
        }

        /// <summary>Raw switching of the kind... no time conversion</summary>
        public static DateTime ToKind(this DateTime date, DateTimeKind kind)
        {
            return date.Kind == kind ? date : new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Millisecond, kind);
        }


        public static string ToRelativeString(this DateTimeOffset date)
        {
            var timeSpan = DateTimeOffset.UtcNow.Subtract(date);
            if (timeSpan.TotalMinutes < 1) return "just now";
            if (timeSpan.TotalMinutes < 2) return "1 minute ago";
            if (timeSpan.TotalMinutes < 60) return string.Format("{0} minutes ago", timeSpan.Minutes);
            if (timeSpan.TotalMinutes < 120) return "1 hour ago";
            if (timeSpan.TotalHours < 24) return string.Format("{0} hours ago", timeSpan.Hours);
            if (timeSpan.TotalDays < 2) return "yesterday";
            if (timeSpan.TotalDays < 7) return string.Format("{0} days ago", timeSpan.Days);
            if (timeSpan.TotalDays < 14) return "last week";
            if (timeSpan.TotalDays < 21) return "2 weeks ago";
            if (timeSpan.TotalDays < 28) return "3 weeks ago";
            if (timeSpan.TotalDays < 60) return "last month";
            if (timeSpan.TotalDays < 365) return string.Format("{0} months ago", Math.Round(timeSpan.TotalDays / 30));
            if (timeSpan.TotalDays < 730) return "last year";
            return string.Format("{0} years ago", Math.Round(timeSpan.TotalDays / 365));
        }

        public static string ToRelativeString(this DateTime date)
        {
            return new DateTimeOffset(date).ToRelativeString();
        }
    }
}
