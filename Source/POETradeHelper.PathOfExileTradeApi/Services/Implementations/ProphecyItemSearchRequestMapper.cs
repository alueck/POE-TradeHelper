using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Constants;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.PathOfExileTradeApi.Services
{
    public class ProphecyItemSearchRequestMapper : ItemSearchRequestMapperBase
    {
        public override bool CanMap(Item item)
        {
            return item is ProphecyItem;
        }

        protected override void MapItemName(SearchQueryRequest result, Item item)
        {
            result.Query.Name = item.Name;
        }

        protected override void MapItemType(SearchQueryRequest result, Item item)
        {
            result.Query.Type = ItemTypeFilterOptions.Prophecy;
        }

        protected override void MapItemRarity(SearchQueryRequest result, Item item)
        {
        }
    }
}