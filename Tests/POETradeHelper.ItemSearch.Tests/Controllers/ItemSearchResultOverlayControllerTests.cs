using Moq;
using NUnit.Framework;
using POETradeHelper.Common.Contract;
using POETradeHelper.ItemSearch.Controllers;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.ItemSearch.Views;
using POETradeHelper.PathOfExileTradeApi.Models;
using System.Collections.Generic;
using System.ComponentModel;

namespace POETradeHelper.ItemSearch.Tests.Controllers
{
    public class ItemSearchResultOverlayControllerTests
    {
        private Mock<IItemSearchResultOverlayView> viewMock;
        private Mock<IItemSearchResultOverlayViewModel> viewModelMock;
        private Mock<IViewLocator> viewLocatorMock;
        private Mock<IUserInputEventProvider> userInputEventProviderMock;
        private Mock<IPathOfExileProcessHelper> pathOfExileProcessHelper;
        private ItemSearchResultOverlayController overlayController;

        [SetUp]
        public void Setup()
        {
            this.viewMock = new Mock<IItemSearchResultOverlayView>();
            this.viewModelMock = new Mock<IItemSearchResultOverlayViewModel>();
            this.viewLocatorMock = new Mock<IViewLocator>();
            this.viewLocatorMock.Setup(x => x.GetView(It.IsAny<IItemSearchResultOverlayViewModel>()))
                .Returns(this.viewMock.Object);

            this.userInputEventProviderMock = new Mock<IUserInputEventProvider>();
            this.pathOfExileProcessHelper = new Mock<IPathOfExileProcessHelper>();
            this.overlayController = new ItemSearchResultOverlayController(this.viewModelMock.Object, this.viewLocatorMock.Object, this.userInputEventProviderMock.Object, this.pathOfExileProcessHelper.Object);
        }

        [Test]
        public void HideOverlayEventShouldCallHideOnOverlayIfOverlayIsVisble()
        {
            viewMock.Setup(x => x.IsVisible).Returns(true);

            this.userInputEventProviderMock.Raise(x => x.HideOverlay += null, new HandledEventArgs());

            viewMock.Verify(x => x.Hide());
        }

        [Test]
        public void HideOverlayShouldNotCallHideOnOverlayIfOverlayIsNotVisible()
        {
            this.userInputEventProviderMock.Raise(x => x.HideOverlay += null, new HandledEventArgs());

            viewMock.Verify(x => x.Hide(), Times.Never);
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void HideOverlayEvenShouldSetEventArgsHandled(bool isVisible, bool expectedEventArgsHandled)
        {
            var handledEventArgs = new HandledEventArgs();
            viewMock.Setup(x => x.IsVisible).Returns(isVisible);

            this.userInputEventProviderMock.Raise(x => x.HideOverlay += null, handledEventArgs);

            Assert.AreEqual(expectedEventArgsHandled, handledEventArgs.Handled);
        }

        [Test]
        public void SearchItemEventShouldCallSetListingForItemUnderCursorAsyncOnViewModel()
        {
            this.pathOfExileProcessHelper.Setup(x => x.IsPathOfExileActiveWindow())
                .Returns(true);

            TriggerSearchItemEvent();

            this.viewModelMock.Verify(x => x.SetListingForItemUnderCursorAsync());
        }

        [Test]
        public void SearchItemShouldNotSetItemListingOnViewModelIfTradeClientReturnsNull()
        {
            TriggerSearchItemEvent();

            this.viewModelMock.VerifySet(x => x.ItemListings = It.IsAny<ItemListingsQueryResult>(), Times.Never);
        }

        [Test]
        public void SearchItemShouldOpenOverlay()
        {
            this.pathOfExileProcessHelper.Setup(x => x.IsPathOfExileActiveWindow())
                .Returns(true);

            TriggerSearchItemEvent();

            this.viewMock.Verify(x => x.Show());
        }

        [Test]
        public void SearchItemShouldNotOpenOverlayIfTradeClientReturnsNull()
        {
            TriggerSearchItemEvent();

            this.viewMock.Verify(x => x.Show(), Times.Never);
        }

        [Test]
        public void SearchItemShouldDoNothingIfPathOfExileIsNotActiveWindow()
        {
            TriggerSearchItemEvent();

            this.viewModelMock.Verify(x => x.SetListingForItemUnderCursorAsync(), Times.Never);
            this.viewMock.Verify(x => x.Show(), Times.Never);
        }

        /// <summary>
        /// The execution order is particularly important if there are any awaited calls in the event handler.
        /// The first await call would return the control immediatly back to the sender so that setting the Handled
        /// property on the event args after such a call would have no effect.
        /// </summary>
        [Test]
        public void SearchItemShouldSetEventHandledBeforeAnyLogicIfPathOfExileIsActiveWindow()
        {
            var handledEventArgs = new HandledEventArgs();

            this.pathOfExileProcessHelper.Setup(x => x.IsPathOfExileActiveWindow())
                .Returns(true);

            this.viewModelMock.Setup(x => x.SetListingForItemUnderCursorAsync())
                .Callback(() => Assert.IsTrue(handledEventArgs.Handled));

            TriggerSearchItemEvent(handledEventArgs);
        }

        private void TriggerSearchItemEvent(HandledEventArgs handledEventArgs = null)
        {
            this.userInputEventProviderMock.Raise(x => x.SearchItem += null, handledEventArgs ?? new HandledEventArgs());
        }
    }
}