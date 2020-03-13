using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.PathOfExileTradeApi.Tests.Services
{
    public class FlaskItemSearchQueryRequestMapperTests : ItemSearchQueryRequestMapperTestsBase<FlaskItem>
    {
        private FlaskItemSearchQueryRequestMapper flaskItemSearchQueryRequestMapper;

        [SetUp]
        public void Setup()
        {
            this.ItemSearchQueryRequestMapper = this.flaskItemSearchQueryRequestMapper = new FlaskItemSearchQueryRequestMapper();
        }

        [Test]
        public void MapToQueryItemShouldMapItemType()
        {
            const string expectedType = "Divine Life Flask";
            var item = new FlaskItem(ItemRarity.Normal)
            {
                Type = expectedType
            };

            SearchQueryRequest result = this.flaskItemSearchQueryRequestMapper.MapToQueryRequest(item) as SearchQueryRequest;

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

            SearchQueryRequest result = this.flaskItemSearchQueryRequestMapper.MapToQueryRequest(item) as SearchQueryRequest;

            Assert.That(result.Query.Name, Is.EqualTo(expected));
        }

        [Test]
        public void MapToQueryItemShouldNotMapItemNameForUnidentifiedUniqueItem()
        {
            var item = new FlaskItem(ItemRarity.Unique)
            {
                Type = "Divine Life Flask"
            };

            SearchQueryRequest result = this.flaskItemSearchQueryRequestMapper.MapToQueryRequest(item) as SearchQueryRequest;

            Assert.IsNull(result.Query.Name);
        }

        [TestCaseSource(nameof(NonUniqueItemRarities))]
        public void MapToQueryItemShouldNotMapItemNameForNonUniqueItems(ItemRarity itemRarity)
        {
            var item = new FlaskItem(itemRarity)
            {
                Name = "Divine Life Flask"
            };

            SearchQueryRequest result = this.flaskItemSearchQueryRequestMapper.MapToQueryRequest(item) as SearchQueryRequest;

            Assert.IsNull(result.Query.Name);
        }
    }
}