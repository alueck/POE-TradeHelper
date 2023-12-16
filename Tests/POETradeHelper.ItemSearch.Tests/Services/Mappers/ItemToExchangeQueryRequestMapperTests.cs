using Microsoft.Extensions.Options;

using NSubstitute;

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
        private readonly IStaticDataService staticDataServiceMock;
        private readonly IOptionsMonitor<ItemSearchOptions> itemSearchOptionsMock;
        private readonly ItemToExchangeQueryRequestMapper itemToExchangeQueryRequestMapper;

        public ItemToExchangeQueryRequestMapperTests()
        {
            this.staticDataServiceMock = Substitute.For<IStaticDataService>();
            this.itemSearchOptionsMock = Substitute.For<IOptionsMonitor<ItemSearchOptions>>();
            this.itemSearchOptionsMock.CurrentValue
                .Returns(
                    new ItemSearchOptions
                    {
                        League = new League(),
                    });

            this.itemToExchangeQueryRequestMapper = new ItemToExchangeQueryRequestMapper(
                this.staticDataServiceMock,
                this.itemSearchOptionsMock);
        }

        [Test]
        public void MapToQueryRequestShouldMapLeague()
        {
            const string expected = "TestLeague";
            CurrencyItem item = new();

            this.itemSearchOptionsMock.CurrentValue
                .Returns(
                    new ItemSearchOptions
                    {
                        League = new League
                        {
                            Id = expected,
                        },
                    });

            ExchangeQueryRequest result = this.itemToExchangeQueryRequestMapper.MapToQueryRequest(item);

            Assert.That(result.League, Is.EqualTo(expected));
        }

        [Test]
        public void MapToQueryRequestShouldCallGetIdOnStaticItemDataService()
        {
            CurrencyItem item = new()
            {
                Name = "Scroll of Wisdom",
            };

            this.itemToExchangeQueryRequestMapper.MapToQueryRequest(item);

            this.staticDataServiceMock
                .Received()
                .GetId(item.Name);
        }

        [Test]
        public void MapToQueryRequestShouldReturnExchangeQueryRequestWithIdFromStaticItemDataService()
        {
            const string expected = "item-id";
            CurrencyItem item = new();

            this.staticDataServiceMock.GetId(Arg.Any<string>())
                .Returns(expected);

            ExchangeQueryRequest result = this.itemToExchangeQueryRequestMapper.MapToQueryRequest(item);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Query.Have, Has.Count.EqualTo(1));
            Assert.That(result.Query.Have, Has.One.EqualTo(expected));
        }

        [Test]
        public void MapToQueryRequestShouldReturnExchangeQueryRequestWithChaosAsWant()
        {
            CurrencyItem item = new();

            ExchangeQueryRequest result = this.itemToExchangeQueryRequestMapper.MapToQueryRequest(item);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Query.Want, Has.Count.EqualTo(1));
            Assert.That(result.Query.Want, Has.One.EqualTo("chaos"));
        }
    }
}