using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.ItemSearch.Services.Factories
{
    public interface IListingViewModelFactory
    {
        SimpleListingViewModel Create(ListingResult listingResult, Item item);
    }
}