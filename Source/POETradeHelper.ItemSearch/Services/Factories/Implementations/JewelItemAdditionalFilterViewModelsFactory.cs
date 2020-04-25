using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using System.Collections.Generic;

namespace POETradeHelper.ItemSearch.Services.Factories
{
    public class JewelItemAdditionalFilterViewModelsFactory : AdditionalFilterViewModelsFactoryBase
    {
        public override IEnumerable<FilterViewModelBase> Create(Item item, SearchQueryRequest searchQueryRequest)
        {
            var result = new List<FilterViewModelBase>();

            if (item is JewelItem jewelItem)
            {
                result.Add(this.GetIdentifiedFilterViewModel(jewelItem, searchQueryRequest));
                result.Add(this.GetCorruptedFilterViewModel(jewelItem, searchQueryRequest));
            }

            return result;
        }
    }
}