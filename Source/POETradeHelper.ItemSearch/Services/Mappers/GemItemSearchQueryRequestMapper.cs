using Microsoft.Extensions.Options;

using POETradeHelper.ItemSearch.Contract.Configuration;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;

namespace POETradeHelper.ItemSearch.Services.Mappers
{
    public class GemItemSearchQueryRequestMapper : ItemSearchRequestMapperBase
    {
        public GemItemSearchQueryRequestMapper(IOptionsMonitor<ItemSearchOptions> itemSearchOptions) : base(itemSearchOptions)
        {
        }

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
            MapGemQualityType(result, gemItem);

            return result;
        }

        protected override void MapItemRarity(SearchQueryRequest result, Item item)
        {
            // rarity is always Gem
        }

        private static void MapGemLevel(SearchQueryRequest result, GemItem gemItem)
        {
            result.Query.Filters.MiscFilters.GemLevel = new MinMaxFilter
            {
                Min = gemItem.Level
            };
        }

        private static void MapQuality(SearchQueryRequest result, GemItem gemItem)
        {
            result.Query.Filters.MiscFilters.Quality = new MinMaxFilter
            {
                Min = gemItem.Quality
            };
        }

        private static void MapGemQualityType(SearchQueryRequest result, GemItem gemItem)
        {
            result.Query.Filters.MiscFilters.GemAlternateQuality = new OptionFilter
            {
                Option = ((int)gemItem.QualityType).ToString()
            };
        }
    }
}
