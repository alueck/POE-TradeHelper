using System;
using System.Threading;
using System.Threading.Tasks;

using Avalonia.Media.Imaging;

using MediatR;

using Microsoft.Extensions.Options;

using Moq;

using NUnit.Framework;

using POETradeHelper.Common.UI.Services;
using POETradeHelper.ItemSearch.Contract.Configuration;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Services;
using POETradeHelper.PricePrediction.Queries;
using POETradeHelper.PricePrediction.UI.Avalonia.ViewModels;

namespace POETradeHelper.PricePrediction.Tests.ViewModels
{
    public class PricePredictionViewModelTests
    {
        private Mock<IOptionsMonitor<ItemSearchOptions>> itemSearchOptionsMock;
        private Mock<IMediator> mediatorMock;
        private Mock<IStaticDataService> staticDataServiceMock;
        private Mock<IImageService> imageServiceMock;
        private PricePredictionViewModel viewModel;

        [SetUp]
        public void Setup()
        {
            this.itemSearchOptionsMock = new Mock<IOptionsMonitor<ItemSearchOptions>>();
            this.itemSearchOptionsMock
                .Setup(x => x.CurrentValue)
                .Returns(new ItemSearchOptions());
            this.mediatorMock = new Mock<IMediator>();
            this.staticDataServiceMock = new Mock<IStaticDataService>();
            this.imageServiceMock = new Mock<IImageService>();

            this.viewModel = CreateViewModel();
        }

        private PricePredictionViewModel CreateViewModel()
        {
            return new PricePredictionViewModel(
                this.itemSearchOptionsMock.Object,
                this.mediatorMock.Object,
                this.staticDataServiceMock.Object,
                this.imageServiceMock.Object);
        }

        [Test]
        public async Task LoadAsyncShouldNotSendGetGetPoePricesInfoPredictionQueryIfPricePredictionIsDisabled()
        {
            // arrange
            var item = new EquippableItem(ItemRarity.Rare)
            {
                ItemText = "abc"
            };

            this.itemSearchOptionsMock.Setup(x => x.CurrentValue)
                .Returns(new ItemSearchOptions
                {
                    PricePredictionEnabled = false
                });

            // act
            await this.viewModel.LoadAsync(item, default);

            // assert
            this.mediatorMock.Verify(x => x.Send(It.IsAny<GetPoePricesInfoPredictionQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task LoadAsyncShouldNotSendGetPoePricesInfoPredictionQueryIfItemTextDidNotChange()
        {
            // arrange
            var item = new EquippableItem(ItemRarity.Rare)
            {
                ItemText = "text"
            };

            this.itemSearchOptionsMock.Setup(x => x.CurrentValue)
                .Returns(new ItemSearchOptions
                {
                    PricePredictionEnabled = true
                });

            // act
            await this.viewModel.LoadAsync(item, default);
            await this.viewModel.LoadAsync(item, default);

            // assert
            this.mediatorMock.Verify(x => x.Send(It.IsAny<GetPoePricesInfoPredictionQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void ShouldResetPricePredictionIfPricePredictionOptionChangesToDisabled()
        {
            // arrange
            Action<ItemSearchOptions, string> listener = null;
            this.itemSearchOptionsMock.Setup(x => x.OnChange(It.IsAny<Action<ItemSearchOptions, string>>()))
                .Callback<Action<ItemSearchOptions, string>>(l => listener = l);

            this.viewModel = this.CreateViewModel();
            this.viewModel.Currency = "chaos";
            this.viewModel.Prediction = "0.25 - 0.33";
            this.viewModel.ConfidenceScore = "89 %";
            this.viewModel.CurrencyImage = Mock.Of<IBitmap>();

            // act
            listener(new ItemSearchOptions
            {
                PricePredictionEnabled = false
            }, null);

            // assert
            Assert.That(this.viewModel.Currency, Is.Null);
            Assert.That(this.viewModel.Prediction, Is.Null);
            Assert.That(this.viewModel.ConfidenceScore, Is.Null);
            Assert.That(this.viewModel.CurrencyImage, Is.Null);
            Assert.That(this.viewModel.HasValue, Is.False);
        }

        [Test]
        public async Task LoadAsyncShouldCatchExceptionFromGetPoePricesInfoPredictionQuery()
        {
            // arrange
            var item = new EquippableItem(ItemRarity.Rare)
            {
                ItemText = "text"
            };

            this.itemSearchOptionsMock.Setup(x => x.CurrentValue)
                .Returns(new ItemSearchOptions
                {
                    PricePredictionEnabled = true
                });

            this.mediatorMock.Setup(x => x.Send(It.IsAny<GetPoePricesInfoPredictionQuery>(), It.IsAny<CancellationToken>()))
                .Throws<Exception>();

            // act
            await this.viewModel.LoadAsync(item, default);
        }
    }
}