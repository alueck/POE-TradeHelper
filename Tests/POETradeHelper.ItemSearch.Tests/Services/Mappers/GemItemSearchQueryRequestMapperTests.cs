using NUnit.Framework;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services.Mappers;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;

namespace POETradeHelper.ItemSearch.Tests.Services.Mappers
{
    public class GemItemSearchQueryRequestMapperTests : ItemSearchQueryRequestMapperTestsBase<GemItem>
    {
        private readonly GemItemSearchQueryRequestMapper gemItemSearchQueryRequestMapper;

        public GemItemSearchQueryRequestMapperTests()
        {
            this.ItemSearchQueryRequestMapper = this.gemItemSearchQueryRequestMapper =
                new GemItemSearchQueryRequestMapper(this.ItemSearchOptionsMock);
        }

        [TestCase("Vaal Flameblast")]
        [TestCase("Flameblast")]
        public void MapToQueryRequestShouldMapType(string expected)
        {
            GemItem item = new()
            {
                Type = expected,
            };

            SearchQueryRequest result = this.gemItemSearchQueryRequestMapper.MapToQueryRequest(item);

            Assert.That(result.Query.Type, Is.EqualTo(expected));
        }

        [Test]
        public void MapToQueryRequestShouldNotMapName()
        {
            const string expected = "Vaal Flameblast";
            GemItem item = new()
            {
                Name = expected,
            };

            SearchQueryRequest result = this.gemItemSearchQueryRequestMapper.MapToQueryRequest(item);

            Assert.That(result.Query.Name, Is.Null);
        }

        [Test]
        public void MapToQueryRequestShouldNotMapRarity()
        {
            GemItem item = new();

            SearchQueryRequest result = this.gemItemSearchQueryRequestMapper.MapToQueryRequest(item);

            Assert.That(result.Query.Filters.TypeFilters.Rarity, Is.Null);
        }

        [TestCase(10)]
        [TestCase(20)]
        public void MapToQueryRequestShouldMapGemLevel(int expected)
        {
            GemItem item = new()
            {
                Level = expected,
            };

            SearchQueryRequest result = this.gemItemSearchQueryRequestMapper.MapToQueryRequest(item);

            MinMaxFilter? gemLevelFilter = result.Query.Filters.MiscFilters.GemLevel;
            Assert.That(gemLevelFilter, Is.Not.Null);
            Assert.That(gemLevelFilter!.Min, Is.EqualTo(expected));
            Assert.That(gemLevelFilter.Max, Is.Null);
        }

        [TestCase(10)]
        [TestCase(20)]
        public void MapToQueryRequestShouldMapGemQuality(int expected)
        {
            GemItem item = new()
            {
                Quality = expected,
            };

            SearchQueryRequest result = this.gemItemSearchQueryRequestMapper.MapToQueryRequest(item);

            MinMaxFilter? qualityFilter = result.Query.Filters.MiscFilters.Quality;
            Assert.That(qualityFilter, Is.Not.Null);
            Assert.That(qualityFilter!.Min, Is.EqualTo(expected));
            Assert.That(qualityFilter.Max, Is.Null);
        }

        [TestCase(GemQualityType.Default)]
        [TestCase(GemQualityType.Anomalous)]
        [TestCase(GemQualityType.Divergent)]
        [TestCase(GemQualityType.Phantasmal)]
        public void MapToQueryRequestShouldMapGemQualityType(GemQualityType gemQualityType)
        {
            string expected = ((int)gemQualityType).ToString();
            GemItem item = new()
            {
                QualityType = gemQualityType,
            };

            SearchQueryRequest result = this.gemItemSearchQueryRequestMapper.MapToQueryRequest(item);

            OptionFilter? gemQualityTypeFilter = result.Query.Filters.MiscFilters.GemAlternateQuality;
            Assert.That(gemQualityTypeFilter, Is.Not.Null);
            Assert.That(gemQualityTypeFilter!.Option, Is.EqualTo(expected));
        }
    }
}