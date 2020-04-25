using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using System.Collections.Generic;

namespace POETradeHelper.ItemSearch.Services.Factories
{
    public interface IAdditionalFilterViewModelsFactory
    {
        IEnumerable<FilterViewModelBase> Create(Item item, SearchQueryRequest searchQueryRequest);
    }
}