using Microsoft.Extensions.Logging;

using POETradeHelper.Common.Extensions;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.ItemSearch.Services.Parsers.ItemStatsParsers
{
    public class OrganItemStatsParser : ItemStatsParserBase, IItemStatsParser<OrganItem>
    {
        public OrganItemStatsParser(IStatsDataService statsDataService, ILogger<OrganItemStatsParser> logger) : base(statsDataService, logger)
        {
        }

        public ItemStats Parse(string[] itemStringLines, bool preferLocalStats, IReadOnlyCollection<StatCategory>? categoriesFilter = null)
        {
            var result = new ItemStats();

            result.AllStats.AddRange(this.ParseMonsterStats(itemStringLines));

            return result;
        }

        private IEnumerable<ItemStat> ParseMonsterStats(string[] itemStringLines)
        {
            int statsStartIndex = GetStatsStartIndex(itemStringLines);

            var groupedItemStatLines = itemStringLines
                .Skip(statsStartIndex)
                .TakeWhile(l => l != ParserConstants.PropertyGroupSeparator)
                .GroupBy(x => x.Replace(Resources.UnscalableValueSuffix, string.Empty));

            var itemStats = groupedItemStatLines
                .Select(group =>
                {
                    var itemStat = this.GetCompleteItemStat([group.Key], false, null, StatCategory.Monster);
                    if (itemStat != null)
                    {
                        return new SingleValueItemStat(itemStat)
                        {
                            Value = group.Count(),
                        };
                    }

                    return null;
                })
                .OfType<ItemStat>()
                .ToList();

            return itemStats;
        }
    }
}