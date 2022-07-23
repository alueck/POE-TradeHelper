using System;
using System.Collections;
using System.Drawing.Imaging;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;
using FluentAssertions.Reactive;

using MediatR;
using Moq;
using NUnit.Framework;
using POETradeHelper.Common.UI.Models;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Queries;
using POETradeHelper.ItemSearch.Exceptions;
using POETradeHelper.ItemSearch.Services.Factories;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Services;
using POETradeHelper.PricePrediction.ViewModels;

using ReactiveUI;

namespace POETradeHelper.ItemSearch.Tests.ViewModels
{
    public class ItemSearchOverlayViewModelTests
    {
        private Mock<IMediator> mediatorMock;
        private Mock<Func<IScreen, IItemResultsViewModel>> itemResultsViewModelFactoryMock;
        private Mock<Func<IScreen, IExchangeResultsViewModel>> exchangeResultsViewModelFactoryMock;
        private ItemSearchResultOverlayViewModel itemSearchOverlayViewModel;

        [SetUp]
        public void Setup()
        {
            this.mediatorMock = new Mock<IMediator>();
            this.itemResultsViewModelFactoryMock = new Mock<Func<IScreen, IItemResultsViewModel>>();
            this.itemResultsViewModelFactoryMock
                .Setup(x => x.Invoke(It.IsAny<IScreen>()))
                .Returns(Mock.Of<IItemResultsViewModel>());
            this.exchangeResultsViewModelFactoryMock = new Mock<Func<IScreen, IExchangeResultsViewModel>>();
            this.exchangeResultsViewModelFactoryMock
                .Setup(x => x.Invoke(It.IsAny<IScreen>()))
                .Returns(Mock.Of<IExchangeResultsViewModel>());

            this.itemSearchOverlayViewModel = new ItemSearchResultOverlayViewModel(
                this.mediatorMock.Object,
                this.itemResultsViewModelFactoryMock.Object,
                this.exchangeResultsViewModelFactoryMock.Object);
        }

        [Test]
        public async Task SetListingForItemUnderCursorAsyncShouldSendGetItemFromCursorQuery()
        {
            CancellationTokenSource cts = new();

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync(cts.Token);

            this.mediatorMock.Verify(x => x.Send(It.IsAny<GetItemFromCursorQuery>(), cts.Token));
        }
        
        [Test]
        public async Task SetListingForItemUnderCursorAsyncShouldSetItem()
        {
            EquippableItem item = new(ItemRarity.Magic);
            this.mediatorMock
                .Setup(x => x.Send(It.IsAny<GetItemFromCursorQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            this.itemSearchOverlayViewModel.Item.Should().Be(item);
        }

        [TestCaseSource(nameof(GetExchangeResultsViewModelItems))]
        public async Task SetListingForItemUnderCursorAsyncShouldNavigateToExchangeResultsView(Item item)
        {
            this.mediatorMock
                .Setup(x => x.Send(It.IsAny<GetItemFromCursorQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);
            IExchangeResultsViewModel expectedViewModel = Mock.Of<IExchangeResultsViewModel>();
            this.exchangeResultsViewModelFactoryMock
                .Setup(x => x.Invoke(this.itemSearchOverlayViewModel))
                .Returns(expectedViewModel);
            
            using var observer = this.itemSearchOverlayViewModel.Router.NavigateAndReset.Observe();

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            await observer.Should().PushMatchAsync(x => x == expectedViewModel);
        }
        
        [TestCaseSource(nameof(GetExchangeResultsViewModelItems))]
        public async Task SetListingForItemUnderCursorAsyncShouldInitializeExchangeResultsView(Item item)
        {
            this.mediatorMock
                .Setup(x => x.Send(It.IsAny<GetItemFromCursorQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);
            Mock<IExchangeResultsViewModel> exchangeResultsViewModelMock = new Mock<IExchangeResultsViewModel>();
            this.exchangeResultsViewModelFactoryMock
                .Setup(x => x.Invoke(this.itemSearchOverlayViewModel))
                .Returns(exchangeResultsViewModelMock.Object);

            CancellationToken token = new();

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync(token);

            exchangeResultsViewModelMock
                .Verify(x => x.InitializeAsync(item, token));
        }

        [TestCaseSource(nameof(GetExchangeResultsViewModelItems))]
        public async Task SetListingForItemUnderCursorAsyncShouldUseExistingExchangeResultsViewModelFromNavigationStack(Item item)
        {
            this.mediatorMock
                .Setup(x => x.Send(It.IsAny<GetItemFromCursorQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);
            IExchangeResultsViewModel expectedViewModel = Mock.Of<IExchangeResultsViewModel>();
            this.itemSearchOverlayViewModel.Router.NavigationStack.Add(expectedViewModel);
            
            using var observer = this.itemSearchOverlayViewModel.Router.NavigateAndReset.Observe();

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            observer.Should().NotPush();
            this.exchangeResultsViewModelFactoryMock
                .Verify(x => x.Invoke(It.IsAny<IScreen>()), Times.Never);
        }
        
        [TestCaseSource(nameof(GetItemResultsViewModelItems))]
        public async Task SetListingForItemUnderCursorAsyncShouldNavigateToItemResultsView(Item item)
        {
            this.mediatorMock
                .Setup(x => x.Send(It.IsAny<GetItemFromCursorQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);
            IItemResultsViewModel expectedViewModel = Mock.Of<IItemResultsViewModel>();
            this.itemResultsViewModelFactoryMock
                .Setup(x => x.Invoke(this.itemSearchOverlayViewModel))
                .Returns(expectedViewModel);
            
            using var observer = this.itemSearchOverlayViewModel.Router.NavigateAndReset.Observe();

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            await observer.Should().PushMatchAsync(x => x == expectedViewModel);
        }

        [TestCaseSource(nameof(GetItemResultsViewModelItems))]
        public async Task SetListingForItemUnderCursorAsyncShouldInitializeItemResultsView(Item item)
        {
            this.mediatorMock
                .Setup(x => x.Send(It.IsAny<GetItemFromCursorQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);
            Mock<IItemResultsViewModel> itemResultsViewModelMock = new Mock<IItemResultsViewModel>();
            this.itemResultsViewModelFactoryMock
                .Setup(x => x.Invoke(this.itemSearchOverlayViewModel))
                .Returns(itemResultsViewModelMock.Object);

            CancellationToken token = new();

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync(token);

            itemResultsViewModelMock
                .Verify(x => x.InitializeAsync(item, token));
        }
        
        [TestCaseSource(nameof(GetItemResultsViewModelItems))]
        public async Task SetListingForItemUnderCursorAsyncShouldUseExistingItemsResultsViewModelFromNavigationStack(Item item)
        {
            this.mediatorMock
                .Setup(x => x.Send(It.IsAny<GetItemFromCursorQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);
            IItemResultsViewModel expectedViewModel = Mock.Of<IItemResultsViewModel>();
            this.itemSearchOverlayViewModel.Router.NavigationStack.Add(expectedViewModel);

            using var observer = this.itemSearchOverlayViewModel.Router.NavigateAndReset.Observe();

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            observer.Should().NotPush();
            this.itemResultsViewModelFactoryMock
                .Verify(x => x.Invoke(It.IsAny<IScreen>()), Times.Never);
        }

        [Test]
        public async Task SetListingForItemUnderCursorAsyncShouldSetMessageIfExceptionOccurs()
        {
            Exception exception = new Exception("Exception text");
        
            this.mediatorMock
                .Setup(x => x.Send(It.IsAny<GetItemFromCursorQuery>(), It.IsAny<CancellationToken>()))
                .Throws(exception);
        
            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            this.itemSearchOverlayViewModel.Message.Should().NotBeNull();
            this.itemSearchOverlayViewModel.Message.Type.Should().Be(MessageType.Error);
            this.itemSearchOverlayViewModel.Message.Text.Should().NotBeNullOrEmpty();
        }

        [Test]
        public async Task SetListingForItemUnderCursorAsyncShouldNotSetMessageIfInvalidItemStringExceptionOccurs()
        {
            var exception = new InvalidItemStringException("invalid item string");
        
            this.mediatorMock
                .Setup(x => x.Send(It.IsAny<GetItemFromCursorQuery>(), It.IsAny<CancellationToken>()))
                .Throws(exception);
        
            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            this.itemSearchOverlayViewModel.Message.Should().BeNull();
        }

        [Test]
        public async Task SetListingForItemUnderCursorAsyncShouldSetMessageToNull()
        {
            this.mediatorMock
                .SetupSequence(x => x.Send(It.IsAny<GetItemFromCursorQuery>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromException<Item>(new Exception()))
                .ReturnsAsync(new CurrencyItem());
            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();
            this.itemSearchOverlayViewModel.Message.Should().NotBeNull();

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            this.itemSearchOverlayViewModel.Message.Should().BeNull();
        }

        private static IEnumerable GetExchangeResultsViewModelItems()
        {
            yield return new CurrencyItem();
            yield return new FragmentItem();
            yield return new DivinationCardItem();
        }
        
        private static IEnumerable GetItemResultsViewModelItems()
        {
            yield return new EquippableItem(ItemRarity.Rare);
            yield return new FlaskItem(ItemRarity.Magic);
            yield return new GemItem();
            yield return new JewelItem(ItemRarity.Rare);
            yield return new MapItem(ItemRarity.Rare);
            yield return new OrganItem();
            yield return new ProphecyItem();
        }
    }
}