using DynamicData;
using POETradeHelper.Common.Extensions;
using POETradeHelper.ItemSearch.Contract;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;
using System;
using System.Linq;

namespace POETradeHelper.ItemSearch.Services.Parsers
{
    public class ItemStatsParser : ItemStatsParserBase<ItemStat>, IItemStatsParser<ItemWithStats>
    {
        public ItemStatsParser(IStatsDataService statsDataService) : base(statsDataService)
        {
        }

        public ItemStats Parse(string[] itemStringLines)
        {
            var result = new ItemStats();

            int statsStartIndex = GetStatsStartIndex(itemStringLines);

            var statTexts = itemStringLines.Skip(statsStartIndex).Where(s => s != ParserConstants.PropertyGroupSeparator);

            var itemStats = statTexts.Select(ParseStatText).Where(x => x != null).ToList();

            result.AllStats.AddRange(itemStats);

            return result;
        }

        private ItemStat ParseStatText(string statText)
        {
            ItemStat result;

            if (!TryGetItemStatForCategoryByMarker(statText, StatCategory.Implicit, out result)
                && !TryGetItemStatForCategoryByMarker(statText, StatCategory.Crafted, out result))
            {
                result = new ItemStat()
                {
                    Text = statText,
                };
            }

            result = this.GetCompleteItemStat(result);

            return result;
        }

        private bool TryGetItemStatForCategoryByMarker(string statText, StatCategory statCategory, out ItemStat itemStat)
        {
            itemStat = null;

            int statCategoryMarkerIndex = statText.IndexOf($"({statCategory.GetDisplayName()})", StringComparison.OrdinalIgnoreCase);
            if (statCategoryMarkerIndex >= 0)
            {
                statText = statText.Substring(0, statCategoryMarkerIndex).Trim();

                itemStat = new ItemStat(statCategory)
                {
                    Text = statText
                };

                return true;
            }

            return false;
        }
    }
}