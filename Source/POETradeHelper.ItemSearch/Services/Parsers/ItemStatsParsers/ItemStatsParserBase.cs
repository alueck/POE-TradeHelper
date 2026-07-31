using System.Text.RegularExpressions;

using POETradeHelper.Common.Extensions;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.ItemSearch.Services.Parsers.ItemStatsParsers
{
    public abstract partial class ItemStatsParserBase
    {
        private readonly IStatsDataService statsDataService;

        protected ItemStatsParserBase(IStatsDataService statsDataService)
        {
            this.statsDataService = statsDataService;
        }

        protected ItemStat? GetCompleteItemStat(IReadOnlyCollection<string> itemStatLines, bool preferLocalStatData, int? tier, StatCategory? statCategoryToSearch = null)
        {
            var statData = this.statsDataService.TryGetStatData(itemStatLines, preferLocalStatData, statCategoryToSearch.HasValue ? [statCategoryToSearch.GetDisplayName()] : []);

            if (statData != null)
            {
                string text = string.Join('\n', itemStatLines.Take(statData.Lines).Select(ReplaceStatCategoryMarkers));

                return new ItemStat(statData.Type.ParseToEnumByDisplayName<StatCategory>(StringComparison.OrdinalIgnoreCase) ?? StatCategory.Unknown)
                {
                    Id = statData.Id,
                    TextWithPlaceholders = statData.Text,
                    Text = text,
                    Tier = tier ?? TryGetTier(statData.Text),
                };
            }

            return null;
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