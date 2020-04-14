using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.ViewModels;
using System.Collections.Generic;

namespace POETradeHelper.ItemSearch.Services.Factories
{
    public interface IAdditionalFiltersViewModelFactory
    {
        IEnumerable<FilterViewModelBase> Create(Item item);
    }
}