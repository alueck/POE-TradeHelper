using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Factories
{
    public interface ISearchQueryRequestFactory
    {
        SearchQueryRequest Create(Item item);

        SearchQueryRequest Create(SearchQueryRequest originalRequest, IAdvancedFiltersViewModel advancedFiltersViewModel);
    }
}
