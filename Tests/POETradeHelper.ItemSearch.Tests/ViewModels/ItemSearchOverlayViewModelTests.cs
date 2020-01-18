using Moq;
using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Services;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi;
using POETradeHelper.PathOfExileTradeApi.Models;
using System.Threading.Tasks;

namespace POETradeHelper.ItemSearch.Tests.ViewModels
{
    public class ItemSearchOverlayViewModelTests
    {
        private Mock<ISearchItemProvider> searchItemProviderMock;
        private Mock<ITradeClient> tradeClientMock;
        private ItemSearchResultOverlayViewModel itemSearchOverlayViewModel;

        [SetUp]
        public void Setup()
        {
            this.searchItemProviderMock = new Mock<ISearchItemProvider>();
            this.tradeClientMock = new Mock<ITradeClient>();
            this.itemSearchOverlayViewModel = new ItemSearchResultOverlayViewModel(this.searchItemProviderMock.Object, this.tradeClientMock.Object);
        }

        [Test]
        public async Task SetListingForItemUnderCursorAsyncShouldCallGetItemFromUnderCursorOnSearchItemProvider()
        {
            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            this.searchItemProviderMock.Verify(x => x.GetItemFromUnderCursorAsync());
        }

        [Test]
        public async Task SetListingForItemUnderCursorAsyncShouldCallGetListingsAsyncOnTraceClient()
        {
            var item = new EquippableItem(ItemRarity.Normal) { Name = "TestItem" };
            this.searchItemProviderMock.Setup(x => x.GetItemFromUnderCursorAsync())
                .ReturnsAsync(item);

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            this.tradeClientMock.Verify(x => x.GetListingAsync(item));
        }

        [Test]
        public async Task SetListingForItemUnderCursorAsyncShouldSetListingIfTradeClientDoesNotReturnNull()
        {
            var itemListing = new ListingResult();
            this.tradeClientMock.Setup(x => x.GetListingAsync(It.IsAny<Item>()))
                .ReturnsAsync(itemListing);

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            Assert.AreSame(itemListing, this.itemSearchOverlayViewModel.ItemListing);
        }
    }
}