using System;
using System.Collections.Generic;
using System.Text.Json;
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
    public class ListingViewModelFactoryTests
    {
        private Mock<IPriceViewModelFactory> priceViewModelFactoryMock;
        private ListingViewModelFactory listingViewModelFactory;

        [SetUp]
        public void Setup()
        {
            this.priceViewModelFactoryMock = new Mock<IPriceViewModelFactory>();
            this.listingViewModelFactory = new ListingViewModelFactory(this.priceViewModelFactoryMock.Object);
        }

        [TestCaseSource(nameof(Items))]
        public async Task CreateShouldCallCreateOnPriceViewModelFactory(Item item)
        {
            // arrange
            ListingResult listingResult = GetListingResult();

            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            // act
            await this.listingViewModelFactory.CreateAsync(listingResult, item, cancellationToken);

            // assert
            this.priceViewModelFactoryMock.Verify(x => x.CreateAsync(listingResult.Listing.Price, cancellationToken));
        }

        [TestCaseSource(nameof(Items))]
        public async Task CreateShouldSetPriceViewModel(Item item)
        {
            var expected = new PriceViewModel { Amount = "2", Currency = "Chaos Orb" };
            ListingResult listingResult = GetListingResult();

            this.priceViewModelFactoryMock.Setup(x => x.CreateAsync(It.IsAny<Price>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            SimpleListingViewModel result = await this.listingViewModelFactory.CreateAsync(listingResult, item);

            Assert.That(result.Price, Is.EqualTo(expected));
        }

        [Test]
        public async Task CreateShouldReturnGemItemListingViewModelForGemItem()
        {
            decimal experience = 0.25m;
            string gemLevel = "15";
            string quality = "+13%";

            string propertiesJson = $@"[{{""name"":""Level"",""values"":[[""{gemLevel}"",0]],""displayMode"":0,""type"":5}},{{""name"":""Quality"",""values"":[[""{quality}"",1]],""displayMode"":0,""type"":6}}]";
            string additionalPropertiesJson = @"[{""name"":""Experience"",""values"":[[""1/15249"",0]],""displayMode"":2,""progress"":0.25,""type"":20}]";

            ListingResult listingResult = GetListingResult();
            listingResult.Item.Properties = GetPropertiesList(propertiesJson);
            listingResult.Item.AdditionalProperties = GetPropertiesList(additionalPropertiesJson);

            Item item = new GemItem();

            GemItemListingViewModel result = await this.listingViewModelFactory.CreateAsync(listingResult, item) as GemItemListingViewModel;

            Assert.NotNull(result);
            AssertSimpleListingViewModelProperties(result, listingResult);
            Assert.That(result.GemExperiencePercent, Is.EqualTo(experience * 100));
            Assert.That(result.Level, Is.EqualTo(gemLevel));
            Assert.That(result.Quality, Is.EqualTo(quality));
        }

        [TestCaseSource(nameof(ItemListingViewModelWithItemLevelItems))]
        public async Task CreateShouldReturnItemListingViewModelWithItemLevel(Item item)
        {
            ListingResult listingResult = GetListingResult();

            ItemListingViewModelWithItemLevel result = await this.listingViewModelFactory.CreateAsync(listingResult, item) as ItemListingViewModelWithItemLevel;

            Assert.NotNull(result);
            AssertSimpleListingViewModelProperties(result, listingResult);
            Assert.That(result.ItemLevel, Is.EqualTo(listingResult.Item.ItemLevel));
        }

        [Test]
        public async Task CreateShouldReturnFlaskItemListingViewModelForFlaskItem()
        {
            string quality = "+20%";

            string propertiesJson = $@"[{{""name"":""Quality"",""values"":[[""{quality}"",1]],""displayMode"":0,""type"":6}}]";

            ListingResult listingResult = GetListingResult();
            listingResult.Item.Properties = GetPropertiesList(propertiesJson);
            Item item = new FlaskItem(ItemRarity.Normal);

            FlaskItemListingViewModel result = await this.listingViewModelFactory.CreateAsync(listingResult, item) as FlaskItemListingViewModel;

            Assert.NotNull(result);
            AssertSimpleListingViewModelProperties(result, listingResult);
            Assert.That(result.Quality, Is.EqualTo(quality));
        }

        [TestCaseSource(nameof(SimpleListingViewModelItems))]
        public async Task CreateShouldReturnSimpleItemListingViewModel(Item item)
        {
            ListingResult listingResult = GetListingResult();

            SimpleListingViewModel result = await this.listingViewModelFactory.CreateAsync(listingResult, item);

            Assert.NotNull(result);
            AssertSimpleListingViewModelProperties(result, listingResult);
        }

        private static IEnumerable<Item> ItemListingViewModelWithItemLevelItems()
        {
            yield return new EquippableItem(ItemRarity.Normal);
            yield return new OrganItem();
        }

        private static IEnumerable<Item> SimpleListingViewModelItems()
        {
            yield return new DivinationCardItem();
            yield return new MapItem(ItemRarity.Normal);
            yield return new CurrencyItem();
            yield return new FragmentItem();
        }

        public static IEnumerable<Item> Items
        {
            get
            {
                yield return new CurrencyItem();
                yield return new DivinationCardItem();
                yield return new FlaskItem(ItemRarity.Normal);
                yield return new FragmentItem();
                yield return new GemItem();
                yield return new MapItem(ItemRarity.Normal);
                yield return new OrganItem();
                yield return new ProphecyItem();
                yield return new JewelItem(ItemRarity.Magic);
                yield return new EquippableItem(ItemRarity.Magic);
            }
        }

        private static ListingResult GetListingResult()
        {
            return new ListingResult
            {
                Listing = new Listing
                {
                    Account = new Account
                    {
                        Name = "accountName"
                    },
                    Price = new Price
                    {
                        Amount = 1.0m,
                        Currency = "chaos"
                    },
                    Indexed = DateTime.UtcNow.AddDays(-3),
                },
                Item = new ItemListing
                {
                    ItemLevel = 75
                }
            };
        }

        private static List<Property> GetPropertiesList(string propertiesJson)
        {
            return JsonSerializer.Deserialize<List<Property>>(propertiesJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        private static void AssertSimpleListingViewModelProperties(SimpleListingViewModel result, ListingResult listingResult)
        {
            Assert.That(result.AccountName, Is.EqualTo(listingResult.Listing.Account.Name));
            Assert.That(result.Age, Is.EqualTo(listingResult.Listing.AgeText));
        }
    }
}