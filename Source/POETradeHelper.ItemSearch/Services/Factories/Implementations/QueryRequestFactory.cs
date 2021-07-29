using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services.Mappers;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;
using ReactiveUI;

namespace POETradeHelper.ItemSearch.Services.Factories
{
    public class QueryRequestFactory : IQueryRequestFactory
    {
        private readonly IEnumerable<IItemSearchQueryRequestMapper> itemToQueryRequestMappers;

        public QueryRequestFactory(IEnumerable<IItemSearchQueryRequestMapper> itemToQueryRequestMappers)
        {
            this.itemToQueryRequestMappers = itemToQueryRequestMappers;
        }

        public IQueryRequest Create(Item item)
        {
            IItemSearchQueryRequestMapper mapper = this.itemToQueryRequestMappers.FirstOrDefault(m => m.CanMap(item));

            return mapper?.MapToQueryRequest(item);
        }

        public IQueryRequest Create(AdvancedQueryViewModel advancedQueryViewModel)
        {
            IQueryRequest result = advancedQueryViewModel.QueryRequest;

            if (advancedQueryViewModel.QueryRequest is SearchQueryRequest sourceSearchQueryRequest)
            {
                var searchQueryRequest = new SearchQueryRequest
                {
                    League = sourceSearchQueryRequest.League,
                    Query =
                    {
                        Name = sourceSearchQueryRequest.Query.Name,
                        Term = sourceSearchQueryRequest.Query.Term,
                        Type = sourceSearchQueryRequest.Query.Type,
                        Filters =
                        {
                            TypeFilters =
                            {
                                Category = (OptionFilter)sourceSearchQueryRequest.Query.Filters.TypeFilters.Category?.Clone(),
                                Rarity = (OptionFilter)sourceSearchQueryRequest.Query.Filters.TypeFilters.Rarity?.Clone()
                            }
                        }
                    }
                };

                SetStatFilters(advancedQueryViewModel, searchQueryRequest);
                SetAdditionalFilters(advancedQueryViewModel, searchQueryRequest);

                result = searchQueryRequest;
            }

            return result;
        }

        private static void SetStatFilters(AdvancedQueryViewModel advancedQueryViewModel, SearchQueryRequest searchQueryRequest)
        {
            var enabledStatFilterViewModels = advancedQueryViewModel.AllFilters.Where(f => f.IsEnabled == true).OfType<StatFilterViewModel>();

            var statFilters = new StatFilters();

            foreach (var enabledStatFilterViewModel in enabledStatFilterViewModels)
            {
                StatFilter statFilter = CreateStatFilter(enabledStatFilterViewModel);

                statFilters.Filters.Add(statFilter);
            }

            searchQueryRequest.Query.Stats.Clear();
            searchQueryRequest.Query.Stats.Add(statFilters);
        }

        private static StatFilter CreateStatFilter(StatFilterViewModel statFilterViewModel)
        {
            var statFilter = new StatFilter
            {
                Id = statFilterViewModel.Id,
                Text = statFilterViewModel.Text,
            };

            if (statFilterViewModel is MinMaxStatFilterViewModel minMaxStatFilterViewModel)
            {
                statFilter.Value = new MinMaxFilter
                {
                    Min = minMaxStatFilterViewModel.Min,
                    Max = GetMaxValue(minMaxStatFilterViewModel)
                };
            }

            return statFilter;
        }

        private static void SetAdditionalFilters(AdvancedQueryViewModel advancedQueryViewModel, SearchQueryRequest searchQueryRequest)
        {
            foreach (var filterViewModel in advancedQueryViewModel.AdditionalFilters.OfType<BindableFilterViewModel>())
            {
                SetValueByExpression(filterViewModel.BindingExpression, searchQueryRequest, filterViewModel);
            }
        }

        private static void SetValueByExpression(Expression<Func<SearchQueryRequest, IFilter>> bindingExpression, SearchQueryRequest searchQueryRequest, BindableFilterViewModel bindableFilterViewModel)
        {
            var expressions = bindingExpression.Body.GetExpressionChain().ToList();
            object parent = searchQueryRequest;

            foreach (MemberExpression expression in expressions)
            {
                PropertyInfo property = ((PropertyInfo)expression.Member);
                if (expression == expressions.Last())
                {
                    IFilter filter = GetFilter(bindableFilterViewModel);
                    property.SetValue(parent, filter);
                    break;
                }

                parent = property.GetValue(parent);
            }
        }

        private static IFilter GetFilter(FilterViewModelBase filterViewModel)
        {
            IFilter filter = null;

            if (filterViewModel is BindableSocketsFilterViewModel socketsFilterViewModel)
            {
                if (socketsFilterViewModel.IsEnabled == true)
                {
                    filter = new SocketsFilter
                    {
                        Min = socketsFilterViewModel.Min,
                        Max = GetMaxValue(socketsFilterViewModel),
                        Red = socketsFilterViewModel.Red,
                        Green = socketsFilterViewModel.Green,
                        Blue = socketsFilterViewModel.Blue,
                        White = socketsFilterViewModel.White
                    };
                }
            }
            else if (filterViewModel is IMinMaxFilterViewModel minMaxFilterViewModel)
            {
                if (minMaxFilterViewModel.IsEnabled == true)
                {
                    filter = new MinMaxFilter
                    {
                        Min = minMaxFilterViewModel.Min,
                        Max = GetMaxValue(minMaxFilterViewModel)
                    };
                }
            }
            else
            {
                if (filterViewModel.IsEnabled.HasValue)
                {
                    filter = new BoolOptionFilter
                    {
                        Option = filterViewModel.IsEnabled.Value
                    };
                }
            }

            return filter;
        }

        private static decimal? GetMaxValue(IMinMaxFilterViewModel minMaxFilterViewModel)
        {
            return minMaxFilterViewModel.Max.HasValue
                ? Math.Max(minMaxFilterViewModel.Min.GetValueOrDefault(), minMaxFilterViewModel.Max.Value)
                : default(decimal?);
        }
    }
}