using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;
using FluentAssertions.Reactive;

using MediatR;

using Moq;

using NUnit.Framework;

using POETradeHelper.Common.UI.Models;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Exceptions;
using POETradeHelper.ItemSearch.Queries;
using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;
using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels.Abstractions;

using ReactiveUI;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Tests.ViewModels
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
            mediatorMock = new Mock<IMediator>();
            itemResultsViewModelFactoryMock = new Mock<Func<IScreen, IItemResultsViewModel>>();
            itemResultsViewModelFactoryMock
                .Setup(x => x.Invoke(It.IsAny<IScreen>()))
                .Returns(Mock.Of<IItemResultsViewModel>());
            exchangeResultsViewModelFactoryMock = new Mock<Func<IScreen, IExchangeResultsViewModel>>();
            exchangeResultsViewModelFactoryMock
                .Setup(x => x.Invoke(It.IsAny<IScreen>()))
                .Returns(Mock.Of<IExchangeResultsViewModel>());

            itemSearchOverlayViewModel = new ItemSearchResultOverlayViewModel(
                mediatorMock.Object,
                itemResultsViewModelFactoryMock.Object,
                exchangeResultsViewModelFactoryMock.Object);
        }

        [Test]
        public async Task SetListingForItemUnderCursorAsyncShouldSendGetItemFromCursorQuery()
        {
            CancellationTokenSource cts = new();

            await itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync(cts.Token);

            mediatorMock.Verify(x => x.Send(It.IsAny<GetItemFromCursorQuery>(), cts.Token));
        }

        [Test]
        public async Task SetListingForItemUnderCursorAsyncShouldSetItem()
        {
            EquippableItem item = new(ItemRarity.Magic);
            mediatorMock
                .Setup(x => x.Send(It.IsAny<GetItemFromCursorQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);

            await itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            itemSearchOverlayViewModel.Item.Should().Be(item);
        }

        [TestCaseSource(nameof(GetExchangeResultsViewModelItems))]
        public async Task SetListingForItemUnderCursorAsyncShouldNavigateToExchangeResultsView(Item item)
        {
            mediatorMock
                .Setup(x => x.Send(It.IsAny<GetItemFromCursorQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);
            IExchangeResultsViewModel expectedViewModel = Mock.Of<IExchangeResultsViewModel>();
            exchangeResultsViewModelFactoryMock
                .Setup(x => x.Invoke(itemSearchOverlayViewModel))
                .Returns(expectedViewModel);

            using var observer = itemSearchOverlayViewModel.Router.NavigateAndReset.Observe();

            await itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            await observer.Should().PushMatchAsync(x => x == expectedViewModel);
        }

        [TestCaseSource(nameof(GetExchangeResultsViewModelItems))]
        public async Task SetListingForItemUnderCursorAsyncShouldInitializeExchangeResultsView(Item item)
        {
            mediatorMock
                .Setup(x => x.Send(It.IsAny<GetItemFromCursorQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);
            Mock<IExchangeResultsViewModel> exchangeResultsViewModelMock = new();
            exchangeResultsViewModelFactoryMock
                .Setup(x => x.Invoke(itemSearchOverlayViewModel))
                .Returns(exchangeResultsViewModelMock.Object);

            CancellationToken token = new();

            await itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync(token);

            exchangeResultsViewModelMock
                .Verify(x => x.InitializeAsync(item, token));
        }

        [TestCaseSource(nameof(GetExchangeResultsViewModelItems))]
        public async Task SetListingForItemUnderCursorAsyncShouldUseExistingExchangeResultsViewModelFromNavigationStack(Item item)
        {
            mediatorMock
                .Setup(x => x.Send(It.IsAny<GetItemFromCursorQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);
            IExchangeResultsViewModel expectedViewModel = Mock.Of<IExchangeResultsViewModel>();
            itemSearchOverlayViewModel.Router.NavigationStack.Add(expectedViewModel);

            using var observer = itemSearchOverlayViewModel.Router.NavigateAndReset.Observe();

            await itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            observer.Should().NotPush();
            exchangeResultsViewModelFactoryMock
                .Verify(x => x.Invoke(It.IsAny<IScreen>()), Times.Never);
        }

        [TestCaseSource(nameof(GetItemResultsViewModelItems))]
        public async Task SetListingForItemUnderCursorAsyncShouldNavigateToItemResultsView(Item item)
        {
            mediatorMock
                .Setup(x => x.Send(It.IsAny<GetItemFromCursorQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);
            IItemResultsViewModel expectedViewModel = Mock.Of<IItemResultsViewModel>();
            itemResultsViewModelFactoryMock
                .Setup(x => x.Invoke(itemSearchOverlayViewModel))
                .Returns(expectedViewModel);

            using var observer = itemSearchOverlayViewModel.Router.NavigateAndReset.Observe();

            await itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            await observer.Should().PushMatchAsync(x => x == expectedViewModel);
        }

        [TestCaseSource(nameof(GetItemResultsViewModelItems))]
        public async Task SetListingForItemUnderCursorAsyncShouldInitializeItemResultsView(Item item)
        {
            mediatorMock
                .Setup(x => x.Send(It.IsAny<GetItemFromCursorQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);
            Mock<IItemResultsViewModel> itemResultsViewModelMock = new();
            itemResultsViewModelFactoryMock
                .Setup(x => x.Invoke(itemSearchOverlayViewModel))
                .Returns(itemResultsViewModelMock.Object);

            CancellationToken token = new();

            await itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync(token);

            itemResultsViewModelMock
                .Verify(x => x.InitializeAsync(item, token));
        }

        [TestCaseSource(nameof(GetItemResultsViewModelItems))]
        public async Task SetListingForItemUnderCursorAsyncShouldUseExistingItemsResultsViewModelFromNavigationStack(Item item)
        {
            mediatorMock
                .Setup(x => x.Send(It.IsAny<GetItemFromCursorQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);
            IItemResultsViewModel expectedViewModel = Mock.Of<IItemResultsViewModel>();
            itemSearchOverlayViewModel.Router.NavigationStack.Add(expectedViewModel);

            using var observer = itemSearchOverlayViewModel.Router.NavigateAndReset.Observe();

            await itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            observer.Should().NotPush();
            itemResultsViewModelFactoryMock
                .Verify(x => x.Invoke(It.IsAny<IScreen>()), Times.Never);
        }

        [Test]
        public async Task SetListingForItemUnderCursorAsyncShouldSetMessageIfExceptionOccurs()
        {
            Exception exception = new("Exception text");

            mediatorMock
                .Setup(x => x.Send(It.IsAny<GetItemFromCursorQuery>(), It.IsAny<CancellationToken>()))
                .Throws(exception);

            await itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            itemSearchOverlayViewModel.Message.Should().NotBeNull();
            itemSearchOverlayViewModel.Message.Type.Should().Be(MessageType.Error);
            itemSearchOverlayViewModel.Message.Text.Should().NotBeNullOrEmpty();
        }

        [Test]
        public async Task SetListingForItemUnderCursorAsyncShouldNotSetMessageIfInvalidItemStringExceptionOccurs()
        {
            var exception = new InvalidItemStringException("invalid item string");

            mediatorMock
                .Setup(x => x.Send(It.IsAny<GetItemFromCursorQuery>(), It.IsAny<CancellationToken>()))
                .Throws(exception);

            await itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            itemSearchOverlayViewModel.Message.Should().BeNull();
        }

        [Test]
        public async Task SetListingForItemUnderCursorAsyncShouldSetMessageToNull()
        {
            mediatorMock
                .SetupSequence(x => x.Send(It.IsAny<GetItemFromCursorQuery>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromException<Item>(new Exception()))
                .ReturnsAsync(new CurrencyItem());
            await itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();
            itemSearchOverlayViewModel.Message.Should().NotBeNull();

            await itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            itemSearchOverlayViewModel.Message.Should().BeNull();
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