using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using POETradeHelper.Common.UI.Models;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Queries;
using POETradeHelper.ItemSearch.Exceptions;
using POETradeHelper.ItemSearch.Services.Factories;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Services;
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
        private readonly IQueryRequestFactory queryRequestFactory;
        private readonly IMediator mediator;

        public ItemSearchResultOverlayViewModel(
            IPoeTradeApiClient tradeClient,
            IItemListingsViewModelFactory itemListingsViewModelFactory,
            IQueryRequestFactory queryRequestFactory,
            IMediator mediator,
            IPricePredictionViewModel pricePredictionViewModel,
            IAdvancedFiltersViewModel advancedFiltersViewModel)
        {
            this.poeTradeApiClient = tradeClient;
            this.itemListingsViewModelFactory = itemListingsViewModelFactory;
            this.queryRequestFactory = queryRequestFactory;
            this.mediator = mediator;
            this.PricePrediction = pricePredictionViewModel;
            this.AdvancedFilters = advancedFiltersViewModel;
            
            this.ExecuteAdvancedQueryCommand = ReactiveCommand.CreateFromTask(this.ExecuteAdvancedQueryAsync);
            this.ExecuteAdvancedQueryCommand.IsExecuting.ToProperty(this, x => x.IsBusy);
        }

        private ItemListingsViewModel itemListing;
        
        public ItemListingsViewModel ItemListings
        {
            get => this.itemListing;
            set => this.RaiseAndSetIfChanged(ref itemListing, value);
        }

        private IAdvancedFiltersViewModel advancedFilters;

        public IAdvancedFiltersViewModel AdvancedFilters
        {
            get => this.advancedFilters;
            set => this.RaiseAndSetIfChanged(ref advancedFilters, value);
        }

        private Message message;
        
        public Message Message
        {
            get => this.message;
            set => this.RaiseAndSetIfChanged(ref message, value);
        }

        private bool isBusy;

        public bool IsBusy
        {
            get => isBusy;
            set => this.RaiseAndSetIfChanged(ref this.isBusy, value);
        }

        public IPricePredictionViewModel PricePrediction { get; set; }

        public ReactiveCommand<Unit, Unit> ExecuteAdvancedQueryCommand { get; }

        internal Item Item { get; set; }

        internal IQueryRequest QueryRequest { get; set; }

        public async Task SetListingForItemUnderCursorAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                this.IsBusy = true;
                this.Message = null;

                this.Item = await this.mediator.Send(new GetItemFromCursorQuery(), cancellationToken).ConfigureAwait(true);
                this.QueryRequest = this.queryRequestFactory.Create(this.Item);
                ItemListingsQueryResult itemListing = await this.poeTradeApiClient.GetListingsAsync(this.QueryRequest, cancellationToken).ConfigureAwait(true);

                if (itemListing != null)
                {
                    this.ItemListings = await this.itemListingsViewModelFactory.CreateAsync(Item, itemListing, cancellationToken).ConfigureAwait(true);
                    await this.AdvancedFilters.LoadAsync(this.Item, this.QueryRequest, cancellationToken);
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

            await this.PricePrediction.LoadAsync(this.Item, cancellationToken);
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

                this.QueryRequest = this.queryRequestFactory.Create(this.QueryRequest, this.AdvancedFilters);
                await this.AdvancedFilters.LoadAsync(this.Item, this.QueryRequest, default);

                ItemListingsQueryResult itemListingsQueryResult = await this.poeTradeApiClient.GetListingsAsync(this.QueryRequest).ConfigureAwait(true);
                this.ItemListings = await this.itemListingsViewModelFactory.CreateAsync(this.Item, itemListingsQueryResult).ConfigureAwait(true);
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
        }
    }
}