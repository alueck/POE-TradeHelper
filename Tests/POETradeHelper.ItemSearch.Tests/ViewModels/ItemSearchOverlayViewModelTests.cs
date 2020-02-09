using Moq;
using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Services;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Services;
using System.Threading;
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
            var cancellationToken = new CancellationToken();

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync(cancellationToken);

            this.searchItemProviderMock.Verify(x => x.GetItemFromUnderCursorAsync(cancellationToken));
        }

        [Test]
        public async Task SetListingForItemUnderCursorAsyncShouldCallGetListingsAsyncOnTraceClient()
        {
            var item = new EquippableItem(ItemRarity.Normal) { Name = "TestItem" };
            var cancellationToken = new CancellationToken();
            this.searchItemProviderMock.Setup(x => x.GetItemFromUnderCursorAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync(cancellationToken);

            this.poeTradeApiClientMock.Verify(x => x.GetListingsAsync(item, cancellationToken));
        }

        [Test]
        public async Task SetListingForItemUnderCursorAsyncShouldSetListingIfTradeClientDoesNotReturnNull()
        {
            var itemListing = new ItemListingsQueryResult();
            this.poeTradeApiClientMock.Setup(x => x.GetListingsAsync(It.IsAny<Item>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(itemListing);

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            Assert.AreSame(itemListing, this.itemSearchOverlayViewModel.ItemListings);
        }
    }
}