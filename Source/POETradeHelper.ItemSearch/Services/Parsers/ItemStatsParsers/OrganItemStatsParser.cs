using POETradeHelper.Common.Extensions;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.ItemSearch.Services.Parsers.ItemStatsParsers
{
    public class OrganItemStatsParser : ItemStatsParserBase, IItemStatsParser<OrganItem>
    {
        public OrganItemStatsParser(IStatsDataService statsDataService) : base(statsDataService)
        {
        }

        public ItemStats Parse(string[] itemStringLines, bool preferLocalStats)
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
                .GroupBy(x => x);

            var itemStats = groupedItemStatLines.Select(group => new SingleValueItemStat(StatCategory.Monster)
                {
                    Text = group.Key,
                    Value = group.Count(),
                })
                .Select(s => this.GetCompleteItemStat(s, false))
                .OfType<ItemStat>()
                .ToList();

            return itemStats;
        }
    }
}