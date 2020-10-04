using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;
using ReactiveUI;

namespace POETradeHelper.ItemSearch.Services.Factories
{
    public class QueryRequestFactory : IQueryRequestFactory
    {
        public IQueryRequest Create(AdvancedQueryViewModel advancedQueryViewModel)
        {
            IQueryRequest result = advancedQueryViewModel.QueryRequest;

            if (advancedQueryViewModel.QueryRequest is SearchQueryRequest sourceSearchQueryRequest)
            {
                var searchQueryRequest = new SearchQueryRequest
                {
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
            var enabledStatFilterViewModels = advancedQueryViewModel.AllFilters.Where(f => f.IsEnabled).OfType<StatFilterViewModel>();

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
                    Max = minMaxStatFilterViewModel.Max
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
                    IFilter filter = GetFilter(bindableFilterViewModel, property.PropertyType);
                    property.SetValue(parent, filter);
                    break;
                }

                parent = property.GetValue(parent);
            }
        }

        private static IFilter GetFilter(BindableFilterViewModel filterViewModel, Type filterType)
        {
            IFilter filter = null;

            if (filterViewModel is BindableMinMaxFilterViewModel minMaxFilterViewModel)
            {
                if (minMaxFilterViewModel.IsEnabled)
                {
                    int? maxValue = minMaxFilterViewModel.Max.HasValue ? Math.Max(minMaxFilterViewModel.Min.GetValueOrDefault(), minMaxFilterViewModel.Max.Value) : (int?)null;

                    if (filterType == typeof(SocketsFilter))
                    {
                        filter = new SocketsFilter
                        {
                            Min = minMaxFilterViewModel.Min,
                            Max = maxValue
                        };
                    }
                    else
                    {
                        filter = new MinMaxFilter
                        {
                            Min = minMaxFilterViewModel.Min,
                            Max = maxValue
                        };
                    }
                }
            }
            else
            {
                filter = new BoolOptionFilter
                {
                    Option = filterViewModel.IsEnabled
                };
            }

            return filter;
        }
    }
}