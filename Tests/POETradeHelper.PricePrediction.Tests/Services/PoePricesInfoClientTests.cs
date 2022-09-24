using System.Net;
using System.Text;
using System.Text.Json;

using Microsoft.Extensions.Logging;

using Moq;

using NUnit.Framework;

using POETradeHelper.Common;
using POETradeHelper.Common.Wrappers;
using POETradeHelper.PricePrediction.Models;
using POETradeHelper.PricePrediction.Services;

namespace POETradeHelper.PricePrediction.Tests.Services
{
    public class PoePricesInfoClientTests
    {
        private readonly Mock<IHttpClientWrapper> httpClientMock;
        private readonly Mock<IJsonSerializerWrapper> jsonSerializerMock;
        private readonly PoePricesInfoClient poePricesInfoClient;

        public PoePricesInfoClientTests()
        {
            this.httpClientMock = new Mock<IHttpClientWrapper>();

            this.httpClientMock.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HttpResponseMessage
                {
                    Content = new StringContent("")
                });

            var httpClientFactoryMock = new Mock<IHttpClientFactoryWrapper>();
            httpClientFactoryMock.Setup(x => x.CreateClient())
                .Returns(this.httpClientMock.Object);

            this.jsonSerializerMock = new Mock<IJsonSerializerWrapper>();

            this.poePricesInfoClient = new PoePricesInfoClient(httpClientFactoryMock.Object, this.jsonSerializerMock.Object, Mock.Of<ILogger<PoePricesInfoClient>>());
        }

        [Test]
        public async Task GetPricePredictionAsyncShouldCallGetAsyncOnHttpClientWithCorrectUrl()
        {
            // arrange
            const string league = "Heist";
            const string itemText = "Scroll of Wisdom\r\nRarity: Currency";

            var expectedUrl = $"https://www.poeprices.info/api?i={Convert.ToBase64String(Encoding.UTF8.GetBytes(itemText))}&l={league}";
            var cancellationToken = new CancellationTokenSource().Token;

            // act
            await this.poePricesInfoClient.GetPricePredictionAsync(league, itemText, cancellationToken);

            // assert
            this.httpClientMock.Verify(x => x.GetAsync(expectedUrl, cancellationToken));
        }

        [Test]
        public async Task GetPricePredictionAsyncShouldCallDeserializeOnJsonSerializerWithHttpContent()
        {
            // arrange
            const string json = "{ \"result\": [ ] }";

            this.httpClientMock.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HttpResponseMessage
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                });

            // act
            await this.poePricesInfoClient.GetPricePredictionAsync("Heist", "Scroll of Wisdom");

            // assert
            this.jsonSerializerMock.Verify(x => x.Deserialize<PoePricesInfoPrediction>(json, It.Is<JsonSerializerOptions>(x => x.PropertyNamingPolicy!.GetType() == typeof(JsonSnakeCaseNamingPolicy))));
        }

        [Test]
        public async Task GetPricePredictionAsyncShouldNotCallDeserializeOnJsonSerializerForNonSuccessResponse()
        {
            // arrange
            this.httpClientMock.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest
                });

            // act
            await this.poePricesInfoClient.GetPricePredictionAsync("Heist", "Scroll of Wisdom");

            // assert
            this.jsonSerializerMock.Verify(x => x.Deserialize<PoePricesInfoPrediction>(It.IsAny<string>(), It.IsAny<JsonSerializerOptions>()), Times.Never);
        }

        [Test]
        public async Task GetPricePredictionAsyncShouldReturnResultFromJsonSerializer()
        {
            // arrange
            var expected = new PoePricesInfoPrediction
            {
                Min = 0.15m,
                Max = 0.25m,
                Currency = "chaos",
                ConfidenceScore = 0.89744m
            };

            this.jsonSerializerMock.Setup(x => x.Deserialize<PoePricesInfoPrediction>(It.IsAny<string>(), It.IsAny<JsonSerializerOptions>()))
                .Returns(expected);

            // act
            PoePricesInfoPrediction? result = await this.poePricesInfoClient.GetPricePredictionAsync("Heist", "Scroll of Wisdom");

            // assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public async Task GetPricePredictionAsyncShouldReturnNullIfExceptionIsThrown()
        {
            // arrange
            this.httpClientMock
                .Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Throws<Exception>();

            // act
            PoePricesInfoPrediction? result = await this.poePricesInfoClient.GetPricePredictionAsync("Heist", "Scroll of Wisdom");

            // assert
            Assert.IsNull(result);
        }

        [Test]
        public void GetPricePredictionAsyncShouldNotCatchOperationCanceledException()
        {
            this.httpClientMock
                .Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Throws<OperationCanceledException>();

            async Task Action() => await this.poePricesInfoClient.GetPricePredictionAsync("Heist", "Scroll of Wisdom");

            Assert.ThrowsAsync<OperationCanceledException>(Action);
        }

        [TestCase("Heist", "")]
        [TestCase("Heist", null)]
        [TestCase("", "Scroll of Wisdom")]
        [TestCase(null, "Scroll of Wisdom")]
        public async Task GetPricePredictionAsyncShouldReturnNullIf(string league, string itemText)
        {
            // act
            PoePricesInfoPrediction? result = await this.poePricesInfoClient.GetPricePredictionAsync(league, itemText);

            // assert
            Assert.IsNull(result);
        }
    }
}
