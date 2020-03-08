using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services.Factories;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace POETradeHelper.ItemSearch.Tests.Services.Factories
{
    public class ListingViewModelFactoryTests
    {
        private ListingViewModelFactory listingViewModelFactory;

        [SetUp]
        public void Setup()
        {
            this.listingViewModelFactory = new ListingViewModelFactory();
        }

        [Test]
        public void CreateShouldReturnGemItemListingViewModelForGemItem()
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

            GemItemListingViewModel result = this.listingViewModelFactory.Create(listingResult, item) as GemItemListingViewModel;

            Assert.NotNull(result);
            AssertSimpleListingViewModelProperties(result, listingResult);
            Assert.That(result.GemExperiencePercent, Is.EqualTo(experience * 100));
            Assert.That(result.Level, Is.EqualTo(gemLevel));
            Assert.That(result.Quality, Is.EqualTo(quality));
        }

        [TestCaseSource(nameof(ItemListingViewModelWithItemLevelItems))]
        public void CreateShouldReturnItemListingViewModelWithItemLevel(Item item)
        {
            ListingResult listingResult = GetListingResult();

            ItemListingViewModelWithItemLevel result = this.listingViewModelFactory.Create(listingResult, item) as ItemListingViewModelWithItemLevel;

            Assert.NotNull(result);
            AssertSimpleListingViewModelProperties(result, listingResult);
            Assert.That(result.ItemLevel, Is.EqualTo(listingResult.Item.ItemLevel));
        }

        [Test]
        public void CreateShouldReturnFlaskItemListingViewModelForFlaskItem()
        {
            string quality = "+20%";

            string propertiesJson = $@"[{{""name"":""Quality"",""values"":[[""{quality}"",1]],""displayMode"":0,""type"":6}}]";

            ListingResult listingResult = GetListingResult();
            listingResult.Item.Properties = GetPropertiesList(propertiesJson);
            Item item = new FlaskItem(ItemRarity.Normal);

            FlaskItemListingViewModel result = this.listingViewModelFactory.Create(listingResult, item) as FlaskItemListingViewModel;

            Assert.NotNull(result);
            AssertSimpleListingViewModelProperties(result, listingResult);
            Assert.That(result.Quality, Is.EqualTo(quality));
        }

        [TestCaseSource(nameof(SimpleListingViewModelItems))]
        public void CreateShouldReturnSimpleItemListingViewModel(Item item)
        {
            ListingResult listingResult = GetListingResult();

            SimpleListingViewModel result = this.listingViewModelFactory.Create(listingResult, item);

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
            Assert.That(result.Price, Is.EqualTo(listingResult.Listing.Price.PriceText));
            Assert.That(result.Age, Is.EqualTo(listingResult.Listing.AgeText));
        }
    }
}