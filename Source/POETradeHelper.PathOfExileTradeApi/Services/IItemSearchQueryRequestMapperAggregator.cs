using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.PathOfExileTradeApi.Services
{
    public interface IItemSearchQueryRequestMapperAggregator
    {
        IQueryRequest MapToQueryRequest(Item item);
    }
}