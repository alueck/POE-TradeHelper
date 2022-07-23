using System.Threading;
using System.Threading.Tasks;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.ItemSearch.Services.Factories
{
    public interface IListingViewModelFactory
    {
        Task<SimpleListingViewModel> CreateAsync(ListingResult listingResult, Item item, CancellationToken cancellationToken = default);
        
        Task<SimpleListingViewModel> CreateAsync(ExchangeListing listing, CancellationToken cancellationToken = default);
    }
}