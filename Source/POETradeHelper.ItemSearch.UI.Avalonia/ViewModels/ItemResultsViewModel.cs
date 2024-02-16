using System;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using DotNext;
using DynamicData;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.UI.Avalonia.Factories;
using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels.Abstractions;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Services;
using POETradeHelper.PricePrediction.UI.Avalonia.ViewModels;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;

public class ItemResultsViewModel : ReactiveObject, IItemResultsViewModel
{
    private readonly IItemSearchResultOverlayViewModel itemSearchResultOverlayViewModel;
    private readonly ISearchQueryRequestFactory searchQueryRequestFactory;
    private readonly IItemListingsViewModelFactory itemListingsViewModelFactory;
    private readonly IPoeTradeApiClient poeTradeApiClient;

    private ItemListingsQueryResult? lastItemListingResult;

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
        this.LoadNextPageCommand = ReactiveCommand.CreateFromTask(this.LoadNextPage);
    }

    public string UrlPathSegment => "item_search";

    public IScreen HostScreen => this.itemSearchResultOverlayViewModel;

    public IPricePredictionViewModel PricePrediction { get; }

    public IAdvancedFiltersViewModel AdvancedFilters { get; }

    public ReactiveCommand<Unit, Unit> ExecuteAdvancedQueryCommand { get; }

    public ReactiveCommand<Unit, Unit> LoadNextPageCommand { get; }

    [Reactive]
    public SearchQueryRequest? QueryRequest { get; set; }

    [Reactive]
    public ItemListingsViewModel? ItemListings { get; private set; }

    private Item? Item { get; set; }

    public async Task InitializeAsync(Item? item, CancellationToken cancellationToken)
    {
        this.Item = item;

        if (this.Item != null)
        {
            this.lastItemListingResult = null;

            this.QueryRequest = this.searchQueryRequestFactory.Create(this.Item);
            this.lastItemListingResult = await this.poeTradeApiClient.GetListingsAsync(this.QueryRequest, cancellationToken);
            this.ItemListings = await this.itemListingsViewModelFactory.CreateAsync(this.Item, this.lastItemListingResult, cancellationToken);
            await this.AdvancedFilters.LoadAsync(this.Item, this.QueryRequest, cancellationToken);

            _ = this.PricePrediction.LoadAsync(this.Item, cancellationToken);
        }
    }

    private async Task ExecuteAdvancedQueryAsync()
    {
        try
        {
            this.lastItemListingResult = null;
            this.QueryRequest = this.searchQueryRequestFactory.Create(this.QueryRequest!, this.AdvancedFilters);
            await this.AdvancedFilters.LoadAsync(this.Item!, this.QueryRequest, default);

            this.lastItemListingResult = await this.poeTradeApiClient.GetListingsAsync(this.QueryRequest);
            this.ItemListings = await this.itemListingsViewModelFactory.CreateAsync(this.Item!, this.lastItemListingResult);
        }
        catch (Exception exception)
        {
            this.itemSearchResultOverlayViewModel.HandleException(exception);
        }
    }

    private async Task LoadNextPage()
    {
        if (this.lastItemListingResult == null || this.Item == null || this.ItemListings == null)
        {
            return;
        }

        Optional<ItemListingsQueryResult> itemListingsQueryResult = await this.poeTradeApiClient.LoadNextPage(this.lastItemListingResult);

        if (itemListingsQueryResult.HasValue)
        {
            this.lastItemListingResult = itemListingsQueryResult.Value;
            ItemListingsViewModel itemListingsViewModel = await this.itemListingsViewModelFactory.CreateAsync(this.Item, itemListingsQueryResult.Value);
            this.ItemListings.Listings.AddRange(itemListingsViewModel.Listings);
        }
    }
}