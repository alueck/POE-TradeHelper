using POETradeHelper.ItemSearch.Contract.Models;

namespace POETradeHelper.ItemSearch.Contract.Services.Parsers
{
    public interface IFlaskItemStatsParser
    {
        FlaskItemStats Parse(string[] itemStringLines);
    }
}