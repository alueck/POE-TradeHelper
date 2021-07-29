using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using POETradeHelper.Common.Contract;
using POETradeHelper.Common.Contract.Commands;
using POETradeHelper.Common.Contract.Queries;
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
        private ItemSearchResultOverlayController overlayController;

        [SetUp]
        public void Setup()
        {
            this.viewMock = new Mock<IItemSearchResultOverlayView>();
            this.viewModelMock = new Mock<IItemSearchResultOverlayViewModel>();
            this.viewLocatorMock = new Mock<IViewLocator>();
            this.viewLocatorMock.Setup(x => x.GetView(It.IsAny<IItemSearchResultOverlayViewModel>()))
                .Returns(this.viewMock.Object);
            this.overlayController = new ItemSearchResultOverlayController(this.viewModelMock.Object, this.viewLocatorMock.Object);
        }

        [Test]
        public async Task HandleHideOverlayQueryShouldCallHideOnOverlayIfOverlayIsVisible()
        {
            viewMock.Setup(x => x.IsVisible).Returns(true);

            await this.overlayController.Handle(new HideOverlayQuery(), default);

            viewMock.Verify(x => x.Hide());
        }

        [Test]
        public async Task HandleHideOverlayQueryShouldNotCallHideOnOverlayIfOverlayIsNotVisible()
        {
            await this.overlayController.Handle(new HideOverlayQuery(), default);

            viewMock.Verify(x => x.Hide(), Times.Never);
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public async Task HandleHideOverlayQueryShouldReturnResponse(bool isVisible, bool expectedHandledValue)
        {
            viewMock.Setup(x => x.IsVisible).Returns(isVisible);

            var result = await this.overlayController.Handle(new HideOverlayQuery(), default);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Handled, Is.EqualTo(expectedHandledValue));
        }

        [Test]
        public async Task HandleSearchItemQueryShouldCallSetListingForItemUnderCursorAsyncOnViewModel()
        {
            await this.overlayController.Handle(new SearchItemCommand(), default);

            this.viewModelMock.Verify(x => x.SetListingForItemUnderCursorAsync(It.Is<CancellationToken>(c => c != CancellationToken.None)));
        }

        [Test]
        public async Task HandleSearchItemQueryShouldOpenOverlay()
        {
            await this.overlayController.Handle(new SearchItemCommand(), default);

            this.viewMock.Verify(x => x.Show());
        }
    }
}