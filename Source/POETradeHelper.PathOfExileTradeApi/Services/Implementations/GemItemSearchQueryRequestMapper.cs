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

        public override IQueryRequest MapToQueryRequest(Item item)
        {
            SearchQueryRequest result = (SearchQueryRequest)base.MapToQueryRequest(item);

            var gemItem = (GemItem)item;

            MapGemLevel(result, gemItem);
            MapQuality(result, gemItem);
            MapGemQualityType(result, gemItem);

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

        private void MapGemQualityType(SearchQueryRequest result, GemItem gemItem)
        {
            result.Query.Filters.MiscFilters.GemAlternateQuality = new OptionFilter
            {
                Option = ((int)gemItem.QualityType).ToString()
            };
        }
    }
}