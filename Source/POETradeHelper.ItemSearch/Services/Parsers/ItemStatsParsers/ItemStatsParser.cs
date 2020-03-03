using DynamicData;
using POETradeHelper.ItemSearch.Contract;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;
using System.Linq;

namespace POETradeHelper.ItemSearch.Services.Parsers
{
    public class ItemStatsParser : ItemStatsParserBase, IItemStatsParser<ItemWithStats>
    {
        public ItemStatsParser(IStatsDataService statsDataService) : base(statsDataService)
        {
        }

        public ItemStats Parse(string[] itemStringLines)
        {
            var result = new ItemStats();

            int statsStartIndex = GetStatsStartIndex(itemStringLines);

            var statTexts = itemStringLines.Skip(statsStartIndex).TakeWhile(x => x != ParserConstants.PropertyGroupSeparator);

            var explicitItemStats = statTexts.Select(x => new ItemStat(StatCategory.Explicit)
            {
                Text = x,
                TextWithPlaceholders = this.GetTextWithPlaceholders(x)
            }).ToList();

            SetStatIds(explicitItemStats);

            result.AllStats.AddRange(explicitItemStats);

            return result;
        }
    }
}