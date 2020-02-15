using NUnit.Framework;
using POETradeHelper.Common.Extensions;
using System;

namespace POETradeHelper.Common.Tests.Extensions
{
    public class TimeSpanExtensionsTests
    {
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(2.5)]
        [TestCase(2.6)]
        public void ToHumanReadableStringShouldReturnDays(double numberOfDays)
        {
            this.ToHumanReadableStringShouldReturnResult(numberOfDays, TimeSpan.FromDays, Properties.Resources.Days);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(2.5)]
        [TestCase(2.6)]
        public void ToHumanReadableStringShouldReturnHours(double numberOfHours)
        {
            this.ToHumanReadableStringShouldReturnResult(numberOfHours, TimeSpan.FromHours, Properties.Resources.Hours);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(2.5)]
        [TestCase(2.6)]
        public void ToHumanReadableStringShouldReturnMinutes(double numberOfMinutes)
        {
            this.ToHumanReadableStringShouldReturnResult(numberOfMinutes, TimeSpan.FromMinutes, Properties.Resources.Minutes);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(2.5)]
        [TestCase(2.6)]
        public void ToHumanReadableStringShouldReturnSeconds(double numberOfMinutes)
        {
            this.ToHumanReadableStringShouldReturnResult(numberOfMinutes, TimeSpan.FromSeconds, Properties.Resources.Seconds);
        }

        [Test]
        public void ToHumanReadableStringShouldReturnDayIfHoursWouldRoundTo24()
        {
            TimeSpan timeSpan = TimeSpan.FromHours(23.7);

            string result = timeSpan.ToHumanReadableString();

            Assert.That(result, Is.EqualTo($" 1{Properties.Resources.Days}"));
        }

        [Test]
        public void ToHumanReadableStringShouldReturnHourIfMinutesWouldRoundTo60()
        {
            TimeSpan timeSpan = TimeSpan.FromMinutes(59.7);

            string result = timeSpan.ToHumanReadableString();

            Assert.That(result, Is.EqualTo($" 1{Properties.Resources.Hours}"));
        }

        [Test]
        public void ToHumanReadableStringShouldReturnMinuteIfSecondsWouldRoundTo60()
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(59.7);

            string result = timeSpan.ToHumanReadableString();

            Assert.That(result, Is.EqualTo($" 1{Properties.Resources.Minutes}"));
        }

        private void ToHumanReadableStringShouldReturnResult(double time, Func<double, TimeSpan> timeSpanFactory, string unit)
        {
            TimeSpan timeSpan = timeSpanFactory(time);

            string result = timeSpan.ToHumanReadableString();

            Assert.That(result, Is.EqualTo($"{time,2:N0}{unit}"));
        }
    }
}