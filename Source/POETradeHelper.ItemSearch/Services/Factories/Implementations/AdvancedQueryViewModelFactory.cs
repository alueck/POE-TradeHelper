using System.Collections.Generic;
using System.Linq;
using DynamicData;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.ItemSearch.Services.Factories
{
    public class AdvancedQueryViewModelFactory : IAdvancedQueryViewModelFactory
    {
        private readonly IStatFilterViewModelFactory statFilterViewModelFactory;
        private readonly IEnumerable<IAdditionalFilterViewModelsFactory> additionalFilterViewModelsFactories;

        public AdvancedQueryViewModelFactory(IStatFilterViewModelFactory statFilterViewModelFactory, IEnumerable<IAdditionalFilterViewModelsFactory> additionalFiltersViewModelFactories)
        {
            this.statFilterViewModelFactory = statFilterViewModelFactory;
            this.additionalFilterViewModelsFactories = additionalFiltersViewModelFactories;
        }

        public AdvancedQueryViewModel Create(Item item, IQueryRequest queryRequest)
        {
            SearchQueryRequest searchQueryRequest = queryRequest as SearchQueryRequest;

            var result = new AdvancedQueryViewModel
            {
                QueryRequest = queryRequest,
                IsEnabled = searchQueryRequest != null && (item is ItemWithStats || item is GemItem)
            };

            if (result.IsEnabled && item is ItemWithStats itemWithStats && itemWithStats.Stats != null)
            {
                result.EnchantedItemStatFilters.AddRange(this.CreateFilterViewModels(itemWithStats.Stats.EnchantedStats, searchQueryRequest));
                result.FracturedItemStatFilters.AddRange(this.CreateFilterViewModels(itemWithStats.Stats.FracturedStats, searchQueryRequest));
                result.ImplicitItemStatFilters.AddRange(this.CreateFilterViewModels(itemWithStats.Stats.ImplicitStats, searchQueryRequest));
                result.ExplicitItemStatFilters.AddRange(this.CreateFilterViewModels(itemWithStats.Stats.ExplicitStats, searchQueryRequest));
                result.CraftedItemStatFilters.AddRange(this.CreateFilterViewModels(itemWithStats.Stats.CraftedStats, searchQueryRequest));
                result.MonsterItemStatFilters.AddRange(this.CreateFilterViewModels(itemWithStats.Stats.MonsterStats, searchQueryRequest));
                result.PseudoItemStatFilters.AddRange(this.CreateFilterViewModels(itemWithStats.Stats.PseudoStats, searchQueryRequest));
            }

            result.AdditionalFilters.AddRange(this.additionalFilterViewModelsFactories.SelectMany(x => x.Create(item, searchQueryRequest)));

            return result;
        }

        private IList<StatFilterViewModel> CreateFilterViewModels(IEnumerable<ItemStat> itemStats, SearchQueryRequest queryRequest)
        {
            return itemStats.Select(stat => this.statFilterViewModelFactory.Create(stat, queryRequest)).ToList();
        }
    }
}