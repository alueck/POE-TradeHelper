using Microsoft.Extensions.Options;

using POETradeHelper.ItemSearch.Contract.Configuration;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;

namespace POETradeHelper.ItemSearch.Services.Mappers
{
    public class MapItemSearchQueryRequestMapper : ItemSearchRequestMapperBase
    {
        public MapItemSearchQueryRequestMapper(IOptionsMonitor<ItemSearchOptions> itemSearchOptions) : base(itemSearchOptions)
        {
        }

        public override bool CanMap(Item item)
        {
            return item is MapItem;
        }

        public override SearchQueryRequest MapToQueryRequest(Item item)
        {
            var result = base.MapToQueryRequest(item);

            var mapItem = (MapItem)item;
            MapIdentified(result, mapItem);
            MapTier(result, mapItem);
            MapBlighted(result, mapItem);
            MapBlightRavaged(result, mapItem);

            return result;
        }

        private static void MapIdentified(SearchQueryRequest result, MapItem mapItem)
        {
            result.Query.Filters.MiscFilters.Identified = new BoolOptionFilter
            {
                Option = mapItem.IsIdentified
            };
        }

        private static void MapTier(SearchQueryRequest result, MapItem mapItem)
        {
            result.Query.Filters.MapFilters.MapTier = new MinMaxFilter
            {
                Min = mapItem.Tier,
                Max = mapItem.Tier
            };
        }

        private static void MapBlighted(SearchQueryRequest result, MapItem mapItem)
        {
            result.Query.Filters.MapFilters.MapBlighted = new BoolOptionFilter
            {
                Option = mapItem.IsBlighted
            };
        }

        private static void MapBlightRavaged(SearchQueryRequest result, MapItem mapItem)
        {
            result.Query.Filters.MapFilters.MapBlightRavaged = new BoolOptionFilter
            {
                Option = mapItem.IsBlightRavaged
            };
        }
    }
}
