using Moq;
using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services.Factories;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            var itemListingsQueryResult = new ItemListingsQueryResult
            {
                Result = new List<ListingResult>
                {
                    new ListingResult(),
                    new ListingResult()
                },
            };
            var item = new CurrencyItem();

            ItemListingsViewModel result = await this.itemListingsViewModelFactory.CreateAsync(item, itemListingsQueryResult);

            this.listingViewModelFactoryMock.Verify(x => x.CreateAsync(itemListingsQueryResult.Result[0], item));
            this.listingViewModelFactoryMock.Verify(x => x.CreateAsync(itemListingsQueryResult.Result[1], item));
        }
    }
}