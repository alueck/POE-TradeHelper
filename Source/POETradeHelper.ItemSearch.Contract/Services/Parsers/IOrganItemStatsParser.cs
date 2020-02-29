using POETradeHelper.ItemSearch.Contract.Models;

namespace POETradeHelper.ItemSearch.Contract.Services.Parsers
{
    public interface IOrganItemStatsParser
    {
        OrganItemStats Parse(string[] itemStringLines);
    }
}