﻿using System;
using System.Threading;
using System.Threading.Tasks;
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

namespace POETradeHelper.ItemSearch.Tests.ViewModels
{
    public class ItemSearchOverlayViewModelTests
    {
        private Mock<IPoeTradeApiClient> poeTradeApiClientMock;
        private Mock<IItemListingsViewModelFactory> itemListingsViewModelFactoryMock;
        private Mock<IQueryRequestFactory> queryRequestFactoryMock;
        private Mock<IMediator> mediatorMock;
        private Mock<IPricePredictionViewModel> pricePredictionViewModelMock;
        private Mock<IAdvancedFiltersViewModel> advancedFiltersViewModelMock;
        private ItemSearchResultOverlayViewModel itemSearchOverlayViewModel;

        [SetUp]
        public void Setup()
        {
            this.poeTradeApiClientMock = new Mock<IPoeTradeApiClient>();
            this.itemListingsViewModelFactoryMock = new Mock<IItemListingsViewModelFactory>();
            this.queryRequestFactoryMock = new Mock<IQueryRequestFactory>();
            this.mediatorMock = new Mock<IMediator>();

            this.pricePredictionViewModelMock = new Mock<IPricePredictionViewModel>();
            this.advancedFiltersViewModelMock = new Mock<IAdvancedFiltersViewModel>();
            this.itemSearchOverlayViewModel = this.CreateViewModel();
        }

        private ItemSearchResultOverlayViewModel CreateViewModel()
        {
            return new ItemSearchResultOverlayViewModel(
                this.poeTradeApiClientMock.Object,
                this.itemListingsViewModelFactoryMock.Object,
                this.queryRequestFactoryMock.Object,
                this.mediatorMock.Object,
                this.pricePredictionViewModelMock.Object,
                this.advancedFiltersViewModelMock.Object);
        }

        [Test]
        public async Task SetListingForItemUnderCursorAsyncShouldSendGetItemFromCursorQuery()
        {
            var cancellationToken = new CancellationToken();

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync(cancellationToken);

            this.mediatorMock.Verify(x => x.Send(It.IsAny<GetItemFromCursorQuery>(), cancellationToken));
        }

        [Test]
        public async Task SetListingForItemUnderCursorAsyncShouldCallCreateOnQueryRequestFactoryWithItem()
        {
            var item = new EquippableItem(ItemRarity.Normal) { Name = "TestItem" };
            this.mediatorMock
                .Setup(x => x.Send(It.IsAny<GetItemFromCursorQuery>(), It.IsAny<CancellationToken>()))
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

            this.mediatorMock
                .Setup(x => x.Send(It.IsAny<GetItemFromCursorQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            Assert.That(this.itemSearchOverlayViewModel.Item, Is.EqualTo(item));
        }

        [Test]
        public async Task SetListingForItemUnderCursorAsyncShouldCallCreateOnItemListingsViewModelFactoryIfTradeClientDoesNotReturnNull()
        {
            // arrange
            var itemListing = new ItemListingsQueryResult();
            this.poeTradeApiClientMock.Setup(x => x.GetListingsAsync(It.IsAny<IQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(itemListing);

            this.itemSearchOverlayViewModel.Item = new EquippableItem(ItemRarity.Unique);

            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            // act
            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync(cancellationToken);

            this.itemListingsViewModelFactoryMock.Verify(x => x.CreateAsync(this.itemSearchOverlayViewModel.Item, itemListing, cancellationToken));
        }

        [Test]
        public async Task SetListingForItemUnderCursorAsyncShouldNotCallCreateOnItemListingsViewModelFactoryIfTradeClientDoesReturnNull()
        {
            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            this.itemListingsViewModelFactoryMock.Verify(x => x.CreateAsync(It.IsAny<Item>(), It.IsAny<ItemListingsQueryResult>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task SetListingForItemUnderCursorAsyncShouldSetItemListingsViewModel()
        {
            ItemListingsViewModel expected = new ItemListingsViewModel();

            this.poeTradeApiClientMock.Setup(x => x.GetListingsAsync(It.IsAny<IQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ItemListingsQueryResult());

            this.itemListingsViewModelFactoryMock.Setup(x => x.CreateAsync(It.IsAny<Item>(), It.IsAny<ItemListingsQueryResult>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            Assert.That(this.itemSearchOverlayViewModel.ItemListings, Is.EqualTo(expected));
        }

        [Test]
        public async Task SetListingForItemUnderCursorAsyncShouldSetMessageIfExceptionOccurs()
        {
            Exception exception = new Exception("Exception text");

            this.mediatorMock
                .Setup(x => x.Send(It.IsAny<GetItemFromCursorQuery>(), It.IsAny<CancellationToken>()))
                .Throws(exception);

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            Assert.IsNotNull(this.itemSearchOverlayViewModel.Message);
            Assert.That(this.itemSearchOverlayViewModel.Message.Type, Is.EqualTo(MessageType.Error));
            Assert.IsNotNull(this.itemSearchOverlayViewModel.Message.Text);
            Assert.IsNotEmpty(this.itemSearchOverlayViewModel.Message.Text);
        }
        
        [Test]
        public async Task SetListingForItemUnderCursorAsyncShouldNotSetMessageIfInvalidItemStringExceptionOccurs()
        {
            var exception = new InvalidItemStringException("invalid item string");

            this.mediatorMock
                .Setup(x => x.Send(It.IsAny<GetItemFromCursorQuery>(), It.IsAny<CancellationToken>()))
                .Throws(exception);

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            Assert.IsNull(this.itemSearchOverlayViewModel.Message);
        }

        [Test]
        public async Task SetListingForItemUnderCursorAsyncShouldSetMessageToNull()
        {
            this.itemSearchOverlayViewModel.Message = new Message();

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();

            Assert.IsNull(this.itemSearchOverlayViewModel.Message);
        }

        [Test]
        public async Task SetListingForItemUnderCursorAsyncShouldCallLoadOnAdvancedFiltersViewModel()
        {
            var item = new EquippableItem(ItemRarity.Unique);
            var searchQueryRequest = new SearchQueryRequest();
            
            var itemListingsResult = new ItemListingsQueryResult
            {
                SearchQueryRequest = searchQueryRequest
            };

            this.mediatorMock
                .Setup(x => x.Send(It.IsAny<GetItemFromCursorQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);

            this.queryRequestFactoryMock
                .Setup(x => x.Create(It.IsAny<Item>()))
                .Returns(searchQueryRequest);

            this.poeTradeApiClientMock.Setup(x => x.GetListingsAsync(It.IsAny<IQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(itemListingsResult);

            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync();
            
            this.advancedFiltersViewModelMock.Verify(x => x.LoadAsync(item, searchQueryRequest, default));
        }

        [Test]
        public async Task ExecuteAdvancedQueryAsyncShouldCallCreateOnQueryRequestFactory()
        {
            var searchQueryRequest = new SearchQueryRequest();
            this.itemSearchOverlayViewModel.QueryRequest = searchQueryRequest;

            await this.itemSearchOverlayViewModel.ExecuteAdvancedQueryAsync();

            this.queryRequestFactoryMock.Verify(x => x.Create(searchQueryRequest, this.advancedFiltersViewModelMock.Object));
        }

        [Test]
        public async Task ExecuteAdvancedQueryAsyncShouldCallGetListingsAsyncOnPoeTradeApiClient()
        {
            var queryRequest = new SearchQueryRequest();

            this.queryRequestFactoryMock.Setup(x => x.Create( It.IsAny<IQueryRequest>(), It.IsAny<IAdvancedFiltersViewModel>()))
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

            this.itemListingsViewModelFactoryMock.Verify(x => x.CreateAsync(this.itemSearchOverlayViewModel.Item, itemListing, It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task ExecuteAdvancedQueryAsyncShouldSetItemListingsViewModel()
        {
            ItemListingsViewModel expected = new ItemListingsViewModel();

            this.poeTradeApiClientMock.Setup(x => x.GetListingsAsync(It.IsAny<IQueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ItemListingsQueryResult());

            this.itemListingsViewModelFactoryMock.Setup(x => x.CreateAsync(It.IsAny<Item>(), It.IsAny<ItemListingsQueryResult>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            await this.itemSearchOverlayViewModel.ExecuteAdvancedQueryAsync();

            Assert.That(this.itemSearchOverlayViewModel.ItemListings, Is.EqualTo(expected));
        }

        [Test]
        public async Task SetListingForItemUnderCursorAsyncShouldCallLoadAsyncOnPricePredictionViewModel()
        {
            // arrange
            var item = new EquippableItem(ItemRarity.Rare)
            {
                ItemText = "text"
            };

            this.mediatorMock
                .Setup(x => x.Send(It.IsAny<GetItemFromCursorQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(item);

            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            // act
            await this.itemSearchOverlayViewModel.SetListingForItemUnderCursorAsync(cancellationToken);

            // assert
            this.pricePredictionViewModelMock.Verify(x => x.LoadAsync(item, cancellationToken));
        }
    }
}