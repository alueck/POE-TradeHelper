using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;
using ReactiveUI;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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
                IFilter filter = GetFilter(filterViewModel);

                if (filter != null)
                {
                    SetValueByExpression(filterViewModel.BindingExpression, searchQueryRequest, filter);
                }
            }
        }

        private static IFilter GetFilter(BindableFilterViewModel filterViewModel)
        {
            IFilter filter = null;

            if (filterViewModel is BindableMinMaxFilterViewModel minMaxFilterViewModel)
            {
                if (minMaxFilterViewModel.IsEnabled)
                {
                    filter = new MinMaxFilter
                    {
                        Min = minMaxFilterViewModel.Min,
                        Max = minMaxFilterViewModel.Max
                    };
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

        private static void SetValueByExpression(Expression<Func<SearchQueryRequest, IFilter>> bindingExpression, SearchQueryRequest searchQueryRequest, IFilter value)
        {
            var expressions = bindingExpression.Body.GetExpressionChain().ToList();
            object parent = searchQueryRequest;

            foreach (MemberExpression expression in expressions)
            {
                if (expression == expressions.Last())
                {
                    ((PropertyInfo)expression.Member).SetValue(parent, value);
                    break;
                }

                parent = ((PropertyInfo)expression.Member).GetValue(parent);
            }
        }
    }
}