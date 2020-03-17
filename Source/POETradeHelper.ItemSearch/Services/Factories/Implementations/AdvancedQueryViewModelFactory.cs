using DynamicData;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using System.Collections.Generic;
using System.Linq;

namespace POETradeHelper.ItemSearch.Services.Factories
{
    public class AdvancedQueryViewModelFactory : IAdvancedQueryViewModelFactory
    {
        private readonly IStatFilterViewModelFactory statFilterViewModelFactory;

        public AdvancedQueryViewModelFactory(IStatFilterViewModelFactory statFilterViewModelFactory)
        {
            this.statFilterViewModelFactory = statFilterViewModelFactory;
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
                result.ImplicitItemStatFilters.AddRange(this.CreateFilterViewModels(itemWithStats.Stats.ImplicitStats, searchQueryRequest));
                result.ExplicitItemStatFilters.AddRange(this.CreateFilterViewModels(itemWithStats.Stats.ExplicitStats, searchQueryRequest));
                result.CraftedItemStatFilters.AddRange(this.CreateFilterViewModels(itemWithStats.Stats.CraftedStats, searchQueryRequest));
                result.MonsterItemStatFilters.AddRange(this.CreateFilterViewModels(itemWithStats.Stats.MonsterStats, searchQueryRequest));
            }

            return result;
        }

        private IList<StatFilterViewModel> CreateFilterViewModels(IEnumerable<ItemStat> itemStats, SearchQueryRequest queryRequest)
        {
            var statFilterViewModelFactoryConfiguration = new StatFilterViewModelFactoryConfiguration
            {
                MinValuePercentageOffset = -0.1,
                MaxValuePercentageOffset = 0.1
            };

            return itemStats.Select(stat => this.statFilterViewModelFactory.Create(stat, queryRequest, statFilterViewModelFactoryConfiguration)).ToList();
        }
    }
}