using POETradeHelper.PathOfExileTradeApi.Models;
using System.Threading.Tasks;

namespace POETradeHelper.ItemSearch.ViewModels
{
    public interface IItemSearchResultOverlayViewModel
    {
        ItemListingsQueryResult ItemListings { get; set; }

        Task SetListingForItemUnderCursorAsync(System.Threading.CancellationToken token = default);
    }
}