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

namespace POETradeHelper.ItemSearch.UI.Avalonia.Tests.Controllers
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
            viewMock = new Mock<IItemSearchResultOverlayView>();
            viewModelMock = new Mock<IItemSearchResultOverlayViewModel>();
            viewLocatorMock = new Mock<IViewLocator>();
            viewLocatorMock.Setup(x => x.GetView(It.IsAny<IItemSearchResultOverlayViewModel>()))
                .Returns(viewMock.Object);
            uiThreadDispatcherMock = new Mock<IUiThreadDispatcher>();
            overlayController = new ItemSearchResultOverlayController(viewModelMock.Object, viewLocatorMock.Object, uiThreadDispatcherMock.Object);
        }

        [Test]
        public async Task HandleHideOverlayQueryShouldCallHideOnOverlayIfOverlayIsVisible()
        {
            viewMock.Setup(x => x.IsVisible).Returns(true);

            await ExecuteHideOverlayCommand(new HideOverlayCommand(() => { }));

            viewMock.Verify(x => x.Hide());
        }

        [Test]
        public async Task HandleHideOverlayQueryShouldInvokeOnHandledActionIfOverlayIsVisible()
        {
            bool handled = false;
            viewMock.Setup(x => x.IsVisible).Returns(true);

            await ExecuteHideOverlayCommand(new HideOverlayCommand(() => handled = true));

            handled.Should().BeTrue();
        }

        [Test]
        public async Task HandleHideOverlayQueryShouldNotCallHideOnOverlayIfOverlayIsNotVisible()
        {
            await ExecuteHideOverlayCommand(new HideOverlayCommand(() => { }));

            viewMock.Verify(x => x.Hide(), Times.Never);
        }

        [Test]
        public async Task HandleSearchItemQueryShouldCallSetListingForItemUnderCursorAsyncOnViewModel()
        {
            await ExecuteSearchItemCommand(new SearchItemCommand());

            viewModelMock.Verify(x => x.SetListingForItemUnderCursorAsync(It.Is<CancellationToken>(c => c != CancellationToken.None)));
        }

        [Test]
        public async Task HandleSearchItemQueryShouldOpenOverlay()
        {
            await ExecuteSearchItemCommand(new SearchItemCommand());

            viewMock.Verify(x => x.Show());
        }

        [Test]
        public async Task HandleSearchItemQueryShouldCatchOperationCancelledException()
        {
            viewModelMock
                .Setup(x => x.SetListingForItemUnderCursorAsync(It.IsAny<CancellationToken>()))
                .Throws<OperationCanceledException>();

            await ExecuteSearchItemCommand(new SearchItemCommand());
        }

        private async Task ExecuteHideOverlayCommand(HideOverlayCommand command)
        {
            Action action = null;
            uiThreadDispatcherMock
                .Setup(x => x.InvokeAsync(It.IsAny<Action>(), It.IsAny<DispatcherPriority>()))
                .Callback((Action func, DispatcherPriority _) => action = func);

            await overlayController.Handle(command, default);
            action();
        }

        private async Task ExecuteSearchItemCommand(SearchItemCommand command)
        {
            Func<Task> action = null;
            uiThreadDispatcherMock
                .Setup(x => x.InvokeAsync(It.IsAny<Func<Task>>(), It.IsAny<DispatcherPriority>()))
                .Callback((Func<Task> func, DispatcherPriority _) => action = func);

            await overlayController.Handle(command, default);
            await action();
        }
    }
}