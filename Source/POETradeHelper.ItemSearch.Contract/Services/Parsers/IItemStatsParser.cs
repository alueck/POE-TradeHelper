using POETradeHelper.ItemSearch.Contract.Models;

namespace POETradeHelper.ItemSearch.Contract.Services.Parsers
{
    public interface IItemStatsParser<T>
        where T : ItemWithStats
    {
        ItemStats Parse(string[] itemStringLines, bool preferLocalStats);
    }
}