using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Factories
{
    public interface IStatFilterViewModelFactory
    {
        StatFilterViewModel Create(ItemStat itemStat, SearchQueryRequest queryRequest);
    }
}