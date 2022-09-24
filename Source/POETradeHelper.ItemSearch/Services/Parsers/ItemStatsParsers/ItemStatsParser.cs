using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;

using POETradeHelper.Common.Extensions;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.ItemSearch.Services.Parsers.ItemStatsParsers
{
    public class ItemStatsParser : ItemStatsParserBase, IItemStatsParser<ItemWithStats>
    {
        private const char Placeholder = '#';
        private readonly IPseudoItemStatsParser pseudoItemStatsParser;

        public ItemStatsParser(IStatsDataService statsDataService, IPseudoItemStatsParser pseudoItemStatsParser) : base(statsDataService)
        {
            this.pseudoItemStatsParser = pseudoItemStatsParser;
        }

        public ItemStats Parse(string[] itemStringLines, bool preferLocalStats)
        {
            var result = new ItemStats();

            int statsStartIndex = GetStatsStartIndex(itemStringLines);

            var statTexts = itemStringLines.Skip(statsStartIndex).Where(s => s != ParserConstants.PropertyGroupSeparator);

            var itemStats = statTexts.Select(s => this.ParseStatText(s, preferLocalStats)).Where(x => x != null).ToList();
            var pseudoItemStats = this.pseudoItemStatsParser.Parse(itemStats);

            result.AllStats.AddRange(itemStats);
            result.AllStats.AddRange(pseudoItemStats);

            return result;
        }

        private ItemStat? ParseStatText(string statText, bool preferLocalStats)
        {
            if (!TryGetItemStatForCategoryByMarker(statText, StatCategory.Enchant, out ItemStat? result)
                && !TryGetItemStatForCategoryByMarker(statText, StatCategory.Implicit, out result)
                && !TryGetItemStatForCategoryByMarker(statText, StatCategory.Crafted, out result)
                && !TryGetItemStatForCategoryByMarker(statText, StatCategory.Fractured, out result))
            {
                result = new ItemStat(StatCategory.Unknown)
                {
                    Text = statText,
                };
            }

            return this.GetCompleteItemStat(result, preferLocalStats);
        }

        private static bool TryGetItemStatForCategoryByMarker(string statText, StatCategory statCategory, [NotNullWhen(returnValue: true)] out ItemStat? itemStat)
        {
            itemStat = null;

            int statCategoryMarkerIndex = statText.IndexOf($"({statCategory.GetDisplayName()})", StringComparison.OrdinalIgnoreCase);
            if (statCategoryMarkerIndex >= 0)
            {
                statText = statText.Substring(0, statCategoryMarkerIndex).Trim();

                itemStat = new ItemStat(statCategory)
                {
                    Text = statText
                };

                return true;
            }

            return false;
        }

        protected override ItemStat? GetCompleteItemStat(ItemStat itemStat, bool preferLocalStatData)
        {
            ItemStat? result = base.GetCompleteItemStat(itemStat, preferLocalStatData);

            var placeholderCount = result?.TextWithPlaceholders?.Count(c => c == Placeholder);
            if (placeholderCount == 1)
            {
                result = GetSingleValueItemStat(itemStat);
            }
            else if (placeholderCount == 2)
            {
                result = GetMinMaxValueItemStat(itemStat);
            }

            return result;
        }

        private static ItemStat GetSingleValueItemStat(ItemStat itemStat)
        {
            decimal? value = GetFirstNumericValue(itemStat.Text[itemStat.TextWithPlaceholders.IndexOf(Placeholder)..]);

            return value.HasValue
                ? new SingleValueItemStat(itemStat)
                {
                    Value = value.Value
                }
                : itemStat;
        }

        private static ItemStat GetMinMaxValueItemStat(ItemStat itemStat)
        {
            var result = new MinMaxValueItemStat(itemStat);

            int maxValueIndex = itemStat.TextWithPlaceholders.LastIndexOf(Placeholder);
            if (maxValueIndex >= 0)
            {
                result.MaxValue = GetFirstNumericValue(itemStat.Text[maxValueIndex..]).GetValueOrDefault();
            }

            int minValueIndex = GetMinValueIndex(itemStat, maxValueIndex);
            if (minValueIndex >= 0)
            {
                result.MinValue = GetFirstNumericValue(itemStat.Text[minValueIndex..]).GetValueOrDefault();
            }

            return result;
        }

        private static decimal? GetFirstNumericValue(string text)
        {
            Match match = Regex.Match(text, @"[\+\-]?\d+(\.\d+)?");

            return match.Success
                ? decimal.Parse(match.Value, CultureInfo.InvariantCulture)
                : null;
        }

        private static int GetMinValueIndex(ItemStat itemStat, int maxValueIndex)
        {
            int minValueIndex = maxValueIndex;

            if (maxValueIndex >= 0)
            {
                minValueIndex = itemStat.TextWithPlaceholders.IndexOf(Placeholder, 0, maxValueIndex);

                if (minValueIndex < 0)
                {
                    minValueIndex = maxValueIndex;
                }
            }

            return minValueIndex;
        }
    }
}
