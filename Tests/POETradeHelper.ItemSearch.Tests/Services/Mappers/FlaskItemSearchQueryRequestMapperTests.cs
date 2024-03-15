using FluentAssertions;

using NUnit.Framework;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services.Mappers;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.ItemSearch.Tests.Services.Mappers
{
    public class FlaskItemSearchQueryRequestMapperTests : ItemSearchQueryRequestMapperTestsBase<FlaskItem>
    {
        private readonly FlaskItemSearchQueryRequestMapper flaskItemSearchQueryRequestMapper;

        public FlaskItemSearchQueryRequestMapperTests()
        {
            this.ItemSearchQueryRequestMapper = this.flaskItemSearchQueryRequestMapper =
                new FlaskItemSearchQueryRequestMapper(this.ItemSearchOptionsMock);
        }

        [Test]
        public void MapToQueryItemShouldMapItemType()
        {
            const string expectedType = "Divine Life Flask";
            FlaskItem item = new(ItemRarity.Normal)
            {
                Type = expectedType,
            };

            SearchQueryRequest result = this.flaskItemSearchQueryRequestMapper.MapToQueryRequest(item);

            result.Query.Type.Should().NotBeNull();
            result.Query.Type!.Option.Should().Be(item.Type);
        }

        [Test]
        public void MapToQueryItemShouldMapItemNameForIdentfiedUniqueItem()
        {
            const string expected = "Rotgut";
            FlaskItem item = new(ItemRarity.Unique)
            {
                Name = expected,
                IsIdentified = true,
            };

            SearchQueryRequest result = this.flaskItemSearchQueryRequestMapper.MapToQueryRequest(item);

            result.Query.Name.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void MapToQueryItemShouldNotMapItemNameForUnidentifiedUniqueItem()
        {
            FlaskItem item = new(ItemRarity.Unique)
            {
                Type = "Divine Life Flask",
            };

            SearchQueryRequest result = this.flaskItemSearchQueryRequestMapper.MapToQueryRequest(item);

            result.Query.Name.Should().BeNull();
        }

        [TestCaseSource(nameof(GetNonUniqueItemRarities))]
        public void MapToQueryItemShouldNotMapItemNameForNonUniqueItems(ItemRarity itemRarity)
        {
            FlaskItem item = new(itemRarity)
            {
                Name = "Divine Life Flask",
            };

            SearchQueryRequest result = this.flaskItemSearchQueryRequestMapper.MapToQueryRequest(item);

            result.Query.Name.Should().BeNull();
        }
    }
}