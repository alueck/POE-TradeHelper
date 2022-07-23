using System;
using System.Collections;
using System.Drawing.Imaging;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions.Reactive;

using MediatR;
using Moq;
using NUnit.Framework;
using POETradeHelper.Common.UI.Models;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Queries;
using POETradeHelper.ItemSearch.Exceptions;
using POETradeHelper.ItemSearch.Services.Factories;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Services;
using POETradeHelper.PricePrediction.ViewModels;

using ReactiveUI;

namespace POETradeHelper.ItemSearch.Tests.ViewModels
{
    public class ItemSearchOverlayViewModelTests
    {
        private Mock<IMediator> mediatorMock;
        private Mock<Func<IScreen, IItemResultsViewModel>> itemResultsViewModelFactoryMock;
        private Mock<Func<IScreen, IExchangeResultsViewModel>> exchangeResultsViewModelFactoryMock;
        private ItemSearchResultOverlayViewModel itemSearchOverlayViewModel;

        [SetUp]
        public void Setup()
        {
            this.mediatorMock = new Mock<IMediator>();
            this.itemResultsViewModelFactoryMock = new Mock<Func<IScreen, IItemResultsViewModel>>();
            this.exchangeResultsViewModelFactoryMock = new Mock<Func<IScreen, IExchangeResultsViewModel>>();

            this.itemSearchOverlayViewModel = new ItemSearchResultOverlayViewModel(
                this.mediatorMock.Object,
                this.itemResultsViewModelFactoryMock.Object,
                this.exchangeResultsViewModelFactoryMock.Object);
        }

        [Test]
        public async Task SetListingForItemUnderCursorAsyncShouldSendGetItemFromCursorQuery()
        {
            var cancellationToken = new CancellationToken();

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync(cancellationToken);

            this.mediatorMock.Verify(x => x.Send(It.IsAny<GetItemFromCursorQuery>(), cancellationToken));
        }

        [TestCaseSource(nameof(GetExchangeResultsViewModelItems))]
        public async Task SetListingForItemUnderCursorAsyncShouldNavigateToExchangeResultsView(Item item)
        {
            this.mediatorMock
                .Setup(x => x.Send(It.IsAny<GetItemFromCursorQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);
            IExchangeResultsViewModel expectedViewModel = Mock.Of<IExchangeResultsViewModel>();
            this.exchangeResultsViewModelFactoryMock
                .Setup(x => x.Invoke(this.itemSearchOverlayViewModel))
                .Returns(expectedViewModel);
            
            using var observer = this.itemSearchOverlayViewModel.Router.NavigateAndReset.Observe();

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            await observer.Should().PushMatchAsync(x => x == expectedViewModel);
        }
        
        [TestCaseSource(nameof(GetExchangeResultsViewModelItems))]
        public async Task SetListingForItemUnderCursorAsyncShouldInitializeExchangeResultsView(Item item)
        {
            this.mediatorMock
                .Setup(x => x.Send(It.IsAny<GetItemFromCursorQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);
            Mock<IExchangeResultsViewModel> exchangeResultsViewModelMock = new Mock<IExchangeResultsViewModel>();
            this.exchangeResultsViewModelFactoryMock
                .Setup(x => x.Invoke(this.itemSearchOverlayViewModel))
                .Returns(exchangeResultsViewModelMock.Object);

            CancellationToken token = new();

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync(token);

            exchangeResultsViewModelMock
                .Verify(x => x.InitializeAsync(item, token));
        }

        [TestCaseSource(nameof(GetExchangeResultsViewModelItems))]
        public async Task SetListingForItemUnderCursorAsyncShouldUseExistingExchangeResultsViewModelFromNavigationStack(Item item)
        {
            this.mediatorMock
                .Setup(x => x.Send(It.IsAny<GetItemFromCursorQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);
            IExchangeResultsViewModel expectedViewModel = Mock.Of<IExchangeResultsViewModel>();
            this.itemSearchOverlayViewModel.Router.NavigationStack.Add(expectedViewModel);
            
            using var observer = this.itemSearchOverlayViewModel.Router.NavigateAndReset.Observe();

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            observer.Should().NotPush();
            this.exchangeResultsViewModelFactoryMock
                .Verify(x => x.Invoke(It.IsAny<IScreen>()), Times.Never);
        }
        
        [TestCaseSource(nameof(GetItemResultsViewModelItems))]
        public async Task SetListingForItemUnderCursorAsyncShouldNavigateToItemResultsView(Item item)
        {
            this.mediatorMock
                .Setup(x => x.Send(It.IsAny<GetItemFromCursorQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);
            IItemResultsViewModel expectedViewModel = Mock.Of<IItemResultsViewModel>();
            this.itemResultsViewModelFactoryMock
                .Setup(x => x.Invoke(this.itemSearchOverlayViewModel))
                .Returns(expectedViewModel);
            
            using var observer = this.itemSearchOverlayViewModel.Router.NavigateAndReset.Observe();

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            await observer.Should().PushMatchAsync(x => x == expectedViewModel);
        }

        [TestCaseSource(nameof(GetItemResultsViewModelItems))]
        public async Task SetListingForItemUnderCursorAsyncShouldInitializeItemResultsView(Item item)
        {
            this.mediatorMock
                .Setup(x => x.Send(It.IsAny<GetItemFromCursorQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);
            Mock<IItemResultsViewModel> itemResultsViewModelMock = new Mock<IItemResultsViewModel>();
            this.itemResultsViewModelFactoryMock
                .Setup(x => x.Invoke(this.itemSearchOverlayViewModel))
                .Returns(itemResultsViewModelMock.Object);

            CancellationToken token = new();

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync(token);

            itemResultsViewModelMock
                .Verify(x => x.InitializeAsync(item, token));
        }
        
        [TestCaseSource(nameof(GetItemResultsViewModelItems))]
        public async Task SetListingForItemUnderCursorAsyncShouldUseExistingItemsResultsViewModelFromNavigationStack(Item item)
        {
            this.mediatorMock
                .Setup(x => x.Send(It.IsAny<GetItemFromCursorQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);
            IItemResultsViewModel expectedViewModel = Mock.Of<IItemResultsViewModel>();
            this.itemSearchOverlayViewModel.Router.NavigationStack.Add(expectedViewModel);

            using var observer = this.itemSearchOverlayViewModel.Router.NavigateAndReset.Observe();

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            observer.Should().NotPush();
            this.itemResultsViewModelFactoryMock
                .Verify(x => x.Invoke(It.IsAny<IScreen>()), Times.Never);
        }

        private static IEnumerable GetExchangeResultsViewModelItems()
        {
            yield return new CurrencyItem();
            yield return new FragmentItem();
            yield return new DivinationCardItem();
        }
        
        private static IEnumerable GetItemResultsViewModelItems()
        {
            yield return new EquippableItem(ItemRarity.Rare);
            yield return new FlaskItem(ItemRarity.Magic);
            yield return new GemItem();
            yield return new JewelItem(ItemRarity.Rare);
            yield return new MapItem(ItemRarity.Rare);
            yield return new OrganItem();
            yield return new ProphecyItem();
        }
        
        //
        // [Test]
        // public async Task SetListingForItemUnderCursorAsyncShouldCallGetListingsAsyncOnTradeClient()
        // {
        //     var queryRequest = new SearchQueryRequest
        //     {
        //         League = "Heist"
        //     };
        //     var cancellationToken = new CancellationToken();
        //
        //     this.queryRequestFactoryMock.Setup(x => x.Create(It.IsAny<Item>()))
        //         .Returns(queryRequest);
        //
        //     await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync(cancellationToken);
        //
        //     this.poeTradeApiClientMock.Verify(x => x.GetListingsAsync(queryRequest, cancellationToken));
        // }
        //
        // [Test]
        // public async Task SetListingForItemUnderCursorAsyncShouldSetItem()
        // {
        //     var item = new EquippableItem(ItemRarity.Unique);
        //
        //     this.mediatorMock
        //         .Setup(x => x.Send(It.IsAny<GetItemFromCursorQuery>(), It.IsAny<CancellationToken>()))
        //         .ReturnsAsync(item);
        //
        //     await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();
        //
        //     Assert.That(this.itemSearchOverlayViewModel.Item, Is.EqualTo(item));
        // }
        //
        // [Test]
        // public async Task SetListingForItemUnderCursorAsyncShouldCallCreateOnItemListingsViewModelFactoryIfTradeClientDoesNotReturnNull()
        // {
        //     // arrange
        //     var itemListing = new ItemListingsQueryResult();
        //     this.poeTradeApiClientMock.Setup(x => x.GetListingsAsync(It.IsAny<IQueryRequest>(), It.IsAny<CancellationToken>()))
        //         .ReturnsAsync(itemListing);
        //
        //     this.itemSearchOverlayViewModel.Item = new EquippableItem(ItemRarity.Unique);
        //
        //     var cancellationTokenSource = new CancellationTokenSource();
        //     var cancellationToken = cancellationTokenSource.Token;
        //
        //     // act
        //     await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync(cancellationToken);
        //
        //     this.itemListingsViewModelFactoryMock.Verify(x => x.CreateAsync(this.itemSearchOverlayViewModel.Item, itemListing, cancellationToken));
        // }
        //
        // [Test]
        // public async Task SetListingForItemUnderCursorAsyncShouldNotCallCreateOnItemListingsViewModelFactoryIfTradeClientDoesReturnNull()
        // {
        //     await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();
        //
        //     this.itemListingsViewModelFactoryMock.Verify(x => x.CreateAsync(It.IsAny<Item>(), It.IsAny<ItemListingsQueryResult>(), It.IsAny<CancellationToken>()), Times.Never);
        // }
        //
        // [Test]
        // public async Task SetListingForItemUnderCursorAsyncShouldSetItemListingsViewModel()
        // {
        //     ItemListingsViewModel expected = new ItemListingsViewModel();
        //
        //     this.poeTradeApiClientMock.Setup(x => x.GetListingsAsync(It.IsAny<IQueryRequest>(), It.IsAny<CancellationToken>()))
        //         .ReturnsAsync(new ItemListingsQueryResult());
        //
        //     this.itemListingsViewModelFactoryMock.Setup(x => x.CreateAsync(It.IsAny<Item>(), It.IsAny<ItemListingsQueryResult>(), It.IsAny<CancellationToken>()))
        //         .ReturnsAsync(expected);
        //
        //     await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();
        //
        //     Assert.That(this.itemSearchOverlayViewModel.ItemListings, Is.EqualTo(expected));
        // }
        //
        // [Test]
        // public async Task SetListingForItemUnderCursorAsyncShouldSetMessageIfExceptionOccurs()
        // {
        //     Exception exception = new Exception("Exception text");
        //
        //     this.mediatorMock
        //         .Setup(x => x.Send(It.IsAny<GetItemFromCursorQuery>(), It.IsAny<CancellationToken>()))
        //         .Throws(exception);
        //
        //     await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();
        //
        //     Assert.IsNotNull(this.itemSearchOverlayViewModel.Message);
        //     Assert.That(this.itemSearchOverlayViewModel.Message.Type, Is.EqualTo(MessageType.Error));
        //     Assert.IsNotNull(this.itemSearchOverlayViewModel.Message.Text);
        //     Assert.IsNotEmpty(this.itemSearchOverlayViewModel.Message.Text);
        // }
        //
        // [Test]
        // public async Task SetListingForItemUnderCursorAsyncShouldNotSetMessageIfInvalidItemStringExceptionOccurs()
        // {
        //     var exception = new InvalidItemStringException("invalid item string");
        //
        //     this.mediatorMock
        //         .Setup(x => x.Send(It.IsAny<GetItemFromCursorQuery>(), It.IsAny<CancellationToken>()))
        //         .Throws(exception);
        //
        //     await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();
        //
        //     Assert.IsNull(this.itemSearchOverlayViewModel.Message);
        // }
        //
        // [Test]
        // public async Task SetListingForItemUnderCursorAsyncShouldSetMessageToNull()
        // {
        //     this.itemSearchOverlayViewModel.Message = new Message();
        //
        //     await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();
        //
        //     Assert.IsNull(this.itemSearchOverlayViewModel.Message);
        // }
        //
        // [Test]
        // public async Task SetListingForItemUnderCursorAsyncShouldCallLoadOnAdvancedFiltersViewModel()
        // {
        //     var item = new EquippableItem(ItemRarity.Unique);
        //     var searchQueryRequest = new SearchQueryRequest();
        //     
        //     var itemListingsResult = new ItemListingsQueryResult
        //     {
        //         SearchQueryRequest = searchQueryRequest
        //     };
        //
        //     this.mediatorMock
        //         .Setup(x => x.Send(It.IsAny<GetItemFromCursorQuery>(), It.IsAny<CancellationToken>()))
        //         .ReturnsAsync(item);
        //
        //     this.queryRequestFactoryMock
        //         .Setup(x => x.Create(It.IsAny<Item>()))
        //         .Returns(searchQueryRequest);
        //
        //     this.poeTradeApiClientMock.Setup(x => x.GetListingsAsync(It.IsAny<IQueryRequest>(), It.IsAny<CancellationToken>()))
        //         .ReturnsAsync(itemListingsResult);
        //
        //     await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();
        //     
        //     this.advancedFiltersViewModelMock.Verify(x => x.LoadAsync(item, searchQueryRequest, default));
        // }
        //
        // [Test]
        // public async Task ExecuteAdvancedQueryAsyncShouldCallCreateOnQueryRequestFactory()
        // {
        //     var searchQueryRequest = new SearchQueryRequest();
        //     this.itemSearchOverlayViewModel.QueryRequest = searchQueryRequest;
        //
        //     await this.itemSearchOverlayViewModel.ExecuteAdvancedQueryAsync();
        //
        //     this.queryRequestFactoryMock.Verify(x => x.Create(searchQueryRequest, this.advancedFiltersViewModelMock.Object));
        // }
        //
        // [Test]
        // public async Task ExecuteAdvancedQueryAsyncShouldCallGetListingsAsyncOnPoeTradeApiClient()
        // {
        //     var queryRequest = new SearchQueryRequest();
        //
        //     this.queryRequestFactoryMock.Setup(x => x.Create( It.IsAny<IQueryRequest>(), It.IsAny<IAdvancedFiltersViewModel>()))
        //         .Returns(queryRequest);
        //
        //     await this.itemSearchOverlayViewModel.ExecuteAdvancedQueryAsync();
        //
        //     this.poeTradeApiClientMock.Verify(x => x.GetListingsAsync(queryRequest, It.IsAny<CancellationToken>()));
        // }
        //
        // [Test]
        // public async Task ExecuteAdvancedQueryAsyncShouldCallCreateOnItemListingsViewModelFactory()
        // {
        //     var itemListing = new ItemListingsQueryResult();
        //     this.poeTradeApiClientMock.Setup(x => x.GetListingsAsync(It.IsAny<IQueryRequest>(), It.IsAny<CancellationToken>()))
        //         .ReturnsAsync(itemListing);
        //
        //     this.itemSearchOverlayViewModel.Item = new EquippableItem(ItemRarity.Unique);
        //
        //     await this.itemSearchOverlayViewModel.ExecuteAdvancedQueryAsync();
        //
        //     this.itemListingsViewModelFactoryMock.Verify(x => x.CreateAsync(this.itemSearchOverlayViewModel.Item, itemListing, It.IsAny<CancellationToken>()));
        // }
        //
        // [Test]
        // public async Task ExecuteAdvancedQueryAsyncShouldSetItemListingsViewModel()
        // {
        //     ItemListingsViewModel expected = new ItemListingsViewModel();
        //
        //     this.poeTradeApiClientMock.Setup(x => x.GetListingsAsync(It.IsAny<IQueryRequest>(), It.IsAny<CancellationToken>()))
        //         .ReturnsAsync(new ItemListingsQueryResult());
        //
        //     this.itemListingsViewModelFactoryMock.Setup(x => x.CreateAsync(It.IsAny<Item>(), It.IsAny<ItemListingsQueryResult>(), It.IsAny<CancellationToken>()))
        //         .ReturnsAsync(expected);
        //
        //     await this.itemSearchOverlayViewModel.ExecuteAdvancedQueryAsync();
        //
        //     Assert.That(this.itemSearchOverlayViewModel.ItemListings, Is.EqualTo(expected));
        // }
        //
        // [Test]
        // public async Task SetListingForItemUnderCursorAsyncShouldCallLoadAsyncOnPricePredictionViewModel()
        // {
        //     // arrange
        //     var item = new EquippableItem(ItemRarity.Rare)
        //     {
        //         ItemText = "text"
        //     };
        //
        //     this.mediatorMock
        //         .Setup(x => x.Send(It.IsAny<GetItemFromCursorQuery>(), It.IsAny<CancellationToken>()))
        //         .ReturnsAsync(item);
        //
        //     var cancellationTokenSource = new CancellationTokenSource();
        //     var cancellationToken = cancellationTokenSource.Token;
        //
        //     // act
        //     await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync(cancellationToken);
        //
        //     // assert
        //     this.pricePredictionViewModelMock.Verify(x => x.LoadAsync(item, cancellationToken));
        // }
    }
}