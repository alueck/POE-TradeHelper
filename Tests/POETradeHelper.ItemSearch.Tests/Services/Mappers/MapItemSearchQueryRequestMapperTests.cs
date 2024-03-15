using FluentAssertions;
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
        private readonly MapItemSearchQueryRequestMapper mapItemSearchQueryRequestMapper;

        public MapItemSearchQueryRequestMapperTests()
        {
            this.ItemSearchQueryRequestMapper = this.mapItemSearchQueryRequestMapper =
                new MapItemSearchQueryRequestMapper(this.ItemSearchOptionsMock);
        }

        [Test]
        public void MapToQueryRequestShouldParseType()
        {
            const string expected = "Strand Map";
            MapItem item = new(ItemRarity.Normal)
            {
                Type = expected,
            };

            SearchQueryRequest result = this.mapItemSearchQueryRequestMapper.MapToQueryRequest(item);

            result.Query.Type.Should().NotBeNull();
            result.Query.Type!.Option.Should().Be(item.Type);
        }

        [Test]
        public void MapToQueryRequestShouldMapNameOfIdentifiedUniqueMap()
        {
            const string expected = "Pillars of Arun";
            MapItem item = new(ItemRarity.Unique)
            {
                Name = expected,
                IsIdentified = true,
            };

            SearchQueryRequest result = this.mapItemSearchQueryRequestMapper.MapToQueryRequest(item);

            result.Query.Name.Should().Be(expected);
        }

        [Test]
        public void MapToQueryRequestShouldNotMapNameOfUnidentifiedUniqueMap()
        {
            const string expected = "Pillars of Arun";
            MapItem item = new(ItemRarity.Unique)
            {
                Name = expected,
            };

            SearchQueryRequest result = this.mapItemSearchQueryRequestMapper.MapToQueryRequest(item);

            result.Query.Name.Should().BeNull();
        }

        [TestCaseSource(nameof(GetNonUniqueItemRarities))]
        public void MapToQueryRequestShouldNotMapNameOfNonUniqueItems(ItemRarity itemRarity)
        {
            MapItem item = new(itemRarity)
            {
                Name = "Pillars of Arun",
            };

            SearchQueryRequest result = this.mapItemSearchQueryRequestMapper.MapToQueryRequest(item);

            result.Query.Name.Should().BeNull();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void MapToQueryRequestShouldMapIdentified(bool identified)
        {
            MapItem item = new(ItemRarity.Rare)
            {
                IsIdentified = identified,
            };

            SearchQueryRequest result = this.mapItemSearchQueryRequestMapper.MapToQueryRequest(item);

            result.Query.Filters.MiscFilters.Identified.Should().BeEquivalentTo(new BoolOptionFilter { Option = identified });
        }

        [Test]
        public void MapToQueryItemShouldMapItemRarityForUniqueItems()
        {
            MapItem item = new(ItemRarity.Unique);

            SearchQueryRequest result = this.mapItemSearchQueryRequestMapper.MapToQueryRequest(item);

            result.Query.Filters.TypeFilters.Rarity.Should().BeEquivalentTo(new OptionFilter { Option = ItemRarityFilterOptions.Unique });
        }

        [TestCaseSource(nameof(GetNonUniqueItemRarities))]
        public void MapToQueryItemShouldMapItemRarityForNonUniqueItems(ItemRarity itemRarity)
        {
            MapItem item = new(itemRarity);

            SearchQueryRequest result = this.mapItemSearchQueryRequestMapper.MapToQueryRequest(item);

            result.Query.Filters.TypeFilters.Rarity.Should().BeEquivalentTo(new OptionFilter { Option = ItemRarityFilterOptions.NonUnique });
        }

        [TestCase(10)]
        [TestCase(15)]
        public void MapToQueryItemShouldMapMapTier(int mapTier)
        {
            MapItem item = new(ItemRarity.Normal)
            {
                Tier = mapTier,
            };

            SearchQueryRequest result = this.mapItemSearchQueryRequestMapper.MapToQueryRequest(item);

            MinMaxFilter? mapTierFilter = result.Query.Filters.MapFilters.MapTier;
            mapTierFilter.Should().BeEquivalentTo(new MinMaxFilter { Min = mapTier, Max = mapTier });
        }

        [TestCase(true)]
        [TestCase(false)]
        public void MapToQueryItemShouldMapBlighted(bool isBlighted)
        {
            MapItem item = new(ItemRarity.Normal)
            {
                IsBlighted = isBlighted,
            };

            SearchQueryRequest result = this.mapItemSearchQueryRequestMapper.MapToQueryRequest(item);

            BoolOptionFilter? blightedFilter = result.Query.Filters.MapFilters.MapBlighted;
            blightedFilter.Should().BeEquivalentTo(new BoolOptionFilter { Option = isBlighted });
        }

        [TestCase(true)]
        [TestCase(false)]
        public void MapToQueryItemShouldMapBlightRavaged(bool isBlightRavaged)
        {
            MapItem item = new(ItemRarity.Normal)
            {
                IsBlightRavaged = isBlightRavaged,
            };

            SearchQueryRequest result = this.mapItemSearchQueryRequestMapper.MapToQueryRequest(item);

            BoolOptionFilter? blightRavaged = result.Query.Filters.MapFilters.MapBlightRavaged;
            blightRavaged.Should().BeEquivalentTo(new BoolOptionFilter { Option = isBlightRavaged });
        }
    }
}