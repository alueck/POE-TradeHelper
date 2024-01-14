using FluentAssertions;
using NUnit.Framework;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services.Mappers;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.ItemSearch.Tests.Services.Mappers
{
    public class DivinationCardItemSearchQueryRequestMapperTests : ItemSearchQueryRequestMapperTestsBase<DivinationCardItem>
    {
        public DivinationCardItemSearchQueryRequestMapperTests()
        {
            this.ItemSearchQueryRequestMapper = new DivinationCardItemSearchQueryRequestMapper(this.ItemSearchOptionsMock);
        }

        [Test]
        public void MapToQueryRequestShouldMapItemType()
        {
            var item = new DivinationCardItem { Type = "Her Mask" };

            SearchQueryRequest result = this.ItemSearchQueryRequestMapper.MapToQueryRequest(item);

            result.Query.Type.Should().NotBeNull();
            result.Query.Type!.Option.Should().Be(item.Type);
        }
    }
}
