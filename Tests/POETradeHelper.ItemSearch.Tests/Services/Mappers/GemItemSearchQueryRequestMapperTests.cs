using AwesomeAssertions;
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
        public void MapToQueryRequest_ShouldMapType(string expected)
        {
            GemItem item = new()
            {
                Type = expected,
                TypeDiscriminator = "alt_x",
            };

            SearchQueryRequest result = this.gemItemSearchQueryRequestMapper.MapToQueryRequest(item);

            result.Query.Type.Should().BeEquivalentTo(new TypeFilter
            {
                Option = expected,
                Discriminator = item.TypeDiscriminator,
            });
        }

        [Test]
        public void MapToQueryRequest_ShouldNotMapName()
        {
            const string expected = "Vaal Flameblast";
            GemItem item = new()
            {
                Name = expected,
            };

            SearchQueryRequest result = this.gemItemSearchQueryRequestMapper.MapToQueryRequest(item);

            result.Query.Name.Should().BeNull();
        }

        [Test]
        public void MapToQueryRequest_ShouldNotMapRarity()
        {
            GemItem item = new();

            SearchQueryRequest result = this.gemItemSearchQueryRequestMapper.MapToQueryRequest(item);

            result.Query.Filters.TypeFilters.Rarity.Should().BeNull();
        }

        [TestCase(10)]
        [TestCase(20)]
        public void MapToQueryRequest_ShouldMapGemLevel(int expected)
        {
            GemItem item = new()
            {
                Level = expected,
            };

            SearchQueryRequest result = this.gemItemSearchQueryRequestMapper.MapToQueryRequest(item);

            MinMaxFilter? gemLevelFilter = result.Query.Filters.MiscFilters.GemLevel;
            gemLevelFilter.Should().BeEquivalentTo(new MinMaxFilter { Min = expected });
        }

        [TestCase(10)]
        [TestCase(20)]
        public void MapToQueryRequest_ShouldMapGemQuality(int expected)
        {
            GemItem item = new()
            {
                Quality = expected,
            };

            SearchQueryRequest result = this.gemItemSearchQueryRequestMapper.MapToQueryRequest(item);

            MinMaxFilter? qualityFilter = result.Query.Filters.MiscFilters.Quality;
            qualityFilter.Should().BeEquivalentTo(new MinMaxFilter { Min = expected });
        }

        [TestCase(true)]
        [TestCase(false)]
        public void MapToQueryRequest_ShouldMapImbued(bool imbued)
        {
            GemItem item = new()
            {
                IsImbued = imbued,
            };

            SearchQueryRequest result = this.gemItemSearchQueryRequestMapper.MapToQueryRequest(item);

            BoolOptionFilter? imbuedFilter = result.Query.Filters.MiscFilters.GemImbued;
            imbuedFilter.Should().BeEquivalentTo(new BoolOptionFilter { Option = imbued });
        }

        [TestCase(true)]
        [TestCase(false)]
        public void MapToQueryRequest_ShouldMapTransfigured(bool transfigured)
        {
            GemItem item = new()
            {
                IsTransfigured = transfigured,
            };

            SearchQueryRequest result = this.gemItemSearchQueryRequestMapper.MapToQueryRequest(item);

            BoolOptionFilter? transfiguredFilter = result.Query.Filters.MiscFilters.GemTransfigured;
            transfiguredFilter.Should().BeEquivalentTo(new BoolOptionFilter { Option = transfigured });
        }

        [Test]
        public void MapToQueryRequest_ShouldMapImbuedStats()
        {
            ItemStat imbuedStat = new(StatCategory.Imbued) { Id = "imbued" };

            GemItem item = new()
            {
                Stats = new ItemStats
                {
                    AllStats = { imbuedStat },
                },
            };

            SearchQueryRequest result = this.gemItemSearchQueryRequestMapper.MapToQueryRequest(item);

            result.Query.Stats.Should().Contain(x => x.Filters.Count == 1 && x.Filters.Any(y => y.Id == imbuedStat.Id));
        }
    }
}