using System;
using System.Threading;
using System.Threading.Tasks;

using Avalonia.Threading;

using FluentAssertions;

using NSubstitute;
using NSubstitute.ExceptionExtensions;

using NUnit.Framework;

using POETradeHelper.Common.Contract;
using POETradeHelper.Common.Contract.Commands;
using POETradeHelper.Common.UI;
using POETradeHelper.ItemSearch.UI.Avalonia.Controllers;
using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels.Abstractions;
using POETradeHelper.ItemSearch.UI.Avalonia.Views;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Tests.Controllers
{
    public class ItemSearchResultOverlayControllerTests
    {
        private IItemSearchResultOverlayView viewMock;
        private IItemSearchResultOverlayViewModel viewModelMock;
        private IViewLocator viewLocatorMock;
        private IUiThreadDispatcher uiThreadDispatcherMock;
        private ItemSearchResultOverlayController overlayController;

        [SetUp]
        public void Setup()
        {
            this.viewMock = Substitute.For<IItemSearchResultOverlayView>();
            this.viewModelMock = Substitute.For<IItemSearchResultOverlayViewModel>();
            this.viewLocatorMock = Substitute.For<IViewLocator>();
            this.viewLocatorMock.GetView(Arg.Any<IItemSearchResultOverlayViewModel>())
                .Returns(this.viewMock);
            this.uiThreadDispatcherMock = Substitute.For<IUiThreadDispatcher>();
            this.overlayController = new ItemSearchResultOverlayController(this.viewModelMock, this.viewLocatorMock, this.uiThreadDispatcherMock);
        }

        [Test]
        public async Task HandleHideOverlayQueryShouldCallHideOnOverlayIfOverlayIsVisible()
        {
            this.viewMock.IsVisible.Returns(true);

            await this.ExecuteHideOverlayCommand(new HideOverlayCommand(() => { }));

            this.viewMock
                .Received()
                .Hide();
        }

        [Test]
        public async Task HandleHideOverlayQueryShouldInvokeOnHandledActionIfOverlayIsVisible()
        {
            bool handled = false;
            this.viewMock.IsVisible.Returns(true);

            await this.ExecuteHideOverlayCommand(new HideOverlayCommand(() => handled = true));

            handled.Should().BeTrue();
        }

        [Test]
        public async Task HandleHideOverlayQueryShouldNotCallHideOnOverlayIfOverlayIsNotVisible()
        {
            await this.ExecuteHideOverlayCommand(new HideOverlayCommand(() => { }));

            this.viewMock
                .DidNotReceive()
                .Hide();
        }

        [Test]
        public async Task HandleSearchItemQueryShouldCallSetListingForItemUnderCursorAsyncOnViewModel()
        {
            await this.ExecuteSearchItemCommand(new SearchItemCommand());

            await this.viewModelMock
                .Received()
                .SetListingForItemUnderCursorAsync(Arg.Is<CancellationToken>(c => c != CancellationToken.None));
        }

        [Test]
        public async Task HandleSearchItemQueryShouldOpenOverlay()
        {
            await this.ExecuteSearchItemCommand(new SearchItemCommand());

            this.viewMock
                .Received()
                .Show();
        }

        [Test]
        public async Task HandleSearchItemQueryShouldCatchOperationCancelledException()
        {
            this.viewModelMock
                .SetListingForItemUnderCursorAsync(Arg.Any<CancellationToken>())
                .Throws<OperationCanceledException>();

            await this.ExecuteSearchItemCommand(new SearchItemCommand());
        }

        private async Task ExecuteHideOverlayCommand(HideOverlayCommand command)
        {
            Action action = null;
            this.uiThreadDispatcherMock
                .When(x => x.InvokeAsync(Arg.Any<Action>(), Arg.Any<DispatcherPriority>()))
                .Do(ctx => action = ctx.Arg<Action>());

            await this.overlayController.Handle(command, default);
            action();
        }

        private async Task ExecuteSearchItemCommand(SearchItemCommand command)
        {
            Func<Task> action = null;
            this.uiThreadDispatcherMock
                .When(x => x.InvokeAsync(Arg.Any<Func<Task>>(), Arg.Any<DispatcherPriority>()))
                .Do(ctx => action = ctx.Arg<Func<Task>>());

            await this.overlayController.Handle(command, default);
            await action();
        }
    }
}