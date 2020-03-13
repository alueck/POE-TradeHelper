using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.PathOfExileTradeApi.Tests.Services
{
    public class OrganItemSearchQueryRequestMapperTests : ItemSearchQueryRequestMapperTestsBase<OrganItem>
    {
        private OrganItemSearchQueryRequestMapper organItemSearchQueryRequestMapper;

        [SetUp]
        public void Setup()
        {
            this.ItemSearchQueryRequestMapper = this.organItemSearchQueryRequestMapper = new OrganItemSearchQueryRequestMapper();
        }

        [Test]
        public void MapToQueryRequestShouldMapItemNameToTerm()
        {
            const string name = "Oriath's Virtue's Eye";
            var item = new OrganItem
            {
                Name = name
            };

            SearchQueryRequest result = this.organItemSearchQueryRequestMapper.MapToQueryRequest(item) as SearchQueryRequest;

            Assert.That(result.Query.Term, Is.EqualTo(name));

            Assert.That(result.Query.Name, Is.Null);
            Assert.That(result.Query.Type, Is.Null);
        }
    }
}