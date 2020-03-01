using DynamicData;
using POETradeHelper.ItemSearch.Contract;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;
using System.Linq;

namespace POETradeHelper.ItemSearch.Services.Parsers
{
    public class FlaskItemStatsParser : ItemStatsParserBase, IFlaskItemStatsParser
    {
        public FlaskItemStatsParser(IStatsDataService statsDataService) : base(statsDataService)
        {
        }

        public FlaskItemStats Parse(string[] itemStringLines)
        {
            var result = new FlaskItemStats();

            int statsStartIndex = GetStatsStartIndex(itemStringLines);

            var statTexts = itemStringLines.Skip(statsStartIndex).TakeWhile(x => x != ParserConstants.PropertyGroupSeparator);

            var explicitItemStats = statTexts.Select(x => new ExplicitItemStat
            {
                Text = x,
                TextWithPlaceholders = this.GetTextWithPlaceholders(x)
            }).ToList();

            SetStatIds(explicitItemStats);

            result.ExplicitStats.AddRange(explicitItemStats);

            return result;
        }
    }
}