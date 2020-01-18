using POETradeHelper.ItemSearch.Contract.Models;

namespace POETradeHelper.ItemSearch.Contract.Services
{
    public interface IItemParserAggregator
    {
        Item Parse(string itemString);

        bool CanParse(string itemString);
    }
}