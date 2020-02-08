using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;
using POETradeHelper.PathOfExileTradeApi.Services.Implementations;

namespace POETradeHelper.PathOfExileTradeApi.Services
{
    public class MapItemSearchQueryRequestMapper : ItemSearchRequestMapperBase
    {
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
    }
}