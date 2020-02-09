using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;

namespace POETradeHelper.PathOfExileTradeApi.Services
{
    public class GemItemSearchQueryRequestMapper : ItemSearchRequestMapperBase
    {
        public override bool CanMap(Item item)
        {
            return item is GemItem;
        }

        public override SearchQueryRequest MapToQueryRequest(Item item)
        {
            SearchQueryRequest result = base.MapToQueryRequest(item);

            var gemItem = (GemItem)item;

            MapGemLevel(result, gemItem);
            MapQuality(result, gemItem);

            return result;
        }

        protected override void MapItemRarity(SearchQueryRequest result, Item item)
        {
        }

        private void MapGemLevel(SearchQueryRequest result, GemItem gemItem)
        {
            result.Query.Filters.MiscFilters.GemLevel = new MinMaxFilter
            {
                Min = gemItem.Level
            };
        }

        private void MapQuality(SearchQueryRequest result, GemItem gemItem)
        {
            result.Query.Filters.MiscFilters.Quality = new MinMaxFilter
            {
                Min = gemItem.Quality
            };
        }
    }
}