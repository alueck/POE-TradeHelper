using System;
using System.Threading;
using System.Threading.Tasks;

using Avalonia.Media;

using FluentAssertions;

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
        private readonly IOptionsMonitor<ItemSearchOptions> itemSearchOptionsMock;
        private readonly IMediator mediatorMock;
        private readonly IStaticDataService staticDataServiceMock;
        private readonly IImageService imageServiceMock;
        private PricePredictionViewModel viewModel;

        public PricePredictionViewModelTests()
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

        [Test]
        public async Task LoadAsyncShouldNotSendGetGetPoePricesInfoPredictionQueryIfPricePredictionIsDisabled()
        {
            // arrange
            EquippableItem item = new(ItemRarity.Rare)
            {
                ExtendedItemText = "abc",
            };

            this.itemSearchOptionsMock.CurrentValue
                .Returns(new ItemSearchOptions
                {
                    PricePredictionEnabled = false,
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
            EquippableItem item = new(ItemRarity.Rare)
            {
                ExtendedItemText = "text",
            };

            this.itemSearchOptionsMock.CurrentValue
                .Returns(new ItemSearchOptions
                {
                    PricePredictionEnabled = true,
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
            Action<ItemSearchOptions, string>? listener = null;
            this.itemSearchOptionsMock
                .When(x => x.OnChange(Arg.Any<Action<ItemSearchOptions, string?>>()))
                .Do(ctx => listener = ctx.Arg<Action<ItemSearchOptions, string?>>());

            this.viewModel = this.CreateViewModel();
            this.viewModel.Currency = "chaos";
            this.viewModel.Prediction = "0.25 - 0.33";
            this.viewModel.ConfidenceScore = "89 %";
            this.viewModel.CurrencyImage = Substitute.For<IImage>();

            // act
            listener?.Invoke(
                new ItemSearchOptions
                {
                    PricePredictionEnabled = false,
                },
                string.Empty);

            // assert
            this.viewModel.Should().BeEquivalentTo(this.CreateViewModel());
        }

        [Test]
        public async Task LoadAsyncShouldCatchExceptionFromGetPoePricesInfoPredictionQuery()
        {
            // arrange
            EquippableItem item = new(ItemRarity.Rare)
            {
                ExtendedItemText = "text",
            };

            this.itemSearchOptionsMock.CurrentValue
                .Returns(new ItemSearchOptions
                {
                    PricePredictionEnabled = true,
                });

            this.mediatorMock.Send(Arg.Any<GetPoePricesInfoPredictionQuery>(), Arg.Any<CancellationToken>())
                .Throws<Exception>();

            // act
            Func<Task> action = () => this.viewModel.LoadAsync(item, default);

            // assert
            await action.Should().NotThrowAsync();
        }

        private PricePredictionViewModel CreateViewModel() => new(
            this.itemSearchOptionsMock,
            this.mediatorMock,
            this.staticDataServiceMock,
            this.imageServiceMock);
    }
}