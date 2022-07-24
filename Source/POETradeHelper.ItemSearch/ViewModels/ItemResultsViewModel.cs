using System;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services.Factories;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Services;
using POETradeHelper.PricePrediction.UI.Avalonia.ViewModels;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace POETradeHelper.ItemSearch.ViewModels;

public class ItemResultsViewModel : ReactiveObject, IItemResultsViewModel
{
    private readonly IItemSearchResultOverlayViewModel itemSearchResultOverlayViewModel;
    private readonly ISearchQueryRequestFactory searchQueryRequestFactory;
    private readonly IItemListingsViewModelFactory itemListingsViewModelFactory;
    private readonly IPoeTradeApiClient poeTradeApiClient;

    public ItemResultsViewModel(
        IItemSearchResultOverlayViewModel itemSearchResultOverlayViewModel,
        ISearchQueryRequestFactory searchQueryRequestFactory,
        IItemListingsViewModelFactory itemListingsViewModelFactory,
        IPoeTradeApiClient poeTradeApiClient,
        IPricePredictionViewModel pricePrediction,
        IAdvancedFiltersViewModel advancedFiltersViewModel)
    {
        this.itemSearchResultOverlayViewModel = itemSearchResultOverlayViewModel;
        this.searchQueryRequestFactory = searchQueryRequestFactory;
        this.itemListingsViewModelFactory = itemListingsViewModelFactory;
        this.poeTradeApiClient = poeTradeApiClient;
        this.PricePrediction = pricePrediction;
        this.AdvancedFilters = advancedFiltersViewModel;

        this.ExecuteAdvancedQueryCommand = ReactiveCommand.CreateFromTask(this.ExecuteAdvancedQueryAsync);
    }
    
    public string UrlPathSegment => "item_search";

    public IScreen HostScreen => this.itemSearchResultOverlayViewModel;
    
    private Item Item { get; set; }

    public IPricePredictionViewModel PricePrediction { get; }
    
    public IAdvancedFiltersViewModel AdvancedFilters { get; }
    
    public ReactiveCommand<Unit, Unit> ExecuteAdvancedQueryCommand { get; }

    [Reactive]
    public SearchQueryRequest QueryRequest { get; set; }

    [Reactive]
    public ItemListingsViewModel ItemListings { get; private set; }

    public async Task InitializeAsync(Item item, CancellationToken cancellationToken)
    {
        this.Item = item;
        this.QueryRequest = this.searchQueryRequestFactory.Create(this.Item);
        ItemListingsQueryResult itemListing = await this.poeTradeApiClient.GetListingsAsync(this.QueryRequest, cancellationToken);
        this.ItemListings = await this.itemListingsViewModelFactory.CreateAsync(this.Item, itemListing, cancellationToken);
        await this.AdvancedFilters.LoadAsync(this.Item, this.QueryRequest, cancellationToken);
        
        _ = this.PricePrediction.LoadAsync(this.Item, cancellationToken);
    }

    private async Task ExecuteAdvancedQueryAsync()
    {
        try
        {
            this.QueryRequest = this.searchQueryRequestFactory.Create(this.QueryRequest, this.AdvancedFilters);
            await this.AdvancedFilters.LoadAsync(this.Item, this.QueryRequest, default);

            ItemListingsQueryResult itemListingsQueryResult = await this.poeTradeApiClient.GetListingsAsync(this.QueryRequest);
            this.ItemListings = await this.itemListingsViewModelFactory.CreateAsync(this.Item, itemListingsQueryResult);
        }
        catch (Exception exception)
        {
            this.itemSearchResultOverlayViewModel.HandleException(exception);
        }
    }
}