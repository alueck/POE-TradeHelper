using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Properties;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;
using System.Collections.Generic;

namespace POETradeHelper.ItemSearch.Services.Factories
{
    public class GemItemAdditionalFilterViewModelsFactory : IAdditionalFilterViewModelsFactory
    {
        public IEnumerable<FilterViewModelBase> Create(Item item, SearchQueryRequest searchQueryRequest)
        {
            var result = new List<FilterViewModelBase>();

            if (item is GemItem gemItem)
            {
                result.Add(GetQualityFilterViewModel(gemItem, searchQueryRequest));
                result.Add(GetGemLevelFilterViewModel(gemItem, searchQueryRequest));
                result.Add(GetGemExperiencePercentFilterViewModel(gemItem, searchQueryRequest));
            }

            return result;
        }

        private static BindableMinMaxFilterViewModel GetQualityFilterViewModel(GemItem gemItem, SearchQueryRequest searchQueryRequest)
        {
            var qualityFilterViewModel = new BindableMinMaxFilterViewModel(x => x.Query.Filters.MiscFilters.Quality)
            {
                Text = Resources.QualityColumn
            };

            SetRemainingProperties(qualityFilterViewModel, gemItem.Quality, searchQueryRequest.Query.Filters.MiscFilters.Quality);

            return qualityFilterViewModel;
        }

        private FilterViewModelBase GetGemLevelFilterViewModel(GemItem gemItem, SearchQueryRequest searchQueryRequest)
        {
            var gemLevelFilterViewModel = new BindableMinMaxFilterViewModel(x => x.Query.Filters.MiscFilters.GemLevel)
            {
                Text = Resources.GemLevelColumn
            };

            SetRemainingProperties(gemLevelFilterViewModel, gemItem.Level, searchQueryRequest.Query.Filters.MiscFilters.GemLevel);

            return gemLevelFilterViewModel;
        }

        private FilterViewModelBase GetGemExperiencePercentFilterViewModel(GemItem gemItem, SearchQueryRequest searchQueryRequest)
        {
            var gemExperiencePercentFilterViewModel = new BindableMinMaxFilterViewModel(x => x.Query.Filters.MiscFilters.GemLevelProgress)
            {
                Text = Resources.GemExperiencePercentColumn
            };

            SetRemainingProperties(gemExperiencePercentFilterViewModel, gemItem.ExperiencePercent, searchQueryRequest.Query.Filters.MiscFilters.GemLevelProgress);

            return gemExperiencePercentFilterViewModel;
        }

        private static void SetRemainingProperties(BindableMinMaxFilterViewModel filterViewModel, int currentValue, MinMaxFilter queryRequestFilter)
        {
            filterViewModel.Current = currentValue.ToString();

            if (queryRequestFilter != null)
            {
                filterViewModel.Min = queryRequestFilter.Min;
                filterViewModel.Max = queryRequestFilter.Max;
                filterViewModel.IsEnabled = true;
            }
            else
            {
                filterViewModel.Min = currentValue;
                filterViewModel.Max = currentValue;
            }
        }
    }
}