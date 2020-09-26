using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.PathOfExileTradeApi.Tests.Services
{
    public class ItemToExchangeQueryRequestMapperTests
    {
        private Mock<IStaticDataService> staticDataServiceMock;
        private ItemToExchangeQueryRequestMapper itemToExchangeQueryRequestMapper;

        [SetUp]
        public void Setup()
        {
            this.staticDataServiceMock = new Mock<IStaticDataService>();
            this.itemToExchangeQueryRequestMapper = new ItemToExchangeQueryRequestMapper(this.staticDataServiceMock.Object);
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

            this.staticDataServiceMock.Verify(x => x.GetId(item));
        }

        [Test]
        public void MapToQueryRequestShouldReturnExchangeQueryRequestWithIdFromStaticItemDataService()
        {
            const string expected = "item-id";
            var item = new CurrencyItem();

            this.staticDataServiceMock.Setup(x => x.GetId(It.IsAny<Item>()))
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