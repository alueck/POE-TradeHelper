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

            this.MapItemName(result, prophecyItem);
            this.MapItemType(result, prophecyItem);

            return result;
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