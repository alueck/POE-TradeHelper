using System.Collections.Generic;
using System.Linq;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.ItemSearch.ViewModels
{
    public class AdvancedQueryViewModel
    {
        public IList<StatFilterViewModel> EnchantedItemStatFilters { get; } = new List<StatFilterViewModel>();
        public IList<StatFilterViewModel> FracturedItemStatFilters { get; } = new List<StatFilterViewModel>();
        public IList<StatFilterViewModel> ImplicitItemStatFilters { get; } = new List<StatFilterViewModel>();
        public IList<StatFilterViewModel> ExplicitItemStatFilters { get; } = new List<StatFilterViewModel>();
        public IList<StatFilterViewModel> CraftedItemStatFilters { get; } = new List<StatFilterViewModel>();
        public IList<StatFilterViewModel> MonsterItemStatFilters { get; } = new List<StatFilterViewModel>();
        public IList<StatFilterViewModel> PseudoItemStatFilters { get; } = new List<StatFilterViewModel>();
        public IList<FilterViewModelBase> AdditionalFilters { get; } = new List<FilterViewModelBase>();

        public IEnumerable<FilterViewModelBase> AllFilters => this.EnchantedItemStatFilters
            .Concat(this.FracturedItemStatFilters)
            .Concat(this.ImplicitItemStatFilters)
            .Concat(this.ExplicitItemStatFilters)
            .Concat(this.CraftedItemStatFilters)
            .Concat(this.MonsterItemStatFilters)
            .Concat(this.PseudoItemStatFilters)
            .Concat(this.AdditionalFilters);

        public bool IsVisible => this.AllFilters.Any(f => f.IsEnabled);

        public bool IsEnabled { get; set; }

        public IQueryRequest QueryRequest { get; set; }
    }
}