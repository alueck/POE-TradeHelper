using System.Text.RegularExpressions;

using Microsoft.Extensions.Logging;

using POETradeHelper.Common.Extensions;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.ItemSearch.Services.Parsers.ItemStatsParsers
{
    public abstract partial class ItemStatsParserBase
    {
        private readonly IStatsDataService statsDataService;
        private readonly ILogger<ItemStatsParserBase> logger;

        protected ItemStatsParserBase(IStatsDataService statsDataService, ILogger<ItemStatsParserBase> logger)
        {
            this.statsDataService = statsDataService;
            this.logger = logger;
        }

        protected ItemStat? GetCompleteItemStat(
            IReadOnlyCollection<string> itemStatLines,
            bool preferLocalStatData,
            int? tier,
            StatCategory? statCategoryToSearch = null,
            IReadOnlyCollection<StatCategory>? categoriesFilter = null)
        {
            var statData = this.statsDataService.TryGetStatData(itemStatLines, preferLocalStatData, statCategoryToSearch.HasValue ? [statCategoryToSearch.GetDisplayName()] : []);

            if (statData == null)
            {
                this.logger.LogDebug("Failed to find stat data for lines {ItemStatLines} and category {StatCategory}", itemStatLines, statCategoryToSearch);
                return null;
            }

            StatCategory statCategory = statData.Type.ParseToEnumByDisplayName<StatCategory>(StringComparison.OrdinalIgnoreCase) ?? StatCategory.Unknown;
            if (categoriesFilter?.Count > 0 && !categoriesFilter.Contains(statCategory))
            {
                return null;
            }

            string text = string.Join('\n', itemStatLines.Take(statData.Lines).Select(ReplaceStatCategoryMarkers));

            return new ItemStat(statCategory)
            {
                Id = statData.Id,
                TextWithPlaceholders = statData.Text,
                Text = text,
                Tier = tier ?? TryGetTier(statData.Text),
            };

        }

        protected static int? TryGetTier(string statDescription)
        {
            Match match = GetTierRegex().Match(statDescription);

            return int.TryParse(match.Groups["tier"].Value, out int tier)
                ? tier
                : null;
        }

        protected static int GetStatsStartIndex(string[] itemStringLines)
        {
            int itemLevelLineIndex = Array.FindIndex(itemStringLines, l => l.StartsWith(Resources.ItemLevelDescriptor));
            return itemLevelLineIndex + 2; // skip property group separator
        }

        private static string ReplaceStatCategoryMarkers(string line)
        {
            return Enum.GetValues<StatCategory>()
                .Aggregate(line, (current, statCategory) => current.Replace($" ({statCategory.GetDisplayName()})", string.Empty, StringComparison.OrdinalIgnoreCase));
        }

        [GeneratedRegex(@"((Rank|Tier): (?<tier>\d+)|\(Tier (?<tier>\d+)\))")]
        private static partial Regex GetTierRegex();
    }
}