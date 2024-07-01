using System;
using System.Text;
using UnityEngine;

namespace Demo.Services
{
    public class VirtualClockService
    {
        private const int DaysInWeek = 7;

        public DateTime Now => DateTime.Now;
        public DateTime Today => new DateTime(Now.Year, Now.Month, Now.Day);
        public DateTime Tomorrow => Today.AddDays(1);
        public DateTime Weekend => Today.AddDays(DaysInWeek - GetDayOfWeekIndex(Today.DayOfWeek));

        public TimeSpan TimeRemaining(DateTime endTime) => endTime - Now;
        public TimeSpan ElapsedTime(DateTime startTime) => Now - startTime;

        public bool IsPreviousDay(DateTime date)
        {
            var yesterday = Now.Date.AddDays(-1);

            return date.Date.Year == yesterday.Year &&
                   date.Date.Month == yesterday.Month &&
                   date.Date.Day == yesterday.Day;
        }

        public int GetDayOfWeekIndex(DayOfWeek dayOfWeek) => dayOfWeek switch
        {
            System.DayOfWeek.Monday => 0,
            System.DayOfWeek.Tuesday => 1,
            System.DayOfWeek.Wednesday => 2,
            System.DayOfWeek.Thursday => 3,
            System.DayOfWeek.Friday => 4,
            System.DayOfWeek.Saturday => 5,
            System.DayOfWeek.Sunday => 6,
            _ => throw new ArgumentOutOfRangeException()
        };

        public string FormatTimeSpanDefault(TimeSpan timeSpan)
        {
            const string secondsSuffix = "s";
            const string minutesSuffix = "m";
            const string hoursSuffix = "h";
            const string daysSuffix = "d";

            var totalSeconds = Mathf.CeilToInt((float)timeSpan.TotalMilliseconds / 1000);

            return timeSpan.Duration().Days >= 1
                ? $"{timeSpan.Days}{daysSuffix} {timeSpan.Hours}{hoursSuffix}"
                : timeSpan.Duration().Hours >= 1
                    ? $"{timeSpan.Hours}{hoursSuffix} {timeSpan.Minutes}{minutesSuffix}"
                    : timeSpan.Duration().Minutes >= 1
                        ? $"{timeSpan.Minutes}{minutesSuffix} {timeSpan.Seconds}{secondsSuffix}"
                        : totalSeconds >= 1
                            ? $"{totalSeconds}{secondsSuffix}"
                            : $"0{secondsSuffix}";
        }

        public string FormatTimeSpanExtended(TimeSpan timeSpan)
        {
            const string secondsSuffix = "s";
            const string minutesSuffix = "m";
            const string hoursSuffix = "h";
            const string daysSuffix = "d";

            var sb = new StringBuilder();
            if (timeSpan.Duration().Days >= 1)
            {
                sb.Append(timeSpan.Days).Append(daysSuffix).Append(" ");
            }

            if (timeSpan.Duration().Hours >= 1)
            {
                sb.Append(timeSpan.Hours).Append(hoursSuffix).Append(" ");
            }

            sb.Append(timeSpan.Minutes).Append(minutesSuffix).Append(" ")
                .Append(timeSpan.Seconds).Append(secondsSuffix);

            return sb.ToString();
        }

        public string FormatNonZeroTimeSpan(TimeSpan timeSpan)
        {
            const string secondsSuffix = "s";
            const string minutesSuffix = "m";
            const string hoursSuffix = "h";
            const string daysSuffix = "d";

            var sb = new StringBuilder();
            if (timeSpan.Duration().Days >= 1)
            {
                sb.Append(timeSpan.Days).Append(daysSuffix).Append(" ");
            }

            if (timeSpan.Duration().Hours >= 1)
            {
                sb.Append(timeSpan.Hours).Append(hoursSuffix).Append(" ");
            }

            if (timeSpan.Duration().Minutes >= 1)
            {
                sb.Append(timeSpan.Minutes).Append(minutesSuffix).Append(" ");
            }

            if (timeSpan.Duration().Seconds >= 1)
            {
                sb.Append(timeSpan.Seconds).Append(secondsSuffix);
            }

            return sb.ToString();
        }
    }
}