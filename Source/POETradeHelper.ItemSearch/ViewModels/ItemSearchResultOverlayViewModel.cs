using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Services;
using POETradeHelper.PathOfExileTradeApi;
using POETradeHelper.PathOfExileTradeApi.Models;
using ReactiveUI;
using System.Threading.Tasks;

namespace POETradeHelper.ItemSearch.ViewModels
{
    public class ItemSearchResultOverlayViewModel : ReactiveObject, IItemSearchResultOverlayViewModel
    {
        private readonly ISearchItemProvider searchItemProvider;
        private readonly ITradeClient tradeClient;

        public ItemSearchResultOverlayViewModel(ISearchItemProvider searchItemProvider, ITradeClient tradeClient)
        {
            this.searchItemProvider = searchItemProvider;
            this.tradeClient = tradeClient;
        }

        private ListingResult itemListing;

        public ListingResult ItemListing
        {
            get => itemListing;
            set => this.RaiseAndSetIfChanged(ref itemListing, value);
        }

        public async Task SetListingForItemUnderCursorAsync()
        {
            Item item = await searchItemProvider.GetItemFromUnderCursorAsync();

            ListingResult itemListing = await this.tradeClient.GetListingAsync(item);

            if (itemListing != null)
            {
                this.ItemListing = itemListing;
            }
        }
    }
}