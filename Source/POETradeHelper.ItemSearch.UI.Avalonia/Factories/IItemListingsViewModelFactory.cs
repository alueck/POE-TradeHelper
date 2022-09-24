using System.Threading;
using System.Threading.Tasks;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Factories
{
    public interface IItemListingsViewModelFactory
    {
        Task<ItemListingsViewModel> CreateAsync(Item item, ItemListingsQueryResult itemListingsQueryResult, CancellationToken cancellationToken = default);

        Task<ItemListingsViewModel> CreateAsync(ExchangeQueryResult exchangeQueryResult, CancellationToken cancellationToken = default);
    }
}