using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Moq;
using NUnit.Framework;
using POETradeHelper.Common.UI.Services;
using POETradeHelper.PathOfExileTradeApi.Services;
using POETradeHelper.PricePrediction.Models;
using POETradeHelper.PricePrediction.Services.Factories;
using POETradeHelper.PricePrediction.ViewModels;

namespace POETradeHelper.PricePrediction.Tests.Factories
{
    public class PricePredictionViewModelFactoryTests
    {
        private Mock<IImageService> imageServiceMock;
        private Mock<IStaticDataService> staticDataServiceMock;
        private PricePredictionViewModelFactory pricePredictionViewModelFactory;

        [SetUp]
        public void Setup()
        {
            this.imageServiceMock = new Mock<IImageService>();
            this.staticDataServiceMock = new Mock<IStaticDataService>();
            this.pricePredictionViewModelFactory = new PricePredictionViewModelFactory(this.imageServiceMock.Object, this.staticDataServiceMock.Object);
        }

        [Test]
        public async Task CreateAsyncShouldSetPredictionText()
        {
            // arrange
            var input = new PoePricesInfoItem
            {
                Min = 0.123m,
                Max = 0.234m
            };

            // act
            PricePredictionViewModel result = await this.pricePredictionViewModelFactory.CreateAsync(input);

            // assert
            Assert.That(result.Prediction, Is.EqualTo($"{input.Min:N}-{input.Max:N}"));
        }

        [Test]
        public async Task CreateAsyncShouldSetConfidenceScoreText()
        {
            // arrange
            var input = new PoePricesInfoItem
            {
                ConfidenceScore = 85.5651455m
            };

            // act
            PricePredictionViewModel result = await this.pricePredictionViewModelFactory.CreateAsync(input);

            // assert
            Assert.That(result.ConfidenceScore, Is.EqualTo($"{input.ConfidenceScore:N} %"));
        }

        [Test]
        public async Task CreateAsyncShouldCallGetTextOnStaticDataService()
        {
            // arrange
            var input = new PoePricesInfoItem
            {
                Currency = "chaos"
            };

            // act
            await this.pricePredictionViewModelFactory.CreateAsync(input);

            // assert
            this.staticDataServiceMock.Verify(x => x.GetText(input.Currency));
        }

        [Test]
        public async Task CreateAsyncShouldSetCurrencyTextToResultFromGetTextOnStaticDataService()
        {
            // arrange
            var input = new PoePricesInfoItem
            {
                Currency = "chaos"
            };

            const string expected = "Chaos Orb";
            this.staticDataServiceMock.Setup(x => x.GetText(It.IsAny<string>()))
                .Returns(expected);

            // act
            PricePredictionViewModel result = await this.pricePredictionViewModelFactory.CreateAsync(input);

            // assert
            Assert.That(result.Currency, Is.EqualTo(expected));
        }

        [Test]
        public async Task CreateAsyncShouldCallGetImageUrlOnStaticDataService()
        {
            // arrange
            var input = new PoePricesInfoItem
            {
                Currency = "chaos"
            };

            // act
            await this.pricePredictionViewModelFactory.CreateAsync(input);

            // assert
            this.staticDataServiceMock.Verify(x => x.GetImageUrl(input.Currency));
        }

        [Test]
        public async Task CreateAsyncShouldCallGetImageAsyncOnImageService()
        {
            // arrange
            var input = new PoePricesInfoItem();

            Uri imageUri = new Uri("https://www.google.de");
            this.staticDataServiceMock.Setup(x => x.GetImageUrl(It.IsAny<string>()))
                .Returns(imageUri);

            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            // act
            await this.pricePredictionViewModelFactory.CreateAsync(input, cancellationToken);

            // assert
            this.imageServiceMock.Verify(x => x.GetImageAsync(imageUri, cancellationToken));
        }

        [Test]
        public async Task CreateAsyncShouldSetCurrencyImageToResultFromImageService()
        {
            // arrange
            var input = new PoePricesInfoItem();
            IBitmap expectedImage = Mock.Of<IBitmap>();
            this.imageServiceMock.Setup(x => x.GetImageAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedImage);

            // act
            PricePredictionViewModel result = await this.pricePredictionViewModelFactory.CreateAsync(input);

            // assert
            Assert.That(result.CurrencyImage, Is.EqualTo(expectedImage));
        }

        [Test]
        public async Task CreateAsyncShouldReturnEmptyResultIfInputIsNull()
        {
            // act
            PricePredictionViewModel result = await this.pricePredictionViewModelFactory.CreateAsync(null);

            // assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.HasValue);
        }

        [TestCase(1)]
        [TestCase(-1)]
        public async Task CreateAsyncShouldReturnEmptyResultIfInputHasError(int errorCode)
        {
            // arrange
            var input = new PoePricesInfoItem
            {
                ErrorCode = errorCode
            };

            // act
            PricePredictionViewModel result = await this.pricePredictionViewModelFactory.CreateAsync(input);

            // assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.HasValue);
        }
    }
}