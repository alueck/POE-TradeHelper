using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Constants;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Services.Implementations;

namespace POETradeHelper.PathOfExileTradeApi.Services
{
    public class ProphecyItemSearchRequestMapper : ItemSearchRequestMapperBase
    {
        public override bool CanMap(Item item)
        {
            return item is ProphecyItem;
        }

        public override SearchQueryRequest MapToQueryRequest(Item item)
        {
            var prophecyItem = (ProphecyItem)item;
            var result = new SearchQueryRequest();

            MapItemName(result, prophecyItem);
            MapItemType(result, prophecyItem);

            return result;
        }

        private void MapItemType(SearchQueryRequest result, ProphecyItem prophecyItem)
        {
            result.Query.Type = ItemTypeFilterOptions.Prophecy;
        }
    }
}