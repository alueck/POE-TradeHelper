using System;
using System.Threading;
using System.Threading.Tasks;

using Avalonia.Media.Imaging;

using MediatR;

using Microsoft.Extensions.Options;

using NSubstitute;
using NSubstitute.ExceptionExtensions;

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
        private IOptionsMonitor<ItemSearchOptions> itemSearchOptionsMock;
        private IMediator mediatorMock;
        private IStaticDataService staticDataServiceMock;
        private IImageService imageServiceMock;
        private PricePredictionViewModel viewModel;

        [SetUp]
        public void Setup()
        {
            this.itemSearchOptionsMock = Substitute.For<IOptionsMonitor<ItemSearchOptions>>();
            this.itemSearchOptionsMock
                .CurrentValue
                .Returns(new ItemSearchOptions());
            this.mediatorMock = Substitute.For<IMediator>();
            this.staticDataServiceMock = Substitute.For<IStaticDataService>();
            this.imageServiceMock = Substitute.For<IImageService>();

            this.viewModel = this.CreateViewModel();
        }

        private PricePredictionViewModel CreateViewModel()
        {
            return new PricePredictionViewModel(this.itemSearchOptionsMock, this.mediatorMock, this.staticDataServiceMock, this.imageServiceMock);
        }

        [Test]
        public async Task LoadAsyncShouldNotSendGetGetPoePricesInfoPredictionQueryIfPricePredictionIsDisabled()
        {
            // arrange
            var item = new EquippableItem(ItemRarity.Rare)
            {
                ItemText = "abc"
            };

            this.itemSearchOptionsMock.CurrentValue
                .Returns(new ItemSearchOptions
                {
                    PricePredictionEnabled = false
                });

            // act
            await this.viewModel.LoadAsync(item, default);

            // assert
            await this.mediatorMock
                .DidNotReceive()
                .Send(Arg.Any<GetPoePricesInfoPredictionQuery>(), Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task LoadAsyncShouldNotSendGetPoePricesInfoPredictionQueryIfItemTextDidNotChange()
        {
            // arrange
            var item = new EquippableItem(ItemRarity.Rare)
            {
                ItemText = "text"
            };

            this.itemSearchOptionsMock.CurrentValue
                .Returns(new ItemSearchOptions
                {
                    PricePredictionEnabled = true
                });

            // act
            await this.viewModel.LoadAsync(item, default);
            await this.viewModel.LoadAsync(item, default);

            // assert
            await this.mediatorMock
                .Received(1)
                .Send(Arg.Any<GetPoePricesInfoPredictionQuery>(), Arg.Any<CancellationToken>());
        }

        [Test]
        public void ShouldResetPricePredictionIfPricePredictionOptionChangesToDisabled()
        {
            // arrange
            Action<ItemSearchOptions, string> listener = null;
            this.itemSearchOptionsMock
                .When(x => x.OnChange(Arg.Any<Action<ItemSearchOptions, string>>()))
                .Do(ctx => listener = ctx.Arg<Action<ItemSearchOptions, string>>());

            this.viewModel = this.CreateViewModel();
            this.viewModel.Currency = "chaos";
            this.viewModel.Prediction = "0.25 - 0.33";
            this.viewModel.ConfidenceScore = "89 %";
            this.viewModel.CurrencyImage = Substitute.For<IBitmap>();

            // act
            listener(new ItemSearchOptions
            {
                PricePredictionEnabled = false
            }, null);

            // assert
            Assert.That(this.viewModel.Currency, Is.Empty);
            Assert.That(this.viewModel.Prediction, Is.Empty);
            Assert.That(this.viewModel.ConfidenceScore, Is.Empty);
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

            this.itemSearchOptionsMock.CurrentValue
                .Returns(new ItemSearchOptions
                {
                    PricePredictionEnabled = true
                });

            this.mediatorMock.Send(Arg.Any<GetPoePricesInfoPredictionQuery>(), Arg.Any<CancellationToken>())
                .Throws<Exception>();

            // act
            await this.viewModel.LoadAsync(item, default);
        }
    }
}
