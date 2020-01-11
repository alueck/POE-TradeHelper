using POETradeHelper.PathOfExileTradeApi.Models;
using System.Threading.Tasks;

namespace POETradeHelper.ItemSearch.ViewModels
{
    public interface IItemSearchResultOverlayViewModel
    {
        ListingResult ItemListing { get; set; }

        Task SetListingForItemUnderCursorAsync();
    }
}