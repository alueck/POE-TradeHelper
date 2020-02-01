using Moq;
using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Services;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Services;
using System.Threading.Tasks;

namespace POETradeHelper.ItemSearch.Tests.ViewModels
{
    public class ItemSearchOverlayViewModelTests
    {
        private Mock<ISearchItemProvider> searchItemProviderMock;
        private Mock<IPoeTradeApiClient> poeTradeApiClientMock;
        private ItemSearchResultOverlayViewModel itemSearchOverlayViewModel;

        [SetUp]
        public void Setup()
        {
            this.searchItemProviderMock = new Mock<ISearchItemProvider>();
            this.poeTradeApiClientMock = new Mock<IPoeTradeApiClient>();
            this.itemSearchOverlayViewModel = new ItemSearchResultOverlayViewModel(this.searchItemProviderMock.Object, this.poeTradeApiClientMock.Object);
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

            this.poeTradeApiClientMock.Verify(x => x.GetListingsAsync(item));
        }

        [Test]
        public async Task SetListingForItemUnderCursorAsyncShouldSetListingIfTradeClientDoesNotReturnNull()
        {
            var itemListing = new ItemListingsQueryResult();
            this.poeTradeApiClientMock.Setup(x => x.GetListingsAsync(It.IsAny<Item>()))
                .ReturnsAsync(itemListing);

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            Assert.AreSame(itemListing, this.itemSearchOverlayViewModel.ItemListings);
        }
    }
}