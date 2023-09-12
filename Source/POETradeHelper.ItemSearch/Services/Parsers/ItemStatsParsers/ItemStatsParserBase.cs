using POETradeHelper.Common.Extensions;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.ItemSearch.Services.Parsers.ItemStatsParsers
{
    public abstract class ItemStatsParserBase
    {
        private readonly IStatsDataService statsDataService;

        protected ItemStatsParserBase(IStatsDataService statsDataService)
        {
            this.statsDataService = statsDataService;
        }

        protected virtual ItemStat? GetCompleteItemStat(ItemStat itemStat, bool preferLocalStatData)
        {
            string statCategoryToSearch = itemStat.StatCategory != StatCategory.Unknown
                ? itemStat.StatCategory.GetDisplayName()
                : StatCategory.Explicit.GetDisplayName();

            var statData = this.statsDataService.GetStatData(itemStat.Text, preferLocalStatData, statCategoryToSearch);

            if (statData != null)
            {
                itemStat.Id = statData.Id;
                itemStat.StatCategory = statData.Type.ParseToEnumByDisplayName<StatCategory>(StringComparison.OrdinalIgnoreCase) ?? StatCategory.Unknown;
                itemStat.TextWithPlaceholders = statData.Text;

                return itemStat;
            }

            return null;
        }

        protected static int GetStatsStartIndex(string[] itemStringLines)
        {
            int itemLevelLineIndex = Array.FindIndex(itemStringLines, l => l.StartsWith(Resources.ItemLevelDescriptor));
            return itemLevelLineIndex + 2; // skip property group separator
        }
    }
}