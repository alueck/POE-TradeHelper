using System.Text.RegularExpressions;

namespace POETradeHelper.ItemSearch.Contract.Extensions;

public static partial class StringExtensions
{
    public static string RemoveStatRanges(this string text) => StatRangeRegex().Replace(text, string.Empty);

    [GeneratedRegex(@"\(\d+(\.\d+)?\-\d+(\.\d+)?\)", RegexOptions.Compiled)]
    private static partial Regex StatRangeRegex();
}