using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services.Mappers;
using POETradeHelper.ItemSearch.UI.Avalonia.Factories;
using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Services;

using ReactiveUI;

namespace POETradeHelper.ItemSearch.Tests.ViewModels;

public class ExchangeResultsViewModelTests
{
    private Mock<IPoeTradeApiClient> poeTradeApiClientMock;
    private Mock<IItemToExchangeQueryRequestMapper> itemToExchangeQueryRequestMapperMock;
    private Mock<IItemListingsViewModelFactory> itemListingsViewModelFactoryMock;
    private ExchangeResultsViewModel viewModel;

    [SetUp]
    public void Setup()
    {
        this.poeTradeApiClientMock = new Mock<IPoeTradeApiClient>();
        this.itemToExchangeQueryRequestMapperMock = new Mock<IItemToExchangeQueryRequestMapper>();
        this.itemListingsViewModelFactoryMock = new Mock<IItemListingsViewModelFactory>();
        this.viewModel = new ExchangeResultsViewModel(
            Mock.Of<IScreen>(),
            this.poeTradeApiClientMock.Object,
            this.itemToExchangeQueryRequestMapperMock.Object,
            this.itemListingsViewModelFactoryMock.Object);
    }

    [Test]
    public async Task InitializeCallsMapToQueryRequestOnItemToExchangeQueryRequestMapper()
    {
        CurrencyItem item = new();

        await this.viewModel.InitializeAsync(item, default);

        this.itemToExchangeQueryRequestMapperMock
            .Verify(x => x.MapToQueryRequest(item));
    }

    [Test]
    public async Task InitializeCallsGetListingsAsyncOnPoeTradeApiClient()
    {
        ExchangeQueryRequest expectedRequest = new() { Query = { Have = { "exalted" } } };
        this.itemToExchangeQueryRequestMapperMock
            .Setup(x => x.MapToQueryRequest(It.IsAny<Item>()))
            .Returns(expectedRequest);
        CancellationTokenSource cts = new();

        await this.viewModel.InitializeAsync(new CurrencyItem(), cts.Token);

        this.poeTradeApiClientMock
            .Verify(x => x.GetListingsAsync(expectedRequest, cts.Token));
    }

    [Test]
    public async Task InitializeCallsCreateAsyncOnItemListingsViewModelFactory()
    {
        ExchangeQueryResult expectedQueryResult = new("a", 1, new Dictionary<string, ExchangeQueryResultListing>());
        this.poeTradeApiClientMock
            .Setup(x => x.GetListingsAsync(It.IsAny<ExchangeQueryRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedQueryResult);

        CancellationTokenSource cts = new();

        await this.viewModel.InitializeAsync(new CurrencyItem(), cts.Token);

        this.itemListingsViewModelFactoryMock
            .Verify(x => x.CreateAsync(expectedQueryResult, cts.Token));
    }

    [Test]
    public async Task InitializeSetsItemListings()
    {
        ItemListingsViewModel expected = new() { ListingsUri = new Uri("https://exchange.results") };
        this.itemListingsViewModelFactoryMock
            .Setup(x => x.CreateAsync(It.IsAny<ExchangeQueryResult>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        await this.viewModel.InitializeAsync(new CurrencyItem(), default);

        this.viewModel.ItemListings.Should().Be(expected);
    }
}