using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Constants;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;
using POETradeHelper.PathOfExileTradeApi.Services;
using System.Collections.Generic;

namespace POETradeHelper.PathOfExileTradeApi.Tests.Services
{
    public class MapItemSearchQueryRequestMapperTests
    {
        private MapItemSearchQueryRequestMapper mapItemSearchQueryRequestMapper;

        [SetUp]
        public void Setup()
        {
            this.mapItemSearchQueryRequestMapper = new MapItemSearchQueryRequestMapper();
        }

        [Test]
        public void CanMapShouldReturnTrueForMapItem()
        {
            var item = new MapItem(ItemRarity.Normal);

            bool result = this.mapItemSearchQueryRequestMapper.CanMap(item);

            Assert.IsTrue(result);
        }

        [TestCaseSource(nameof(NonMapItems))]
        public void CanMapShouldReturnFalseForNonMapItems(Item nonMapItem)
        {
            bool result = this.mapItemSearchQueryRequestMapper.CanMap(nonMapItem);

            Assert.IsFalse(result);
        }

        [Test]
        public void MapToQueryRequestShouldParseType()
        {
            const string expected = "Strand Map";
            var item = new MapItem(ItemRarity.Normal)
            {
                Type = expected
            };

            SearchQueryRequest result = this.mapItemSearchQueryRequestMapper.MapToQueryRequest(item);

            Assert.That(result.Query.Type, Is.EqualTo(expected));
        }

        [Test]
        public void MapToQueryRequestShouldMapNameOfIdentifiedUniqueMap()
        {
            const string expected = "Pillars of Arun";
            var item = new MapItem(ItemRarity.Unique)
            {
                Name = expected,
                IsIdentified = true
            };

            SearchQueryRequest result = this.mapItemSearchQueryRequestMapper.MapToQueryRequest(item);

            Assert.That(result.Query.Name, Is.EqualTo(expected));
        }

        [Test]
        public void MapToQueryRequestShouldNotMapNameOfUnidentifiedUniqueMap()
        {
            const string expected = "Pillars of Arun";
            var item = new MapItem(ItemRarity.Unique)
            {
                Name = expected,
            };

            SearchQueryRequest result = this.mapItemSearchQueryRequestMapper.MapToQueryRequest(item);

            Assert.IsNull(result.Query.Name);
        }

        [TestCaseSource(nameof(NonUniqueItemRarities))]
        public void MapToQueryRequestShouldNotMapNameOfNonUniqueItems(ItemRarity itemRarity)
        {
            var item = new MapItem(itemRarity)
            {
                Name = "Pillars of Arun"
            };

            SearchQueryRequest result = this.mapItemSearchQueryRequestMapper.MapToQueryRequest(item);

            Assert.IsNull(result.Query.Name);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void MapToQueryRequestShouldMapIdentified(bool identified)
        {
            var item = new MapItem(ItemRarity.Rare)
            {
                IsIdentified = identified
            };

            SearchQueryRequest result = this.mapItemSearchQueryRequestMapper.MapToQueryRequest(item);

            Assert.That(result.Query.Filters.MiscFilters.Identified.Option, Is.EqualTo(identified));
        }

        [Test]
        public void MapToQueryItemShouldMapItemRarityForUniqueItems()
        {
            var item = new MapItem(ItemRarity.Unique);

            SearchQueryRequest result = this.mapItemSearchQueryRequestMapper.MapToQueryRequest(item);

            Assert.NotNull(result.Query.Filters.TypeFilters);
            Assert.That(result.Query.Filters.TypeFilters.Rarity.Option, Is.EqualTo(ItemRarityFilterOptions.Unique));
        }

        [TestCaseSource(nameof(NonUniqueItemRarities))]
        public void MapToQueryItemShouldMapItemRarityForNonUniqueItems(ItemRarity itemRarity)
        {
            var item = new MapItem(itemRarity);

            SearchQueryRequest result = this.mapItemSearchQueryRequestMapper.MapToQueryRequest(item);

            Assert.NotNull(result.Query.Filters.TypeFilters);
            Assert.That(result.Query.Filters.TypeFilters.Rarity.Option, Is.EqualTo(ItemRarityFilterOptions.NonUnique));
        }

        [TestCase(10)]
        [TestCase(15)]
        public void MapToQueryItemShouldMapMapTier(int mapTier)
        {
            var item = new MapItem(ItemRarity.Normal)
            {
                Tier = mapTier
            };

            SearchQueryRequest result = this.mapItemSearchQueryRequestMapper.MapToQueryRequest(item);

            MinMaxFilter mapTierFilter = result.Query.Filters.MapFilters.MapTier;
            Assert.NotNull(mapTierFilter);
            Assert.That(mapTierFilter.Min, Is.EqualTo(mapTier));
            Assert.That(mapTierFilter.Max, Is.EqualTo(mapTier));
        }

        private static IEnumerable<Item> NonMapItems
        {
            get
            {
                yield return new CurrencyItem();
                yield return new DivinationCardItem();
                yield return new FlaskItem(ItemRarity.Normal);
                yield return new FragmentItem();
                yield return new GemItem();
                yield return new OrganItem();
                yield return new ProphecyItem();
                yield return new EquippableItem(ItemRarity.Normal);
            }
        }

        private static IEnumerable<ItemRarity> NonUniqueItemRarities
        {
            get
            {
                yield return ItemRarity.Normal;
                yield return ItemRarity.Magic;
                yield return ItemRarity.Rare;
            }
        }
    }
}