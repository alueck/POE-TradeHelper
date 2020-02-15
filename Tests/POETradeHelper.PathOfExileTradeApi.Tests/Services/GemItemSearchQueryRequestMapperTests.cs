using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;
using POETradeHelper.PathOfExileTradeApi.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace POETradeHelper.PathOfExileTradeApi.Tests.Services
{
    public class GemItemSearchQueryRequestMapperTests
    {
        private GemItemSearchQueryRequestMapper gemItemSearchQueryRequestMapper;

        [SetUp]
        public void Setup()
        {
            this.gemItemSearchQueryRequestMapper = new GemItemSearchQueryRequestMapper();
        }

        [Test]
        public void CanMapShouldReturnTrueForGemItem()
        {
            var item = new GemItem();

            bool result = this.gemItemSearchQueryRequestMapper.CanMap(item);

            Assert.IsTrue(result);
        }

        [TestCaseSource(nameof(NonGemItems))]
        public void CanMapShouldReturnFalseForNonGemItems(Item nonGemItem)
        {
            bool result = this.gemItemSearchQueryRequestMapper.CanMap(nonGemItem);

            Assert.IsFalse(result);
        }

        [TestCase("Vaal Flameblast")]
        [TestCase("Flameblast")]
        public void MapToQueryRequestShouldMapType(string expected)
        {
            var item = new GemItem
            {
                Type = expected
            };

            SearchQueryRequest result = this.gemItemSearchQueryRequestMapper.MapToQueryRequest(item) as SearchQueryRequest;

            Assert.That(result.Query.Type, Is.EqualTo(expected));
        }

        [Test]
        public void MapToQueryRequestShouldNotMapName()
        {
            const string expected = "Vaal Flameblast";
            var item = new GemItem
            {
                Name = expected
            };

            SearchQueryRequest result = this.gemItemSearchQueryRequestMapper.MapToQueryRequest(item) as SearchQueryRequest;

            Assert.IsNull(result.Query.Name);
        }

        [Test]
        public void MapToQueryRequestShouldNotMapRarity()
        {
            var item = new GemItem();

            SearchQueryRequest result = this.gemItemSearchQueryRequestMapper.MapToQueryRequest(item) as SearchQueryRequest;

            Assert.IsNull(result.Query.Filters.TypeFilters.Rarity);
        }

        [TestCase(10)]
        [TestCase(20)]
        public void MapToQueryRequestShouldMapGemLevel(int expected)
        {
            var item = new GemItem
            {
                Level = expected
            };

            SearchQueryRequest result = this.gemItemSearchQueryRequestMapper.MapToQueryRequest(item) as SearchQueryRequest;

            MinMaxFilter gemLevelFilter = result.Query.Filters.MiscFilters.GemLevel;
            Assert.IsNotNull(gemLevelFilter);
            Assert.That(gemLevelFilter.Min, Is.EqualTo(expected));
            Assert.IsNull(gemLevelFilter.Max);
        }

        [TestCase(10)]
        [TestCase(20)]
        public void MapToQueryRequestShouldMapGemQuality(int expected)
        {
            var item = new GemItem
            {
                Quality = expected
            };

            SearchQueryRequest result = this.gemItemSearchQueryRequestMapper.MapToQueryRequest(item) as SearchQueryRequest;

            MinMaxFilter qualityFilter = result.Query.Filters.MiscFilters.Quality;
            Assert.IsNotNull(qualityFilter);
            Assert.That(qualityFilter.Min, Is.EqualTo(expected));
            Assert.IsNull(qualityFilter.Max);
        }

        private static IEnumerable<Item> NonGemItems
        {
            get
            {
                yield return new CurrencyItem();
                yield return new DivinationCardItem();
                yield return new FlaskItem(ItemRarity.Normal);
                yield return new FragmentItem();
                yield return new MapItem(ItemRarity.Normal);
                yield return new OrganItem();
                yield return new ProphecyItem();
                yield return new EquippableItem(ItemRarity.Normal);
            }
        }
    }
}