using Moq;
using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Services;
using System.Collections.Generic;

namespace POETradeHelper.PathOfExileTradeApi.Tests.Services
{
    public class ItemToExchangeQueryRequestMapperTests
    {
        private Mock<IStaticItemDataService> staticItemDataServiceMock;
        private ItemToExchangeQueryRequestMapper itemToExchangeQueryRequestMapper;

        [SetUp]
        public void Setup()
        {
            this.staticItemDataServiceMock = new Mock<IStaticItemDataService>();
            this.itemToExchangeQueryRequestMapper = new ItemToExchangeQueryRequestMapper(this.staticItemDataServiceMock.Object);
        }

        [TestCaseSource(nameof(SupportedItems))]
        public void CanMapShouldReturnTrueForSupportedItemTypes(Item item)
        {
            bool result = this.itemToExchangeQueryRequestMapper.CanMap(item);

            Assert.IsTrue(result);
        }

        [TestCaseSource(nameof(UnsupportedItems))]
        public void CanMapShouldReturnFalseForUnsupportedItemTypes(Item item)
        {
            bool result = this.itemToExchangeQueryRequestMapper.CanMap(item);

            Assert.IsFalse(result);
        }

        [Test]
        public void MapToQueryRequestShouldCallGetIdOnStaticItemDataService()
        {
            var item = new CurrencyItem();

            this.itemToExchangeQueryRequestMapper.MapToQueryRequest(item);

            this.staticItemDataServiceMock.Verify(x => x.GetId(item));
        }

        [Test]
        public void MapToQueryRequestShouldReturnExchangeQueryRequestWithIdFromStaticItemDataService()
        {
            const string expected = "item-id";
            var item = new CurrencyItem();

            this.staticItemDataServiceMock.Setup(x => x.GetId(It.IsAny<Item>()))
                .Returns(expected);

            var result = this.itemToExchangeQueryRequestMapper.MapToQueryRequest(item) as ExchangeQueryRequest;

            Assert.NotNull(result);
            Assert.That(result.Exchange.Want, Has.Count.EqualTo(1));
            Assert.That(result.Exchange.Want, Has.One.EqualTo(expected));
        }

        [Test]
        public void MapToQueryRequestShouldReturnExchangeQueryRequestWithChaosAsHave()
        {
            var item = new CurrencyItem();

            var result = this.itemToExchangeQueryRequestMapper.MapToQueryRequest(item) as ExchangeQueryRequest;

            Assert.NotNull(result);
            Assert.That(result.Exchange.Have, Has.Count.EqualTo(1));
            Assert.That(result.Exchange.Have, Has.One.EqualTo("chaos"));
        }

        private static IEnumerable<Item> SupportedItems
        {
            get
            {
                yield return new CurrencyItem();
                yield return new FragmentItem();
                yield return new DivinationCardItem();
            }
        }

        private static IEnumerable<Item> UnsupportedItems
        {
            get
            {
                yield return new FlaskItem(ItemRarity.Normal);
                yield return new MapItem(ItemRarity.Normal);
                yield return new OrganItem();
                yield return new ProphecyItem();
                yield return new EquippableItem(ItemRarity.Normal);
                yield return new GemItem();
            }
        }
    }
}