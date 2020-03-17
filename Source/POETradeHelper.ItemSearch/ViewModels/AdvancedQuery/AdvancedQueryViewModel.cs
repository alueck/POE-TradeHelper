﻿using POETradeHelper.PathOfExileTradeApi.Models;
using System.Collections.Generic;
using System.Linq;

namespace POETradeHelper.ItemSearch.ViewModels
{
    public class AdvancedQueryViewModel
    {
        public IList<StatFilterViewModel> EnchantedItemStatFilters { get; } = new List<StatFilterViewModel>();
        public IList<StatFilterViewModel> ImplicitItemStatFilters { get; } = new List<StatFilterViewModel>();
        public IList<StatFilterViewModel> ExplicitItemStatFilters { get; } = new List<StatFilterViewModel>();
        public IList<StatFilterViewModel> CraftedItemStatFilters { get; } = new List<StatFilterViewModel>();
        public IList<StatFilterViewModel> MonsterItemStatFilters { get; } = new List<StatFilterViewModel>();
        public IList<FilterViewModel> AdditionalFilters { get; } = new List<FilterViewModel>();

        public IEnumerable<FilterViewModel> AllFilters => this.EnchantedItemStatFilters
            .Concat(this.ImplicitItemStatFilters)
            .Concat(this.ExplicitItemStatFilters)
            .Concat(this.CraftedItemStatFilters)
            .Concat(this.MonsterItemStatFilters)
            .Concat(this.AdditionalFilters);

        public bool IsVisible => this.AllFilters.Any(f => f.IsEnabled);

        public bool IsEnabled { get; set; }

        public IQueryRequest QueryRequest { get; set; }
    }
}