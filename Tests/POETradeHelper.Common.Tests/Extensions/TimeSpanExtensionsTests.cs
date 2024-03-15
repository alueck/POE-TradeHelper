using System;

using FluentAssertions;

using NUnit.Framework;

using POETradeHelper.Common.Extensions;
using POETradeHelper.Common.Properties;

namespace POETradeHelper.Common.Tests.Extensions
{
    public class TimeSpanExtensionsTests
    {
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(2.5)]
        [TestCase(2.6)]
        public void ToHumanReadableStringShouldReturnDays(double numberOfDays) =>
            ToHumanReadableStringShouldReturnResult(numberOfDays, TimeSpan.FromDays, Resources.Days);

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(2.5)]
        [TestCase(2.6)]
        public void ToHumanReadableStringShouldReturnHours(double numberOfHours) =>
            ToHumanReadableStringShouldReturnResult(numberOfHours, TimeSpan.FromHours, Resources.Hours);

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(2.5)]
        [TestCase(2.6)]
        public void ToHumanReadableStringShouldReturnMinutes(double numberOfMinutes) =>
            ToHumanReadableStringShouldReturnResult(numberOfMinutes, TimeSpan.FromMinutes, Resources.Minutes);

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(2.5)]
        [TestCase(2.6)]
        public void ToHumanReadableStringShouldReturnSeconds(double numberOfMinutes) =>
            ToHumanReadableStringShouldReturnResult(numberOfMinutes, TimeSpan.FromSeconds, Resources.Seconds);

        [Test]
        public void ToHumanReadableStringShouldReturnDayIfHoursWouldRoundTo24()
        {
            TimeSpan timeSpan = TimeSpan.FromHours(23.7);

            string result = timeSpan.ToHumanReadableString();

            result.Should().Be($" 1{Resources.Days}");
        }

        [Test]
        public void ToHumanReadableStringShouldReturnHourIfMinutesWouldRoundTo60()
        {
            TimeSpan timeSpan = TimeSpan.FromMinutes(59.7);

            string result = timeSpan.ToHumanReadableString();

            result.Should().Be($" 1{Resources.Hours}");
        }

        [Test]
        public void ToHumanReadableStringShouldReturnMinuteIfSecondsWouldRoundTo60()
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(59.7);

            string result = timeSpan.ToHumanReadableString();

            result.Should().Be($" 1{Resources.Minutes}");
        }

        private static void ToHumanReadableStringShouldReturnResult(
            double time,
            Func<double, TimeSpan> timeSpanFactory,
            string unit)
        {
            TimeSpan timeSpan = timeSpanFactory(time);

            string result = timeSpan.ToHumanReadableString();

            result.Should().Be($"{time,2:N0}{unit}");
        }
    }
}