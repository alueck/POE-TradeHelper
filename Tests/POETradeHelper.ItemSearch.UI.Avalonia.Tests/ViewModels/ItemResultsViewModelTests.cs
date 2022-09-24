using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.UI.Avalonia.Factories;
using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;
using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels.Abstractions;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Services;
using POETradeHelper.PricePrediction.UI.Avalonia.ViewModels;

namespace POETradeHelper.ItemSearch.Tests.ViewModels;

public class ItemResultsViewModelTests
{
    private Mock<IItemSearchResultOverlayViewModel> itemSearchResultsOverlayViewModelMock;
    private Mock<ISearchQueryRequestFactory> searchQueryRequestFactoryMock;
    private Mock<IItemListingsViewModelFactory> itemListingsViewModelFactoryMock;
    private Mock<IPoeTradeApiClient> poeTradeApiClientMock;
    private Mock<IPricePredictionViewModel> pricePredictionViewModelMock;
    private Mock<IAdvancedFiltersViewModel> advancedFiltersViewModelMock;
    private ItemResultsViewModel viewModel;

    [SetUp]
    public void Setup()
    {
        this.itemSearchResultsOverlayViewModelMock = new Mock<IItemSearchResultOverlayViewModel>();
        this.searchQueryRequestFactoryMock = new Mock<ISearchQueryRequestFactory>();
        this.itemListingsViewModelFactoryMock = new Mock<IItemListingsViewModelFactory>();
        this.poeTradeApiClientMock = new Mock<IPoeTradeApiClient>();
        this.pricePredictionViewModelMock = new Mock<IPricePredictionViewModel>();
        this.advancedFiltersViewModelMock = new Mock<IAdvancedFiltersViewModel>();
        this.viewModel = new ItemResultsViewModel(
            this.itemSearchResultsOverlayViewModelMock.Object,
            this.searchQueryRequestFactoryMock.Object,
            this.itemListingsViewModelFactoryMock.Object,
            this.poeTradeApiClientMock.Object,
            this.pricePredictionViewModelMock.Object,
            this.advancedFiltersViewModelMock.Object);
    }

    [Test]
    public async Task InitializeCallsCreateOnSearchQueryRequestFactory()
    {
        EquippableItem item = new(ItemRarity.Rare);

        await this.viewModel.InitializeAsync(item, default);

        this.searchQueryRequestFactoryMock
            .Verify(x => x.Create(item));
    }

    [Test]
    public async Task InitializeSetsQueryRequest()
    {
        EquippableItem item = new(ItemRarity.Rare);
        SearchQueryRequest expected = new() { League = "Test" };
        this.searchQueryRequestFactoryMock
            .Setup(x => x.Create(It.IsAny<Item>()))
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
            .Setup(x => x.Create(It.IsAny<Item>()))
            .Returns(expected);
        CancellationTokenSource cts = new();

        await this.viewModel.InitializeAsync(item, cts.Token);

        this.poeTradeApiClientMock
            .Verify(x => x.GetListingsAsync(expected, cts.Token));
    }

    [Test]
    public async Task InitializeCallsCreateAsyncOnItemListingsViewModelFactory()
    {
        EquippableItem item = new(ItemRarity.Rare);
        ItemListingsQueryResult result = new() { TotalCount = 1 };
        this.poeTradeApiClientMock
            .Setup(x => x.GetListingsAsync(It.IsAny<SearchQueryRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);
        CancellationTokenSource cts = new();

        await this.viewModel.InitializeAsync(item, cts.Token);

        this.itemListingsViewModelFactoryMock
            .Verify(x => x.CreateAsync(item, result, cts.Token));
    }

    [Test]
    public async Task InitializeSetsItemListings()
    {
        EquippableItem item = new(ItemRarity.Rare);
        ItemListingsViewModel expected = new() { ListingsUri = new Uri("https://test.uri") };
        this.itemListingsViewModelFactoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Item>(), It.IsAny<ItemListingsQueryResult>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        await this.viewModel.InitializeAsync(item, default);

        this.viewModel.ItemListings.Should().Be(expected);
    }

    [Test]
    public async Task InitializeCallsLoadAsyncOnAdvancedFiltersViewModel()
    {
        EquippableItem item = new(ItemRarity.Rare);
        SearchQueryRequest queryRequest = new() { League = "Test" };
        this.searchQueryRequestFactoryMock
            .Setup(x => x.Create(It.IsAny<Item>()))
            .Returns(queryRequest);
        CancellationTokenSource cts = new();

        await this.viewModel.InitializeAsync(item, cts.Token);

        this.advancedFiltersViewModelMock
            .Verify(x => x.LoadAsync(item, queryRequest, cts.Token));
    }

    [Test]
    public async Task InitializeCallsLoadAsyncOnPricePredictionViewModel()
    {
        EquippableItem item = new(ItemRarity.Rare);
        CancellationTokenSource cts = new();

        await this.viewModel.InitializeAsync(item, cts.Token);

        this.pricePredictionViewModelMock
            .Verify(x => x.LoadAsync(item, cts.Token));
    }

    [Test]
    public async Task ExecuteAdvancedQueryCommandCallsCreateOnSearchQueryRequestFactory()
    {
        SearchQueryRequest originalQueryRequest = new() { League = "test" };
        this.viewModel.QueryRequest = originalQueryRequest;

        await this.viewModel.ExecuteAdvancedQueryCommand.Execute();

        this.searchQueryRequestFactoryMock
            .Verify(x => x.Create(originalQueryRequest, this.advancedFiltersViewModelMock.Object));
    }

    [Test]
    public async Task ExecuteAdvancedQueryCommandSetsQueryRequest()
    {
        SearchQueryRequest expected = new() { League = "abc" };
        this.searchQueryRequestFactoryMock
            .Setup(x => x.Create(It.IsAny<SearchQueryRequest>(), It.IsAny<IAdvancedFiltersViewModel>()))
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
            .SetupSequence(x => x.Create(It.IsAny<SearchQueryRequest>(), It.IsAny<IAdvancedFiltersViewModel>()))
            .Returns(queryRequest);
        await this.viewModel.InitializeAsync(item, default);

        await this.viewModel.ExecuteAdvancedQueryCommand.Execute();

        this.advancedFiltersViewModelMock
            .Verify(x => x.LoadAsync(item, queryRequest, default));
    }

    [Test]
    public async Task ExecuteAdvancedQueryCommandCallsGetListingsAsyncOnPoeTradeApiClient()
    {
        EquippableItem item = new(ItemRarity.Rare);
        SearchQueryRequest queryRequest = new() { League = "Test" };
        this.searchQueryRequestFactoryMock
            .SetupSequence(x => x.Create(It.IsAny<SearchQueryRequest>(), It.IsAny<IAdvancedFiltersViewModel>()))
            .Returns(queryRequest);
        await this.viewModel.InitializeAsync(item, default);

        await this.viewModel.ExecuteAdvancedQueryCommand.Execute();

        this.poeTradeApiClientMock
            .Verify(x => x.GetListingsAsync(queryRequest, default));
    }

    [Test]
    public async Task ExecuteAdvancedQueryCommandCallsCreateAsyncOnItemListingsViewModelFactory()
    {
        EquippableItem item = new(ItemRarity.Rare);
        ItemListingsQueryResult result = new() { TotalCount = 1 };
        this.poeTradeApiClientMock
            .SetupSequence(x => x.GetListingsAsync(It.IsAny<SearchQueryRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ItemListingsQueryResult)null)
            .ReturnsAsync(result);
        await this.viewModel.InitializeAsync(item, default);

        await this.viewModel.ExecuteAdvancedQueryCommand.Execute();

        this.itemListingsViewModelFactoryMock
            .Verify(x => x.CreateAsync(item, result, default));
    }

    [Test]
    public async Task ExecuteAdvancedQueryCommandSetsItemListings()
    {
        EquippableItem item = new(ItemRarity.Rare);
        ItemListingsViewModel expected = new() { ListingsUri = new Uri("https://test.test") };
        this.itemListingsViewModelFactoryMock
            .SetupSequence(x => x.CreateAsync(It.IsAny<Item>(), It.IsAny<ItemListingsQueryResult>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ItemListingsViewModel)null)
            .ReturnsAsync(expected);
        await this.viewModel.InitializeAsync(item, default);

        await this.viewModel.ExecuteAdvancedQueryCommand.Execute();

        this.viewModel.ItemListings.Should().Be(expected);
    }

    [Test]
    public async Task ExecuteAdvancedQueryCommandCallsHandleExceptionOnOverlayViewModelOnException()
    {
        EquippableItem item = new(ItemRarity.Rare);
        Exception exception = new();
        this.searchQueryRequestFactoryMock
            .Setup(x => x.Create(It.IsAny<SearchQueryRequest>(), It.IsAny<IAdvancedFiltersViewModel>()))
            .Throws(exception);

        await this.viewModel.ExecuteAdvancedQueryCommand.Execute();

        this.itemSearchResultsOverlayViewModelMock
            .Verify(x => x.HandleException(exception));
    }
}