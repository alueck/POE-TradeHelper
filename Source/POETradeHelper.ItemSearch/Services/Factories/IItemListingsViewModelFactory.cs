using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.ItemSearch.Services.Factories
{
    public interface IItemListingsViewModelFactory
    {
        ItemListingsViewModel Create(Item item, ItemListingsQueryResult itemListingsQueryResult);
    }
}