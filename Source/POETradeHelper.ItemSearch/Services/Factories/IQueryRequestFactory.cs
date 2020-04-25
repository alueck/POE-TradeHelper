using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.ItemSearch.Services.Factories
{
    public interface IQueryRequestFactory
    {
        IQueryRequest Create(AdvancedQueryViewModel advancedQueryViewModel);
    }
}