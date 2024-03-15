using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Logging;

using NSubstitute;
using NSubstitute.ExceptionExtensions;

using NUnit.Framework;

using POETradeHelper.Common;
using POETradeHelper.Common.Wrappers;
using POETradeHelper.PricePrediction.Models;
using POETradeHelper.PricePrediction.Services;

namespace POETradeHelper.PricePrediction.Tests.Services
{
    public class PoePricesInfoClientTests
    {
        private readonly IHttpClientWrapper httpClientMock;
        private readonly IJsonSerializerWrapper jsonSerializerMock;
        private readonly PoePricesInfoClient poePricesInfoClient;

        public PoePricesInfoClientTests()
        {
            this.httpClientMock = Substitute.For<IHttpClientWrapper>();

            this.httpClientMock.GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(new HttpResponseMessage
                {
                    Content = new StringContent(string.Empty),
                });

            IHttpClientFactoryWrapper? httpClientFactoryMock = Substitute.For<IHttpClientFactoryWrapper>();
            httpClientFactoryMock.CreateClient()
                .Returns(this.httpClientMock);

            this.jsonSerializerMock = Substitute.For<IJsonSerializerWrapper>();

            this.poePricesInfoClient = new PoePricesInfoClient(
                httpClientFactoryMock,
                this.jsonSerializerMock,
                Substitute.For<ILogger<PoePricesInfoClient>>());
        }

        [Test]
        public async Task GetPricePredictionAsyncShouldCallGetAsyncOnHttpClientWithCorrectUrl()
        {
            // arrange
            const string league = "Heist";
            const string itemText = "Scroll of Wisdom\r\nRarity: Currency";

            string? expectedUrl =
                $"https://www.poeprices.info/api?i={Convert.ToBase64String(Encoding.UTF8.GetBytes(itemText))}&l={league}";
            CancellationToken cancellationToken = new CancellationTokenSource().Token;

            // act
            await this.poePricesInfoClient.GetPricePredictionAsync(league, itemText, cancellationToken);

            // assert
            await this.httpClientMock
                .Received()
                .GetAsync(expectedUrl, cancellationToken);
        }

        [Test]
        public async Task GetPricePredictionAsyncShouldCallDeserializeOnJsonSerializerWithHttpContent()
        {
            // arrange
            const string json = "{ \"result\": [ ] }";

            this.httpClientMock.GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(new HttpResponseMessage
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json"),
                });

            // act
            await this.poePricesInfoClient.GetPricePredictionAsync("Heist", "Scroll of Wisdom");

            // assert
            this.jsonSerializerMock
                .Received()
                .Deserialize<PoePricesInfoPrediction>(
                    json,
                    Arg.Is<JsonSerializerOptions>(x => x.PropertyNamingPolicy == JsonNamingPolicy.SnakeCaseLower));
        }

        [Test]
        public async Task GetPricePredictionAsyncShouldNotCallDeserializeOnJsonSerializerForNonSuccessResponse()
        {
            // arrange
            this.httpClientMock.GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                });

            // act
            await this.poePricesInfoClient.GetPricePredictionAsync("Heist", "Scroll of Wisdom");

            // assert
            this.jsonSerializerMock
                .DidNotReceive()
                .Deserialize<PoePricesInfoPrediction>(Arg.Any<string>(), Arg.Any<JsonSerializerOptions>());
        }

        [Test]
        public async Task GetPricePredictionAsyncShouldReturnResultFromJsonSerializer()
        {
            // arrange
            PoePricesInfoPrediction? expected = new PoePricesInfoPrediction
            {
                Min = 0.15m,
                Max = 0.25m,
                Currency = "chaos",
                ConfidenceScore = 0.89744m,
            };

            this.jsonSerializerMock
                .Deserialize<PoePricesInfoPrediction>(Arg.Any<string>(), Arg.Any<JsonSerializerOptions>())
                .Returns(expected);

            // act
            PoePricesInfoPrediction? result =
                await this.poePricesInfoClient.GetPricePredictionAsync("Heist", "Scroll of Wisdom");

            // assert
            result.Should().Be(expected);
        }

        [Test]
        public async Task GetPricePredictionAsyncShouldReturnNullIfExceptionIsThrown()
        {
            // arrange
            this.httpClientMock
                .GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Throws<Exception>();

            // act
            PoePricesInfoPrediction? result =
                await this.poePricesInfoClient.GetPricePredictionAsync("Heist", "Scroll of Wisdom");

            // assert
            result.Should().BeNull();
        }

        [Test]
        public async Task GetPricePredictionAsyncShouldNotCatchOperationCanceledException()
        {
            this.httpClientMock
                .GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Throws<OperationCanceledException>();

            Func<Task> action = () => this.poePricesInfoClient.GetPricePredictionAsync("Heist", "Scroll of Wisdom");

            await action.Should().ThrowAsync<OperationCanceledException>();
        }

        [TestCase("Heist", "")]
        [TestCase("", "Scroll of Wisdom")]
        public async Task GetPricePredictionAsyncShouldReturnNullIf(string league, string itemText)
        {
            // act
            PoePricesInfoPrediction? result = await this.poePricesInfoClient.GetPricePredictionAsync(league, itemText);

            // assert
            result.Should().BeNull();
        }
    }
}