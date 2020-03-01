using POETradeHelper.ItemSearch.Contract.Models;

namespace POETradeHelper.ItemSearch.Contract.Services.Parsers
{
    public interface IMapItemStatsParser
    {
        MapItemStats Parse(string[] itemStringLines);
    }
}