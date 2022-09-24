using System;
using System.Threading;
using System.Threading.Tasks;

using Avalonia.Threading;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using POETradeHelper.Common.Contract;
using POETradeHelper.Common.Contract.Commands;
using POETradeHelper.Common.UI;
using POETradeHelper.ItemSearch.UI.Avalonia.Controllers;
using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels.Abstractions;
using POETradeHelper.ItemSearch.UI.Avalonia.Views;

namespace POETradeHelper.ItemSearch.Tests.Controllers
{
    public class ItemSearchResultOverlayControllerTests
    {
        private Mock<IItemSearchResultOverlayView> viewMock;
        private Mock<IItemSearchResultOverlayViewModel> viewModelMock;
        private Mock<IViewLocator> viewLocatorMock;
        private Mock<IUiThreadDispatcher> uiThreadDispatcherMock;
        private ItemSearchResultOverlayController overlayController;

        [SetUp]
        public void Setup()
        {
            this.viewMock = new Mock<IItemSearchResultOverlayView>();
            this.viewModelMock = new Mock<IItemSearchResultOverlayViewModel>();
            this.viewLocatorMock = new Mock<IViewLocator>();
            this.viewLocatorMock.Setup(x => x.GetView(It.IsAny<IItemSearchResultOverlayViewModel>()))
                .Returns(this.viewMock.Object);
            this.uiThreadDispatcherMock = new Mock<IUiThreadDispatcher>();
            this.overlayController = new ItemSearchResultOverlayController(this.viewModelMock.Object, this.viewLocatorMock.Object, this.uiThreadDispatcherMock.Object);
        }

        [Test]
        public async Task HandleHideOverlayQueryShouldCallHideOnOverlayIfOverlayIsVisible()
        {
            this.viewMock.Setup(x => x.IsVisible).Returns(true);

            await this.ExecuteHideOverlayCommand(new HideOverlayCommand(() => { }));

            this.viewMock.Verify(x => x.Hide());
        }

        [Test]
        public async Task HandleHideOverlayQueryShouldInvokeOnHandledActionIfOverlayIsVisible()
        {
            bool handled = false;
            this.viewMock.Setup(x => x.IsVisible).Returns(true);

            await this.ExecuteHideOverlayCommand(new HideOverlayCommand(() => handled = true));

            handled.Should().BeTrue();
        }

        [Test]
        public async Task HandleHideOverlayQueryShouldNotCallHideOnOverlayIfOverlayIsNotVisible()
        {
            await this.ExecuteHideOverlayCommand(new HideOverlayCommand(() => { }));

            this.viewMock.Verify(x => x.Hide(), Times.Never);
        }

        [Test]
        public async Task HandleSearchItemQueryShouldCallSetListingForItemUnderCursorAsyncOnViewModel()
        {
            await this.ExecuteSearchItemCommand(new SearchItemCommand());

            this.viewModelMock.Verify(x => x.SetListingForItemUnderCursorAsync(It.Is<CancellationToken>(c => c != CancellationToken.None)));
        }

        [Test]
        public async Task HandleSearchItemQueryShouldOpenOverlay()
        {
            await this.ExecuteSearchItemCommand(new SearchItemCommand());

            this.viewMock.Verify(x => x.Show());
        }

        [Test]
        public async Task HandleSearchItemQueryShouldCatchOperationCancelledException()
        {
            this.viewModelMock
                .Setup(x => x.SetListingForItemUnderCursorAsync(It.IsAny<CancellationToken>()))
                .Throws<OperationCanceledException>();

            await this.ExecuteSearchItemCommand(new SearchItemCommand());
        }

        private async Task ExecuteHideOverlayCommand(HideOverlayCommand command)
        {
            Action action = null;
            this.uiThreadDispatcherMock
                .Setup(x => x.InvokeAsync(It.IsAny<Action>(), It.IsAny<DispatcherPriority>()))
                .Callback((Action func, DispatcherPriority _) => action = func);

            await this.overlayController.Handle(command, default);
            action();
        }

        private async Task ExecuteSearchItemCommand(SearchItemCommand command)
        {
            Func<Task> action = null;
            this.uiThreadDispatcherMock
                .Setup(x => x.InvokeAsync(It.IsAny<Func<Task>>(), It.IsAny<DispatcherPriority>()))
                .Callback((Func<Task> func, DispatcherPriority _) => action = func);

            await this.overlayController.Handle(command, default);
            await action();
        }
    }
}