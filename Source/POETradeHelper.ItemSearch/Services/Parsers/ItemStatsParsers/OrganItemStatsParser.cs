using DynamicData;
using POETradeHelper.ItemSearch.Contract;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;
using System.Collections.Generic;
using System.Linq;

namespace POETradeHelper.ItemSearch.Services.Parsers
{
    public class OrganItemStatsParser : ItemStatsParserBase, IItemStatsParser<OrganItem>
    {
        public OrganItemStatsParser(IStatsDataService statsDataService) : base(statsDataService)
        {
        }

        public ItemStats Parse(string[] itemStringLines)
        {
            var result = new ItemStats();

            result.AllStats.AddRange(this.ParseMonsterStats(itemStringLines));

            return result;
        }

        private IEnumerable<MonsterItemStat> ParseMonsterStats(string[] itemStringLines)
        {
            int statsStartIndex = GetStatsStartIndex(itemStringLines);

            var groupedItemStatLines = itemStringLines
                .Skip(statsStartIndex)
                .TakeWhile(l => l != ParserConstants.PropertyGroupSeparator)
                .GroupBy(x => x);

            var monsterItemStats = groupedItemStatLines.Select(group => new MonsterItemStat
            {
                Text = group.Key,
                Count = group.Count(),
                TextWithPlaceholders = this.GetTextWithPlaceholders(group.Key)
            }).ToList();
            this.SetStatIds(monsterItemStats);

            return monsterItemStats;
        }

        protected override string GetTextWithPlaceholders(string text)
        {
            return $"{text} (×#)";
        }
    }
}