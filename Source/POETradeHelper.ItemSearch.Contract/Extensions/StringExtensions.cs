using System.Text.RegularExpressions;

namespace POETradeHelper.ItemSearch.Contract.Extensions;

public static partial class StringExtensions
{
    public static string RemoveStatRanges(this string text) => StatRangeRegex().Replace(text, string.Empty);

    public static string RemoveBracketedText(this string text) =>  BracketedTextRegex().Replace(text, match => match.Groups["Text"].Value);

    [GeneratedRegex(@"\(\d+(\.\d+)?\-\d+(\.\d+)?\)", RegexOptions.Compiled)]
    private static partial Regex StatRangeRegex();

    [GeneratedRegex(@"\[[^|]+\|(?<Text>[^\]]+)\]", RegexOptions.Compiled)]
    private static partial Regex BracketedTextRegex();
}