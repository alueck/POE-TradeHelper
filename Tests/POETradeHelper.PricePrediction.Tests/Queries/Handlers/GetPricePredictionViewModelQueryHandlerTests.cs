using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using MediatR;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using POETradeHelper.Common.UI.Services;
using POETradeHelper.ItemSearch.Contract;
using POETradeHelper.ItemSearch.Contract.Configuration;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Services;
using POETradeHelper.PricePrediction.Models;
using POETradeHelper.PricePrediction.Queries;
using POETradeHelper.PricePrediction.Queries.Handlers;
using POETradeHelper.PricePrediction.ViewModels;

namespace POETradeHelper.PricePrediction.Tests.Queries.Handlers
{
    public class GetPricePredictionViewModelQueryHandlerTests
    {
        private Mock<IStaticDataService> staticDataServiceMock;
        private Mock<IImageService> imageServiceMock;
        private Mock<IMediator> mediatorMock;
        private Mock<IOptionsMonitor<ItemSearchOptions>> itemSearchOptionsMock;
        private GetPricePredictionViewModelQueryHandler pricePredictionViewModelFactory;

        private GetPricePredictionViewModelQuery validRequest;

        [SetUp]
        public void Setup()
        {
            this.itemSearchOptionsMock = new Mock<IOptionsMonitor<ItemSearchOptions>>();
            this.itemSearchOptionsMock.Setup(x => x.CurrentValue)
                .Returns(new ItemSearchOptions
                {
                    League = new League()
                });
            this.staticDataServiceMock = new Mock<IStaticDataService>();
            this.imageServiceMock = new Mock<IImageService>();
            this.mediatorMock = new Mock<IMediator>();
            
            this.pricePredictionViewModelFactory = new GetPricePredictionViewModelQueryHandler(
                this.itemSearchOptionsMock.Object,
                this.staticDataServiceMock.Object,
                this.imageServiceMock.Object,
                this.mediatorMock.Object);

            this.validRequest = new GetPricePredictionViewModelQuery(new EquippableItem(ItemRarity.Rare)
            {
                ItemText = "abc"
            });
        }

        [Test]
        public async Task HandleShouldSendGetPoePricesInfoPredictionQuery()
        {
            var league = new League { Id = "Heist" };
            this.itemSearchOptionsMock
                .Setup(x => x.CurrentValue)
                .Returns(new ItemSearchOptions { League = league });
            
            await this.pricePredictionViewModelFactory.Handle(this.validRequest, default);
            
            this.mediatorMock.Verify(x => x.Send(
                It.Is<GetPoePricesInfoPredictionQuery>(q => q.Item == this.validRequest.Item && q.League == league),
                It.IsAny<CancellationToken>()));
        }
        
        [Test]
        public async Task HandleShouldSetPredictionText()
        {
            // arrange
            var prediction = new PoePricesInfoPrediction
            {
                Min = 0.123m,
                Max = 0.234m
            };
            this.mediatorMock
                .Setup(x => x.Send(It.IsAny<GetPoePricesInfoPredictionQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(prediction);

            // act
            PricePredictionViewModel result = await this.pricePredictionViewModelFactory.Handle(this.validRequest, default);

            // assert
            Assert.That(result.Prediction, Is.EqualTo($"{prediction.Min:N}-{prediction.Max:N}"));
        }

        [Test]
        public async Task HandleShouldSetConfidenceScoreText()
        {
            // arrange
            var prediction = new PoePricesInfoPrediction
            {
                ConfidenceScore = 85.5651455m
            };
            this.mediatorMock
                .Setup(x => x.Send(It.IsAny<GetPoePricesInfoPredictionQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(prediction);

            // act
            PricePredictionViewModel result = await this.pricePredictionViewModelFactory.Handle(this.validRequest, default);

            // assert
            Assert.That(result.ConfidenceScore, Is.EqualTo($"{prediction.ConfidenceScore:N} %"));
        }

        [Test]
        public async Task HandleShouldCallGetTextOnStaticDataService()
        {
            // arrange
            var prediction = new PoePricesInfoPrediction
            {
                Currency = "chaos"
            };
            this.mediatorMock
                .Setup(x => x.Send(It.IsAny<GetPoePricesInfoPredictionQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(prediction);

            // act
            await this.pricePredictionViewModelFactory.Handle(this.validRequest, default);

            // assert
            this.staticDataServiceMock.Verify(x => x.GetText(prediction.Currency));
        }

        [Test]
        public async Task HandleShouldSetCurrencyTextToResultFromGetTextOnStaticDataService()
        {
            // arrange
            var prediction = new PoePricesInfoPrediction
            {
                Currency = "chaos"
            };
            this.mediatorMock
                .Setup(x => x.Send(It.IsAny<GetPoePricesInfoPredictionQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(prediction);

            const string expected = "Chaos Orb";
            this.staticDataServiceMock.Setup(x => x.GetText(It.IsAny<string>()))
                .Returns(expected);

            // act
            PricePredictionViewModel result = await this.pricePredictionViewModelFactory.Handle(this.validRequest, default);

            // assert
            Assert.That(result.Currency, Is.EqualTo(expected));
        }

        [Test]
        public async Task HandleShouldCallGetImageUrlOnStaticDataService()
        {
            // arrange
            var prediction = new PoePricesInfoPrediction
            {
                Currency = "chaos"
            };
            this.mediatorMock
                .Setup(x => x.Send(It.IsAny<GetPoePricesInfoPredictionQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(prediction);

            // act
            await this.pricePredictionViewModelFactory.Handle(this.validRequest, default);

            // assert
            this.staticDataServiceMock.Verify(x => x.GetImageUrl(prediction.Currency));
        }

        [Test]
        public async Task HandleShouldCallGetImageAsyncOnImageService()
        {
            // arrange
            var prediction = new PoePricesInfoPrediction();
            this.mediatorMock
                .Setup(x => x.Send(It.IsAny<GetPoePricesInfoPredictionQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(prediction);

            Uri imageUri = new Uri("https://www.google.de");
            this.staticDataServiceMock.Setup(x => x.GetImageUrl(It.IsAny<string>()))
                .Returns(imageUri);

            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            // act
            await this.pricePredictionViewModelFactory.Handle(this.validRequest, cancellationToken);

            // assert
            this.imageServiceMock.Verify(x => x.GetImageAsync(imageUri, cancellationToken));
        }

        [Test]
        public async Task HandleShouldSetCurrencyImageToResultFromImageService()
        {
            // arrange
            var prediction = new PoePricesInfoPrediction();
            this.mediatorMock
                .Setup(x => x.Send(It.IsAny<GetPoePricesInfoPredictionQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(prediction);
            
            IBitmap expectedImage = Mock.Of<IBitmap>();
            this.imageServiceMock.Setup(x => x.GetImageAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedImage);

            // act
            PricePredictionViewModel result = await this.pricePredictionViewModelFactory.Handle(this.validRequest, default);

            // assert
            Assert.That(result.CurrencyImage, Is.EqualTo(expectedImage));
        }

        [Test]
        public async Task HandleShouldReturnEmptyResultIfInputIsNull()
        {
            // act
            PricePredictionViewModel result = await this.pricePredictionViewModelFactory.Handle(new GetPricePredictionViewModelQuery(null), default);

            // assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.HasValue);
        }

        [TestCase(1)]
        [TestCase(-1)]
        public async Task HandleShouldReturnEmptyResultIfInputHasError(int errorCode)
        {
            // arrange
            var prediction = new PoePricesInfoPrediction
            {
                ErrorCode = errorCode
            };
            this.mediatorMock
                .Setup(x => x.Send(It.IsAny<GetPoePricesInfoPredictionQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(prediction);

            // act
            PricePredictionViewModel result = await this.pricePredictionViewModelFactory.Handle(this.validRequest, default);

            // assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.HasValue);
        }
    }
}