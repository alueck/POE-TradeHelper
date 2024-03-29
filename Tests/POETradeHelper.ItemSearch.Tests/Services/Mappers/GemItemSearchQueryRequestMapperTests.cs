﻿using FluentAssertions;
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
        public void MapToQueryRequestShouldNotMapName()
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
        public void MapToQueryRequestShouldNotMapRarity()
        {
            GemItem item = new();

            SearchQueryRequest result = this.gemItemSearchQueryRequestMapper.MapToQueryRequest(item);

            result.Query.Filters.TypeFilters.Rarity.Should().BeNull();
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
            gemLevelFilter.Should().BeEquivalentTo(new MinMaxFilter { Min = expected });
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
            qualityFilter.Should().BeEquivalentTo(new MinMaxFilter { Min = expected });
        }
    }
}