using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.ItemSearch.Services.Factories
{
    public interface IItemListingsViewModelFactory
    {
        ItemListingsViewModel Create(ItemListingsQueryResult itemListingsQueryResult);
    }
}