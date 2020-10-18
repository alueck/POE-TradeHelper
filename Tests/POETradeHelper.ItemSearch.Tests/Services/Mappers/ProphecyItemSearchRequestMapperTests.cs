using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services.Mappers;
using POETradeHelper.PathOfExileTradeApi.Constants;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.ItemSearch.Tests.Services.Mappers
{
    public class ProphecyItemSearchRequestMapperTests : ItemSearchQueryRequestMapperTestsBase<ProphecyItem>
    {
        private ProphecyItemSearchRequestMapper prophecyItemSearchRequestMapper;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            this.ItemSearchQueryRequestMapper = this.prophecyItemSearchRequestMapper = new ProphecyItemSearchRequestMapper(this.ItemSearchOptionsMock.Object);
        }

        [Test]
        public void MapToQueryItemShouldMapItemName()
        {
            const string expected = "The Dreamer's Dream";
            var item = new ProphecyItem()
            {
                Name = expected
            };

            SearchQueryRequest result = this.prophecyItemSearchRequestMapper.MapToQueryRequest(item) as SearchQueryRequest;

            Assert.That(result.Query.Name, Is.EqualTo(expected));
        }

        [Test]
        public void MapToQueryRequestShouldMapItemType()
        {
            var item = new ProphecyItem();

            SearchQueryRequest result = this.prophecyItemSearchRequestMapper.MapToQueryRequest(item) as SearchQueryRequest;

            Assert.That(result.Query.Type, Is.EqualTo(ItemTypeFilterOptions.Prophecy));
        }
    }
}