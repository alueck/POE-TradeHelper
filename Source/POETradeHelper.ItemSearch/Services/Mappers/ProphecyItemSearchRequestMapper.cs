using Microsoft.Extensions.Options;
using POETradeHelper.ItemSearch.Contract.Configuration;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Constants;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.ItemSearch.Services.Mappers
{
    public class ProphecyItemSearchRequestMapper : ItemSearchRequestMapperBase
    {
        public ProphecyItemSearchRequestMapper(IOptionsMonitor<ItemSearchOptions> itemSearchOptions) : base(itemSearchOptions)
        {
        }

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