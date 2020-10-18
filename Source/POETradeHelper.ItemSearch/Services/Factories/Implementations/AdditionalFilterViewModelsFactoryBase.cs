using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Properties;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;

namespace POETradeHelper.ItemSearch.Services.Factories
{
    public abstract class AdditionalFilterViewModelsFactoryBase : IAdditionalFilterViewModelsFactory
    {
        public abstract IEnumerable<FilterViewModelBase> Create(Item item, SearchQueryRequest searchQueryRequest);

        protected BindableMinMaxFilterViewModel GetQualityFilterViewModel(IQualityItem qualityItem, SearchQueryRequest searchQueryRequest)
        {
            return this.CreateBindableMinMaxFilterViewModel(
                x => x.Query.Filters.MiscFilters.Quality,
                Resources.QualityColumn,
                qualityItem.Quality,
                searchQueryRequest.Query.Filters.MiscFilters.Quality);
        }

        protected BindableFilterViewModel GetIdentifiedFilterViewModel(SearchQueryRequest searchQueryRequest)
        {
            return new BindableFilterViewModel(x => x.Query.Filters.MiscFilters.Identified)
            {
                Text = Resources.Identified,
                IsEnabled = searchQueryRequest.Query.Filters.MiscFilters.Identified?.Option
            };
        }

        protected FilterViewModelBase GetCorruptedFilterViewModel(SearchQueryRequest searchQueryRequest)
        {
            return new BindableFilterViewModel(x => x.Query.Filters.MiscFilters.Corrupted)
            {
                Text = Resources.Corrupted,
                IsEnabled = searchQueryRequest.Query.Filters.MiscFilters.Corrupted?.Option
            };
        }

        protected virtual BindableMinMaxFilterViewModel CreateBindableMinMaxFilterViewModel(Expression<Func<SearchQueryRequest, IFilter>> bindingExpression, string text, int currentValue, MinMaxFilter queryRequestFilter)
        {
            var result = new BindableMinMaxFilterViewModel(bindingExpression)
            {
                Current = currentValue.ToString(),
                Text = text
            };

            if (queryRequestFilter != null)
            {
                result.Min = queryRequestFilter.Min;
                result.Max = queryRequestFilter.Max;
                result.IsEnabled = true;
            }
            else
            {
                result.Min = currentValue;
                result.Max = currentValue;
            }

            return result;
        }
    }
}