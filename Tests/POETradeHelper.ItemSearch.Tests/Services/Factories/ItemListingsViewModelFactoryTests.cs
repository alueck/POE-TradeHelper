using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services.Factories;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.ItemSearch.Tests.Services.Factories
{
    public class ItemListingsViewModelFactoryTests
    {
        private Mock<IListingViewModelFactory> listingViewModelFactoryMock;
        private ItemListingsViewModelFactory itemListingsViewModelFactory;

        [SetUp]
        public void Setup()
        {
            this.listingViewModelFactoryMock = new Mock<IListingViewModelFactory>();
            this.itemListingsViewModelFactory = new ItemListingsViewModelFactory(this.listingViewModelFactoryMock.Object);
        }

        [Test]
        public async Task CreateAsyncShouldSetUri()
        {
            var itemListingsQueryResult = new ItemListingsQueryResult
            {
                Uri = new Uri("http://www.google.de")
            };

            ItemListingsViewModel result = await this.itemListingsViewModelFactory.CreateAsync(new CurrencyItem(), itemListingsQueryResult);

            Assert.That(result.ListingsUri, Is.EqualTo(itemListingsQueryResult.Uri));
        }

        [Test]
        public async Task CreateAsyncShouldSetItemDescription()
        {
            var itemListingsQueryResult = new ItemListingsQueryResult();
            var item = new EquippableItem(ItemRarity.Rare)
            {
                Name = "Brood Blast",
                Type = "Thicket Bow"
            };

            ItemListingsViewModel result = await this.itemListingsViewModelFactory.CreateAsync(item, itemListingsQueryResult);

            Assert.That(result.ItemDescription, Is.EqualTo(item.DisplayName));
        }

        [Test]
        public async Task CreateAsyncShouldSetItemRarity()
        {
            var itemListingsQueryResult = new ItemListingsQueryResult();
            var item = new EquippableItem(ItemRarity.Rare);

            ItemListingsViewModel result = await this.itemListingsViewModelFactory.CreateAsync(item, itemListingsQueryResult);

            Assert.That(result.ItemRarity, Is.EqualTo(item.Rarity));
        }

        [Test]
        public async Task CreateAsyncShouldCallCreateOnListingViewModelFactory()
        {
            // arrange
            var itemListingsQueryResult = new ItemListingsQueryResult
            {
                Result = new List<ListingResult>
                {
                    new ListingResult(),
                    new ListingResult()
                },
            };
            var item = new CurrencyItem();

            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            // act
            ItemListingsViewModel result = await this.itemListingsViewModelFactory.CreateAsync(item, itemListingsQueryResult, cancellationToken);

            // assert
            this.listingViewModelFactoryMock.Verify(x => x.CreateAsync(itemListingsQueryResult.Result[0], item, cancellationToken));
            this.listingViewModelFactoryMock.Verify(x => x.CreateAsync(itemListingsQueryResult.Result[1], item, cancellationToken));
        }

        [Test]
        public async Task CreateAsyncShouldNotCallCreateOnListingViewModelFactoryIfCancellationRequested()
        {
            // arrange
            var itemListingsQueryResult = new ItemListingsQueryResult
            {
                Result = new List<ListingResult>
                {
                    new ListingResult(),
                    new ListingResult()
                },
            };
            var item = new CurrencyItem();

            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            this.listingViewModelFactoryMock.Setup(x => x.CreateAsync(itemListingsQueryResult.Result[0], It.IsAny<Item>(), It.IsAny<CancellationToken>()))
                .Callback(() => cancellationTokenSource.Cancel());

            // act
            ItemListingsViewModel result = await this.itemListingsViewModelFactory.CreateAsync(item, itemListingsQueryResult, cancellationToken);

            // assert
            this.listingViewModelFactoryMock.Verify(x => x.CreateAsync(It.IsAny<ListingResult>(), It.IsAny<Item>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}