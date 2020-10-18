using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using POETradeHelper.Common.UI.Models;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Services;
using POETradeHelper.ItemSearch.Services.Factories;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.ItemSearch.Tests.ViewModels
{
    public class ItemSearchOverlayViewModelTests
    {
        private Mock<ISearchItemProvider> searchItemProviderMock;
        private Mock<IPoeTradeApiClient> poeTradeApiClientMock;
        private Mock<IItemListingsViewModelFactory> itemListingsViewModelFactoryMock;
        private Mock<IAdvancedQueryViewModelFactory> advancedQueryViewModelFactoryMock;
        private Mock<IQueryRequestFactory> queryRequestFactoryMock;
        private ItemSearchResultOverlayViewModel itemSearchOverlayViewModel;

        [SetUp]
        public void Setup()
        {
            this.searchItemProviderMock = new Mock<ISearchItemProvider>();
            this.poeTradeApiClientMock = new Mock<IPoeTradeApiClient>();
            this.itemListingsViewModelFactoryMock = new Mock<IItemListingsViewModelFactory>();
            this.advancedQueryViewModelFactoryMock = new Mock<IAdvancedQueryViewModelFactory>();
            this.queryRequestFactoryMock = new Mock<IQueryRequestFactory>();
            this.itemSearchOverlayViewModel = new ItemSearchResultOverlayViewModel(
                this.searchItemProviderMock.Object,
                this.poeTradeApiClientMock.Object,
                this.itemListingsViewModelFactoryMock.Object,
                this.advancedQueryViewModelFactoryMock.Object,
                this.queryRequestFactoryMock.Object);
        }

        [Test]
        public async Task SetListingForItemUnderCursorAsyncShouldCallGetItemFromUnderCursorOnSearchItemProvider()
        {
            var cancellationToken = new CancellationToken();

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync(cancellationToken);

            this.searchItemProviderMock.Verify(x => x.GetItemFromUnderCursorAsync(cancellationToken));
        }

        [Test]
        public async Task SetListingForItemUnderCursorAsyncShouldCallCreateOnQueryRequestFactoryWithItem()
        {
            var item = new EquippableItem(ItemRarity.Normal) { Name = "TestItem" };
            this.searchItemProviderMock.Setup(x => x.GetItemFromUnderCursorAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            this.queryRequestFactoryMock.Verify(x => x.Create(item));
        }

        [Test]
        public async Task SetListingForItemUnderCursorAsyncShouldCallGetListingsAsyncOnTradeClient()
        {
            var queryRequest = new SearchQueryRequest
            {
                League = "Heist"
            };
            var cancellationToken = new CancellationToken();

            this.queryRequestFactoryMock.Setup(x => x.Create(It.IsAny<Item>()))
                .Returns(queryRequest);

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync(cancellationToken);

            this.poeTradeApiClientMock.Verify(x => x.GetListingsAsync(queryRequest, cancellationToken));
        }

        [Test]
        public async Task SetListingForItemUnderCursorAsyncShouldSetItem()
        {
            var item = new EquippableItem(ItemRarity.Unique);

            this.searchItemProviderMock.Setup(x => x.GetItemFromUnderCursorAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            Assert.That(this.itemSearchOverlayViewModel.Item, Is.EqualTo(item));
        }

        [Test]
        public async Task SetListingForItemUnderCursorAsyncShouldCallCreateOnItemListingsViewModelFactoryIfTradeClientDoesNotReturnNull()
        {
            var itemListing = new ItemListingsQueryResult();
            this.poeTradeApiClientMock.Setup(x => x.GetListingsAsync(It.IsAny<IQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(itemListing);

            this.itemSearchOverlayViewModel.Item = new EquippableItem(ItemRarity.Unique);

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            this.itemListingsViewModelFactoryMock.Verify(x => x.CreateAsync(this.itemSearchOverlayViewModel.Item, itemListing));
        }

        [Test]
        public async Task SetListingForItemUnderCursorAsyncShouldNotCallCreateOnItemListingsViewModelFactoryIfTradeClientDoesReturnNull()
        {
            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            this.itemListingsViewModelFactoryMock.Verify(x => x.CreateAsync(It.IsAny<Item>(), It.IsAny<ItemListingsQueryResult>()), Times.Never);
        }

        [Test]
        public async Task SetListingForItemUnderCursorAsyncShouldSetItemListingsViewModel()
        {
            ItemListingsViewModel expected = new ItemListingsViewModel();

            this.poeTradeApiClientMock.Setup(x => x.GetListingsAsync(It.IsAny<IQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ItemListingsQueryResult());

            this.itemListingsViewModelFactoryMock.Setup(x => x.CreateAsync(It.IsAny<Item>(), It.IsAny<ItemListingsQueryResult>()))
                .ReturnsAsync(expected);

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

        [Test]
        public async Task SetListingForItemUnderCursorAsyncShouldCallCreateOnAdvancedQueryViewModelFactory()
        {
            var item = new EquippableItem(ItemRarity.Unique);

            var itemListingsResult = new ItemListingsQueryResult
            {
                SearchQueryRequest = new SearchQueryRequest()
            };

            this.searchItemProviderMock.Setup(x => x.GetItemFromUnderCursorAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);

            this.poeTradeApiClientMock.Setup(x => x.GetListingsAsync(It.IsAny<IQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(itemListingsResult);

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            this.advancedQueryViewModelFactoryMock.Verify(x => x.Create(item, itemListingsResult.SearchQueryRequest));
        }

        [Test]
        public async Task SetListingForItemUnderCursorAsyncShouldSetAdvancedQueryViewModel()
        {
            var expected = new AdvancedQueryViewModel();

            this.advancedQueryViewModelFactoryMock.Setup(x => x.Create(It.IsAny<Item>(), It.IsAny<IQueryRequest>()))
                .Returns(expected);

            this.poeTradeApiClientMock.Setup(x => x.GetListingsAsync(It.IsAny<IQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ItemListingsQueryResult());

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            Assert.That(this.itemSearchOverlayViewModel.AdvancedQuery, Is.SameAs(expected));
        }

        [Test]
        public async Task ExecuteAdvancedQueryAsyncShouldCallMapOnAdvancedQueryViewModelToSearchQueryMapper()
        {
            AdvancedQueryViewModel advancedQueryViewModel = new AdvancedQueryViewModel();
            this.itemSearchOverlayViewModel.AdvancedQuery = advancedQueryViewModel;

            await this.itemSearchOverlayViewModel.ExecuteAdvancedQueryAsync();

            this.queryRequestFactoryMock.Verify(x => x.Create(advancedQueryViewModel));
        }

        [Test]
        public async Task ExecuteAdvancedQueryAsyncShouldCallGetListingsAsyncOnPoeTradeApiClient()
        {
            var queryRequest = new SearchQueryRequest();

            this.queryRequestFactoryMock.Setup(x => x.Create(It.IsAny<AdvancedQueryViewModel>()))
                .Returns(queryRequest);

            await this.itemSearchOverlayViewModel.ExecuteAdvancedQueryAsync();

            this.poeTradeApiClientMock.Verify(x => x.GetListingsAsync(queryRequest, It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task ExecuteAdvancedQueryAsyncShouldCallCreateOnItemListingsViewModelFactory()
        {
            var itemListing = new ItemListingsQueryResult();
            this.poeTradeApiClientMock.Setup(x => x.GetListingsAsync(It.IsAny<IQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(itemListing);

            this.itemSearchOverlayViewModel.Item = new EquippableItem(ItemRarity.Unique);

            await this.itemSearchOverlayViewModel.ExecuteAdvancedQueryAsync();

            this.itemListingsViewModelFactoryMock.Verify(x => x.CreateAsync(this.itemSearchOverlayViewModel.Item, itemListing));
        }

        [Test]
        public async Task ExecuteAdvancedQueryAsyncShouldSetItemListingsViewModel()
        {
            ItemListingsViewModel expected = new ItemListingsViewModel();

            this.poeTradeApiClientMock.Setup(x => x.GetListingsAsync(It.IsAny<IQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ItemListingsQueryResult());

            this.itemListingsViewModelFactoryMock.Setup(x => x.CreateAsync(It.IsAny<Item>(), It.IsAny<ItemListingsQueryResult>()))
                .ReturnsAsync(expected);

            await this.itemSearchOverlayViewModel.ExecuteAdvancedQueryAsync();

            Assert.That(this.itemSearchOverlayViewModel.ItemListings, Is.EqualTo(expected));
        }

        [Test]
        public async Task ExecuteAdvancedQueryAsyncShouldCallCreateOnAdvancedQueryViewModelFactory()
        {
            var queryRequest = new SearchQueryRequest();
            this.queryRequestFactoryMock.Setup(x => x.Create(It.IsAny<AdvancedQueryViewModel>()))
                .Returns(queryRequest);

            this.itemSearchOverlayViewModel.Item = new EquippableItem(ItemRarity.Unique);

            await this.itemSearchOverlayViewModel.ExecuteAdvancedQueryAsync();

            this.advancedQueryViewModelFactoryMock.Verify(x => x.Create(this.itemSearchOverlayViewModel.Item, queryRequest));
        }

        [Test]
        public async Task ExecuteAdvancedQueryAsyncShouldSetAdvancedQuery()
        {
            var advancedQueryViewModel = new AdvancedQueryViewModel();

            this.advancedQueryViewModelFactoryMock.Setup(x => x.Create(It.IsAny<Item>(), It.IsAny<IQueryRequest>()))
                .Returns(advancedQueryViewModel);

            await this.itemSearchOverlayViewModel.ExecuteAdvancedQueryAsync();

            Assert.That(this.itemSearchOverlayViewModel.AdvancedQuery, Is.EqualTo(advancedQueryViewModel));
        }
    }
}