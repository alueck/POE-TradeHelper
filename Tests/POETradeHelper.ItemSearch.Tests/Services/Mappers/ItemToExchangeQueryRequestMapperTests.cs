using Microsoft.Extensions.Options;

using Moq;

using NUnit.Framework;

using POETradeHelper.ItemSearch.Contract;
using POETradeHelper.ItemSearch.Contract.Configuration;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services.Mappers;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.ItemSearch.Tests.Services.Mappers
{
    public class ItemToExchangeQueryRequestMapperTests
    {
        private Mock<IStaticDataService> staticDataServiceMock;
        private Mock<IOptionsMonitor<ItemSearchOptions>> itemSearchOptionsMock;
        private ItemToExchangeQueryRequestMapper itemToExchangeQueryRequestMapper;

        [SetUp]
        public void Setup()
        {
            this.staticDataServiceMock = new Mock<IStaticDataService>();
            this.itemSearchOptionsMock = new Mock<IOptionsMonitor<ItemSearchOptions>>();
            this.itemSearchOptionsMock.Setup(x => x.CurrentValue)
                .Returns(new ItemSearchOptions
                {
                    League = new League()
                });

            this.itemToExchangeQueryRequestMapper = new ItemToExchangeQueryRequestMapper(this.staticDataServiceMock.Object, this.itemSearchOptionsMock.Object);
        }

        [Test]
        public void MapToQueryRequestShouldMapLeague()
        {
            const string expected = "TestLeague";
            var item = new CurrencyItem();

            this.itemSearchOptionsMock.Setup(x => x.CurrentValue)
                .Returns(new ItemSearchOptions
                {
                    League = new League
                    {
                        Id = expected
                    }
                });

            ExchangeQueryRequest result = this.itemToExchangeQueryRequestMapper.MapToQueryRequest(item);

            Assert.That(result.League, Is.EqualTo(expected));
        }

        [Test]
        public void MapToQueryRequestShouldCallGetIdOnStaticItemDataService()
        {
            var item = new CurrencyItem
            {
                Name = "Scroll of Wisdom"
            };

            this.itemToExchangeQueryRequestMapper.MapToQueryRequest(item);

            this.staticDataServiceMock.Verify(x => x.GetId(item.Name));
        }

        [Test]
        public void MapToQueryRequestShouldReturnExchangeQueryRequestWithIdFromStaticItemDataService()
        {
            const string expected = "item-id";
            var item = new CurrencyItem();

            this.staticDataServiceMock.Setup(x => x.GetId(It.IsAny<string>()))
                .Returns(expected);

            var result = this.itemToExchangeQueryRequestMapper.MapToQueryRequest(item);

            Assert.NotNull(result);
            Assert.That(result.Query.Have, Has.Count.EqualTo(1));
            Assert.That(result.Query.Have, Has.One.EqualTo(expected));
        }

        [Test]
        public void MapToQueryRequestShouldReturnExchangeQueryRequestWithChaosAsWant()
        {
            var item = new CurrencyItem();

            var result = this.itemToExchangeQueryRequestMapper.MapToQueryRequest(item);

            Assert.NotNull(result);
            Assert.That(result.Query.Want, Has.Count.EqualTo(1));
            Assert.That(result.Query.Want, Has.One.EqualTo("chaos"));
        }
    }
}