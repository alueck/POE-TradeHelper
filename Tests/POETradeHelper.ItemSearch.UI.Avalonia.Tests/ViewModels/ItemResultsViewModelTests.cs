using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using NSubstitute;
using NSubstitute.ExceptionExtensions;

using NUnit.Framework;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.UI.Avalonia.Factories;
using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;
using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels.Abstractions;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Services;
using POETradeHelper.PricePrediction.UI.Avalonia.ViewModels;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Tests.ViewModels;

public class ItemResultsViewModelTests
{
    private readonly IItemSearchResultOverlayViewModel itemSearchResultsOverlayViewModelMock;
    private readonly ISearchQueryRequestFactory searchQueryRequestFactoryMock;
    private readonly IItemListingsViewModelFactory itemListingsViewModelFactoryMock;
    private readonly IPoeTradeApiClient poeTradeApiClientMock;
    private readonly IPricePredictionViewModel pricePredictionViewModelMock;
    private readonly IAdvancedFiltersViewModel advancedFiltersViewModelMock;
    private readonly ItemResultsViewModel viewModel;

    public ItemResultsViewModelTests()
    {
        this.itemSearchResultsOverlayViewModelMock = Substitute.For<IItemSearchResultOverlayViewModel>();
        this.searchQueryRequestFactoryMock = Substitute.For<ISearchQueryRequestFactory>();
        this.itemListingsViewModelFactoryMock = Substitute.For<IItemListingsViewModelFactory>();
        this.poeTradeApiClientMock = Substitute.For<IPoeTradeApiClient>();
        this.pricePredictionViewModelMock = Substitute.For<IPricePredictionViewModel>();
        this.advancedFiltersViewModelMock = Substitute.For<IAdvancedFiltersViewModel>();
        this.viewModel = new ItemResultsViewModel(this.itemSearchResultsOverlayViewModelMock, this.searchQueryRequestFactoryMock, this.itemListingsViewModelFactoryMock, this.poeTradeApiClientMock, this.pricePredictionViewModelMock, this.advancedFiltersViewModelMock);
    }

    [Test]
    public async Task InitializeCallsCreateOnSearchQueryRequestFactory()
    {
        EquippableItem item = new(ItemRarity.Rare);

        await this.viewModel.InitializeAsync(item, default);

        this.searchQueryRequestFactoryMock
            .Received()
            .Create(item);
    }

    [Test]
    public async Task InitializeSetsQueryRequest()
    {
        EquippableItem item = new(ItemRarity.Rare);
        SearchQueryRequest expected = new() { League = "Test" };
        this.searchQueryRequestFactoryMock
            .Create(Arg.Any<Item>())
            .Returns(expected);

        await this.viewModel.InitializeAsync(item, default);

        this.viewModel.QueryRequest.Should().Be(expected);
    }

    [Test]
    public async Task InitializeCallsGetListingsAsyncOnPoeTradeApiClient()
    {
        EquippableItem item = new(ItemRarity.Rare);
        SearchQueryRequest expected = new() { League = "Test" };
        this.searchQueryRequestFactoryMock
            .Create(Arg.Any<Item>())
            .Returns(expected);
        CancellationTokenSource cts = new();

        await this.viewModel.InitializeAsync(item, cts.Token);

        await this.poeTradeApiClientMock
            .Received()
            .GetListingsAsync(expected, cts.Token);
    }

    [Test]
    public async Task InitializeCallsCreateAsyncOnItemListingsViewModelFactory()
    {
        EquippableItem item = new(ItemRarity.Rare);
        ItemListingsQueryResult result = new() { TotalCount = 1 };
        this.poeTradeApiClientMock
            .GetListingsAsync(Arg.Any<SearchQueryRequest>(), Arg.Any<CancellationToken>())
            .Returns(result);
        CancellationTokenSource cts = new();

        await this.viewModel.InitializeAsync(item, cts.Token);

        await this.itemListingsViewModelFactoryMock
            .Received()
            .CreateAsync(item, result, cts.Token);
    }

    [Test]
    public async Task InitializeSetsItemListings()
    {
        EquippableItem item = new(ItemRarity.Rare);
        ItemListingsViewModel expected = new() { ListingsUri = new Uri("https://test.uri") };
        this.itemListingsViewModelFactoryMock
            .CreateAsync(Arg.Any<Item>(), Arg.Any<ItemListingsQueryResult>(), Arg.Any<CancellationToken>())
            .Returns(expected);

        await this.viewModel.InitializeAsync(item, default);

        this.viewModel.ItemListings.Should().Be(expected);
    }

    [Test]
    public async Task InitializeCallsLoadAsyncOnAdvancedFiltersViewModel()
    {
        EquippableItem item = new(ItemRarity.Rare);
        SearchQueryRequest queryRequest = new() { League = "Test" };
        this.searchQueryRequestFactoryMock
            .Create(Arg.Any<Item>())
            .Returns(queryRequest);
        CancellationTokenSource cts = new();

        await this.viewModel.InitializeAsync(item, cts.Token);

        await this.advancedFiltersViewModelMock
            .Received()
            .LoadAsync(item, queryRequest, cts.Token);
    }

    [Test]
    public async Task InitializeCallsLoadAsyncOnPricePredictionViewModel()
    {
        EquippableItem item = new(ItemRarity.Rare);
        CancellationTokenSource cts = new();

        await this.viewModel.InitializeAsync(item, cts.Token);

        await this.pricePredictionViewModelMock
            .Received()
            .LoadAsync(item, cts.Token);
    }

    [Test]
    public async Task ExecuteAdvancedQueryCommandCallsCreateOnSearchQueryRequestFactory()
    {
        SearchQueryRequest originalQueryRequest = new() { League = "test" };
        this.viewModel.QueryRequest = originalQueryRequest;

        await this.viewModel.ExecuteAdvancedQueryCommand.Execute();

        this.searchQueryRequestFactoryMock
            .Received()
            .Create(originalQueryRequest, this.advancedFiltersViewModelMock);
    }

    [Test]
    public async Task ExecuteAdvancedQueryCommandSetsQueryRequest()
    {
        SearchQueryRequest expected = new() { League = "abc" };
        this.searchQueryRequestFactoryMock
            .Create(Arg.Any<SearchQueryRequest>(), Arg.Any<IAdvancedFiltersViewModel>())
            .Returns(expected);

        await this.viewModel.ExecuteAdvancedQueryCommand.Execute();

        this.viewModel.QueryRequest.Should().Be(expected);
    }

    [Test]
    public async Task ExecuteAdvancedQueryCommandCallsLoadAsyncOnAdvancedFiltersViewModel()
    {
        EquippableItem item = new(ItemRarity.Magic);
        SearchQueryRequest queryRequest = new() { League = "abc" };
        this.searchQueryRequestFactoryMock
            .Create(Arg.Any<SearchQueryRequest>(), Arg.Any<IAdvancedFiltersViewModel>())
            .Returns(queryRequest);
        await this.viewModel.InitializeAsync(item, default);

        await this.viewModel.ExecuteAdvancedQueryCommand.Execute();

        await this.advancedFiltersViewModelMock
            .Received()
            .LoadAsync(item, queryRequest, default);
    }

    [Test]
    public async Task ExecuteAdvancedQueryCommandCallsGetListingsAsyncOnPoeTradeApiClient()
    {
        EquippableItem item = new(ItemRarity.Rare);
        SearchQueryRequest queryRequest = new() { League = "Test" };
        this.searchQueryRequestFactoryMock
            .Create(Arg.Any<SearchQueryRequest>(), Arg.Any<IAdvancedFiltersViewModel>())
            .Returns(queryRequest);
        await this.viewModel.InitializeAsync(item, default);

        await this.viewModel.ExecuteAdvancedQueryCommand.Execute();

        await this.poeTradeApiClientMock
            .Received()
            .GetListingsAsync(queryRequest);
    }

    [Test]
    public async Task ExecuteAdvancedQueryCommandCallsCreateAsyncOnItemListingsViewModelFactory()
    {
        EquippableItem item = new(ItemRarity.Rare);
        ItemListingsQueryResult result = new() { TotalCount = 1 };
        this.poeTradeApiClientMock
            .GetListingsAsync(Arg.Any<SearchQueryRequest>(), Arg.Any<CancellationToken>())
            .Returns(new ItemListingsQueryResult(), result);
        await this.viewModel.InitializeAsync(item, default);

        await this.viewModel.ExecuteAdvancedQueryCommand.Execute();

        await this.itemListingsViewModelFactoryMock
            .Received()
            .CreateAsync(item, result);
    }

    [Test]
    public async Task ExecuteAdvancedQueryCommandSetsItemListings()
    {
        EquippableItem item = new(ItemRarity.Rare);
        ItemListingsViewModel expected = new() { ListingsUri = new Uri("https://test.test") };
        this.itemListingsViewModelFactoryMock
            .CreateAsync(Arg.Any<Item>(), Arg.Any<ItemListingsQueryResult>(), Arg.Any<CancellationToken>())
            .Returns(new ItemListingsViewModel(), expected);
        await this.viewModel.InitializeAsync(item, default);

        await this.viewModel.ExecuteAdvancedQueryCommand.Execute();

        this.viewModel.ItemListings.Should().Be(expected);
    }

    [Test]
    public async Task ExecuteAdvancedQueryCommandCallsHandleExceptionOnOverlayViewModelOnException()
    {
        Exception exception = new();
        this.searchQueryRequestFactoryMock
            .Create(Arg.Any<SearchQueryRequest>(), Arg.Any<IAdvancedFiltersViewModel>())
            .Throws(exception);

        await this.viewModel.ExecuteAdvancedQueryCommand.Execute();

        this.itemSearchResultsOverlayViewModelMock
            .Received()
            .HandleException(exception);
    }

    [Test]
    public async Task LoadNextPageCommandAddsNewPageItemsToListings()
    {
        // arrange
        EquippableItem item = new(ItemRarity.Rare);
        ItemListingsQueryResult previousListingResult = new() { TotalCount = 100, CurrentPage = 2 };
        this.poeTradeApiClientMock
            .GetListingsAsync(Arg.Any<SearchQueryRequest>(), Arg.Any<CancellationToken>())
            .Returns(previousListingResult);
        this.itemListingsViewModelFactoryMock
            .CreateAsync(Arg.Any<Item>(), previousListingResult, Arg.Any<CancellationToken>())
            .Returns(new ItemListingsViewModel());

        ItemListingsQueryResult nextPageResult = new() { Uri = new Uri("http://test.com") };
        this.poeTradeApiClientMock
            .LoadNextPage(previousListingResult, Arg.Any<CancellationToken>())
            .Returns(nextPageResult);

        SimpleListingViewModel[] nextPageListings = [new() { AccountName = "Test" }, new() { AccountName = "Test2" }];
        this.itemListingsViewModelFactoryMock
            .CreateAsync(Arg.Any<Item>(), nextPageResult, Arg.Any<CancellationToken>())
            .Returns(new ItemListingsViewModel { Listings = { nextPageListings } });

        await this.viewModel.InitializeAsync(item, default);

        // act
        await this.viewModel.LoadNextPageCommand.Execute();

        // assert
        this.viewModel.ItemListings!.Listings.Should().ContainEquivalentOf(nextPageListings);
    }
}