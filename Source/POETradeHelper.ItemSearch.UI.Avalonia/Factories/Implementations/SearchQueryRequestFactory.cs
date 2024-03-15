using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services.Mappers;
using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;

using ReactiveUI;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Factories.Implementations
{
    public class SearchQueryRequestFactory : ISearchQueryRequestFactory
    {
        private readonly IEnumerable<IItemSearchQueryRequestMapper> itemToQueryRequestMappers;

        public SearchQueryRequestFactory(IEnumerable<IItemSearchQueryRequestMapper> itemToQueryRequestMappers)
        {
            this.itemToQueryRequestMappers = itemToQueryRequestMappers;
        }

        public SearchQueryRequest Create(Item item)
        {
            IItemSearchQueryRequestMapper mapper = this.itemToQueryRequestMappers.First(m => m.CanMap(item));

            return mapper.MapToQueryRequest(item);
        }

        public SearchQueryRequest Create(SearchQueryRequest originalRequest, IAdvancedFiltersViewModel advancedFiltersViewModel)
        {
            SearchQueryRequest searchQueryRequest = new()
            {
                League = originalRequest.League,
                Query =
                {
                    Name = originalRequest.Query.Name,
                    Term = originalRequest.Query.Term,
                    Type = originalRequest.Query.Type,
                    Filters =
                    {
                        TypeFilters =
                        {
                            Category = (OptionFilter?)originalRequest.Query.Filters.TypeFilters.Category?.Clone(),
                            Rarity = (OptionFilter?)originalRequest.Query.Filters.TypeFilters.Rarity?.Clone(),
                        },
                    },
                },
            };

            SetStatFilters(advancedFiltersViewModel, searchQueryRequest);
            SetAdditionalFilters(advancedFiltersViewModel, searchQueryRequest);

            return searchQueryRequest;
        }

        private static void SetStatFilters(IAdvancedFiltersViewModel advancedFiltersViewModel, SearchQueryRequest searchQueryRequest)
        {
            IEnumerable<StatFilterViewModel> enabledStatFilterViewModels = advancedFiltersViewModel.AllStatFilters.Where(f => f.IsEnabled == true);

            StatFilters statFilters = new();

            foreach (StatFilterViewModel enabledStatFilterViewModel in enabledStatFilterViewModels)
            {
                StatFilter statFilter = CreateStatFilter(enabledStatFilterViewModel);

                statFilters.Filters.Add(statFilter);
            }

            searchQueryRequest.Query.Stats.Clear();

            if (statFilters.Filters.Count > 0)
            {
                searchQueryRequest.Query.Stats.Add(statFilters);
            }
        }

        private static StatFilter CreateStatFilter(StatFilterViewModel statFilterViewModel)
        {
            StatFilter statFilter = new()
            {
                Id = statFilterViewModel.Id,
                Text = statFilterViewModel.Text,
            };

            if (statFilterViewModel is MinMaxStatFilterViewModel minMaxStatFilterViewModel)
            {
                statFilter.Value = new MinMaxFilter
                {
                    Min = minMaxStatFilterViewModel.Min,
                    Max = GetMaxValue(minMaxStatFilterViewModel),
                };
            }

            return statFilter;
        }

        private static void SetAdditionalFilters(IAdvancedFiltersViewModel advancedFiltersViewModel, SearchQueryRequest searchQueryRequest)
        {
            foreach (IBindableFilterViewModel filterViewModel in advancedFiltersViewModel.AdditionalFilters.OfType<IBindableFilterViewModel>())
            {
                SetValueByExpression(filterViewModel.BindingExpression, searchQueryRequest, filterViewModel);
            }
        }

        private static void SetValueByExpression(
            Expression<Func<SearchQueryRequest, IFilter?>> bindingExpression,
            SearchQueryRequest searchQueryRequest,
            IBindableFilterViewModel bindableFilterViewModel)
        {
            List<Expression> expressions = bindingExpression.Body.GetExpressionChain().ToList();
            object? parent = searchQueryRequest;

            foreach (MemberExpression expression in expressions.OfType<MemberExpression>())
            {
                PropertyInfo property = (PropertyInfo)expression.Member;
                if (expression == expressions[^1])
                {
                    IFilter? filter = GetFilter(bindableFilterViewModel);
                    property.SetValue(parent, filter);
                    break;
                }

                parent = property.GetValue(parent);
            }
        }

        private static IFilter? GetFilter(IFilterViewModel filterViewModel)
        {
            IFilter? filter = null;

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
                        White = socketsFilterViewModel.White,
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
                        Max = GetMaxValue(minMaxFilterViewModel),
                    };
                }
            }
            else
            {
                if (filterViewModel.IsEnabled.HasValue)
                {
                    filter = new BoolOptionFilter
                    {
                        Option = filterViewModel.IsEnabled.Value,
                    };
                }
            }

            return filter;
        }

        private static decimal? GetMaxValue(IMinMaxFilterViewModel minMaxFilterViewModel) =>
            minMaxFilterViewModel.Max.HasValue
                ? Math.Max(minMaxFilterViewModel.Min.GetValueOrDefault(), minMaxFilterViewModel.Max.Value)
                : default(decimal?);
    }
}