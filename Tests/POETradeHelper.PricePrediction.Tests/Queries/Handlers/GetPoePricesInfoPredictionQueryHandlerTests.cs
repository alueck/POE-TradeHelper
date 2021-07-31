using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PricePrediction.Models;
using POETradeHelper.PricePrediction.Queries;
using POETradeHelper.PricePrediction.Queries.Handlers;
using POETradeHelper.PricePrediction.Services;

namespace POETradeHelper.PricePrediction.Tests.Queries.Handlers
{
    public class GetPoePricesInfoPredictionQueryHandlerTests
    {
        private Mock<IPoePricesInfoClient> poePricesInfoClientMock;
        private Mock<ICacheEntry> cacheEntryMock;
        private Mock<IMemoryCache> memoryCacheMock;
        private GetPoePricesInfoPredictionQueryHandler handler;
        
        private EquippableItem validItem;
        private GetPoePricesInfoPredictionQuery validRequest;

        [SetUp]
        public void Setup()
        {
            this.poePricesInfoClientMock = new Mock<IPoePricesInfoClient>();
            this.cacheEntryMock = new Mock<ICacheEntry>();
            this.memoryCacheMock = new Mock<IMemoryCache>();
            this.memoryCacheMock.Setup(x => x.CreateEntry(It.IsAny<object>()))
                .Returns(this.cacheEntryMock.Object);
            this.handler = new GetPoePricesInfoPredictionQueryHandler(
                this.poePricesInfoClientMock.Object,
                this.memoryCacheMock.Object);

            this.validItem = new EquippableItem(ItemRarity.Rare)
            {
                ItemText = "abc"
            };
            this.validRequest = new GetPoePricesInfoPredictionQuery(this.validItem, new League { Id = "Heist" });
        }
        
        [Test]
        public async Task HandleShouldCallGetPricePredictionAsyncOnPoePricesInfoClient()
        {
            // arrange
            var cancellationToken = new CancellationToken();
            const string league = "Heist";
            var request = new GetPoePricesInfoPredictionQuery(this.validItem, new League { Id = league });

            // act
            await this.handler.Handle(request, cancellationToken);

            // assert
            this.poePricesInfoClientMock.Verify(x => x.GetPricePredictionAsync(league, this.validItem.ItemText, cancellationToken));
        }
        
        [Test]
        public async Task HandleShouldCallTryGetValueOnMemoryCache()
        {
            // act
            await this.handler.Handle(this.validRequest, default);

            // assert
            object obj = null;
            this.memoryCacheMock.Verify(x => x.TryGetValue(this.validRequest.Item.ItemText, out obj));
        }

        [Test]
        public async Task HandleShouldNotCallHandleOnPoePricesInfoClientIfPredictionIsCached()
        {
            // arrange
            object obj = null;
            this.memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out obj))
                .Returns(true);

            // act
            await this.handler.Handle(this.validRequest, default);

            // assert
            this.poePricesInfoClientMock.Verify(
                x => x.GetPricePredictionAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Test]
        public async Task HandleShouldCallCreateEntryOnMemoryCache()
        {
            // arrange
            this.MockPoePricesInfoClient();

            // act
            await this.handler.Handle(this.validRequest, default);

            // assert
            this.memoryCacheMock.Verify(x => x.CreateEntry(this.validRequest.Item.ItemText));
        }

        [Test]
        public async Task HandleShouldNotCallCreateEntryOnMemoryCacheIfCancellationIsRequested()
        {
            // arrange
            this.MockPoePricesInfoClient();

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            // act
            await this.handler.Handle(this.validRequest, cancellationTokenSource.Token);

            // assert
            this.memoryCacheMock.Verify(x => x.CreateEntry(It.IsAny<object>()), Times.Never);
        }

        [Test]
        public async Task HandleShouldNotCallCreateEntryOnMemoryCacheIfPredictionIsNull()
        {
            // act
            await this.handler.Handle(this.validRequest, default);

            // assert
            this.memoryCacheMock.Verify(x => x.CreateEntry(It.IsAny<object>()), Times.Never);
        }

        [Test]
        public async Task HandleShouldSetValueOnCacheEntry()
        {
            // arrange
            PoePricesInfoPrediction poePricesInfoPrediction = this.MockPoePricesInfoClient();

            // act
            await this.handler.Handle(this.validRequest, default);

            // assert
            this.cacheEntryMock.VerifySet(x => x.Value = poePricesInfoPrediction);
        }

        [Test]
        public async Task HandleShouldCallSetSizeOnCacheEntry()
        {
            // arrange
            this.MockPoePricesInfoClient();

            // act
            await this.handler.Handle(this.validRequest, default);

            // assert
            this.cacheEntryMock.VerifySet(x => x.Size = It.IsAny<long>());
        }

        [Test]
        public async Task HandleShouldCallSetAbsoluteExpirationOnCacheEntry()
        {
            // arrange
            this.MockPoePricesInfoClient();

            // act
            await this.handler.Handle(this.validRequest, default);

            // assert
            this.cacheEntryMock.VerifySet(x => x.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30));
        }

        [Test]
        public async Task HandleShouldCallDisposeOnCacheEntry()
        {
            // arrange
            this.MockPoePricesInfoClient();

            // act
            await this.handler.Handle(this.validRequest, default);

            // assert
            this.cacheEntryMock.Verify(x => x.Dispose());
        }
        
        private PoePricesInfoPrediction MockPoePricesInfoClient()
        {
            PoePricesInfoPrediction poePricesInfoPrediction = GetPoePricesInfoItem();
            this.poePricesInfoClientMock.Setup(x => x.GetPricePredictionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(poePricesInfoPrediction);

            return poePricesInfoPrediction;
        }

        private static PoePricesInfoPrediction GetPoePricesInfoItem()
        {
            return new PoePricesInfoPrediction
            {
                Min = 0.15m,
                Max = 0.25m,
                Currency = "chaos",
                ConfidenceScore = 85.978m
            };
        }
    }
}