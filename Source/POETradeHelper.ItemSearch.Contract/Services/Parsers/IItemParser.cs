using POETradeHelper.ItemSearch.Contract.Models;

namespace POETradeHelper.ItemSearch.Contract.Services.Parsers
{
    public interface IItemParser
    {
        Item Parse(string[] itemStringLines);

        bool CanParse(string[] itemStringLines);
    }
}