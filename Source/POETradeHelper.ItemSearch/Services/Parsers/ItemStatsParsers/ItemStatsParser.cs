using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;

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

        public ItemStatsParser(IStatsDataService statsDataService, IPseudoItemStatsParser pseudoItemStatsParser) : base(
            statsDataService)
        {
            this.pseudoItemStatsParser = pseudoItemStatsParser;
        }

        public ItemStats Parse(string[] itemStringLines, bool preferLocalStats)
        {
            ItemStats result = new();

            int statsStartIndex = GetStatsStartIndex(itemStringLines);

            string[] statTextLines = itemStringLines.Skip(statsStartIndex).ToArray();

            int? tier = null;
            List<string> statTexts = [];
            List<ItemStat> itemStats = [];
            foreach (string statTextLine in statTextLines)
            {
                if ((statTexts.Count > 0 && statTextLine.StartsWith('{')) || statTextLine.StartsWith('(') || statTextLine == ParserConstants.PropertyGroupSeparator)
                {
                    itemStats.AddRange(this.GetItemStats(preferLocalStats, statTexts, tier));
                    statTexts.Clear();
                    tier = TryGetTier(statTextLine);
                    continue;
                }

                if (statTextLine.StartsWith('{'))
                {
                    tier = TryGetTier(statTextLine);
                    continue;
                }

                statTexts.Add(statTextLine.Replace(Resources.UnscalableValueSuffix, string.Empty).RemoveStatRanges());
            }

            itemStats.AddRange(this.GetItemStats(preferLocalStats, statTexts, tier));

            IEnumerable<ItemStat> pseudoItemStats = this.pseudoItemStatsParser.Parse(itemStats);

            result.AllStats.AddRange(itemStats);
            result.AllStats.AddRange(pseudoItemStats);

            return result;
        }

        protected override ItemStat? GetCompleteItemStat(ItemStat itemStat, bool preferLocalStatData)
        {
            ItemStat? result = base.GetCompleteItemStat(itemStat, preferLocalStatData);

            int? placeholderCount = result?.TextWithPlaceholders.Count(c => c == Placeholder);
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

        private IEnumerable<ItemStat> GetItemStats(bool preferLocalStats, IReadOnlyCollection<string> statTexts, int? tier)
        {
            List<ItemStat> result = [];
            ItemStat? stat = this.ParseStatText(string.Join('\n', statTexts), tier, preferLocalStats);
            if (stat != null)
            {
                result.Add(stat);
            }
            else
            {
                result.AddRange(statTexts.Select(x => this.ParseStatText(x, tier, preferLocalStats)).OfType<ItemStat>());
            }

            return result;
        }

        private static int? TryGetTier(string statDescription)
        {
            Match match = Regex.Match(statDescription, @"(Rank|Tier): (?<tier>\d+)");

            return int.TryParse(match.Groups["tier"].Value, out int tier)
                ? tier
                : null;
        }

        private ItemStat? ParseStatText(string statText, int? tier, bool preferLocalStats)
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

            result.Tier = tier;

            return this.GetCompleteItemStat(result, preferLocalStats);
        }

        private static bool TryGetItemStatForCategoryByMarker(
            string statText,
            StatCategory statCategory,
            [NotNullWhen(true)] out ItemStat? itemStat)
        {
            itemStat = null;

            int statCategoryMarkerIndex =
                statText.IndexOf($"({statCategory.GetDisplayName()})", StringComparison.OrdinalIgnoreCase);
            if (statCategoryMarkerIndex >= 0)
            {
                statText = statText[..statCategoryMarkerIndex].Trim();

                itemStat = new ItemStat(statCategory)
                {
                    Text = statText,
                };

                return true;
            }

            return false;
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