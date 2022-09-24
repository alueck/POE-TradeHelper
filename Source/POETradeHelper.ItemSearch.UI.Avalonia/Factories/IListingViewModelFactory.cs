using System.Threading;
using System.Threading.Tasks;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Factories
{
    public interface IListingViewModelFactory
    {
        Task<SimpleListingViewModel> CreateAsync(ListingResult listingResult, Item item, CancellationToken cancellationToken = default);

        Task<SimpleListingViewModel> CreateAsync(ExchangeListing listing, CancellationToken cancellationToken = default);
    }
}