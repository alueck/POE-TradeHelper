using System;
using System.Globalization;

using Avalonia.Data;

using FluentAssertions;

using NUnit.Framework;

using POETradeHelper.Common.UI.Converters;

namespace POETradeHelper.Common.UI.Tests.Converters
{
    public class NullableDecimalStringConverterTests
    {
        private readonly NullableDecimalStringConverter converter;

        public NullableDecimalStringConverterTests()
        {
            this.converter = new NullableDecimalStringConverter();
        }

        [SetCulture("")]
        [TestCase(null, "")]
        [TestCase(10, "10")]
        [TestCase(1.23, "1.23")]
        [TestCase(1.2345, "1.2345")]
        public void ConvertShouldReturnCorrectString(decimal? value, string expected)
        {
            var result = this.converter.Convert(value, typeof(string), null, CultureInfo.CurrentCulture);

            result.Should().Be(expected);
        }

        [SetCulture("de-DE")]
        [TestCase("", null)]
        [TestCase(null, null)]
        [TestCase("1,23", 1.23)]
        [TestCase("-1,23", -1.23)]
        [TestCase("2.34", 2.34)]
        public void ConvertBackShouldReturnCorrectDecimal(string? value, decimal? expected)
        {
            var result = this.converter.ConvertBack(value, typeof(decimal?), null, CultureInfo.CurrentCulture);

            result.Should().Be(expected);
        }

        [Test]
        public void ConvertBackShouldReturnBindingNotificationForInvalidInput()
        {
            var result = this.converter.ConvertBack("abc", typeof(decimal?), null, CultureInfo.CurrentCulture);

            BindingNotification bindingNotification = result.Should().BeOfType<BindingNotification>().Which;
            bindingNotification.Error.Should()
                .BeOfType<InvalidCastException>()
                .Which.Message.Should().Be("'abc' is not a valid number.");
            bindingNotification.ErrorType.Should().Be(BindingErrorType.Error);
        }
    }
}
