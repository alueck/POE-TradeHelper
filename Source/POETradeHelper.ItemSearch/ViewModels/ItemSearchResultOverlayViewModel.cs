using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Services;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Services;
using ReactiveUI;
using System.Threading;
using System.Threading.Tasks;

namespace POETradeHelper.ItemSearch.ViewModels
{
    public class ItemSearchResultOverlayViewModel : ReactiveObject, IItemSearchResultOverlayViewModel
    {
        private readonly ISearchItemProvider searchItemProvider;
        private readonly IPoeTradeApiClient poeTradeApiClient;

        public ItemSearchResultOverlayViewModel(ISearchItemProvider searchItemProvider, IPoeTradeApiClient tradeClient)
        {
            this.searchItemProvider = searchItemProvider;
            this.poeTradeApiClient = tradeClient;
        }

        private ItemListingsQueryResult itemListing;

        public ItemListingsQueryResult ItemListings
        {
            get => itemListing;
            set => this.RaiseAndSetIfChanged(ref itemListing, value);
        }

        public async Task SetListingForItemUnderCursorAsync(CancellationToken cancellationToken = default)
        {
            Item item = await searchItemProvider.GetItemFromUnderCursorAsync(cancellationToken);

            ItemListingsQueryResult itemListing = await this.poeTradeApiClient.GetListingsAsync(item, cancellationToken);

            if (itemListing != null && !cancellationToken.IsCancellationRequested)
            {
                this.ItemListings = itemListing;
            }
        }
    }
}