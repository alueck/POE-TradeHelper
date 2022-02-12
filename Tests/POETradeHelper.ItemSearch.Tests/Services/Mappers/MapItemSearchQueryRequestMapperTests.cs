using NUnit.Framework;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services.Mappers;
using POETradeHelper.PathOfExileTradeApi.Constants;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;

namespace POETradeHelper.ItemSearch.Tests.Services.Mappers
{
    public class MapItemSearchQueryRequestMapperTests : ItemSearchQueryRequestMapperTestsBase<MapItem>
    {
        private MapItemSearchQueryRequestMapper mapItemSearchQueryRequestMapper;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            this.ItemSearchQueryRequestMapper = this.mapItemSearchQueryRequestMapper = new MapItemSearchQueryRequestMapper(this.ItemSearchOptionsMock.Object);
        }

        [Test]
        public void MapToQueryRequestShouldParseType()
        {
            const string expected = "Strand Map";
            var item = new MapItem(ItemRarity.Normal)
            {
                Type = expected
            };

            SearchQueryRequest result = this.mapItemSearchQueryRequestMapper.MapToQueryRequest(item) as SearchQueryRequest;

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

            SearchQueryRequest result = this.mapItemSearchQueryRequestMapper.MapToQueryRequest(item) as SearchQueryRequest;

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

            SearchQueryRequest result = this.mapItemSearchQueryRequestMapper.MapToQueryRequest(item) as SearchQueryRequest;

            Assert.IsNull(result.Query.Name);
        }

        [TestCaseSource(nameof(NonUniqueItemRarities))]
        public void MapToQueryRequestShouldNotMapNameOfNonUniqueItems(ItemRarity itemRarity)
        {
            var item = new MapItem(itemRarity)
            {
                Name = "Pillars of Arun"
            };

            SearchQueryRequest result = this.mapItemSearchQueryRequestMapper.MapToQueryRequest(item) as SearchQueryRequest;

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

            SearchQueryRequest result = this.mapItemSearchQueryRequestMapper.MapToQueryRequest(item) as SearchQueryRequest;

            Assert.That(result.Query.Filters.MiscFilters.Identified.Option, Is.EqualTo(identified));
        }

        [Test]
        public void MapToQueryItemShouldMapItemRarityForUniqueItems()
        {
            var item = new MapItem(ItemRarity.Unique);

            SearchQueryRequest result = this.mapItemSearchQueryRequestMapper.MapToQueryRequest(item) as SearchQueryRequest;

            Assert.NotNull(result.Query.Filters.TypeFilters);
            Assert.That(result.Query.Filters.TypeFilters.Rarity.Option, Is.EqualTo(ItemRarityFilterOptions.Unique));
        }

        [TestCaseSource(nameof(NonUniqueItemRarities))]
        public void MapToQueryItemShouldMapItemRarityForNonUniqueItems(ItemRarity itemRarity)
        {
            var item = new MapItem(itemRarity);

            SearchQueryRequest result = this.mapItemSearchQueryRequestMapper.MapToQueryRequest(item) as SearchQueryRequest;

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

            SearchQueryRequest result = this.mapItemSearchQueryRequestMapper.MapToQueryRequest(item) as SearchQueryRequest;

            MinMaxFilter mapTierFilter = result.Query.Filters.MapFilters.MapTier;
            Assert.NotNull(mapTierFilter);
            Assert.That(mapTierFilter.Min, Is.EqualTo(mapTier));
            Assert.That(mapTierFilter.Max, Is.EqualTo(mapTier));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void MapToQueryItemShouldMapBlighted(bool isBlighted)
        {
            var item = new MapItem(ItemRarity.Normal)
            {
                IsBlighted = isBlighted
            };

            SearchQueryRequest result = this.mapItemSearchQueryRequestMapper.MapToQueryRequest(item) as SearchQueryRequest;

            BoolOptionFilter blightedFilter = result.Query.Filters.MapFilters.MapBlighted;
            Assert.NotNull(blightedFilter);
            Assert.That(blightedFilter.Option, Is.EqualTo(isBlighted));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void MapToQueryItemShouldMapBlightRavaged(bool isBlightRavaged)
        {
            var item = new MapItem(ItemRarity.Normal)
            {
                IsBlightRavaged = isBlightRavaged
            };

            SearchQueryRequest result = this.mapItemSearchQueryRequestMapper.MapToQueryRequest(item) as SearchQueryRequest;

            BoolOptionFilter blightRavaged = result.Query.Filters.MapFilters.MapBlightRavaged;
            Assert.NotNull(blightRavaged);
            Assert.That(blightRavaged.Option, Is.EqualTo(isBlightRavaged));
        }
    }
}
