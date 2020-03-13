using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.PathOfExileTradeApi.Tests.Services
{
    public class GemItemSearchQueryRequestMapperTests : ItemSearchQueryRequestMapperTestsBase<GemItem>
    {
        private GemItemSearchQueryRequestMapper gemItemSearchQueryRequestMapper;

        [SetUp]
        public void Setup()
        {
            this.ItemSearchQueryRequestMapper = this.gemItemSearchQueryRequestMapper = new GemItemSearchQueryRequestMapper();
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
    }
}