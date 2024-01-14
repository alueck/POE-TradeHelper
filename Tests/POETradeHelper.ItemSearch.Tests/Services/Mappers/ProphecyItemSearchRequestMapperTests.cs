using FluentAssertions;
using NUnit.Framework;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services.Mappers;
using POETradeHelper.PathOfExileTradeApi.Constants;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.ItemSearch.Tests.Services.Mappers
{
    public class ProphecyItemSearchRequestMapperTests : ItemSearchQueryRequestMapperTestsBase<ProphecyItem>
    {
        private readonly ProphecyItemSearchRequestMapper prophecyItemSearchRequestMapper;

        public ProphecyItemSearchRequestMapperTests()
        {
            this.ItemSearchQueryRequestMapper = this.prophecyItemSearchRequestMapper =
                new ProphecyItemSearchRequestMapper(this.ItemSearchOptionsMock);
        }

        [Test]
        public void MapToQueryItemShouldMapItemName()
        {
            const string expected = "The Dreamer's Dream";
            ProphecyItem item = new()
            {
                Name = expected,
            };

            SearchQueryRequest result = this.prophecyItemSearchRequestMapper.MapToQueryRequest(item);

            result.Query.Name.Should().Be(expected);
        }

        [Test]
        public void MapToQueryRequestShouldMapItemType()
        {
            ProphecyItem item = new();

            SearchQueryRequest result = this.prophecyItemSearchRequestMapper.MapToQueryRequest(item);

            result.Query.Type.Should().NotBeNull();
            result.Query.Type!.Option.Should().Be(ItemTypeFilterOptions.Prophecy);
        }
    }
}