using System;
using System.Collections.Generic;
using System.Linq;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;
using POETradeHelper.PathOfExileTradeApi.Constants;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.ItemSearch.Services.Parsers
{
    public class PseudoItemStatsParser : IPseudoItemStatsParser
    {
        private readonly IPseudoStatDataMappingService pseudoStatDataMappingService;

        public PseudoItemStatsParser(IPseudoStatDataMappingService pseudoStatDataMappingService)
        {
            this.pseudoStatDataMappingService = pseudoStatDataMappingService;
        }

        public IEnumerable<ItemStat> Parse(IEnumerable<ItemStat> itemStats)
        {
            IList<ItemStat> result = new List<ItemStat>();
            IDictionary<StatData, IList<ItemStat>> pseudoStatDataMapping = GetRelevantPseudoStatDataMappings(itemStats);

            foreach (var entry in pseudoStatDataMapping)
            {
                switch (entry.Value[0])
                {
                    case SingleValueItemStat _:
                        result.Add(GetSingleValueItemStat(entry));
                        break;

                    case MinMaxValueItemStat _:
                        result.Add(GetMinMaxValueItemStat(entry));
                        break;
                }
            }

            return result;
        }

        private IDictionary<StatData, IList<ItemStat>> GetRelevantPseudoStatDataMappings(IEnumerable<ItemStat> itemStats)
        {
            IDictionary<StatData, IList<ItemStat>> pseudoStatDataMapping = GetPseudoStatDataMappings(itemStats);

            return pseudoStatDataMapping
                .Where(x => x.Value.Count > 1)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        private IDictionary<StatData, IList<ItemStat>> GetPseudoStatDataMappings(IEnumerable<ItemStat> itemStats)
        {
            IDictionary<StatData, IList<ItemStat>> pseudoStatDataMapping = new Dictionary<StatData, IList<ItemStat>>();
            foreach (var itemStat in itemStats)
            {
                var pseudoStatDataList = this.pseudoStatDataMappingService.GetPseudoStatData(itemStat.Id);

                foreach (var pseudoStatData in pseudoStatDataList)
                {
                    if (!pseudoStatDataMapping.TryGetValue(pseudoStatData, out IList<ItemStat> mappedItemStats))
                    {
                        pseudoStatDataMapping[pseudoStatData] = mappedItemStats = new List<ItemStat>();
                    }

                    mappedItemStats.Add(itemStat);
                }
            }

            return pseudoStatDataMapping;
        }

        private static SingleValueItemStat GetSingleValueItemStat(KeyValuePair<StatData, IList<ItemStat>> entry)
        {
            return new SingleValueItemStat(StatCategory.Pseudo)
            {
                Id = entry.Key.Id,
                TextWithPlaceholders = entry.Key.Text,
                Value = GetSingleValueItemStatValue(entry)
            };
        }

        private static int GetSingleValueItemStatValue(KeyValuePair<StatData, IList<ItemStat>> entry)
        {
            Func<SingleValueItemStat, int> sumFunction = GetSingleValueItemStatValueSumFunction(entry.Key);

            return entry.Value.Cast<SingleValueItemStat>().Sum(sumFunction);
        }

        private static Func<SingleValueItemStat, int> GetSingleValueItemStatValueSumFunction(StatData pseudoStatData)
        {
            Func<SingleValueItemStat, int> sumFunction = itemStat => itemStat.Value;

            if (pseudoStatData.Id == PseudoStatId.TotalElementalResistance || pseudoStatData.Id == PseudoStatId.TotalResistance)
            {
                sumFunction = itemStat =>
                {
                    switch (itemStat.Id)
                    {
                        case StatId.FireAndColdResistance:
                        case StatId.FireAndLightningResistance:
                        case StatId.ColdAndLightningResistance:
                            return itemStat.Value + itemStat.Value;

                        default:
                            return itemStat.Value;
                    }
                };
            }

            return sumFunction;
        }

        private static MinMaxValueItemStat GetMinMaxValueItemStat(KeyValuePair<StatData, IList<ItemStat>> entry)
        {
            return new MinMaxValueItemStat(StatCategory.Pseudo)
            {
                Id = entry.Key.Id,
                TextWithPlaceholders = entry.Key.Text,
                MinValue = entry.Value.Cast<MinMaxValueItemStat>().Sum(itemStat => itemStat.MinValue),
                MaxValue = entry.Value.Cast<MinMaxValueItemStat>().Sum(itemStat => itemStat.MaxValue)
            };
        }
    }
}