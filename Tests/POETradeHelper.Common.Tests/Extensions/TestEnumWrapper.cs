using System.ComponentModel.DataAnnotations;

namespace POETradeHelper.Common.Tests.Extensions
{
    public static class TestEnumWrapper
    {
        internal const string DisplayName = "Test Value";

        internal enum TestEnum
        {
            [Display(Name = DisplayName)]
            TestValue,

            TestValue1,
        }
    }
}