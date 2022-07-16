using System.Threading;
using System.Threading.Tasks;

namespace POETradeHelper.ItemSearch.ViewModels
{
    public interface IItemSearchResultOverlayViewModel
    {
        ItemListingsViewModel ItemListings { get; set; }

        Task SetListingForItemUnderCursorAsync(CancellationToken token = default);
    }
}