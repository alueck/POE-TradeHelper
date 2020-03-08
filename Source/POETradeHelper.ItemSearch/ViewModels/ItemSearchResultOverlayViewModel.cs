using POETradeHelper.Common.UI;
using POETradeHelper.Common.UI.Models;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Services;
using POETradeHelper.ItemSearch.Services.Factories;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Services;
using ReactiveUI;
using Splat;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace POETradeHelper.ItemSearch.ViewModels
{
    public class ItemSearchResultOverlayViewModel : ReactiveObject, IItemSearchResultOverlayViewModel
    {
        private readonly ISearchItemProvider searchItemProvider;
        private readonly IPoeTradeApiClient poeTradeApiClient;

        public ItemSearchResultOverlayViewModel(ISearchItemProvider searchItemProvider, IPoeTradeApiClient tradeClient, IItemListingsViewModelFactory itemListingsViewModelFactory)
        {
            this.searchItemProvider = searchItemProvider;
            this.poeTradeApiClient = tradeClient;
            this.itemListingsViewModelFactory = itemListingsViewModelFactory;

            var canExecuteOpenQueryInBrowser = this.WhenAnyValue(x => x.ItemListings, (ItemListingsViewModel itemListings) => itemListings != null);
            this.OpenQueryInBrowserCommand = ReactiveCommand.Create((IHideable hideableWindow) => this.OpenUrl(this.ItemListings.ListingsUri.ToString(), hideableWindow), canExecuteOpenQueryInBrowser);
        }

        private ItemListingsViewModel itemListing;
        private Message message;
        private readonly IItemListingsViewModelFactory itemListingsViewModelFactory;

        public ItemListingsViewModel ItemListings
        {
            get => this.itemListing;
            set => this.RaiseAndSetIfChanged(ref itemListing, value);
        }

        public Message Message
        {
            get => this.message;
            set => this.RaiseAndSetIfChanged(ref message, value);
        }

        public IReactiveCommand OpenQueryInBrowserCommand { get; }

        public async Task SetListingForItemUnderCursorAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                this.Message = null;

                Item item = await searchItemProvider.GetItemFromUnderCursorAsync(cancellationToken);

                ItemListingsQueryResult itemListing = await this.poeTradeApiClient.GetListingsAsync(item, cancellationToken);

                if (itemListing != null && !cancellationToken.IsCancellationRequested)
                {
                    this.ItemListings = this.itemListingsViewModelFactory.Create(itemListing);
                }
            }
            catch (Exception exception)
            {
                this.Message = new Message
                {
                    Text = $"An error occurred. Please try again.{Environment.NewLine}If the error persists, check the logs and create an issue on GitHub.",
                    Type = MessageType.Error
                };

                this.Log().Error(exception);
            }
        }

        private void OpenUrl(string url, IHideable hideableWindow)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });

            hideableWindow.Hide();
        }
    }
}