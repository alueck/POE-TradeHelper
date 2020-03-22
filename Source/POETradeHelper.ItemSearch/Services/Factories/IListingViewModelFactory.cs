using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using System.Threading.Tasks;

namespace POETradeHelper.ItemSearch.Services.Factories
{
    public interface IListingViewModelFactory
    {
        Task<SimpleListingViewModel> CreateAsync(ListingResult listingResult, Item item);
    }
}