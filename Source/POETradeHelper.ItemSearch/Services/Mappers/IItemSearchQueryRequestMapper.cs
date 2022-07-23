using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.ItemSearch.Services.Mappers
{
    public interface IItemSearchQueryRequestMapper
    {
        bool CanMap(Item item);

        SearchQueryRequest MapToQueryRequest(Item item);
    }
}