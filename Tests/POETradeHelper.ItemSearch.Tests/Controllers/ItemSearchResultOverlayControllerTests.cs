using System.ComponentModel;
using System.Threading;
using Moq;
using NUnit.Framework;
using POETradeHelper.Common.Contract;
using POETradeHelper.ItemSearch.Controllers;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.ItemSearch.Views;

namespace POETradeHelper.ItemSearch.Tests.Controllers
{
    public class ItemSearchResultOverlayControllerTests
    {
        private Mock<IItemSearchResultOverlayView> viewMock;
        private Mock<IItemSearchResultOverlayViewModel> viewModelMock;
        private Mock<IViewLocator> viewLocatorMock;
        private Mock<IUserInputEventProvider> userInputEventProviderMock;
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
            this.overlayController = new ItemSearchResultOverlayController(this.viewModelMock.Object, this.viewLocatorMock.Object, this.userInputEventProviderMock.Object);
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
            TriggerSearchItemEvent();

            this.viewModelMock.Verify(x => x.SetListingForItemUnderCursorAsync(It.Is<CancellationToken>(c => c != CancellationToken.None)));
        }

        [Test]
        public void SearchItemShouldOpenOverlay()
        {
            TriggerSearchItemEvent();

            this.viewMock.Verify(x => x.Show());
        }

        /// <summary>
        /// The execution order is particularly important if there are any awaited calls in the event handler.
        /// The first await call would return the control immediatly back to the sender so that setting the Handled
        /// property on the event args after such a call would have no effect.
        /// </summary>
        [Test]
        public void SearchItemShouldSetEventHandledBeforeAnyLogic()
        {
            var handledEventArgs = new HandledEventArgs();

            this.viewModelMock.Setup(x => x.SetListingForItemUnderCursorAsync(It.IsAny<CancellationToken>()))
                .Callback(() => Assert.IsTrue(handledEventArgs.Handled));

            TriggerSearchItemEvent(handledEventArgs);
        }

        private void TriggerSearchItemEvent(HandledEventArgs handledEventArgs = null)
        {
            this.userInputEventProviderMock.Raise(x => x.SearchItem += null, handledEventArgs ?? new HandledEventArgs());
        }
    }
}