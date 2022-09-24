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
            this.ItemSearchQueryRequestMapper = this.flaskItemSearchQueryRequestMapper = new FlaskItemSearchQueryRequestMapper(this.ItemSearchOptionsMock.Object);
        }

        [Test]
        public void MapToQueryItemShouldMapItemType()
        {
            const string expectedType = "Divine Life Flask";
            var item = new FlaskItem(ItemRarity.Normal)
            {
                Type = expectedType
            };

            SearchQueryRequest result = this.flaskItemSearchQueryRequestMapper.MapToQueryRequest(item);

            Assert.That(result.Query.Type, Is.EqualTo(expectedType));
        }

        [Test]
        public void MapToQueryItemShouldMapItemNameForIdentfiedUniqueItem()
        {
            const string expected = "Rotgut";
            var item = new FlaskItem(ItemRarity.Unique)
            {
                Name = expected,
                IsIdentified = true
            };

            SearchQueryRequest result = this.flaskItemSearchQueryRequestMapper.MapToQueryRequest(item);

            Assert.That(result.Query.Name, Is.EqualTo(expected));
        }

        [Test]
        public void MapToQueryItemShouldNotMapItemNameForUnidentifiedUniqueItem()
        {
            var item = new FlaskItem(ItemRarity.Unique)
            {
                Type = "Divine Life Flask"
            };

            SearchQueryRequest result = this.flaskItemSearchQueryRequestMapper.MapToQueryRequest(item);

            Assert.IsNull(result.Query.Name);
        }

        [TestCaseSource(nameof(NonUniqueItemRarities))]
        public void MapToQueryItemShouldNotMapItemNameForNonUniqueItems(ItemRarity itemRarity)
        {
            var item = new FlaskItem(itemRarity)
            {
                Name = "Divine Life Flask"
            };

            SearchQueryRequest result = this.flaskItemSearchQueryRequestMapper.MapToQueryRequest(item);

            Assert.IsNull(result.Query.Name);
        }
    }
}