using POETradeHelper.ItemSearch.Contract;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;

namespace POETradeHelper.ItemSearch.Services.Parsers
{
    public class MapItemStatsParser : ExplicitItemStatsParserBase<MapItemStats>, IMapItemStatsParser
    {
        public MapItemStatsParser(IStatsDataService statsDataService) : base(statsDataService)
        {
        }
    }
}