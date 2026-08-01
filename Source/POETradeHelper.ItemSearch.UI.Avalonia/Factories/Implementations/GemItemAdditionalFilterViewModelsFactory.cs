using System.Collections.Generic;

using Microsoft.Extensions.Options;

using POETradeHelper.ItemSearch.Contract.Configuration;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.UI.Avalonia.Properties;
using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Factories.Implementations
{
    public class GemItemAdditionalFilterViewModelsFactory : AdditionalFilterViewModelsFactoryBase
    {
        public GemItemAdditionalFilterViewModelsFactory(IOptionsMonitor<ItemSearchOptions> itemSearchOptions) : base(itemSearchOptions)
        {
        }

        public override IEnumerable<FilterViewModelBase> Create(Item item, SearchQueryRequest searchQueryRequest)
        {
            var result = new List<FilterViewModelBase>();

            if (item is GemItem gemItem)
            {
                result.Add(this.GetQualityFilterViewModel(gemItem, searchQueryRequest));
                result.Add(this.GetGemLevelFilterViewModel(gemItem, searchQueryRequest));
                result.Add(this.GetGemExperiencePercentFilterViewModel(gemItem, searchQueryRequest));

                if (!gemItem.IsVaalVersion)
                {
                    result.Add(this.GetCorruptedFilterViewModel(searchQueryRequest));
                    result.Add(this.GetGemImbuedFilterViewModel(searchQueryRequest));
                }

                result.Add(this.GetGemTransfiguredFilterViewModel(searchQueryRequest));
            }

            return result;
        }

        private FilterViewModelBase GetGemLevelFilterViewModel(GemItem gemItem, SearchQueryRequest searchQueryRequest)
        {
            return this.CreateBindableMinMaxFilterViewModel(
                x => x.Query.Filters.MiscFilters.GemLevel,
                Resources.GemLevelColumn,
                gemItem.Level,
                searchQueryRequest);
        }

        private FilterViewModelBase GetGemExperiencePercentFilterViewModel(GemItem gemItem, SearchQueryRequest searchQueryRequest)
        {
            return this.CreateBindableMinMaxFilterViewModel(
                x => x.Query.Filters.MiscFilters.GemLevelProgress,
                Resources.GemExperiencePercentColumn,
                gemItem.ExperiencePercent,
                searchQueryRequest);
        }

        private FilterViewModelBase GetGemImbuedFilterViewModel(SearchQueryRequest searchQueryRequest)
        {
            return new BindableFilterViewModel<BoolOptionFilter>(x => x.Query.Filters.MiscFilters.GemImbued)
            {
                Text = Resources.Imbued,
                IsEnabled = searchQueryRequest.Query.Filters.MiscFilters.GemImbued?.Option,
            };
        }

        private FilterViewModelBase GetGemTransfiguredFilterViewModel(SearchQueryRequest searchQueryRequest)
        {
            return new BindableFilterViewModel<BoolOptionFilter>(x => x.Query.Filters.MiscFilters.GemTransfigured)
            {
                Text = Resources.Transfigured,
                IsEnabled = searchQueryRequest.Query.Filters.MiscFilters.GemTransfigured?.Option,
            };
        }
    }
}