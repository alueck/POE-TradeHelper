using POETradeHelper.ItemSearch.Contract.Models;

namespace POETradeHelper.ItemSearch.Services.Parsers
{
    public interface IItemTypeParser
    {
        string ParseType(string[] itemStringLines, ItemRarity itemRarity, bool isIdentified);
    }
}