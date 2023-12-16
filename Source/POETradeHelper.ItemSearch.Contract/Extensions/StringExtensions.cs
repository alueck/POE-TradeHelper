using System.Text.RegularExpressions;

namespace POETradeHelper.ItemSearch.Contract.Extensions;

public static class StringExtensions
{
    private static readonly Regex StatRangeRegex = new(@"\(\d+(\.\d+)?\-\d+(\.\d+)?\)", RegexOptions.Compiled);

    public static string RemoveStatRanges(this string text) => StatRangeRegex.Replace(text, string.Empty);
}