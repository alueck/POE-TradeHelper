using Moq;
using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services.Factories;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using System;
using System.Collections.Generic;

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
        public void CreateShouldSetUri()
        {
            var itemListingsQueryResult = new ItemListingsQueryResult
            {
                Uri = new Uri("http://www.google.de"),
                Item = new CurrencyItem()
            };

            ItemListingsViewModel result = this.itemListingsViewModelFactory.Create(itemListingsQueryResult);

            Assert.That(result.ListingsUri, Is.EqualTo(itemListingsQueryResult.Uri));
        }

        [Test]
        public void CreateShouldSetItemDescription()
        {
            var itemListingsQueryResult = new ItemListingsQueryResult
            {
                Item = new EquippableItem(ItemRarity.Rare)
                {
                    Name = "Brood Blast",
                    Type = "Thicket Bow"
                }
            };

            ItemListingsViewModel result = this.itemListingsViewModelFactory.Create(itemListingsQueryResult);

            Assert.That(result.ItemDescription, Is.EqualTo(itemListingsQueryResult.Item.DisplayName));
        }

        [Test]
        public void CreateShouldSetItemRarity()
        {
            var itemListingsQueryResult = new ItemListingsQueryResult
            {
                Item = new EquippableItem(ItemRarity.Rare)
            };

            ItemListingsViewModel result = this.itemListingsViewModelFactory.Create(itemListingsQueryResult);

            Assert.That(result.ItemRarity, Is.EqualTo(itemListingsQueryResult.Item.Rarity));
        }

        [Test]
        public void CreateShouldCallCreateOnListingViewModelFactory()
        {
            var itemListingsQueryResult = new ItemListingsQueryResult
            {
                Result = new List<ListingResult>
                {
                    new ListingResult(),
                    new ListingResult()
                },
                Item = new CurrencyItem()
            };

            ItemListingsViewModel result = this.itemListingsViewModelFactory.Create(itemListingsQueryResult);

            this.listingViewModelFactoryMock.Verify(x => x.Create(itemListingsQueryResult.Result[0], itemListingsQueryResult.Item));
            this.listingViewModelFactoryMock.Verify(x => x.Create(itemListingsQueryResult.Result[1], itemListingsQueryResult.Item));
        }
    }
}