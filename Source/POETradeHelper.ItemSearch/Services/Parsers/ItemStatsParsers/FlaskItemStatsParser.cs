using POETradeHelper.ItemSearch.Contract;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;

namespace POETradeHelper.ItemSearch.Services.Parsers
{
    public class FlaskItemStatsParser : ExplicitItemStatsParserBase<FlaskItemStats>, IFlaskItemStatsParser
    {
        public FlaskItemStatsParser(IStatsDataService statsDataService) : base(statsDataService)
        {
        }
    }
}