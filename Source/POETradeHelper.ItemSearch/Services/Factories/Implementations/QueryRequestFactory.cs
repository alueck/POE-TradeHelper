using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;
using System.Linq;

namespace POETradeHelper.ItemSearch.Services.Factories
{
    public class QueryRequestFactory : IQueryRequestFactory
    {
        public IQueryRequest Map(AdvancedQueryViewModel advancedQueryViewModel)
        {
            IQueryRequest result = (IQueryRequest)advancedQueryViewModel.QueryRequest.Clone();

            if (result is SearchQueryRequest searchQueryRequest)
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

            return result;
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
    }
}