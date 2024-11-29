using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Microsoft.Extensions.Options;

using POETradeHelper.ItemSearch.Contract.Configuration;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.UI.Avalonia.Properties;
using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Factories.Implementations
{
    public abstract class AdditionalFilterViewModelsFactoryBase : IAdditionalFilterViewModelsFactory
    {
        private static readonly Dictionary<Expression<Func<SearchQueryRequest, MinMaxFilter?>>, Func<SearchQueryRequest, MinMaxFilter?>> GettersCache = [];

        protected AdditionalFilterViewModelsFactoryBase(IOptionsMonitor<ItemSearchOptions> itemSearchOptions)
        {
            this.ItemSearchOptions = itemSearchOptions;
        }

        protected IOptionsMonitor<ItemSearchOptions> ItemSearchOptions { get; }

        public abstract IEnumerable<FilterViewModelBase> Create(Item item, SearchQueryRequest searchQueryRequest);

        protected BindableMinMaxFilterViewModel GetQualityFilterViewModel(IQualityItem qualityItem, SearchQueryRequest searchQueryRequest) =>
            this.CreateBindableMinMaxFilterViewModel(
                x => x.Query.Filters.MiscFilters.Quality,
                Resources.QualityColumn,
                qualityItem.Quality,
                searchQueryRequest);

        protected BindableFilterViewModel<BoolOptionFilter> GetIdentifiedFilterViewModel(SearchQueryRequest searchQueryRequest) =>
            new(x => x.Query.Filters.MiscFilters.Identified)
            {
                Text = Resources.Identified,
                IsEnabled = searchQueryRequest.Query.Filters.MiscFilters.Identified?.Option,
            };

        protected FilterViewModelBase GetCorruptedFilterViewModel(SearchQueryRequest searchQueryRequest) =>
            new BindableFilterViewModel<BoolOptionFilter>(x => x.Query.Filters.MiscFilters.Corrupted)
            {
                Text = Resources.Corrupted,
                IsEnabled = searchQueryRequest.Query.Filters.MiscFilters.Corrupted?.Option,
            };

        protected FilterViewModelBase GetSynthesisedFilterViewModel(SearchQueryRequest searchQueryRequest) =>
            new BindableFilterViewModel<BoolOptionFilter>(x => x.Query.Filters.MiscFilters.SynthesisedItem)
            {
                Text = Resources.Synthesised,
                IsEnabled = searchQueryRequest.Query.Filters.MiscFilters.SynthesisedItem?.Option,
            };

        protected BindableMinMaxFilterViewModel CreateBindableMinMaxFilterViewModel(
            Expression<Func<SearchQueryRequest, MinMaxFilter?>> bindingExpression,
            string text,
            int currentValue,
            SearchQueryRequest searchQueryRequest,
            bool offsetCurrentValue = false)
        {
            BindableMinMaxFilterViewModel result = new(bindingExpression)
            {
                Current = currentValue.ToString(),
                Text = text,
            };

            MinMaxFilter? queryRequestFilter = GetValueGetter(bindingExpression)(searchQueryRequest);
            if (queryRequestFilter != null)
            {
                result.Min = queryRequestFilter.Min;
                result.Max = queryRequestFilter.Max;
                result.IsEnabled = true;
            }
            else
            {
                result.Min = Math.Floor(currentValue * (offsetCurrentValue ? 1 + this.ItemSearchOptions.CurrentValue.AdvancedQueryOptions.MinValuePercentageOffset : 1));
                result.Max = Math.Ceiling(currentValue * (offsetCurrentValue ? 1 + this.ItemSearchOptions.CurrentValue.AdvancedQueryOptions.MaxValuePercentageOffset : 1));
            }

            return result;
        }

        protected static Func<SearchQueryRequest, MinMaxFilter?> GetValueGetter(Expression<Func<SearchQueryRequest, MinMaxFilter?>> bindingExpression)
        {
            if (!GettersCache.TryGetValue(bindingExpression, out var getter))
            {
                GettersCache[bindingExpression] = getter = bindingExpression.Compile();
            }

            return getter;
        }
    }
}