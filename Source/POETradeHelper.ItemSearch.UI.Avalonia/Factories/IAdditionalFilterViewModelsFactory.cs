using System.Collections.Generic;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Factories
{
    public interface IAdditionalFilterViewModelsFactory
    {
        IEnumerable<FilterViewModelBase> Create(Item item, SearchQueryRequest searchQueryRequest);
    }
}