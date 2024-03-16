using FluentAssertions;

using NUnit.Framework;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services.Mappers;
using POETradeHelper.PathOfExileTradeApi.Constants;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;

namespace POETradeHelper.ItemSearch.Tests.Services.Mappers
{
    public class JewelItemSearchQueryRequestMapperTests : ItemSearchQueryRequestMapperTestsBase<JewelItem>
    {
        private readonly JewelItemSearchQueryRequestMapper jewelItemSearchQueryRequestMapper;

        public JewelItemSearchQueryRequestMapperTests()
        {
            this.ItemSearchQueryRequestMapper = this.jewelItemSearchQueryRequestMapper =
                new JewelItemSearchQueryRequestMapper(this.ItemSearchOptionsMock);
        }

        [Test]
        public void MapToQueryItemShouldMapItemType()
        {
            const string expectedType = "Cobalt Jewel";
            JewelItem item = new(ItemRarity.Magic)
            {
                Type = expectedType,
            };

            SearchQueryRequest result = this.jewelItemSearchQueryRequestMapper.MapToQueryRequest(item);

            result.Query.Type.Should().NotBeNull();
            result.Query.Type!.Option.Should().Be(item.Type);
        }

        [Test]
        public void MapToQueryItemShouldMapItemNameForIdentfiedUniqueItem()
        {
            const string expected = "Armageddon Joy";
            JewelItem item = new(ItemRarity.Unique)
            {
                Name = expected,
                IsIdentified = true,
            };

            SearchQueryRequest result = this.jewelItemSearchQueryRequestMapper.MapToQueryRequest(item);

            result.Query.Name.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void MapToQueryItemShouldNotMapItemNameForUnidentifiedUniqueItem()
        {
            JewelItem item = new(ItemRarity.Unique)
            {
                Type = "Cobalt Jewel",
            };

            SearchQueryRequest result = this.jewelItemSearchQueryRequestMapper.MapToQueryRequest(item);

            result.Query.Name.Should().BeNull();
        }

        [TestCaseSource(nameof(GetNonUniqueItemRarities))]
        public void MapToQueryItemShouldNotMapItemNameForNonUniqueItems(ItemRarity itemRarity)
        {
            JewelItem item = new(itemRarity)
            {
                Name = "Cobalt Jewel",
            };

            SearchQueryRequest result = this.jewelItemSearchQueryRequestMapper.MapToQueryRequest(item);

            result.Query.Name.Should().BeNull();
        }

        [Test]
        public void MapToQueryItemShouldMapItemRarityForUniqueItems()
        {
            JewelItem item = new(ItemRarity.Unique);

            SearchQueryRequest result = this.jewelItemSearchQueryRequestMapper.MapToQueryRequest(item);

            result.Query.Filters.TypeFilters.Rarity.Should().BeEquivalentTo(new OptionFilter { Option = ItemRarityFilterOptions.Unique });
        }

        [TestCaseSource(nameof(GetNonUniqueItemRarities))]
        public void MapToQueryItemShouldMapItemRarityForNonUniqueItems(ItemRarity itemRarity)
        {
            JewelItem item = new(itemRarity);

            SearchQueryRequest result = this.jewelItemSearchQueryRequestMapper.MapToQueryRequest(item);

            result.Query.Filters.TypeFilters.Rarity.Should().BeEquivalentTo(new OptionFilter { Option = ItemRarityFilterOptions.NonUnique });
        }

        [TestCase(true)]
        [TestCase(false)]
        public void MapToQueryRequestShouldMapSynthesised(bool synthesised)
        {
            JewelItem item = new(ItemRarity.Normal)
            {
                IsSynthesised = synthesised,
            };

            SearchQueryRequest result = this.jewelItemSearchQueryRequestMapper.MapToQueryRequest(item);

            result.Query.Filters.MiscFilters.SynthesisedItem.Should()
                .BeEquivalentTo(new BoolOptionFilter { Option = synthesised });
        }
    }
}