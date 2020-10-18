using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services.Mappers;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.ItemSearch.Tests.Services.Mappers
{
    public class OrganItemSearchQueryRequestMapperTests : ItemSearchQueryRequestMapperTestsBase<OrganItem>
    {
        private OrganItemSearchQueryRequestMapper organItemSearchQueryRequestMapper;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            this.ItemSearchQueryRequestMapper = this.organItemSearchQueryRequestMapper = new OrganItemSearchQueryRequestMapper(this.ItemSearchOptionsMock.Object);
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