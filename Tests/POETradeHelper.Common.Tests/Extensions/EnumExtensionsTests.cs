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