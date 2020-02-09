using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Constants;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Services;
using System.Collections.Generic;

namespace POETradeHelper.PathOfExileTradeApi.Tests.Services
{
    public class ProphecyItemSearchRequestMapperTests
    {
        private ProphecyItemSearchRequestMapper prophecyItemSearchRequestMapper;

        [SetUp]
        public void Setup()
        {
            this.prophecyItemSearchRequestMapper = new ProphecyItemSearchRequestMapper();
        }

        [Test]
        public void CanMapShouldReturnTrueForProphecyItem()
        {
            var item = new ProphecyItem();

            bool result = this.prophecyItemSearchRequestMapper.CanMap(item);

            Assert.IsTrue(result);
        }

        [TestCaseSource(nameof(NonProphecyItems))]
        public void CanMapShouldReturnFalseForNonProphecyItems(Item nonProphecyItem)
        {
            bool result = this.prophecyItemSearchRequestMapper.CanMap(nonProphecyItem);

            Assert.IsFalse(result);
        }

        [Test]
        public void MapToQueryItemShouldMapItemName()
        {
            const string expected = "The Dreamer's Dream";
            var item = new ProphecyItem()
            {
                Name = expected
            };

            SearchQueryRequest result = this.prophecyItemSearchRequestMapper.MapToQueryRequest(item);

            Assert.That(result.Query.Name, Is.EqualTo(expected));
        }

        [Test]
        public void MapToQueryRequestShouldMapItemType()
        {
            var item = new ProphecyItem();

            SearchQueryRequest result = this.prophecyItemSearchRequestMapper.MapToQueryRequest(item);

            Assert.That(result.Query.Type, Is.EqualTo(ItemTypeFilterOptions.Prophecy));
        }

        private static IEnumerable<Item> NonProphecyItems
        {
            get
            {
                yield return new CurrencyItem();
                yield return new DivinationCardItem();
                yield return new FlaskItem(ItemRarity.Normal);
                yield return new FragmentItem();
                yield return new GemItem();
                yield return new MapItem(ItemRarity.Normal);
                yield return new OrganItem();
                yield return new EquippableItem(ItemRarity.Normal);
            }
        }
    }
}