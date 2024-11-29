using System.Collections.Generic;

using Microsoft.Extensions.Options;

using POETradeHelper.ItemSearch.Contract.Configuration;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.UI.Avalonia.Properties;
using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Factories.Implementations
{
    public class MapItemAdditionalFilterViewModelsFactory : AdditionalFilterViewModelsFactoryBase
    {
        public MapItemAdditionalFilterViewModelsFactory(IOptionsMonitor<ItemSearchOptions> itemSearchOptions) : base(itemSearchOptions)
        {
        }

        public override IEnumerable<FilterViewModelBase> Create(Item item, SearchQueryRequest searchQueryRequest)
        {
            List<FilterViewModelBase> result = [];

            if (item is MapItem mapItem)
            {
                result.Add(this.GetQualityFilterViewModel(mapItem, searchQueryRequest));
                result.Add(this.GetItemQuantityFilterViewModel(mapItem, searchQueryRequest));
                result.Add(this.GetItemRarityFilterViewModel(mapItem, searchQueryRequest));
                result.Add(this.GetMonsterPacksizeFilterViewModel(mapItem, searchQueryRequest));
                result.Add(this.GetMapTierFilterViewModel(mapItem, searchQueryRequest));
                result.Add(this.GetMapBlightedFilterViewModel(searchQueryRequest));
                result.Add(this.GetMapBlightRavagedFilterViewModel(searchQueryRequest));
                result.Add(this.GetCorruptedFilterViewModel(searchQueryRequest));
                result.Add(this.GetIdentifiedFilterViewModel(searchQueryRequest));
            }

            return result;
        }

        private FilterViewModelBase GetItemQuantityFilterViewModel(MapItem mapItem, SearchQueryRequest searchQueryRequest) =>
            this.CreateBindableMinMaxFilterViewModel(
                x => x.Query.Filters.MapFilters.MapIncreasedItemQuantity,
                Resources.MapItemQuantity,
                mapItem.ItemQuantity,
                searchQueryRequest);

        private FilterViewModelBase GetItemRarityFilterViewModel(MapItem mapItem, SearchQueryRequest searchQueryRequest) =>
            this.CreateBindableMinMaxFilterViewModel(
                x => x.Query.Filters.MapFilters.MapIncreasedItemRarity,
                Resources.MapItemRarity,
                mapItem.ItemRarity,
                searchQueryRequest);

        private FilterViewModelBase GetMonsterPacksizeFilterViewModel(MapItem mapItem, SearchQueryRequest searchQueryRequest) =>
            this.CreateBindableMinMaxFilterViewModel(
                x => x.Query.Filters.MapFilters.MapPacksize,
                Resources.MapMonsterPacksize,
                mapItem.MonsterPackSize,
                searchQueryRequest);

        private FilterViewModelBase GetMapTierFilterViewModel(MapItem mapItem, SearchQueryRequest searchQueryRequest) =>
            this.CreateBindableMinMaxFilterViewModel(
                x => x.Query.Filters.MapFilters.MapTier,
                Resources.MapTier,
                mapItem.Tier,
                searchQueryRequest);

        private FilterViewModelBase GetMapBlightedFilterViewModel(SearchQueryRequest searchQueryRequest) =>
            new BindableFilterViewModel<BoolOptionFilter>(x => x.Query.Filters.MapFilters.MapBlighted)
            {
                Text = Resources.MapBlighted,
                IsEnabled = searchQueryRequest.Query.Filters.MapFilters.MapBlighted?.Option,
            };

        private FilterViewModelBase GetMapBlightRavagedFilterViewModel(SearchQueryRequest searchQueryRequest) =>
            new BindableFilterViewModel<BoolOptionFilter>(x => x.Query.Filters.MapFilters.MapBlightRavaged)
            {
                Text = Resources.MapBlightRavaged,
                IsEnabled = searchQueryRequest.Query.Filters.MapFilters.MapBlightRavaged?.Option,
            };
    }
}