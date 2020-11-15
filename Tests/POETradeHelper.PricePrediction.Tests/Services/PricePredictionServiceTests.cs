using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract;
using POETradeHelper.ItemSearch.Contract.Configuration;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PricePrediction.Models;
using POETradeHelper.PricePrediction.Services;
using POETradeHelper.PricePrediction.Services.Factories;
using POETradeHelper.PricePrediction.ViewModels;

namespace POETradeHelper.PricePrediction.Tests.Services
{
    public class PricePredictionServiceTests
    {
        private Mock<IPoePricesInfoClient> poePricesInfoClientMock;
        private Mock<IOptionsMonitor<ItemSearchOptions>> itemSearchOptionsMock;
        private Mock<ICacheEntry> cacheEntryMock;
        private Mock<IMemoryCache> memoryCacheMock;
        private Mock<IPricePredictionViewModelFactory> pricePredictionViewModelFactory;
        private PricePredictionService pricePredictionService;

        [SetUp]
        public void Setup()
        {
            this.poePricesInfoClientMock = new Mock<IPoePricesInfoClient>();
            this.itemSearchOptionsMock = new Mock<IOptionsMonitor<ItemSearchOptions>>();
            this.itemSearchOptionsMock.Setup(x => x.CurrentValue)
                .Returns(new ItemSearchOptions
                {
                    League = new League()
                });

            this.cacheEntryMock = new Mock<ICacheEntry>();
            this.memoryCacheMock = new Mock<IMemoryCache>();
            this.memoryCacheMock.Setup(x => x.CreateEntry(It.IsAny<object>()))
                .Returns(this.cacheEntryMock.Object);

            this.pricePredictionViewModelFactory = new Mock<IPricePredictionViewModelFactory>();

            this.pricePredictionService = new PricePredictionService(
                this.poePricesInfoClientMock.Object,
                this.itemSearchOptionsMock.Object,
                this.memoryCacheMock.Object,
                this.pricePredictionViewModelFactory.Object);
        }

        [Test]
        public async Task GetPricePredictionAsyncShouldCallPricePredictionViewModelFactoryWithNullIfItemIsNull()
        {
            // act
            await this.pricePredictionService.GetPricePredictionAsync(null);

            // assert
            VerifyNothingButViewModelFactoryInvoked();
        }

        [Test]
        public async Task GetPricePredictionAsyncShouldCallPricePredictionViewModelFactoryWithNullIfItemTextIsNull()
        {
            // arrange
            var item = new EquippableItem(ItemRarity.Rare);

            // act
            await this.pricePredictionService.GetPricePredictionAsync(item);

            // assert
            VerifyNothingButViewModelFactoryInvoked();
        }

        [TestCase(ItemRarity.Normal)]
        [TestCase(ItemRarity.Magic)]
        [TestCase(ItemRarity.Unique)]
        [TestCase(ItemRarity.Currency)]
        [TestCase(ItemRarity.DivinationCard)]
        [TestCase(ItemRarity.Gem)]
        public async Task GetPricePredictionAsyncShouldCallPricePredictionViewModelFactoryWithNullIfItemRarityIsNotRare(ItemRarity itemRarity)
        {
            // arrange
            var item = new EquippableItem(itemRarity)
            {
                ItemText = "abc"
            };

            // act
            await this.pricePredictionService.GetPricePredictionAsync(item);

            // assert
            VerifyNothingButViewModelFactoryInvoked();
        }

        private void VerifyNothingButViewModelFactoryInvoked()
        {
            object obj = null;
            this.memoryCacheMock.Verify(x => x.TryGetValue(It.IsAny<object>(), out obj), Times.Never);
            this.poePricesInfoClientMock.Verify(x => x.GetPricePredictionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            this.pricePredictionViewModelFactory.Verify(x => x.CreateAsync(null, It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task GetPricePredictionAsyncShouldCallGetPricePredictionAsyncOnPoePricesInfoClient()
        {
            // arrange
            Item item = new EquippableItem(ItemRarity.Rare)
            {
                ItemText = "abc"
            };
            var cancellationToken = new CancellationToken();

            const string league = "Heist";
            this.itemSearchOptionsMock.Setup(x => x.CurrentValue)
                .Returns(new ItemSearchOptions
                {
                    League = new League
                    {
                        Id = league
                    }
                });

            // act
            await this.pricePredictionService.GetPricePredictionAsync(item, cancellationToken);

            // assert
            this.poePricesInfoClientMock.Verify(x => x.GetPricePredictionAsync(league, item.ItemText, cancellationToken));
        }

        [Test]
        public async Task GetPricePredictionAsyncShouldCallTryGetValueOnMemoryCache()
        {
            // arrange
            Item item = new EquippableItem(ItemRarity.Rare)
            {
                ItemText = "abc"
            };

            // act
            await this.pricePredictionService.GetPricePredictionAsync(item);

            // assert
            object obj = null;
            this.memoryCacheMock.Verify(x => x.TryGetValue(item.ItemText, out obj));
        }

        [Test]
        public async Task GetPricePredictionAsyncShouldNotCallGetPricePredictionAsyncOnPoePricesInfoClientIfPredictionIsCached()
        {
            // arrange
            Item item = new EquippableItem(ItemRarity.Rare)
            {
                ItemText = "abc"
            };

            object obj = null;
            this.memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out obj))
                .Returns(true);

            // act
            await this.pricePredictionService.GetPricePredictionAsync(item);

            // assert
            this.poePricesInfoClientMock.Verify(x => x.GetPricePredictionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task GetPricePredictionAsyncShouldCallCreateEntryOnMemoryCache()
        {
            // arrange
            this.MockPoePricesInfoClient();

            Item item = new EquippableItem(ItemRarity.Rare)
            {
                ItemText = "abc"
            };

            // act
            await this.pricePredictionService.GetPricePredictionAsync(item);

            // assert
            this.memoryCacheMock.Verify(x => x.CreateEntry(item.ItemText));
        }

        [Test]
        public async Task GetPricePredictionAsyncShouldNotCallCreateEntryOnMemoryCacheIfCancellationIsRequested()
        {
            // arrange
            this.MockPoePricesInfoClient();

            Item item = new EquippableItem(ItemRarity.Rare)
            {
                ItemText = "abc"
            };

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            // act
            await this.pricePredictionService.GetPricePredictionAsync(item, cancellationTokenSource.Token);

            // assert
            this.memoryCacheMock.Verify(x => x.CreateEntry(item.ItemText), Times.Never);
        }

        [Test]
        public async Task GetPricePredictionAsyncShouldNotCallCreateEntryOnMemoryCacheIfPredictionIsNull()
        {
            // arrange
            Item item = new EquippableItem(ItemRarity.Rare)
            {
                ItemText = "abc"
            };

            // act
            await this.pricePredictionService.GetPricePredictionAsync(item);

            // assert
            this.memoryCacheMock.Verify(x => x.CreateEntry(item.ItemText), Times.Never);
        }

        [Test]
        public async Task GetPricePredictionAsyncShouldSetValueOnCacheEntry()
        {
            // arrange
            PoePricesInfoItem poePricesInfoItem = this.MockPoePricesInfoClient();

            Item item = new EquippableItem(ItemRarity.Rare)
            {
                ItemText = "abc"
            };

            // act
            await this.pricePredictionService.GetPricePredictionAsync(item);

            // assert
            this.cacheEntryMock.VerifySet(x => x.Value = poePricesInfoItem);
        }

        [Test]
        public async Task GetPricePredictionAsyncShouldCallSetSizeOnCacheEntry()
        {
            // arrange
            this.MockPoePricesInfoClient();

            Item item = new EquippableItem(ItemRarity.Rare)
            {
                ItemText = "abc"
            };

            // act
            await this.pricePredictionService.GetPricePredictionAsync(item);

            // assert
            this.cacheEntryMock.VerifySet(x => x.Size = It.IsAny<long>());
        }

        [Test]
        public async Task GetPricePredictionAsyncShouldCallSetAbsoluteExpirationOnCacheEntry()
        {
            // arrange
            this.MockPoePricesInfoClient();

            Item item = new EquippableItem(ItemRarity.Rare)
            {
                ItemText = "abc"
            };

            // act
            await this.pricePredictionService.GetPricePredictionAsync(item);

            // assert
            this.cacheEntryMock.VerifySet(x => x.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30));
        }

        [Test]
        public async Task GetPricePredictionAsyncShouldCallDisposeOnCacheEntry()
        {
            // arrange
            this.MockPoePricesInfoClient();

            Item item = new EquippableItem(ItemRarity.Rare)
            {
                ItemText = "abc"
            };

            // act
            await this.pricePredictionService.GetPricePredictionAsync(item);

            // assert
            this.cacheEntryMock.Verify(x => x.Dispose());
        }

        [Test]
        public async Task GetPricePredictionAsyncShouldCallCreateOnPricePredictionViewModelFactory()
        {
            // arrange
            PoePricesInfoItem poePricesInfoItem = this.MockPoePricesInfoClient();

            Item item = new EquippableItem(ItemRarity.Rare)
            {
                ItemText = "abc"
            };

            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            // act
            PricePredictionViewModel result = await this.pricePredictionService.GetPricePredictionAsync(item, cancellationToken);

            // assert
            this.pricePredictionViewModelFactory.Verify(x => x.CreateAsync(poePricesInfoItem, cancellationToken));
        }

        [Test]
        public async Task GetPricePredictionAsyncShouldCallCallCreateOnPricePredictionViewModelFactoryForCachedResult()
        {
            // arrange
            object poePricesInfoItem = GetPoePricesInfoItem();
            this.memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out poePricesInfoItem))
                .Returns(true);

            Item item = new EquippableItem(ItemRarity.Rare)
            {
                ItemText = "abc"
            };

            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            // act
            PricePredictionViewModel result = await this.pricePredictionService.GetPricePredictionAsync(item, cancellationToken);

            // assert
            this.pricePredictionViewModelFactory.Verify(x => x.CreateAsync((PoePricesInfoItem)poePricesInfoItem, cancellationToken));
        }

        [Test]
        public async Task GetPricePredictionAsyncShouldReturnResultFromPricePredictionViewModelFactory()
        {
            // arrange
            PricePredictionViewModel expected = new PricePredictionViewModel
            {
                ConfidenceScore = "85.79 %"
            };
            this.pricePredictionViewModelFactory.Setup(x => x.CreateAsync(It.IsAny<PoePricesInfoItem>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            this.MockPoePricesInfoClient();
            Item item = new EquippableItem(ItemRarity.Rare)
            {
                ItemText = "abc"
            };

            // act
            PricePredictionViewModel result = await this.pricePredictionService.GetPricePredictionAsync(item);

            // assert
            Assert.That(result, Is.EqualTo(expected));
        }

        private PoePricesInfoItem MockPoePricesInfoClient()
        {
            PoePricesInfoItem poePricesInfoItem = GetPoePricesInfoItem();
            this.poePricesInfoClientMock.Setup(x => x.GetPricePredictionAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(poePricesInfoItem);

            return poePricesInfoItem;
        }

        private static PoePricesInfoItem GetPoePricesInfoItem()
        {
            return new PoePricesInfoItem
            {
                Min = 0.15m,
                Max = 0.25m,
                Currency = "chaos",
                ConfidenceScore = 85.978m
            };
        }
    }
}