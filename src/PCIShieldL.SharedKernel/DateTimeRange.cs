using System;
using System.Collections.Generic;
using Ardalis.GuardClauses;
namespace PCIShieldLib.SharedKernel
{
    public class DateTimeRange : ValueObject
    {
        public DateTimeRange(DateTime start, DateTime end)
        {
            Guard.Against.OutOfRange(start, nameof(start), start, end);
            Start = start;
            End = end;
        }
        public DateTimeRange(DateTime start, TimeSpan duration) : this(start, start.Add(duration))
        {
        }
        public DateTime Start { get; }
        public DateTime End { get; }
        public int DurationInMinutes() => (int)Math.Round((End - Start).TotalMinutes, 0);
        public DateTimeRange NewDuration(TimeSpan newDuration) => new DateTimeRange(Start, newDuration);
        public DateTimeRange NewEnd(DateTime newEnd) => new DateTimeRange(Start, newEnd);
        public DateTimeRange NewStart(DateTime newStart) => new DateTimeRange(newStart, End);
        public static DateTimeRange CreateOneDayRange(DateTime day) => new DateTimeRange(day, day.AddDays(1));
        public static DateTimeRange CreateOneWeekRange(DateTime startDay) => new DateTimeRange(startDay, startDay.AddDays(7));
        public bool Overlaps(DateTimeRange dateTimeRange) => Start < dateTimeRange.End && End > dateTimeRange.Start;
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Start;
            yield return End;
        }
    }
    public class DateTimeOffsetRange : ValueObject
    {
        public DateTimeOffsetRange(DateTimeOffset start, DateTimeOffset end)
        {
            Guard.Against.OutOfRange(start, nameof(start), start, end);
            Start = start;
            End = end;
        }
        public DateTimeOffsetRange(DateTimeOffset start, TimeSpan duration) : this(start, start.Add(duration))
        {
        }
        public DateTimeOffset Start { get; }
        public DateTimeOffset End { get; }
        public int DurationInMinutes() => (int)Math.Round((End - Start).TotalMinutes, 0);
        public DateTimeOffsetRange NewDuration(TimeSpan newDuration) => new DateTimeOffsetRange(Start, newDuration);
        public DateTimeOffsetRange NewEnd(DateTimeOffset newEnd) => new DateTimeOffsetRange(Start, newEnd);
        public DateTimeOffsetRange NewStart(DateTimeOffset newStart) => new DateTimeOffsetRange(newStart, End);
        public static DateTimeOffsetRange CreateOneDayRange(DateTimeOffset day) => new DateTimeOffsetRange(day, day.AddDays(1));
        public static DateTimeOffsetRange CreateOneWeekRange(DateTimeOffset startDay) => new DateTimeOffsetRange(startDay, startDay.AddDays(7));
        public bool Overlaps(DateTimeOffsetRange dateTimeRange) => Start < dateTimeRange.End && End > dateTimeRange.Start;
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Start;
            yield return End;
        }
    }
}