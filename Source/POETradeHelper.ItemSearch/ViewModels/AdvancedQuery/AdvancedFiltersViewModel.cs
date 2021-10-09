using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services.Factories;
using POETradeHelper.PathOfExileTradeApi.Models;
using ReactiveUI;

namespace POETradeHelper.ItemSearch.ViewModels
{
    public class AdvancedFiltersViewModel : ReactiveObject, IAdvancedFiltersViewModel
    {
        private readonly IStatFilterViewModelFactory statFilterViewModelFactory;
        private readonly IEnumerable<IAdditionalFilterViewModelsFactory> additionalFilterViewModelsFactories;

        private bool isEnabled;
        private IList<StatFilterViewModel> enchantedItemStatFilters;
        private IList<StatFilterViewModel> fracturedItemStatFilters;
        private IList<StatFilterViewModel> implicitItemStatFilters;
        private IList<StatFilterViewModel> explicitItemStatFilters;
        private IList<StatFilterViewModel> craftedItemStatFilters;
        private IList<StatFilterViewModel> monsterItemStatFilters;
        private IList<StatFilterViewModel> pseudoItemStatFilters;
        private IList<FilterViewModelBase> additionalFilters;

        public AdvancedFiltersViewModel(
            IStatFilterViewModelFactory statFilterViewModelFactory,
            IEnumerable<IAdditionalFilterViewModelsFactory> additionalFilterViewModelsFactories)
        {
            this.statFilterViewModelFactory = statFilterViewModelFactory;
            this.additionalFilterViewModelsFactories = additionalFilterViewModelsFactories;
            this.Reset();
        }

        public IList<StatFilterViewModel> EnchantedItemStatFilters
        {
            get => enchantedItemStatFilters;
            private set => this.RaiseAndSetIfChanged(ref enchantedItemStatFilters, value);
        }

        public IList<StatFilterViewModel> FracturedItemStatFilters
        {
            get => fracturedItemStatFilters;
            private set => this.RaiseAndSetIfChanged(ref fracturedItemStatFilters, value);
        }

        public IList<StatFilterViewModel> ImplicitItemStatFilters
        {
            get => implicitItemStatFilters;
            private set => this.RaiseAndSetIfChanged(ref implicitItemStatFilters, value);
        }

        public IList<StatFilterViewModel> ExplicitItemStatFilters
        {
            get => explicitItemStatFilters;
            private set => this.RaiseAndSetIfChanged(ref explicitItemStatFilters, value);
        }

        public IList<StatFilterViewModel> CraftedItemStatFilters
        {
            get => craftedItemStatFilters;
            private set => this.RaiseAndSetIfChanged(ref craftedItemStatFilters, value);
        }

        public IList<StatFilterViewModel> MonsterItemStatFilters
        {
            get => monsterItemStatFilters;
            private set => this.RaiseAndSetIfChanged(ref monsterItemStatFilters, value);
        }

        public IList<StatFilterViewModel> PseudoItemStatFilters
        {
            get => pseudoItemStatFilters;
            private set => this.RaiseAndSetIfChanged(ref pseudoItemStatFilters, value);
        }

        public IList<FilterViewModelBase> AdditionalFilters
        {
            get => additionalFilters;
            private set => this.RaiseAndSetIfChanged(ref additionalFilters, value);
        }

        public IEnumerable<StatFilterViewModel> AllStatFilters => this.EnchantedItemStatFilters
            .Concat(this.FracturedItemStatFilters)
            .Concat(this.ImplicitItemStatFilters)
            .Concat(this.ExplicitItemStatFilters)
            .Concat(this.CraftedItemStatFilters)
            .Concat(this.MonsterItemStatFilters)
            .Concat(this.PseudoItemStatFilters);

        public bool IsEnabled
        {
            get => isEnabled;
            set => this.RaiseAndSetIfChanged(ref isEnabled, value);
        }

        public Task LoadAsync(Item item, IQueryRequest queryRequest, CancellationToken cancellationToken)
        {
            SearchQueryRequest searchQueryRequest = queryRequest as SearchQueryRequest;
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
            this.enchantedItemStatFilters = new List<StatFilterViewModel>();
            this.fracturedItemStatFilters = new List<StatFilterViewModel>();
            this.implicitItemStatFilters = new List<StatFilterViewModel>();
            this.explicitItemStatFilters = new List<StatFilterViewModel>();
            this.craftedItemStatFilters = new List<StatFilterViewModel>();
            this.monsterItemStatFilters = new List<StatFilterViewModel>();
            this.pseudoItemStatFilters = new List<StatFilterViewModel>();
            this.additionalFilters = new List<FilterViewModelBase>();
        }
    }
}