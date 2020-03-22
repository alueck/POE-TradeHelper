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
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;

namespace POETradeHelper.ItemSearch.ViewModels
{
    public class ItemSearchResultOverlayViewModel : ReactiveObject, IItemSearchResultOverlayViewModel
    {
        private readonly ISearchItemProvider searchItemProvider;
        private readonly IPoeTradeApiClient poeTradeApiClient;
        private readonly IItemListingsViewModelFactory itemListingsViewModelFactory;
        private readonly IAdvancedQueryViewModelFactory advancedQueryViewModelFactory;
        private readonly IQueryRequestFactory queryRequestFactory;

        public ItemSearchResultOverlayViewModel(
            ISearchItemProvider searchItemProvider,
            IPoeTradeApiClient tradeClient,
            IItemListingsViewModelFactory itemListingsViewModelFactory,
            IAdvancedQueryViewModelFactory advancedQueryViewModelFactory,
            IQueryRequestFactory queryRequestFactory)
        {
            this.searchItemProvider = searchItemProvider;
            this.poeTradeApiClient = tradeClient;
            this.itemListingsViewModelFactory = itemListingsViewModelFactory;
            this.advancedQueryViewModelFactory = advancedQueryViewModelFactory;

            var canExecuteOpenQueryInBrowser = this.WhenAnyValue(x => x.ItemListings, (ItemListingsViewModel itemListings) => itemListings != null);
            this.OpenQueryInBrowserCommand = ReactiveCommand.Create((IHideable hideableWindow) => this.OpenUrl(this.ItemListings.ListingsUri.ToString(), hideableWindow), canExecuteOpenQueryInBrowser);

            this.ExecuteAdvancedQueryCommand = ReactiveCommand.CreateFromTask(() => this.ExecuteAdvancedQueryAsync());
            this.ExecuteAdvancedQueryCommand.IsExecuting.ToProperty(this, x => x.IsBusy);
            this.queryRequestFactory = queryRequestFactory;
        }

        private ItemListingsViewModel itemListing;
        private Message message;

        public ItemListingsViewModel ItemListings
        {
            get => this.itemListing;
            set => this.RaiseAndSetIfChanged(ref itemListing, value);
        }

        private AdvancedQueryViewModel advancedQuery;

        public AdvancedQueryViewModel AdvancedQuery
        {
            get => this.advancedQuery;
            set => this.RaiseAndSetIfChanged(ref advancedQuery, value);
        }

        public Message Message
        {
            get => this.message;
            set => this.RaiseAndSetIfChanged(ref message, value);
        }

        private bool isBusy;

        public bool IsBusy
        {
            get { return isBusy; }
            set { this.RaiseAndSetIfChanged(ref this.isBusy, value); }
        }

        public ReactiveCommand<IHideable, Unit> OpenQueryInBrowserCommand { get; }

        public ReactiveCommand<Unit, Unit> ExecuteAdvancedQueryCommand { get; }

        internal Item Item { get; set; }

        public async Task SetListingForItemUnderCursorAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                this.IsBusy = true;

                this.Message = null;

                Item = await searchItemProvider.GetItemFromUnderCursorAsync(cancellationToken);

                ItemListingsQueryResult itemListing = await this.poeTradeApiClient.GetListingsAsync(Item, cancellationToken);

                if (itemListing != null && !cancellationToken.IsCancellationRequested)
                {
                    this.ItemListings = await this.itemListingsViewModelFactory.CreateAsync(Item, itemListing);
                    this.AdvancedQuery = this.advancedQueryViewModelFactory.Create(Item, itemListing.SearchQueryRequest);
                }
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
            finally
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    this.IsBusy = false;
                }
            }
        }

        private void HandleException(Exception exception)
        {
            this.Message = new Message
            {
                Text = $"An error occurred. Please try again.{Environment.NewLine}If the error persists, check the logs and create an issue on GitHub.",
                Type = MessageType.Error
            };

            this.Log().Error(exception);
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

        internal async Task ExecuteAdvancedQueryAsync()
        {
            try
            {
                this.Message = null;

                var queryRequest = this.queryRequestFactory.Map(this.AdvancedQuery);

                ItemListingsQueryResult itemListingsQueryResult = await this.poeTradeApiClient.GetListingsAsync(queryRequest);

                this.ItemListings = await this.itemListingsViewModelFactory.CreateAsync(this.Item, itemListingsQueryResult);
                this.AdvancedQuery = this.advancedQueryViewModelFactory.Create(this.Item, queryRequest);
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
        }
    }
}