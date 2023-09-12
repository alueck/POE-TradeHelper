using System;
using System.Collections.Generic;
using System.Text.Json;
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
    public class ListingViewModelFactoryTests
    {
        private IPriceViewModelFactory priceViewModelFactoryMock;
        private ListingViewModelFactory listingViewModelFactory;

        [SetUp]
        public void Setup()
        {
            this.priceViewModelFactoryMock = Substitute.For<IPriceViewModelFactory>();
            this.listingViewModelFactory = new ListingViewModelFactory(this.priceViewModelFactoryMock);
        }

        [TestCaseSource(nameof(GetListingResultItems))]
        public async Task CreateShouldCallCreateOnPriceViewModelFactory(Item item)
        {
            // arrange
            ListingResult listingResult = GetListingResult();

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            // act
            await this.listingViewModelFactory.CreateAsync(listingResult, item, cancellationToken);

            // assert
            await this.priceViewModelFactoryMock
                .Received()
                .CreateAsync(listingResult.Listing.Price, cancellationToken);
        }

        [TestCaseSource(nameof(GetListingResultItems))]
        public async Task CreateShouldSetPriceViewModel(Item item)
        {
            PriceViewModel expected = new PriceViewModel { Amount = "2", Currency = "Chaos Orb" };
            ListingResult listingResult = GetListingResult();

            this.priceViewModelFactoryMock.CreateAsync(Arg.Any<Price>(), Arg.Any<CancellationToken>())
                .Returns(expected);

            SimpleListingViewModel result = await this.listingViewModelFactory.CreateAsync(listingResult, item);

            result.Price.Should().Be(expected);
        }

        [Test]
        public async Task CreateShouldReturnGemItemListingViewModelForGemItem()
        {
            const decimal experience = 0.25m;
            const string gemLevel = "15";
            const string quality = "+13%";

            const string propertiesJson =
                $@"[{{""name"":""Level"",""values"":[[""{gemLevel}"",0]],""displayMode"":0,""type"":5}},{{""name"":""Quality"",""values"":[[""{quality}"",1]],""displayMode"":0,""type"":6}}]";
            const string additionalPropertiesJson =
                @"[{""name"":""Experience"",""values"":[[""1/15249"",0]],""displayMode"":2,""progress"":0.25,""type"":20}]";

            ListingResult listingResult = GetListingResult();
            listingResult.Item.Properties = GetPropertiesList(propertiesJson);
            listingResult.Item.AdditionalProperties = GetPropertiesList(additionalPropertiesJson);

            Item item = new GemItem();

            GemItemListingViewModel result = await this.listingViewModelFactory.CreateAsync(listingResult, item) as GemItemListingViewModel;

            result.Should().NotBeNull();
            AssertSimpleListingViewModelProperties(result, listingResult);
            result.GemExperiencePercent.Should().Be(experience * 100);
            result.Level.Should().Be(gemLevel);
            result.Quality.Should().Be(quality);
        }

        [TestCaseSource(nameof(ItemListingViewModelWithItemLevelItems))]
        public async Task CreateShouldReturnItemListingViewModelWithItemLevel(Item item)
        {
            ListingResult listingResult = GetListingResult();

            ItemListingViewModelWithItemLevel result =
                await this.listingViewModelFactory.CreateAsync(listingResult, item) as ItemListingViewModelWithItemLevel;

            result.Should().NotBeNull();
            AssertSimpleListingViewModelProperties(result, listingResult);
            result.ItemLevel.Should().Be(listingResult.Item.ItemLevel);
        }

        [Test]
        public async Task CreateShouldReturnFlaskItemListingViewModelForFlaskItem()
        {
            const string quality = "+20%";

            const string propertiesJson = $@"[{{""name"":""Quality"",""values"":[[""{quality}"",1]],""displayMode"":0,""type"":6}}]";

            ListingResult listingResult = GetListingResult();
            listingResult.Item.Properties = GetPropertiesList(propertiesJson);
            Item item = new FlaskItem(ItemRarity.Normal);

            FlaskItemListingViewModel result = await this.listingViewModelFactory.CreateAsync(listingResult, item) as FlaskItemListingViewModel;

            result.Should().NotBeNull();
            AssertSimpleListingViewModelProperties(result, listingResult);
            result.Quality.Should().Be(quality);
        }

        [TestCaseSource(nameof(SimpleListingViewModelItems))]
        public async Task CreateShouldReturnSimpleItemListingViewModel(Item item)
        {
            ListingResult listingResult = GetListingResult();

            SimpleListingViewModel result = await this.listingViewModelFactory.CreateAsync(listingResult, item);

            result.Should().NotBeNull();
            AssertSimpleListingViewModelProperties(result, listingResult);
        }

        [Test]
        public async Task CreateWithExchangeListingShouldReturnResultWithAccountName()
        {
            const string expectedName = "xXFighterXx";
            ExchangeListing exchangeListing = new(DateTime.Now, new Account { Name = expectedName }, new List<ExchangeOffer>());

            SimpleListingViewModel result = await this.listingViewModelFactory.CreateAsync(exchangeListing);

            result.AccountName.Should().Be(expectedName);
        }

        [Test]
        public async Task CreateWithExchangeListingShouldReturnResultWithAge()
        {
            ExchangeListing exchangeListing = new(DateTime.Now, new Account(), new List<ExchangeOffer>());

            SimpleListingViewModel result = await this.listingViewModelFactory.CreateAsync(exchangeListing);

            result.Age.Should().Be(exchangeListing.AgeText);
        }

        [Test]
        public async Task CreateWithExchangeListingShouldCallCreateOnPriceViewModelFactory()
        {
            Price itemPrice = new() { Amount = 10 };
            Price exchangePrice = new() { Amount = 5 };
            Price expectedPrice = itemPrice with { Amount = itemPrice.Amount / exchangePrice.Amount };
            ExchangeOffer exchangeOffer = new(itemPrice, exchangePrice);
            ExchangeListing exchangeListing = new(DateTime.Now, new Account(), new List<ExchangeOffer> { exchangeOffer });
            CancellationTokenSource cts = new();

            await this.listingViewModelFactory.CreateAsync(exchangeListing, cts.Token);

            await this.priceViewModelFactoryMock
                .Received()
                .CreateAsync(expectedPrice, cts.Token);
        }

        [Test]
        public async Task CreateWithExchangeListingShouldReturnResultWithPrice()
        {
            PriceViewModel expected = new() { Currency = "chaos" };
            ExchangeListing exchangeListing = new(DateTime.Now, new Account(), new List<ExchangeOffer>());

            this.priceViewModelFactoryMock
                .CreateAsync(Arg.Any<Price>(), Arg.Any<CancellationToken>())
                .Returns(expected);

            SimpleListingViewModel result = await this.listingViewModelFactory.CreateAsync(exchangeListing);

            result.Price.Should().Be(expected);
        }

        private static IEnumerable<Item> ItemListingViewModelWithItemLevelItems()
        {
            yield return new EquippableItem(ItemRarity.Normal);
            yield return new OrganItem();
        }

        private static IEnumerable<Item> SimpleListingViewModelItems()
        {
            yield return new MapItem(ItemRarity.Normal);
        }

        private static IEnumerable<Item> GetListingResultItems()
        {
            yield return new FlaskItem(ItemRarity.Normal);
            yield return new GemItem();
            yield return new MapItem(ItemRarity.Normal);
            yield return new OrganItem();
            yield return new ProphecyItem();
            yield return new JewelItem(ItemRarity.Magic);
            yield return new EquippableItem(ItemRarity.Magic);
        }

        private static ListingResult GetListingResult() =>
            new()
            {
                Listing = new Listing
                {
                    Account = new Account
                    {
                        Name = "accountName",
                    },
                    Price = new Price
                    {
                        Amount = 1.0m,
                        Currency = "chaos",
                    },
                    Indexed = DateTime.UtcNow.AddDays(-3),
                },
                Item = new ItemListing
                {
                    ItemLevel = 75,
                },
            };

        private static List<Property> GetPropertiesList(string propertiesJson) => JsonSerializer.Deserialize<List<Property>>(
            propertiesJson,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        private static void AssertSimpleListingViewModelProperties(SimpleListingViewModel result, ListingResult listingResult)
        {
            result.AccountName.Should().Be(listingResult.Listing.Account.Name);
            result.Age.Should().Be(listingResult.Listing.AgeText);
        }
    }
}