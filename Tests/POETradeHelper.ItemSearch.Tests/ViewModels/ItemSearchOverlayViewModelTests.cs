using Moq;
using NUnit.Framework;
using POETradeHelper.Common.UI.Models;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Services;
using POETradeHelper.ItemSearch.Services.Factories;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace POETradeHelper.ItemSearch.Tests.ViewModels
{
    public class ItemSearchOverlayViewModelTests
    {
        private Mock<ISearchItemProvider> searchItemProviderMock;
        private Mock<IPoeTradeApiClient> poeTradeApiClientMock;
        private Mock<IItemListingsViewModelFactory> itemListingsViewModelFactoryMock;
        private ItemSearchResultOverlayViewModel itemSearchOverlayViewModel;

        [SetUp]
        public void Setup()
        {
            this.searchItemProviderMock = new Mock<ISearchItemProvider>();
            this.poeTradeApiClientMock = new Mock<IPoeTradeApiClient>();
            this.itemListingsViewModelFactoryMock = new Mock<IItemListingsViewModelFactory>();
            this.itemSearchOverlayViewModel = new ItemSearchResultOverlayViewModel(this.searchItemProviderMock.Object, this.poeTradeApiClientMock.Object, this.itemListingsViewModelFactoryMock.Object);
        }

        [Test]
        public async Task SetListingForItemUnderCursorAsyncShouldCallGetItemFromUnderCursorOnSearchItemProvider()
        {
            var cancellationToken = new CancellationToken();

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync(cancellationToken);

            this.searchItemProviderMock.Verify(x => x.GetItemFromUnderCursorAsync(cancellationToken));
        }

        [Test]
        public async Task SetListingForItemUnderCursorAsyncShouldCallGetListingsAsyncOnTradeClient()
        {
            var item = new EquippableItem(ItemRarity.Normal) { Name = "TestItem" };
            var cancellationToken = new CancellationToken();
            this.searchItemProviderMock.Setup(x => x.GetItemFromUnderCursorAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync(cancellationToken);

            this.poeTradeApiClientMock.Verify(x => x.GetListingsAsync(item, cancellationToken));
        }

        [Test]
        public async Task SetListingForItemUnderCursorAsyncShouldCallCreateOnItemListingsViewModelFactoryIfTradeClientDoesNotReturnNull()
        {
            var itemListing = new ItemListingsQueryResult();
            this.poeTradeApiClientMock.Setup(x => x.GetListingsAsync(It.IsAny<Item>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(itemListing);

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            this.itemListingsViewModelFactoryMock.Verify(x => x.Create(itemListing));
        }

        [Test]
        public async Task SetListingForItemUnderCursorAsyncShouldNotCallCreateOnItemListingsViewModelFactoryIfTradeClientDoesReturnNull()
        {
            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            this.itemListingsViewModelFactoryMock.Verify(x => x.Create(It.IsAny<ItemListingsQueryResult>()), Times.Never);
        }

        [Test]
        public async Task SetListingForItemUnderCursorAsyncShouldSetItemListingsViewModel()
        {
            ItemListingsViewModel expected = new ItemListingsViewModel();

            this.poeTradeApiClientMock.Setup(x => x.GetListingsAsync(It.IsAny<Item>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ItemListingsQueryResult());

            this.itemListingsViewModelFactoryMock.Setup(x => x.Create(It.IsAny<ItemListingsQueryResult>()))
                .Returns(expected);

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            Assert.That(this.itemSearchOverlayViewModel.ItemListings, Is.EqualTo(expected));
        }

        [Test]
        public async Task SetListingForItemUnderCursorAsyncShouldSetMessageIfExceptionOccurs()
        {
            Exception exception = new Exception("Exception text");

            this.searchItemProviderMock.Setup(x => x.GetItemFromUnderCursorAsync(It.IsAny<CancellationToken>()))
                .Throws(exception);

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            Assert.IsNotNull(this.itemSearchOverlayViewModel.Message);
            Assert.That(this.itemSearchOverlayViewModel.Message.Type, Is.EqualTo(MessageType.Error));
            Assert.IsNotNull(this.itemSearchOverlayViewModel.Message.Text);
            Assert.IsNotEmpty(this.itemSearchOverlayViewModel.Message.Text);
        }

        [Test]
        public async Task SetListingForItemUnderCursorAsyncShouldSetMessageToNull()
        {
            this.itemSearchOverlayViewModel.Message = new Message();

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            Assert.IsNull(this.itemSearchOverlayViewModel.Message);
        }
    }
}