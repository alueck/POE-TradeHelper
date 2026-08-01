using System.Globalization;
using System.Text.RegularExpressions;

using Microsoft.Extensions.Logging;

using POETradeHelper.Common.Extensions;
using POETradeHelper.ItemSearch.Contract.Extensions;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.ItemSearch.Services.Parsers.ItemStatsParsers
{
    public class ItemStatsParser : ItemStatsParserBase, IItemStatsParser<ItemWithStats>
    {
        private const char Placeholder = '#';
        private readonly IPseudoItemStatsParser pseudoItemStatsParser;

        public ItemStatsParser(IStatsDataService statsDataService, IPseudoItemStatsParser pseudoItemStatsParser, ILogger<ItemStatsParser> logger)
            : base(statsDataService, logger)
        {
            this.pseudoItemStatsParser = pseudoItemStatsParser;
        }

        public ItemStats Parse(string[] itemStringLines, bool preferLocalStats, IReadOnlyCollection<StatCategory>? categoriesFilter = null)
        {
            ItemStats result = new();

            int statsStartIndex = GetStatsStartIndex(itemStringLines);

            string[] statTextLines = itemStringLines.Skip(statsStartIndex).ToArray();

            int? tier = null;
            StatCategory? category = null;
            List<string> statTexts = [];
            List<ItemStat> itemStats = [];
            foreach (var statTextLine in statTextLines)
            {
                if ((statTexts.Count > 0 && statTextLine.StartsWith('{')) || statTextLine.StartsWith('(') ||
                    statTextLine == ParserConstants.PropertyGroupSeparator)
                {
                    itemStats.AddRange(this.GetItemStats(preferLocalStats, statTexts, tier, category, categoriesFilter));
                    statTexts.Clear();
                    tier = TryGetTier(statTextLine);
                    category = TryGetCategory(statTextLine);
                    continue;
                }

                if (statTextLine.StartsWith('{'))
                {
                    tier = TryGetTier(statTextLine);
                    category = TryGetCategory(statTextLine);
                    continue;
                }

                if (category != null && categoriesFilter?.Count > 0 && !categoriesFilter.Contains(category.Value))
                {
                    continue;
                }

                statTexts.Add(statTextLine.Replace(Resources.UnscalableValueSuffix, string.Empty).RemoveStatRanges().RemoveBracketedText());
            }

            itemStats.AddRange(this.GetItemStats(preferLocalStats, statTexts, tier, category, categoriesFilter));

            IEnumerable<ItemStat> pseudoItemStats = this.pseudoItemStatsParser.Parse(itemStats);

            result.AllStats.AddRange(itemStats);
            result.AllStats.AddRange(pseudoItemStats);

            return result;
        }

        private IEnumerable<ItemStat> GetItemStats(
            bool preferLocalStats,
            IReadOnlyList<string> statTexts,
            int? tier,
            StatCategory? category,
            IReadOnlyCollection<StatCategory>? categoriesFilter)
        {
            for (int index = 0; index < statTexts.Count; index++)
            {
                var itemStat = this.GetCompleteItemStat(statTexts.Skip(index).ToArray(), preferLocalStats, tier, category, categoriesFilter);

                if (itemStat != null)
                {
                    int placeholderCount = itemStat.TextWithPlaceholders.Count(c => c == Placeholder);
                    itemStat = placeholderCount switch
                    {
                        1 => GetSingleValueItemStat(itemStat),
                        2 => GetMinMaxValueItemStat(itemStat),
                        _ => itemStat,
                    };

                    index += itemStat.Lines - 1;

                    yield return itemStat;
                }
            }
        }

        private static StatCategory? TryGetCategory(string line)
        {
            if (line.Contains(nameof(StatCategory.Implicit), StringComparison.OrdinalIgnoreCase))
            {
                return StatCategory.Implicit;
            }

            if (line.Contains(nameof(StatCategory.Crafted), StringComparison.OrdinalIgnoreCase))
            {
                return  StatCategory.Crafted;
            }

            if (line.Contains(nameof(StatCategory.Fractured), StringComparison.OrdinalIgnoreCase))
            {
                return StatCategory.Fractured;
            }

            if (line.Contains(nameof(StatCategory.Crucible), StringComparison.OrdinalIgnoreCase))
            {
                return StatCategory.Crucible;
            }

            return null;
        }

        private static ItemStat GetSingleValueItemStat(ItemStat itemStat)
        {
            decimal? value = GetFirstNumericValue(itemStat.Text[itemStat.TextWithPlaceholders.IndexOf(Placeholder)..]);

            return value.HasValue
                ? new SingleValueItemStat(itemStat)
                {
                    Value = value.Value,
                }
                : itemStat;
        }

        private static ItemStat GetMinMaxValueItemStat(ItemStat itemStat)
        {
            MinMaxValueItemStat result = new(itemStat);

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