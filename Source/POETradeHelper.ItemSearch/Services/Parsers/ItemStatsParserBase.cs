using POETradeHelper.ItemSearch.Contract;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace POETradeHelper.ItemSearch.Services.Parsers
{
    public abstract class ItemStatsParserBase
    {
        private readonly IStatsDataService statsDataService;

        protected ItemStatsParserBase(IStatsDataService statsDataService)
        {
            this.statsDataService = statsDataService;
        }

        protected void SetStatIds(IEnumerable<ItemStat> itemStats)
        {
            foreach (var itemStat in itemStats)
            {
                itemStat.Id = this.statsDataService.GetId(itemStat);
            }
        }

        protected static int GetStatsStartIndex(string[] itemStringLines)
        {
            int itemLevelLineIndex = Array.FindIndex(itemStringLines, l => l.StartsWith(Resources.ItemLevelDescriptor));
            return itemLevelLineIndex + 2; //skip property group separator
        }

        protected virtual string GetTextWithPlaceholders(string statText)
        {
            return Regex.Replace(statText, @"[\+\-]?\d+", "#");
        }
    }
}