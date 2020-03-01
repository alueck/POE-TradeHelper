using DynamicData;
using POETradeHelper.ItemSearch.Contract;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Models.ItemStats;
using System.Linq;

namespace POETradeHelper.ItemSearch.Services.Parsers
{
    public abstract class ExplicitItemStatsParserBase<TItemStats> : ItemStatsParserBase
        where TItemStats : IHasExplicitStats, new()
    {
        protected ExplicitItemStatsParserBase(IStatsDataService statsDataService) : base(statsDataService)
        {
        }

        public TItemStats Parse(string[] itemStringLines)
        {
            var result = new TItemStats();

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