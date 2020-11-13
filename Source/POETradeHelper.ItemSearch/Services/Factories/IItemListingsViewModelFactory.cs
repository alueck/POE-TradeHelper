using System.Threading;
using System.Threading.Tasks;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.ItemSearch.Services.Factories
{
    public interface IItemListingsViewModelFactory
    {
        Task<ItemListingsViewModel> CreateAsync(Item item, ItemListingsQueryResult itemListingsQueryResult, CancellationToken cancellationToken = default);
    }
}