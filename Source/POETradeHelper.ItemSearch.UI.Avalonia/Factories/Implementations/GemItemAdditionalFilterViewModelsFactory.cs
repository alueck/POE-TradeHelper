using System.Collections.Generic;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.UI.Avalonia.Properties;
using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Factories.Implementations
{
    public class GemItemAdditionalFilterViewModelsFactory : AdditionalFilterViewModelsFactoryBase
    {
        public override IEnumerable<FilterViewModelBase> Create(Item item, SearchQueryRequest searchQueryRequest)
        {
            var result = new List<FilterViewModelBase>();

            if (item is GemItem gemItem)
            {
                result.Add(this.GetQualityFilterViewModel(gemItem, searchQueryRequest));
                result.Add(this.GetGemLevelFilterViewModel(gemItem, searchQueryRequest));
                result.Add(this.GetGemExperiencePercentFilterViewModel(gemItem, searchQueryRequest));
            }

            return result;
        }

        private FilterViewModelBase GetGemLevelFilterViewModel(GemItem gemItem, SearchQueryRequest searchQueryRequest)
        {
            return this.CreateBindableMinMaxFilterViewModel(
                x => x.Query.Filters.MiscFilters.GemLevel,
                Resources.GemLevelColumn,
                gemItem.Level,
                searchQueryRequest.Query.Filters.MiscFilters.GemLevel);
        }

        private FilterViewModelBase GetGemExperiencePercentFilterViewModel(GemItem gemItem, SearchQueryRequest searchQueryRequest)
        {
            return this.CreateBindableMinMaxFilterViewModel(
                x => x.Query.Filters.MiscFilters.GemLevelProgress,
                Resources.GemExperiencePercentColumn,
                gemItem.ExperiencePercent,
                searchQueryRequest.Query.Filters.MiscFilters.GemLevelProgress);
        }
    }
}