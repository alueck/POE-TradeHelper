using System.Text;

namespace POETradeHelper.ItemSearch.Tests.TestHelpers
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder AppendLineIfNotEmpty(this StringBuilder stringBuilder, string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                stringBuilder.AppendLine(text);
            }

            return stringBuilder;
        }

        public static StringBuilder AppendLine(this StringBuilder stringBuilder, string text, Func<bool> condition)
        {
            return condition() ? stringBuilder.AppendLine(text) : stringBuilder;
        }
    }
}