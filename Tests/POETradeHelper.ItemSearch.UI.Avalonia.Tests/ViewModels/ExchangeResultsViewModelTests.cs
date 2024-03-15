using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using NSubstitute;

using NUnit.Framework;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services.Mappers;
using POETradeHelper.ItemSearch.UI.Avalonia.Factories;
using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Services;

using ReactiveUI;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Tests.ViewModels;

public class ExchangeResultsViewModelTests
{
    private readonly IPoeTradeApiClient poeTradeApiClientMock;
    private readonly IItemToExchangeQueryRequestMapper itemToExchangeQueryRequestMapperMock;
    private readonly IItemListingsViewModelFactory itemListingsViewModelFactoryMock;
    private readonly ExchangeResultsViewModel viewModel;

    public ExchangeResultsViewModelTests()
    {
        this.poeTradeApiClientMock = Substitute.For<IPoeTradeApiClient>();
        this.itemToExchangeQueryRequestMapperMock = Substitute.For<IItemToExchangeQueryRequestMapper>();
        this.itemListingsViewModelFactoryMock = Substitute.For<IItemListingsViewModelFactory>();
        this.viewModel = new ExchangeResultsViewModel(
            Substitute.For<IScreen>(), this.poeTradeApiClientMock, this.itemToExchangeQueryRequestMapperMock, this.itemListingsViewModelFactoryMock);
    }

    [Test]
    public async Task InitializeCallsMapToQueryRequestOnItemToExchangeQueryRequestMapper()
    {
        CurrencyItem item = new();

        await this.viewModel.InitializeAsync(item, default);

        this.itemToExchangeQueryRequestMapperMock
            .Received()
            .MapToQueryRequest(item);
    }

    [Test]
    public async Task InitializeCallsGetListingsAsyncOnPoeTradeApiClient()
    {
        ExchangeQueryRequest expectedRequest = new() { Query = { Have = { "exalted" } } };
        this.itemToExchangeQueryRequestMapperMock
            .MapToQueryRequest(Arg.Any<Item>())
            .Returns(expectedRequest);
        CancellationTokenSource cts = new();

        await this.viewModel.InitializeAsync(new CurrencyItem(), cts.Token);

        await this.poeTradeApiClientMock
            .Received()
            .GetListingsAsync(expectedRequest, cts.Token);
    }

    [Test]
    public async Task InitializeCallsCreateAsyncOnItemListingsViewModelFactory()
    {
        ExchangeQueryResult expectedQueryResult = new("a", 1, new Dictionary<string, ExchangeQueryResultListing>());
        this.poeTradeApiClientMock
            .GetListingsAsync(Arg.Any<ExchangeQueryRequest>(), Arg.Any<CancellationToken>())
            .Returns(expectedQueryResult);

        CancellationTokenSource cts = new();

        await this.viewModel.InitializeAsync(new CurrencyItem(), cts.Token);

        await this.itemListingsViewModelFactoryMock
            .Received()
            .CreateAsync(expectedQueryResult, cts.Token);
    }

    [Test]
    public async Task InitializeSetsItemListings()
    {
        ItemListingsViewModel expected = new() { ListingsUri = new Uri("https://exchange.results") };
        this.itemListingsViewModelFactoryMock
            .CreateAsync(Arg.Any<ExchangeQueryResult>(), Arg.Any<CancellationToken>())
            .Returns(expected);

        await this.viewModel.InitializeAsync(new CurrencyItem(), default);

        this.viewModel.ItemListings.Should().Be(expected);
    }
}