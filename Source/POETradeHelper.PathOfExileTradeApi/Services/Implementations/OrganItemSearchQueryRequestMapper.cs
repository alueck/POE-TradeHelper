using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.PathOfExileTradeApi.Services
{
    public class OrganItemSearchQueryRequestMapper : IItemSearchQueryRequestMapper
    {
        public bool CanMap(Item item)
        {
            return item is OrganItem;
        }

        public IQueryRequest MapToQueryRequest(Item item)
        {
            var organItem = (OrganItem)item;

            var result = new SearchQueryRequest();
            MapItemName(result, organItem);

            return result;
        }

        private static void MapItemName(SearchQueryRequest result, OrganItem organItem)
        {
            result.Query.Term = organItem.Name;
        }
    }
}