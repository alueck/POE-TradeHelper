using FluentAssertions;

using NUnit.Framework;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services.Mappers;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.ItemSearch.Tests.Services.Mappers
{
    public class OrganItemSearchQueryRequestMapperTests : ItemSearchQueryRequestMapperTestsBase<OrganItem>
    {
        private readonly OrganItemSearchQueryRequestMapper organItemSearchQueryRequestMapper;

        public OrganItemSearchQueryRequestMapperTests()
        {
            this.ItemSearchQueryRequestMapper = this.organItemSearchQueryRequestMapper =
                new OrganItemSearchQueryRequestMapper(this.ItemSearchOptionsMock);
        }

        [Test]
        public void MapToQueryRequestShouldMapItemNameToTerm()
        {
            const string name = "Oriath's Virtue's Eye";
            OrganItem item = new()
            {
                Name = name,
            };

            SearchQueryRequest result = this.organItemSearchQueryRequestMapper.MapToQueryRequest(item);

            result.Query.Term.Should().Be(name);
            result.Query.Name.Should().BeNullOrEmpty();
            result.Query.Type.Should().BeNullOrEmpty();

            Assert.That(result.Query.Term, Is.EqualTo(name));
        }
    }
}