using System.Collections.Generic;

using Microsoft.Extensions.Options;

using POETradeHelper.ItemSearch.Contract.Configuration;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Factories.Implementations
{
    public class FlaskItemAdditionalFilterViewModelsFactory : AdditionalFilterViewModelsFactoryBase
    {
        public FlaskItemAdditionalFilterViewModelsFactory(IOptionsMonitor<ItemSearchOptions> itemSearchOptions) : base(itemSearchOptions)
        {
        }

        public override IEnumerable<FilterViewModelBase> Create(Item item, SearchQueryRequest searchQueryRequest)
        {
            var result = new List<FilterViewModelBase>();

            if (item is FlaskItem flaskItem)
            {
                result.Add(this.GetQualityFilterViewModel(flaskItem, searchQueryRequest));
                result.Add(this.GetIdentifiedFilterViewModel(searchQueryRequest));
            }

            return result;
        }
    }
}