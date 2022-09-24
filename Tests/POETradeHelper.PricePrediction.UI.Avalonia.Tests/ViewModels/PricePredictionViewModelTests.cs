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

namespace POETradeHelper.PricePrediction.UI.Avalonia.Tests.ViewModels
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
            itemSearchOptionsMock = new Mock<IOptionsMonitor<ItemSearchOptions>>();
            itemSearchOptionsMock
                .Setup(x => x.CurrentValue)
                .Returns(new ItemSearchOptions());
            mediatorMock = new Mock<IMediator>();
            staticDataServiceMock = new Mock<IStaticDataService>();
            imageServiceMock = new Mock<IImageService>();

            viewModel = CreateViewModel();
        }

        private PricePredictionViewModel CreateViewModel()
        {
            return new PricePredictionViewModel(
                itemSearchOptionsMock.Object,
                mediatorMock.Object,
                staticDataServiceMock.Object,
                imageServiceMock.Object);
        }

        [Test]
        public async Task LoadAsyncShouldNotSendGetGetPoePricesInfoPredictionQueryIfPricePredictionIsDisabled()
        {
            // arrange
            var item = new EquippableItem(ItemRarity.Rare)
            {
                ItemText = "abc"
            };

            itemSearchOptionsMock.Setup(x => x.CurrentValue)
                .Returns(new ItemSearchOptions
                {
                    PricePredictionEnabled = false
                });

            // act
            await viewModel.LoadAsync(item, default);

            // assert
            mediatorMock.Verify(x => x.Send(It.IsAny<GetPoePricesInfoPredictionQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task LoadAsyncShouldNotSendGetPoePricesInfoPredictionQueryIfItemTextDidNotChange()
        {
            // arrange
            var item = new EquippableItem(ItemRarity.Rare)
            {
                ItemText = "text"
            };

            itemSearchOptionsMock.Setup(x => x.CurrentValue)
                .Returns(new ItemSearchOptions
                {
                    PricePredictionEnabled = true
                });

            // act
            await viewModel.LoadAsync(item, default);
            await viewModel.LoadAsync(item, default);

            // assert
            mediatorMock.Verify(x => x.Send(It.IsAny<GetPoePricesInfoPredictionQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void ShouldResetPricePredictionIfPricePredictionOptionChangesToDisabled()
        {
            // arrange
            Action<ItemSearchOptions, string> listener = null;
            itemSearchOptionsMock.Setup(x => x.OnChange(It.IsAny<Action<ItemSearchOptions, string>>()))
                .Callback<Action<ItemSearchOptions, string>>(l => listener = l);

            viewModel = CreateViewModel();
            viewModel.Currency = "chaos";
            viewModel.Prediction = "0.25 - 0.33";
            viewModel.ConfidenceScore = "89 %";
            viewModel.CurrencyImage = Mock.Of<IBitmap>();

            // act
            listener(new ItemSearchOptions
            {
                PricePredictionEnabled = false
            }, null);

            // assert
            Assert.That(viewModel.Currency, Is.Empty);
            Assert.That(viewModel.Prediction, Is.Empty);
            Assert.That(viewModel.ConfidenceScore, Is.Empty);
            Assert.That(viewModel.CurrencyImage, Is.Null);
            Assert.That(viewModel.HasValue, Is.False);
        }

        [Test]
        public async Task LoadAsyncShouldCatchExceptionFromGetPoePricesInfoPredictionQuery()
        {
            // arrange
            var item = new EquippableItem(ItemRarity.Rare)
            {
                ItemText = "text"
            };

            itemSearchOptionsMock.Setup(x => x.CurrentValue)
                .Returns(new ItemSearchOptions
                {
                    PricePredictionEnabled = true
                });

            mediatorMock.Setup(x => x.Send(It.IsAny<GetPoePricesInfoPredictionQuery>(), It.IsAny<CancellationToken>()))
                .Throws<Exception>();

            // act
            await viewModel.LoadAsync(item, default);
        }
    }
}
