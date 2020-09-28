using POETradeHelper.ItemSearch.Contract.Models;

namespace POETradeHelper.ItemSearch.Contract.Services.Parsers
{
    public interface IItemParserAggregator
    {
        Item Parse(string itemString);

        bool IsParseable(string itemString);
    }
}