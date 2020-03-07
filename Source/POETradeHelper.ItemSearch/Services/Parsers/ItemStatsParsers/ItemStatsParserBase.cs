using POETradeHelper.ItemSearch.Contract;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace POETradeHelper.ItemSearch.Services.Parsers
{
    public abstract class ItemStatsParserBase<TItemStat>
        where TItemStat : ItemStat
    {
        private readonly IStatsDataService statsDataService;

        protected ItemStatsParserBase(IStatsDataService statsDataService)
        {
            this.statsDataService = statsDataService;
        }

        protected TItemStat GetCompleteItemStat(TItemStat itemStat)
        {
            var statData = this.statsDataService.GetStatData(itemStat, this.GetStatCategoriesToSearch(itemStat));

            if (statData != null)
            {
                itemStat.Id = statData.Id;
                itemStat.StatCategory = statData.StatCategory;
                itemStat.TextWithPlaceholders = statData.Text;

                return itemStat;
            }

            return null;
        }

        protected StatCategory[] GetStatCategoriesToSearch(ItemStat itemStat)
        {
            IList<StatCategory> statCategoriesToSearch = new List<StatCategory>();

            if (itemStat.StatCategory != StatCategory.Unknown)
            {
                statCategoriesToSearch.Add(itemStat.StatCategory);
            }
            else
            {
                statCategoriesToSearch.Add(StatCategory.Explicit);
                statCategoriesToSearch.Add(StatCategory.Enchant);
            }

            return statCategoriesToSearch.ToArray();
        }

        protected static int GetStatsStartIndex(string[] itemStringLines)
        {
            int itemLevelLineIndex = Array.FindIndex(itemStringLines, l => l.StartsWith(Resources.ItemLevelDescriptor));
            return itemLevelLineIndex + 2; //skip property group separator
        }
    }
}