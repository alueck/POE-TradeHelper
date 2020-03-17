using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.ItemSearch.Services.Factories
{
    public interface IAdvancedQueryViewModelFactory
    {
        AdvancedQueryViewModel Create(Item item, IQueryRequest searchQueryRequest);
    }
}