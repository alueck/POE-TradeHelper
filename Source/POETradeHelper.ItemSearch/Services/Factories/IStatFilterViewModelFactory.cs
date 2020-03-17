using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.ItemSearch.Services.Factories
{
    public interface IStatFilterViewModelFactory
    {
        StatFilterViewModel Create(ItemStat itemStat, SearchQueryRequest queryRequest, StatFilterViewModelFactoryConfiguration configuration);
    }
}