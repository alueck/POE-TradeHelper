using FluentAssertions;
using NUnit.Framework;

using POETradeHelper.Common.Extensions;

namespace POETradeHelper.Common.Tests.Extensions
{
    public class StringExtensionsTests
    {
        [Test]
        public void ParseToEnumByDisplayNameShouldReturnCorrectEnumValueIfDisplayNameIsFound()
        {
            TestEnumWrapper.TestEnum? result =
                TestEnumWrapper.DisplayName.ParseToEnumByDisplayName<TestEnumWrapper.TestEnum>();

            result.Should().Be(TestEnumWrapper.TestEnum.TestValue);
        }

        [Test]
        public void ParseToEnumByDisplayNameShouldReturnNullIfDisplayNameIsNotFound()
        {
            const string invalidDisplayName = "abc";

            TestEnumWrapper.TestEnum? result = invalidDisplayName.ParseToEnumByDisplayName<TestEnumWrapper.TestEnum>();

            result.Should().BeNull();
        }
    }
}