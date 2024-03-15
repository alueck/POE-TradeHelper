using FluentAssertions;

using NUnit.Framework;

using static POETradeHelper.Common.Extensions.EnumExtensions;
using static POETradeHelper.Common.Tests.Extensions.TestEnumWrapper;

namespace POETradeHelper.Common.Tests.Extensions
{
    public class EnumExtensionsTests
    {
        [Test]
        public void GetDisplayNameShouldReturnDisplayNameIfAttributeIsSet()
        {
            string result = TestEnum.TestValue.GetDisplayName();

            result.Should().Be(DisplayName);
        }

        [Test]
        public void GetDisplayNameShouldReturnFallbackValueIfAttributeIsNotSet()
        {
            string result = TestEnum.TestValue1.GetDisplayName();

            result.Should().Be(TestEnum.TestValue1.ToString());
        }
    }
}