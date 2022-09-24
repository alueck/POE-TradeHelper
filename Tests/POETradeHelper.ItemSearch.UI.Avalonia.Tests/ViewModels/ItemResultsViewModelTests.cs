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

namespace POETradeHelper.ItemSearch.UI.Avalonia.Tests.ViewModels;

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
        itemSearchResultsOverlayViewModelMock = new Mock<IItemSearchResultOverlayViewModel>();
        searchQueryRequestFactoryMock = new Mock<ISearchQueryRequestFactory>();
        itemListingsViewModelFactoryMock = new Mock<IItemListingsViewModelFactory>();
        poeTradeApiClientMock = new Mock<IPoeTradeApiClient>();
        pricePredictionViewModelMock = new Mock<IPricePredictionViewModel>();
        advancedFiltersViewModelMock = new Mock<IAdvancedFiltersViewModel>();
        viewModel = new ItemResultsViewModel(
            itemSearchResultsOverlayViewModelMock.Object,
            searchQueryRequestFactoryMock.Object,
            itemListingsViewModelFactoryMock.Object,
            poeTradeApiClientMock.Object,
            pricePredictionViewModelMock.Object,
            advancedFiltersViewModelMock.Object);
    }

    [Test]
    public async Task InitializeCallsCreateOnSearchQueryRequestFactory()
    {
        EquippableItem item = new(ItemRarity.Rare);

        await viewModel.InitializeAsync(item, default);

        searchQueryRequestFactoryMock
            .Verify(x => x.Create(item));
    }

    [Test]
    public async Task InitializeSetsQueryRequest()
    {
        EquippableItem item = new(ItemRarity.Rare);
        SearchQueryRequest expected = new() { League = "Test" };
        searchQueryRequestFactoryMock
            .Setup(x => x.Create(It.IsAny<Item>()))
            .Returns(expected);

        await viewModel.InitializeAsync(item, default);

        viewModel.QueryRequest.Should().Be(expected);
    }

    [Test]
    public async Task InitializeCallsGetListingsAsyncOnPoeTradeApiClient()
    {
        EquippableItem item = new(ItemRarity.Rare);
        SearchQueryRequest expected = new() { League = "Test" };
        searchQueryRequestFactoryMock
            .Setup(x => x.Create(It.IsAny<Item>()))
            .Returns(expected);
        CancellationTokenSource cts = new();

        await viewModel.InitializeAsync(item, cts.Token);

        poeTradeApiClientMock
            .Verify(x => x.GetListingsAsync(expected, cts.Token));
    }

    [Test]
    public async Task InitializeCallsCreateAsyncOnItemListingsViewModelFactory()
    {
        EquippableItem item = new(ItemRarity.Rare);
        ItemListingsQueryResult result = new() { TotalCount = 1 };
        poeTradeApiClientMock
            .Setup(x => x.GetListingsAsync(It.IsAny<SearchQueryRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);
        CancellationTokenSource cts = new();

        await viewModel.InitializeAsync(item, cts.Token);

        itemListingsViewModelFactoryMock
            .Verify(x => x.CreateAsync(item, result, cts.Token));
    }

    [Test]
    public async Task InitializeSetsItemListings()
    {
        EquippableItem item = new(ItemRarity.Rare);
        ItemListingsViewModel expected = new() { ListingsUri = new Uri("https://test.uri") };
        itemListingsViewModelFactoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Item>(), It.IsAny<ItemListingsQueryResult>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        await viewModel.InitializeAsync(item, default);

        viewModel.ItemListings.Should().Be(expected);
    }

    [Test]
    public async Task InitializeCallsLoadAsyncOnAdvancedFiltersViewModel()
    {
        EquippableItem item = new(ItemRarity.Rare);
        SearchQueryRequest queryRequest = new() { League = "Test" };
        searchQueryRequestFactoryMock
            .Setup(x => x.Create(It.IsAny<Item>()))
            .Returns(queryRequest);
        CancellationTokenSource cts = new();

        await viewModel.InitializeAsync(item, cts.Token);

        advancedFiltersViewModelMock
            .Verify(x => x.LoadAsync(item, queryRequest, cts.Token));
    }

    [Test]
    public async Task InitializeCallsLoadAsyncOnPricePredictionViewModel()
    {
        EquippableItem item = new(ItemRarity.Rare);
        CancellationTokenSource cts = new();

        await viewModel.InitializeAsync(item, cts.Token);

        pricePredictionViewModelMock
            .Verify(x => x.LoadAsync(item, cts.Token));
    }

    [Test]
    public async Task ExecuteAdvancedQueryCommandCallsCreateOnSearchQueryRequestFactory()
    {
        SearchQueryRequest originalQueryRequest = new() { League = "test" };
        viewModel.QueryRequest = originalQueryRequest;

        await viewModel.ExecuteAdvancedQueryCommand.Execute();

        searchQueryRequestFactoryMock
            .Verify(x => x.Create(originalQueryRequest, advancedFiltersViewModelMock.Object));
    }

    [Test]
    public async Task ExecuteAdvancedQueryCommandSetsQueryRequest()
    {
        SearchQueryRequest expected = new() { League = "abc" };
        searchQueryRequestFactoryMock
            .Setup(x => x.Create(It.IsAny<SearchQueryRequest>(), It.IsAny<IAdvancedFiltersViewModel>()))
            .Returns(expected);

        await viewModel.ExecuteAdvancedQueryCommand.Execute();

        viewModel.QueryRequest.Should().Be(expected);
    }

    [Test]
    public async Task ExecuteAdvancedQueryCommandCallsLoadAsyncOnAdvancedFiltersViewModel()
    {
        EquippableItem item = new(ItemRarity.Magic);
        SearchQueryRequest queryRequest = new() { League = "abc" };
        searchQueryRequestFactoryMock
            .SetupSequence(x => x.Create(It.IsAny<SearchQueryRequest>(), It.IsAny<IAdvancedFiltersViewModel>()))
            .Returns(queryRequest);
        await viewModel.InitializeAsync(item, default);

        await viewModel.ExecuteAdvancedQueryCommand.Execute();

        advancedFiltersViewModelMock
            .Verify(x => x.LoadAsync(item, queryRequest, default));
    }

    [Test]
    public async Task ExecuteAdvancedQueryCommandCallsGetListingsAsyncOnPoeTradeApiClient()
    {
        EquippableItem item = new(ItemRarity.Rare);
        SearchQueryRequest queryRequest = new() { League = "Test" };
        searchQueryRequestFactoryMock
            .SetupSequence(x => x.Create(It.IsAny<SearchQueryRequest>(), It.IsAny<IAdvancedFiltersViewModel>()))
            .Returns(queryRequest);
        await viewModel.InitializeAsync(item, default);

        await viewModel.ExecuteAdvancedQueryCommand.Execute();

        poeTradeApiClientMock
            .Verify(x => x.GetListingsAsync(queryRequest, default));
    }

    [Test]
    public async Task ExecuteAdvancedQueryCommandCallsCreateAsyncOnItemListingsViewModelFactory()
    {
        EquippableItem item = new(ItemRarity.Rare);
        ItemListingsQueryResult result = new() { TotalCount = 1 };
        poeTradeApiClientMock
            .SetupSequence(x => x.GetListingsAsync(It.IsAny<SearchQueryRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ItemListingsQueryResult)null)
            .ReturnsAsync(result);
        await viewModel.InitializeAsync(item, default);

        await viewModel.ExecuteAdvancedQueryCommand.Execute();

        itemListingsViewModelFactoryMock
            .Verify(x => x.CreateAsync(item, result, default));
    }

    [Test]
    public async Task ExecuteAdvancedQueryCommandSetsItemListings()
    {
        EquippableItem item = new(ItemRarity.Rare);
        ItemListingsViewModel expected = new() { ListingsUri = new Uri("https://test.test") };
        itemListingsViewModelFactoryMock
            .SetupSequence(x => x.CreateAsync(It.IsAny<Item>(), It.IsAny<ItemListingsQueryResult>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ItemListingsViewModel)null)
            .ReturnsAsync(expected);
        await viewModel.InitializeAsync(item, default);

        await viewModel.ExecuteAdvancedQueryCommand.Execute();

        viewModel.ItemListings.Should().Be(expected);
    }

    [Test]
    public async Task ExecuteAdvancedQueryCommandCallsHandleExceptionOnOverlayViewModelOnException()
    {
        EquippableItem item = new(ItemRarity.Rare);
        Exception exception = new();
        searchQueryRequestFactoryMock
            .Setup(x => x.Create(It.IsAny<SearchQueryRequest>(), It.IsAny<IAdvancedFiltersViewModel>()))
            .Throws(exception);

        await viewModel.ExecuteAdvancedQueryCommand.Execute();

        itemSearchResultsOverlayViewModelMock
            .Verify(x => x.HandleException(exception));
    }
}