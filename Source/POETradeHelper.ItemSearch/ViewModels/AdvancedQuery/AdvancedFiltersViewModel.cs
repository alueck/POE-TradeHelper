using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services.Factories;
using POETradeHelper.PathOfExileTradeApi.Models;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace POETradeHelper.ItemSearch.ViewModels
{
    public class AdvancedFiltersViewModel : ReactiveObject, IAdvancedFiltersViewModel
    {
        private readonly IStatFilterViewModelFactory statFilterViewModelFactory;
        private readonly IEnumerable<IAdditionalFilterViewModelsFactory> additionalFilterViewModelsFactories;

        public AdvancedFiltersViewModel(
            IStatFilterViewModelFactory statFilterViewModelFactory,
            IEnumerable<IAdditionalFilterViewModelsFactory> additionalFilterViewModelsFactories)
        {
            this.statFilterViewModelFactory = statFilterViewModelFactory;
            this.additionalFilterViewModelsFactories = additionalFilterViewModelsFactories;
            this.Reset();
        }

        [Reactive]
        public IList<StatFilterViewModel> EnchantedItemStatFilters { get; private set; }
        
        [Reactive]
        public IList<StatFilterViewModel> FracturedItemStatFilters { get; private set; }
        
        [Reactive]
        public IList<StatFilterViewModel> ImplicitItemStatFilters { get; private set; }

        [Reactive]
        public IList<StatFilterViewModel> ExplicitItemStatFilters { get; private set; }

        [Reactive]
        public IList<StatFilterViewModel> CraftedItemStatFilters { get; private set; }

        [Reactive]
        public IList<StatFilterViewModel> MonsterItemStatFilters { get; private set; }
        
        [Reactive]
        public IList<StatFilterViewModel> PseudoItemStatFilters { get; private set; }

        [Reactive]
        public IList<FilterViewModelBase> AdditionalFilters { get; private set; }

        public IEnumerable<StatFilterViewModel> AllStatFilters => this.EnchantedItemStatFilters
            .Concat(this.FracturedItemStatFilters)
            .Concat(this.ImplicitItemStatFilters)
            .Concat(this.ExplicitItemStatFilters)
            .Concat(this.CraftedItemStatFilters)
            .Concat(this.MonsterItemStatFilters)
            .Concat(this.PseudoItemStatFilters);

        [Reactive]
        public bool IsEnabled { get; private set; }

        public Task LoadAsync(Item item, SearchQueryRequest searchQueryRequest, CancellationToken cancellationToken)
        {
            this.IsEnabled = searchQueryRequest != null && item is ItemWithStats or GemItem;

            if (!this.IsEnabled)
            {
                this.Reset();
                return Task.CompletedTask;
            }

            if (item is ItemWithStats { Stats: {}} itemWithStats)
            {
                this.EnchantedItemStatFilters = this.CreateFilterViewModels(itemWithStats.Stats.EnchantedStats, searchQueryRequest);
                this.FracturedItemStatFilters = this.CreateFilterViewModels(itemWithStats.Stats.FracturedStats, searchQueryRequest);
                this.ImplicitItemStatFilters = this.CreateFilterViewModels(itemWithStats.Stats.ImplicitStats, searchQueryRequest);
                this.ExplicitItemStatFilters = this.CreateFilterViewModels(itemWithStats.Stats.ExplicitStats, searchQueryRequest);
                this.CraftedItemStatFilters = this.CreateFilterViewModels(itemWithStats.Stats.CraftedStats, searchQueryRequest);
                this.MonsterItemStatFilters = this.CreateFilterViewModels(itemWithStats.Stats.MonsterStats, searchQueryRequest);
                this.PseudoItemStatFilters = this.CreateFilterViewModels(itemWithStats.Stats.PseudoStats, searchQueryRequest);
            }

            this.AdditionalFilters = this.additionalFilterViewModelsFactories.SelectMany(x => x.Create(item, searchQueryRequest)).ToList();

            return Task.CompletedTask;
        }

        private IList<StatFilterViewModel> CreateFilterViewModels(IEnumerable<ItemStat> itemStats, SearchQueryRequest queryRequest)
        {
            return itemStats.Select(stat => this.statFilterViewModelFactory.Create(stat, queryRequest)).ToList();
        }

        private void Reset()
        {
            this.EnchantedItemStatFilters = new List<StatFilterViewModel>();
            this.FracturedItemStatFilters = new List<StatFilterViewModel>();
            this.ImplicitItemStatFilters = new List<StatFilterViewModel>();
            this.ExplicitItemStatFilters = new List<StatFilterViewModel>();
            this.CraftedItemStatFilters = new List<StatFilterViewModel>();
            this.MonsterItemStatFilters = new List<StatFilterViewModel>();
            this.PseudoItemStatFilters = new List<StatFilterViewModel>();
            this.AdditionalFilters = new List<FilterViewModelBase>();
        }
    }
}