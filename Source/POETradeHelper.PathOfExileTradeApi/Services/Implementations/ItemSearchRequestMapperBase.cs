using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.PathOfExileTradeApi.Services.Implementations
{
    public abstract class ItemSearchRequestMapperBase : IItemSearchQueryRequestMapper
    {
        public abstract bool CanMap(Item item);

        public abstract SearchQueryRequest MapToQueryRequest(Item item);

        protected void MapItemName(SearchQueryRequest result, Item item)
        {
            result.Query.Name = item.Name;
        }
    }
}