using FluentAssertions;

using Moq;

using NUnit.Framework;

using POETradeHelper.ItemSearch.Contract;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PricePrediction.Models;
using POETradeHelper.PricePrediction.Queries;
using POETradeHelper.PricePrediction.Queries.Handlers;
using POETradeHelper.PricePrediction.Services;

namespace POETradeHelper.PricePrediction.Tests.Queries.Handlers
{
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class GetPoePricesInfoPredictionQueryHandlerTests
    {
        private readonly Mock<IPoePricesInfoClient> poePricesInfoClientMock;
        private readonly GetPoePricesInfoPredictionQueryHandler handler;
        
        private readonly GetPoePricesInfoPredictionQuery validRequest;

        public GetPoePricesInfoPredictionQueryHandlerTests()
        {
            this.poePricesInfoClientMock = new Mock<IPoePricesInfoClient>();
            this.handler = new GetPoePricesInfoPredictionQueryHandler(this.poePricesInfoClientMock.Object);

            var validItem = new EquippableItem(ItemRarity.Rare)
            {
                ItemText = "abc"
            };
            this.validRequest = new GetPoePricesInfoPredictionQuery(validItem, new League { Id = "Heist" });
        }
        
        [Test]
        public async Task HandleShouldCallGetPricePredictionAsyncOnPoePricesInfoClient()
        {
            // arrange
            var cancellationToken = new CancellationToken();

            // act
            await this.handler.Handle(this.validRequest, cancellationToken);

            // assert
            this.poePricesInfoClientMock.Verify(x => x.GetPricePredictionAsync(this.validRequest.League.Id, this.validRequest.Item.ItemText, cancellationToken));
        }

        [Test]
        public async Task HandleShouldReturnResult()
        {
            // arrange
            PoePricesInfoPrediction expected = GetPoePricesInfoItem();
            this.poePricesInfoClientMock
                .Setup(x => x.GetPricePredictionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);
            // act
            PoePricesInfoPrediction? result = await this.handler.Handle(this.validRequest, default);

            // assert
            result.Should().BeEquivalentTo(expected);
        }

        private static PoePricesInfoPrediction GetPoePricesInfoItem()
        {
            return new PoePricesInfoPrediction
            {
                Min = 0.15m,
                Max = 0.25m,
                Currency = "chaos",
                ConfidenceScore = 85.978m
            };
        }
    }
}