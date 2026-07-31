using System.Text;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services.Parsers;

namespace POETradeHelper.ItemSearch.Tests.TestHelpers
{
    public static class StringBuilderExtensions
    {
        extension(StringBuilder stringBuilder)
        {
            public StringBuilder AppendLineIfNotEmpty(string text)
            {
                if (!string.IsNullOrEmpty(text))
                {
                    stringBuilder.AppendLine(text);
                }

                return stringBuilder;
            }

            public StringBuilder AppendLine(string text, Func<bool> condition)
            {
                return condition() ? stringBuilder.AppendLine(text) : stringBuilder;
            }

            public StringBuilder AppendItemStats(IEnumerable<ItemStat> itemStats, params StatCategory[] categoriesToPrint)
            {
                var groupedItemStats = itemStats.GroupBy(x => x.StatCategory).ToArray();

                var sb = new StringBuilder();

                if (categoriesToPrint.Length == 0)
                {
                    categoriesToPrint = Enum.GetValues<StatCategory>();
                }

                foreach (var statCategory in categoriesToPrint)
                {
                    var stats = groupedItemStats.FirstOrDefault(x => x.Key == statCategory);
                    var prefixLine = statCategory switch
                    {
                        StatCategory.Implicit => "{ Implicit Modifier }",
                        StatCategory.Fractured => "{ Fractured Modifier }",
                        StatCategory.Crafted => "{ Master Crafted Modifier }",
                        StatCategory.Crucible => "{ Allocated Crucible Passive Skill }",
                        _ => null,
                    };

                    if (stats != null)
                    {
                        foreach (var stat in stats)
                        {
                            if (prefixLine != null)
                            {
                                sb.AppendLine(prefixLine);
                            }

                            sb.AppendLine(stat.Text);
                        }
                    }
                }

                if (sb.Length > 0)
                {
                    stringBuilder.AppendLine(ParserConstants.PropertyGroupSeparator)
                        .AppendLine(sb.ToString());
                }

                return stringBuilder;
            }
        }
    }
}