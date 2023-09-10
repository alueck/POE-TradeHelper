using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using NSubstitute;

using NUnit.Framework;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.UI.Avalonia.Factories;
using POETradeHelper.ItemSearch.UI.Avalonia.Factories.Implementations;
using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Tests.Services.Factories
{
    public class ItemListingsViewModelFactoryTests
    {
        private IListingViewModelFactory listingViewModelFactoryMock;
        private ItemListingsViewModelFactory itemListingsViewModelFactory;

        [SetUp]
        public void Setup()
        {
            this.listingViewModelFactoryMock = Substitute.For<IListingViewModelFactory>();
            this.itemListingsViewModelFactory = new ItemListingsViewModelFactory(this.listingViewModelFactoryMock);
        }

        [Test]
        public async Task CreateAsyncWithItemListingsQueryResultShouldSetUri()
        {
            var itemListingsQueryResult = new ItemListingsQueryResult
            {
                Uri = new Uri("http://www.google.de")
            };

            ItemListingsViewModel result = await this.itemListingsViewModelFactory.CreateAsync(new EquippableItem(ItemRarity.Rare), itemListingsQueryResult);

            result.ListingsUri.Should().Be(itemListingsQueryResult.Uri);
        }

        [Test]
        public async Task CreateAsyncWithItemListingsQueryResultShouldCallCreateOnListingViewModelFactory()
        {
            // arrange
            var itemListingsQueryResult = new ItemListingsQueryResult
            {
                Result = new List<ListingResult>
                {
                    new(),
                    new()
                },
            };
            var item = new EquippableItem(ItemRarity.Rare);

            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            // act
            await this.itemListingsViewModelFactory.CreateAsync(item, itemListingsQueryResult, cancellationToken);

            // assert
            await this.listingViewModelFactoryMock
                .Received()
                .CreateAsync(itemListingsQueryResult.Result[0], item, cancellationToken);
            await this.listingViewModelFactoryMock
                .Received()
                .CreateAsync(itemListingsQueryResult.Result[1], item, cancellationToken);
        }

        [Test]
        public async Task CreateAsyncWithItemListingsQueryResultShouldThrowIfCancellationRequested()
        {
            // arrange
            var itemListingsQueryResult = new ItemListingsQueryResult
            {
                Result = new List<ListingResult>
                {
                    new(),
                    new()
                },
            };
            var item = new EquippableItem(ItemRarity.Rare);

            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            this.listingViewModelFactoryMock
                .When(x => x.CreateAsync(itemListingsQueryResult.Result[0], Arg.Any<Item>(), Arg.Any<CancellationToken>()))
                .Do(_ => cancellationTokenSource.Cancel());

            // act
            Func<Task> action = () => this.itemListingsViewModelFactory.CreateAsync(item, itemListingsQueryResult, cancellationToken);

            // assert
            await action.Should().ThrowAsync<OperationCanceledException>();
            await this.listingViewModelFactoryMock
                .Received(1)
                .CreateAsync(Arg.Any<ListingResult>(), Arg.Any<Item>(), Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task CreateAsyncWithItemListingsQueryResultShouldSetListingsOnResult()
        {
            // arrange
            var itemListingsQueryResult = new ItemListingsQueryResult
            {
                Result = new List<ListingResult>
                {
                    new(),
                    new()
                },
            };
            SimpleListingViewModel[] expected =
            {
                new() { AccountName = "Test" },
                new() { AccountName = "Test1" },
            };
            this.listingViewModelFactoryMock
                .CreateAsync(Arg.Any<ListingResult>(), Arg.Any<Item>(), Arg.Any<CancellationToken>())
                .Returns(expected[0], expected[1]);

            // act
            ItemListingsViewModel result = await this.itemListingsViewModelFactory.CreateAsync(new EquippableItem(ItemRarity.Rare), itemListingsQueryResult);

            // assert
            result.Listings.Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task CreateAsyncWithExchangeQueryResultShouldSetUri()
        {
            ExchangeQueryResult exchangeQueryResult = new("", 1, new Dictionary<string, ExchangeQueryResultListing>())
            {
                Uri = new Uri("https://result.link"),
            };

            ItemListingsViewModel result = await this.itemListingsViewModelFactory.CreateAsync(exchangeQueryResult);

            result.ListingsUri.Should().Be(exchangeQueryResult.Uri);
        }

        [Test]
        public async Task CreateAsyncWithExchangeQueryResultShouldCallCreateOnListingViewModelFactory()
        {
            // arrange
            ExchangeListing exchangeListing1 = new(DateTime.Now, new Account { Name = "Test" }, new List<ExchangeOffer>());
            ExchangeListing exchangeListing2 = new(DateTime.Now, new Account { Name = "Test2" }, new List<ExchangeOffer>());
            ExchangeQueryResult exchangeQueryResult = new("", 1, new Dictionary<string, ExchangeQueryResultListing>
            {
                ["a"] = new("a", exchangeListing1),
                ["b"] = new("b", exchangeListing2),
            });
            CancellationTokenSource cts = new();

            // act
            await this.itemListingsViewModelFactory.CreateAsync(exchangeQueryResult, cts.Token);

            // assert
            await this.listingViewModelFactoryMock
                .Received()
                .CreateAsync(exchangeListing1, cts.Token);
            await this.listingViewModelFactoryMock
                .Received()
                .CreateAsync(exchangeListing2, cts.Token);
        }

        [Test]
        public async Task CreateAsyncWithExchangeQueryResultShouldThrowIfCancellationRequested()
        {
            // arrange
            ExchangeListing exchangeListing1 = new(DateTime.Now, new Account(), new List<ExchangeOffer>());
            ExchangeListing exchangeListing2 = new(DateTime.Now, new Account(), new List<ExchangeOffer>());
            ExchangeQueryResult exchangeQueryResult = new("", 1, new Dictionary<string, ExchangeQueryResultListing>
            {
                ["a"] = new("a", exchangeListing1),
                ["b"] = new("b", exchangeListing2),
            });
            CancellationTokenSource cts = new();

            this.listingViewModelFactoryMock
                .When(x => x.CreateAsync(exchangeQueryResult.Result.Values.First().Listing, Arg.Any<CancellationToken>()))
                .Do(_ => cts.Cancel());

            // act
            Func<Task> action = () => this.itemListingsViewModelFactory.CreateAsync(exchangeQueryResult, cts.Token);

            // assert
            await action.Should().ThrowAsync<OperationCanceledException>();
            await this.listingViewModelFactoryMock
                .Received(1)
                .CreateAsync(Arg.Any<ExchangeListing>(), Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task CreateAsyncWithExchangeQueryResultShouldSetListingsOnResult()
        {
            // arrange
            ExchangeListing exchangeListing1 = new(DateTime.Now, new Account(), new List<ExchangeOffer>());
            ExchangeListing exchangeListing2 = new(DateTime.Now, new Account(), new List<ExchangeOffer>());
            ExchangeQueryResult exchangeQueryResult = new("", 1, new Dictionary<string, ExchangeQueryResultListing>
            {
                ["a"] = new("a", exchangeListing1),
                ["b"] = new("b", exchangeListing2),
            });

            SimpleListingViewModel[] expected =
            {
                new() { AccountName = "a" },
                new() { AccountName = "b" },
            };
            this.listingViewModelFactoryMock
                .CreateAsync(Arg.Any<ExchangeListing>(), Arg.Any<CancellationToken>())
                .Returns(expected[0], expected[1]);

            // act
            ItemListingsViewModel result = await this.itemListingsViewModelFactory.CreateAsync(exchangeQueryResult);

            // assert
            result.Listings.Should().BeEquivalentTo(expected);
        }
    }
}