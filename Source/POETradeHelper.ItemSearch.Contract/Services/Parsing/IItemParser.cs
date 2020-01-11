using POETradeHelper.ItemSearch.Contract.Models;

namespace POETradeHelper.ItemSearch.Contract.Services
{
    public interface IItemParser
    {
        Item Parse(string itemString);

        bool CanParse(string itemString);
    }
}