using System.Collections.Generic;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Factories.Implementations
{
    public class JewelItemAdditionalFilterViewModelsFactory : AdditionalFilterViewModelsFactoryBase
    {
        public override IEnumerable<FilterViewModelBase> Create(Item item, SearchQueryRequest searchQueryRequest)
        {
            var result = new List<FilterViewModelBase>();

            if (item is JewelItem)
            {
                result.Add(this.GetIdentifiedFilterViewModel(searchQueryRequest));
                result.Add(this.GetCorruptedFilterViewModel(searchQueryRequest));
            }

            return result;
        }
    }
}