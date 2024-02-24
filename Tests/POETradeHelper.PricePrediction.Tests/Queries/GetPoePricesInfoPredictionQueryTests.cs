using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PricePrediction.Models;
using POETradeHelper.PricePrediction.Queries;
using POETradeHelper.PricePrediction.Services;

namespace POETradeHelper.PricePrediction.Tests.Queries
{
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class GetPoePricesInfoPredictionQueryTests
    {
        private readonly IPoePricesInfoClient poePricesInfoClientMock;
        private readonly GetPoePricesInfoPredictionQueryHandler handler;

        private readonly GetPoePricesInfoPredictionQuery validRequest;

        public GetPoePricesInfoPredictionQueryTests()
        {
            this.poePricesInfoClientMock = Substitute.For<IPoePricesInfoClient>();
            this.handler = new GetPoePricesInfoPredictionQueryHandler(this.poePricesInfoClientMock);

            EquippableItem? validItem = new EquippableItem(ItemRarity.Rare)
            {
                ExtendedItemText = @"
{ Extended stat info }
Stat text
(stat description)",
            };
            this.validRequest = new GetPoePricesInfoPredictionQuery(validItem, new League { Id = "Heist" });
        }

        [Test]
        public async Task HandleShouldCallGetPricePredictionAsyncOnPoePricesInfoClient()
        {
            // arrange
            CancellationToken cancellationToken = CancellationToken.None;

            // act
            await this.handler.Handle(this.validRequest, cancellationToken);

            // assert
            await this.poePricesInfoClientMock
                .Received()
                .GetPricePredictionAsync(this.validRequest.League.Id, this.validRequest.Item.PlainItemText, cancellationToken);
        }

        [Test]
        public async Task HandleShouldReturnResult()
        {
            // arrange
            PoePricesInfoPrediction expected = GetPoePricesInfoItem();
            this.poePricesInfoClientMock
                .GetPricePredictionAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(expected);

            // act
            PoePricesInfoPrediction? result = await this.handler.Handle(this.validRequest, default);

            // assert
            result.Should().BeEquivalentTo(expected);
        }

        private static PoePricesInfoPrediction GetPoePricesInfoItem() =>
            new()
            {
                Min = 0.15m,
                Max = 0.25m,
                Currency = "chaos",
                ConfidenceScore = 85.978m,
            };
    }
}