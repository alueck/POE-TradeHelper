using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Properties;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using System.Collections.Generic;

namespace POETradeHelper.ItemSearch.Services.Factories
{
    public class MapItemAdditionalFilterViewModelsFactory : AdditionalFilterViewModelsFactoryBase
    {
        public override IEnumerable<FilterViewModelBase> Create(Item item, SearchQueryRequest searchQueryRequest)
        {
            var result = new List<FilterViewModelBase>();

            if (item is MapItem mapItem)
            {
                result.Add(this.GetQualityFilterViewModel(mapItem, searchQueryRequest));
                result.Add(this.GetItemQuantityFilterViewModel(mapItem, searchQueryRequest));
                result.Add(this.GetItemRarityFilterViewModel(mapItem, searchQueryRequest));
                result.Add(this.GetMonsterPacksizeFilterViewModel(mapItem, searchQueryRequest));
                result.Add(this.GetMapTierFilterViewModel(mapItem, searchQueryRequest));
                result.Add(this.GetMapBlightedFilterViewModel(mapItem, searchQueryRequest));
                result.Add(this.GetCorruptedFilterViewModel(mapItem, searchQueryRequest));
                result.Add(this.GetIdentifiedFilterViewModel(mapItem, searchQueryRequest));
            }

            return result;
        }

        private FilterViewModelBase GetItemQuantityFilterViewModel(MapItem mapItem, SearchQueryRequest searchQueryRequest)
        {
            return this.CreateBindableMinMaxFilterViewModel(
                x => x.Query.Filters.MapFilters.MapIncreasedItemQuantity,
                Resources.MapItemQuantity,
                mapItem.ItemQuantity,
                searchQueryRequest.Query.Filters.MapFilters.MapIncreasedItemQuantity);
        }

        private FilterViewModelBase GetItemRarityFilterViewModel(MapItem mapItem, SearchQueryRequest searchQueryRequest)
        {
            return this.CreateBindableMinMaxFilterViewModel(
                x => x.Query.Filters.MapFilters.MapIncreasedItemRarity,
                Resources.MapItemRarity,
                mapItem.ItemRarity,
                searchQueryRequest.Query.Filters.MapFilters.MapIncreasedItemRarity);
        }

        private FilterViewModelBase GetMonsterPacksizeFilterViewModel(MapItem mapItem, SearchQueryRequest searchQueryRequest)
        {
            return this.CreateBindableMinMaxFilterViewModel(
                x => x.Query.Filters.MapFilters.MapPacksize,
                Resources.MapMonsterPacksize,
                mapItem.MonsterPackSize,
                searchQueryRequest.Query.Filters.MapFilters.MapPacksize);
        }

        private FilterViewModelBase GetMapTierFilterViewModel(MapItem mapItem, SearchQueryRequest searchQueryRequest)
        {
            return this.CreateBindableMinMaxFilterViewModel(
                x => x.Query.Filters.MapFilters.MapTier,
                Resources.MapTier,
                mapItem.Tier,
                searchQueryRequest.Query.Filters.MapFilters.MapTier);
        }

        private FilterViewModelBase GetMapBlightedFilterViewModel(MapItem mapItem, SearchQueryRequest searchQueryRequest)
        {
            return new BindableFilterViewModel(x => x.Query.Filters.MapFilters.MapBlighted)
            {
                Text = Resources.MapBlighted,
                IsEnabled = searchQueryRequest.Query.Filters.MapFilters.MapBlighted?.Option ?? mapItem.IsBlighted
            };
        }
    }
}