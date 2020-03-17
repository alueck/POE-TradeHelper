using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.PathOfExileTradeApi.Services
{
    public interface IItemSearchQueryRequestMapper
    {
        bool CanMap(Item item);

        IQueryRequest MapToQueryRequest(Item item);
    }
}