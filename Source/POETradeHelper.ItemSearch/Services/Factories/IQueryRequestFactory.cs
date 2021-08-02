using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.ItemSearch.Services.Factories
{
    public interface IQueryRequestFactory
    {
        IQueryRequest Create(Item item);

        IQueryRequest Create(IQueryRequest originalRequest, IAdvancedFiltersViewModel advancedFiltersViewModel);
    }
}