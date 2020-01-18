using System;
using System.Text;

namespace POETradeHelper.ItemSearch.Tests
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
            if (condition?.Invoke() == true)
            {
                stringBuilder.AppendLine(text);
            }

            return stringBuilder;
        }
    }
}