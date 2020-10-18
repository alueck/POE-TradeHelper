using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services.Mappers.Implementations;
using POETradeHelper.PathOfExileTradeApi.Constants;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.ItemSearch.Tests.Services.Mappers
{
    public class JewelItemSearchQueryRequestMapperTests : ItemSearchQueryRequestMapperTestsBase<JewelItem>
    {
        private JewelItemSearchQueryRequestMapper jewelItemSearchQueryRequestMapper;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            this.ItemSearchQueryRequestMapper = this.jewelItemSearchQueryRequestMapper = new JewelItemSearchQueryRequestMapper(this.ItemSearchOptionsMock.Object);
        }

        [Test]
        public void MapToQueryItemShouldMapItemType()
        {
            const string expectedType = "Cobalt Jewel";
            var item = new JewelItem(ItemRarity.Magic)
            {
                Type = expectedType
            };

            SearchQueryRequest result = this.jewelItemSearchQueryRequestMapper.MapToQueryRequest(item) as SearchQueryRequest;

            Assert.That(result.Query.Type, Is.EqualTo(expectedType));
        }

        [Test]
        public void MapToQueryItemShouldMapItemNameForIdentfiedUniqueItem()
        {
            const string expected = "Armageddon Joy";
            var item = new JewelItem(ItemRarity.Unique)
            {
                Name = expected,
                IsIdentified = true
            };

            SearchQueryRequest result = this.jewelItemSearchQueryRequestMapper.MapToQueryRequest(item) as SearchQueryRequest;

            Assert.That(result.Query.Name, Is.EqualTo(expected));
        }

        [Test]
        public void MapToQueryItemShouldNotMapItemNameForUnidentifiedUniqueItem()
        {
            var item = new JewelItem(ItemRarity.Unique)
            {
                Type = "Cobalt Jewel"
            };

            SearchQueryRequest result = this.jewelItemSearchQueryRequestMapper.MapToQueryRequest(item) as SearchQueryRequest;

            Assert.IsNull(result.Query.Name);
        }

        [TestCaseSource(nameof(NonUniqueItemRarities))]
        public void MapToQueryItemShouldNotMapItemNameForNonUniqueItems(ItemRarity itemRarity)
        {
            var item = new EquippableItem(itemRarity)
            {
                Name = "Dire Nock"
            };

            SearchQueryRequest result = this.jewelItemSearchQueryRequestMapper.MapToQueryRequest(item) as SearchQueryRequest;

            Assert.IsNull(result.Query.Name);
        }

        [Test]
        public void MapToQueryItemShouldMapItemRarityForUniqueItems()
        {
            var item = new JewelItem(ItemRarity.Unique);

            SearchQueryRequest result = this.jewelItemSearchQueryRequestMapper.MapToQueryRequest(item) as SearchQueryRequest;

            Assert.NotNull(result.Query.Filters.TypeFilters);
            Assert.That(result.Query.Filters.TypeFilters.Rarity.Option, Is.EqualTo(ItemRarityFilterOptions.Unique));
        }

        [TestCaseSource(nameof(NonUniqueItemRarities))]
        public void MapToQueryItemShouldMapItemRarityForNonUniqueItems(ItemRarity itemRarity)
        {
            var item = new JewelItem(itemRarity);

            SearchQueryRequest result = this.jewelItemSearchQueryRequestMapper.MapToQueryRequest(item) as SearchQueryRequest;

            Assert.NotNull(result.Query.Filters.TypeFilters);
            Assert.That(result.Query.Filters.TypeFilters.Rarity.Option, Is.EqualTo(ItemRarityFilterOptions.NonUnique));
        }
    }
}