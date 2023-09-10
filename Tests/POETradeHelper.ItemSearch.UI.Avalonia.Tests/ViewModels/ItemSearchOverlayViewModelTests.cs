using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;
using FluentAssertions.Reactive;

using MediatR;

using NSubstitute;
using NSubstitute.ExceptionExtensions;

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
        private IMediator mediatorMock;
        private Func<IScreen, IItemResultsViewModel> itemResultsViewModelFactoryMock;
        private Func<IScreen, IExchangeResultsViewModel> exchangeResultsViewModelFactoryMock;
        private ItemSearchResultOverlayViewModel itemSearchOverlayViewModel;

        [SetUp]
        public void Setup()
        {
            this.mediatorMock = Substitute.For<IMediator>();
            this.itemResultsViewModelFactoryMock = Substitute.For<Func<IScreen, IItemResultsViewModel>>();
            this.itemResultsViewModelFactoryMock
                .Invoke(Arg.Any<IScreen>())
                .Returns(Substitute.For<IItemResultsViewModel>());
            this.exchangeResultsViewModelFactoryMock = Substitute.For<Func<IScreen, IExchangeResultsViewModel>>();
            this.exchangeResultsViewModelFactoryMock
                .Invoke(Arg.Any<IScreen>())
                .Returns(Substitute.For<IExchangeResultsViewModel>());

            this.itemSearchOverlayViewModel = new ItemSearchResultOverlayViewModel(this.mediatorMock, this.itemResultsViewModelFactoryMock, this.exchangeResultsViewModelFactoryMock);
        }

        [Test]
        public async Task SetListingForItemUnderCursorAsyncShouldSendGetItemFromCursorQuery()
        {
            CancellationTokenSource cts = new();

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync(cts.Token);

            await this.mediatorMock
                .Received()
                .Send(Arg.Any<GetItemFromCursorQuery>(), cts.Token);
        }

        [Test]
        public async Task SetListingForItemUnderCursorAsyncShouldSetItem()
        {
            EquippableItem item = new(ItemRarity.Magic);
            this.mediatorMock
                .Send(Arg.Any<GetItemFromCursorQuery>(), Arg.Any<CancellationToken>())
                .Returns(item);

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            this.itemSearchOverlayViewModel.Item.Should().Be(item);
        }

        [TestCaseSource(nameof(GetExchangeResultsViewModelItems))]
        public async Task SetListingForItemUnderCursorAsyncShouldNavigateToExchangeResultsView(Item item)
        {
            this.mediatorMock
                .Send(Arg.Any<GetItemFromCursorQuery>(), Arg.Any<CancellationToken>())
                .Returns(item);
            IExchangeResultsViewModel expectedViewModel = Substitute.For<IExchangeResultsViewModel>();
            this.exchangeResultsViewModelFactoryMock
                .Invoke(this.itemSearchOverlayViewModel)
                .Returns(expectedViewModel);

            using var observer = this.itemSearchOverlayViewModel.Router.NavigateAndReset.Observe();

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            await observer.Should().PushMatchAsync(x => x == expectedViewModel);
        }

        [TestCaseSource(nameof(GetExchangeResultsViewModelItems))]
        public async Task SetListingForItemUnderCursorAsyncShouldInitializeExchangeResultsView(Item item)
        {
            this.mediatorMock
                .Send(Arg.Any<GetItemFromCursorQuery>(), Arg.Any<CancellationToken>())
                .Returns(item);
            IExchangeResultsViewModel exchangeResultsViewModelMock = Substitute.For<IExchangeResultsViewModel>();
            this.exchangeResultsViewModelFactoryMock
                .Invoke(this.itemSearchOverlayViewModel)
                .Returns(exchangeResultsViewModelMock);

            CancellationToken token = new();

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync(token);

            await exchangeResultsViewModelMock
                .Received()
                .InitializeAsync(item, token);
        }

        [TestCaseSource(nameof(GetExchangeResultsViewModelItems))]
        public async Task SetListingForItemUnderCursorAsyncShouldUseExistingExchangeResultsViewModelFromNavigationStack(Item item)
        {
            this.mediatorMock
                .Send(Arg.Any<GetItemFromCursorQuery>(), Arg.Any<CancellationToken>())
                .Returns(item);
            IExchangeResultsViewModel expectedViewModel = Substitute.For<IExchangeResultsViewModel>();
            this.itemSearchOverlayViewModel.Router.NavigationStack.Add(expectedViewModel);

            using var observer = this.itemSearchOverlayViewModel.Router.NavigateAndReset.Observe();

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            observer.Should().NotPush();
            this.exchangeResultsViewModelFactoryMock
                .DidNotReceive()
                .Invoke(Arg.Any<IScreen>());
        }

        [TestCaseSource(nameof(GetItemResultsViewModelItems))]
        public async Task SetListingForItemUnderCursorAsyncShouldNavigateToItemResultsView(Item item)
        {
            this.mediatorMock
                .Send(Arg.Any<GetItemFromCursorQuery>(), Arg.Any<CancellationToken>())
                .Returns(item);
            IItemResultsViewModel expectedViewModel = Substitute.For<IItemResultsViewModel>();
            this.itemResultsViewModelFactoryMock
                .Invoke(this.itemSearchOverlayViewModel)
                .Returns(expectedViewModel);

            using var observer = this.itemSearchOverlayViewModel.Router.NavigateAndReset.Observe();

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            await observer.Should().PushMatchAsync(x => x == expectedViewModel);
        }

        [TestCaseSource(nameof(GetItemResultsViewModelItems))]
        public async Task SetListingForItemUnderCursorAsyncShouldInitializeItemResultsView(Item item)
        {
            this.mediatorMock
                .Send(Arg.Any<GetItemFromCursorQuery>(), Arg.Any<CancellationToken>())
                .Returns(item);
            IItemResultsViewModel itemResultsViewModelMock = Substitute.For<IItemResultsViewModel>();
            this.itemResultsViewModelFactoryMock
                .Invoke(this.itemSearchOverlayViewModel)
                .Returns(itemResultsViewModelMock);

            CancellationToken token = new();

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync(token);

            await itemResultsViewModelMock
                .Received()
                .InitializeAsync(item, token);
        }

        [TestCaseSource(nameof(GetItemResultsViewModelItems))]
        public async Task SetListingForItemUnderCursorAsyncShouldUseExistingItemsResultsViewModelFromNavigationStack(Item item)
        {
            this.mediatorMock
                .Send(Arg.Any<GetItemFromCursorQuery>(), Arg.Any<CancellationToken>())
                .Returns(item);
            IItemResultsViewModel expectedViewModel = Substitute.For<IItemResultsViewModel>();
            this.itemSearchOverlayViewModel.Router.NavigationStack.Add(expectedViewModel);

            using var observer = this.itemSearchOverlayViewModel.Router.NavigateAndReset.Observe();

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            observer.Should().NotPush();
            this.itemResultsViewModelFactoryMock
                .DidNotReceive()
                .Invoke(Arg.Any<IScreen>());
        }

        [Test]
        public async Task SetListingForItemUnderCursorAsyncShouldSetMessageIfExceptionOccurs()
        {
            Exception exception = new("Exception text");

            this.mediatorMock
                .Send(Arg.Any<GetItemFromCursorQuery>(), Arg.Any<CancellationToken>())
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
                .Send(Arg.Any<GetItemFromCursorQuery>(), Arg.Any<CancellationToken>())
                .Throws(exception);

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            this.itemSearchOverlayViewModel.Message.Should().BeNull();
        }

        [Test]
        public async Task SetListingForItemUnderCursorAsyncShouldSetMessageToNull()
        {
            this.mediatorMock
                .Send(Arg.Any<GetItemFromCursorQuery>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromException<Item>(new Exception()), Task.FromResult<Item>(new CurrencyItem()));
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