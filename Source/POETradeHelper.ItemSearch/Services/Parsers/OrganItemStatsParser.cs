using DynamicData;
using POETradeHelper.ItemSearch.Contract;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace POETradeHelper.ItemSearch.Services.Parsers
{
    public class OrganItemStatsParser : IOrganItemStatsParser
    {
        private IStatsDataService statsDataService;

        public OrganItemStatsParser(IStatsDataService statsDataService)
        {
            this.statsDataService = statsDataService;
        }

        public OrganItemStats Parse(string[] itemStringLines)
        {
            var result = new OrganItemStats();

            result.Stats.AddRange(this.ParseMonsterStats(itemStringLines));

            return result;
        }

        private IEnumerable<MonsterItemStat> ParseMonsterStats(string[] itemStringLines)
        {
            int itemLevelLineIndex = Array.FindIndex(itemStringLines, l => l.StartsWith(Resources.ItemLevelDescriptor));
            int statsStartIndex = itemLevelLineIndex + 2; //skip item level line itself and following property group separator

            var groupedItemStatLines = itemStringLines
                .Skip(statsStartIndex)
                .TakeWhile(l => l != ParserConstants.PropertyGroupSeparator)
                .GroupBy(x => x);

            var monsterItemStats = groupedItemStatLines.Select(group => new MonsterItemStat
            {
                Text = group.Key,
                Count = group.Count(),
                TextWithPlaceholders = GetTextWithPlaceholders(group.Key)
            }).ToList();
            this.SetIds(monsterItemStats);

            return monsterItemStats;
        }

        private string GetTextWithPlaceholders(string text)
        {
            return $"{text} (×#)";
        }

        private void SetIds(IEnumerable<MonsterItemStat> monsterItemStats)
        {
            foreach (var monsterItemStat in monsterItemStats)
            {
                monsterItemStat.Id = this.statsDataService.GetId(monsterItemStat);
            }
        }
    }
}