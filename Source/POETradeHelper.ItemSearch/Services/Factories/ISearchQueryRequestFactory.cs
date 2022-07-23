using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.ItemSearch.Services.Factories
{
    public interface ISearchQueryRequestFactory
    {
        SearchQueryRequest Create(Item item);

        SearchQueryRequest Create(SearchQueryRequest originalRequest, IAdvancedFiltersViewModel advancedFiltersViewModel);
    }
}