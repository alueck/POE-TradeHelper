using POETradeHelper.PathOfExileTradeApi.Models;
using System.Threading.Tasks;

namespace POETradeHelper.ItemSearch.ViewModels
{
    public interface IItemSearchResultOverlayViewModel
    {
        ItemListingsViewModel ItemListings { get; set; }

        Task SetListingForItemUnderCursorAsync(System.Threading.CancellationToken token = default);
    }
}