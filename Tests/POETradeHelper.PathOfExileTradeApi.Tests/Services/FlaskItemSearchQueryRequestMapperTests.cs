using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Services;
using System.Collections.Generic;

namespace POETradeHelper.PathOfExileTradeApi.Tests.Services
{
    public class FlaskItemSearchQueryRequestMapperTests
    {
        private FlaskItemSearchQueryRequestMapper flaskItemSearchQueryRequestMapper;

        [SetUp]
        public void Setup()
        {
            this.flaskItemSearchQueryRequestMapper = new FlaskItemSearchQueryRequestMapper();
        }

        [Test]
        public void CanMapShouldReturnTrueForFlaskItem()
        {
            var item = new FlaskItem(ItemRarity.Normal);

            bool result = this.flaskItemSearchQueryRequestMapper.CanMap(item);

            Assert.IsTrue(result);
        }

        [TestCaseSource(nameof(NonFlaskItems))]
        public void CanMapShouldReturnFalseForNonFlaskItems(Item nonFlaskItem)
        {
            bool result = this.flaskItemSearchQueryRequestMapper.CanMap(nonFlaskItem);

            Assert.IsFalse(result);
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

        private static IEnumerable<Item> NonFlaskItems
        {
            get
            {
                yield return new CurrencyItem();
                yield return new DivinationCardItem();
                yield return new FragmentItem();
                yield return new GemItem();
                yield return new MapItem(ItemRarity.Normal);
                yield return new OrganItem();
                yield return new ProphecyItem();
                yield return new EquippableItem(ItemRarity.Normal);
            }
        }

        private static IEnumerable<ItemRarity> NonUniqueItemRarities
        {
            get
            {
                yield return ItemRarity.Normal;
                yield return ItemRarity.Magic;
                yield return ItemRarity.Rare;
            }
        }
    }
}