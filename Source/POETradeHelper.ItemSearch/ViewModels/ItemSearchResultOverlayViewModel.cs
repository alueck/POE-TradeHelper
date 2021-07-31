using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using POETradeHelper.Common.UI.Models;
using POETradeHelper.ItemSearch.Contract.Configuration;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Queries;
using POETradeHelper.ItemSearch.Exceptions;
using POETradeHelper.ItemSearch.Services.Factories;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Services;
using POETradeHelper.PricePrediction.Queries;
using POETradeHelper.PricePrediction.ViewModels;
using ReactiveUI;
using Splat;
using Unit = System.Reactive.Unit;

namespace POETradeHelper.ItemSearch.ViewModels
{
    public class ItemSearchResultOverlayViewModel : ReactiveObject, IItemSearchResultOverlayViewModel
    {
        private readonly IPoeTradeApiClient poeTradeApiClient;
        private readonly IItemListingsViewModelFactory itemListingsViewModelFactory;
        private readonly IAdvancedQueryViewModelFactory advancedQueryViewModelFactory;
        private readonly IQueryRequestFactory queryRequestFactory;
        private readonly IMediator mediator;
        private readonly IOptionsMonitor<ItemSearchOptions> itemSearchOptions;

        public ItemSearchResultOverlayViewModel(
            IPoeTradeApiClient tradeClient,
            IItemListingsViewModelFactory itemListingsViewModelFactory,
            IAdvancedQueryViewModelFactory advancedQueryViewModelFactory,
            IQueryRequestFactory queryRequestFactory,
            IMediator mediator,
            IOptionsMonitor<ItemSearchOptions> itemSearchOptions)
        {
            this.poeTradeApiClient = tradeClient;
            this.itemListingsViewModelFactory = itemListingsViewModelFactory;
            this.advancedQueryViewModelFactory = advancedQueryViewModelFactory;

            this.ExecuteAdvancedQueryCommand = ReactiveCommand.CreateFromTask(() => this.ExecuteAdvancedQueryAsync());
            this.ExecuteAdvancedQueryCommand.IsExecuting.ToProperty(this, x => x.IsBusy);
            this.queryRequestFactory = queryRequestFactory;
            this.mediator = mediator;
            this.itemSearchOptions = itemSearchOptions;
            this.itemSearchOptions.OnChange(newValue =>
            {
                if (!newValue.PricePredictionEnabled)
                {
                    this.PricePrediction = new PricePredictionViewModel();
                }
            });

            this.PricePrediction = new PricePredictionViewModel();
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

        private PricePredictionViewModel pricePredicition;

        public PricePredictionViewModel PricePrediction
        {
            get => this.pricePredicition;
            set => this.RaiseAndSetIfChanged(ref pricePredicition, value);
        }

        public ReactiveCommand<Unit, Unit> ExecuteAdvancedQueryCommand { get; }

        internal Item Item { get; set; }

        public async Task SetListingForItemUnderCursorAsync(CancellationToken cancellationToken = default)
        {
            var oldItem = this.Item;
            try
            {
                this.IsBusy = true;
                this.Message = null;

                this.Item = await this.mediator.Send(new GetItemFromCursorQuery(), cancellationToken).ConfigureAwait(true);
                IQueryRequest queryRequest = this.queryRequestFactory.Create(this.Item);
                ItemListingsQueryResult itemListing = await this.poeTradeApiClient.GetListingsAsync(queryRequest, cancellationToken).ConfigureAwait(true);

                if (itemListing != null)
                {
                    this.ItemListings = await this.itemListingsViewModelFactory.CreateAsync(Item, itemListing, cancellationToken).ConfigureAwait(true);
                    this.AdvancedQuery = this.advancedQueryViewModelFactory.Create(Item, itemListing.SearchQueryRequest);
                }
            }
            catch (InvalidItemStringException exception)
            {
                this.Log().Error(exception); 
                
            }
            catch (Exception exception)
            {
                if (exception is not OperationCanceledException and not TaskCanceledException)
                {
                    this.HandleException(exception);
                }
            }
            finally
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    this.IsBusy = false;
                }
            }
            
            if (this.itemSearchOptions.CurrentValue.PricePredictionEnabled && !string.Equals(oldItem?.ItemText, this.Item?.ItemText, StringComparison.Ordinal))
            {
                await GetPricePrediction(cancellationToken).ConfigureAwait(true);
            }
        }

        private async Task GetPricePrediction(CancellationToken cancellationToken)
        {
            try
            {
                this.PricePrediction = new PricePredictionViewModel();
                this.PricePrediction = await this.mediator.Send(new GetPricePredictionViewModelQuery(this.Item), cancellationToken);
            }
            catch (Exception exception)
            {
                this.Log().Error(exception);
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

        internal async Task ExecuteAdvancedQueryAsync()
        {
            try
            {
                this.Message = null;

                var queryRequest = this.queryRequestFactory.Create(this.AdvancedQuery);
                this.AdvancedQuery = this.advancedQueryViewModelFactory.Create(this.Item, queryRequest);

                ItemListingsQueryResult itemListingsQueryResult = await this.poeTradeApiClient.GetListingsAsync(queryRequest).ConfigureAwait(true);
                this.ItemListings = await this.itemListingsViewModelFactory.CreateAsync(this.Item, itemListingsQueryResult).ConfigureAwait(true);
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
        }
    }
}