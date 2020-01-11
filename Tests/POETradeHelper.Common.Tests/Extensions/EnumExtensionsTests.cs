using NUnit.Framework;
using System;
using System.ComponentModel.DataAnnotations;
using static POETradeHelper.Common.Extensions.EnumExtensions;

namespace POETradeHelper.Common.Tests.Extensions
{
    public class EnumExtensionsTests
    {
        private const string DisplayName = "Test Value";

        private enum TestEnum
        {
            [Display(Name = DisplayName)]
            TestValue,

            TestValue1
        }

        [Test]
        public void GetDisplayNameShouldReturnDisplayNameIfAttributeIsSet()
        {
            string result = TestEnum.TestValue.GetDisplayName();

            Assert.That(result, Is.EqualTo(DisplayName));
        }

        [Test]
        public void GetDisplayNameShouldReturnFallbackValueIfAttributeIsNotSet()
        {
            string result = TestEnum.TestValue1.GetDisplayName();

            Assert.That(result, Is.EqualTo(TestEnum.TestValue1.ToString()));
        }
    }
}