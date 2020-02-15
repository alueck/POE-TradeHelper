using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Services;
using System.Collections.Generic;

namespace POETradeHelper.PathOfExileTradeApi.Tests.Services
{
    public class OrganItemSearchQueryRequestMapperTests
    {
        private OrganItemSearchQueryRequestMapper organItemSearchQueryRequestMapper;

        [SetUp]
        public void Setup()
        {
            this.organItemSearchQueryRequestMapper = new OrganItemSearchQueryRequestMapper();
        }

        [Test]
        public void CanMapShouldReturnTrueForOrganItem()
        {
            var item = new OrganItem();

            bool result = this.organItemSearchQueryRequestMapper.CanMap(item);

            Assert.IsTrue(result);
        }

        [TestCaseSource(nameof(NonOrganItems))]
        public void CanMapShouldReturnFalseForNonOrganItems(Item nonOrganItem)
        {
            bool result = this.organItemSearchQueryRequestMapper.CanMap(nonOrganItem);

            Assert.IsFalse(result);
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

        private static IEnumerable<Item> NonOrganItems
        {
            get
            {
                yield return new CurrencyItem();
                yield return new DivinationCardItem();
                yield return new FlaskItem(ItemRarity.Normal);
                yield return new FragmentItem();
                yield return new GemItem();
                yield return new MapItem(ItemRarity.Normal);
                yield return new ProphecyItem();
                yield return new EquippableItem(ItemRarity.Normal);
            }
        }
    }
}