using System;
using System.Threading;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Media;

using NSubstitute;

using NUnit.Framework;

using POETradeHelper.Common.UI.Services;
using POETradeHelper.ItemSearch.UI.Avalonia.Factories.Implementations;
using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Tests.Services.Factories
{
    public class PriceViewModelFactoryTests
    {
        private IStaticDataService staticDataServiceMock;
        private IImageService imageServiceMock;
        private PriceViewModelFactory priceViewModelFactory;

        [SetUp]
        public void Setup()
        {
            this.staticDataServiceMock = Substitute.For<IStaticDataService>();
            this.imageServiceMock = Substitute.For<IImageService>();
            this.priceViewModelFactory = new PriceViewModelFactory(this.staticDataServiceMock, this.imageServiceMock);
        }

        [Test]
        public async Task CreateAsyncShouldReturnNullIfPriceIsNull()
        {
            PriceViewModel result = await this.priceViewModelFactory.CreateAsync(null);

            Assert.IsNull(result);
        }

        [TestCase(1.0)]
        [TestCase(10.0)]
        [TestCase(100.0)]
        [TestCase(1.234)]
        public async Task CreateAsyncShouldSetAmount(decimal amount)
        {
            Price price = new()
            {
                Amount = amount,
            };

            PriceViewModel result = await this.priceViewModelFactory.CreateAsync(price);

            Assert.That(result.Amount, Is.EqualTo(amount.ToString("0.##").PadLeft(6)));
        }

        [Test]
        public async Task CreateAsyncShouldCallGetTextOnStaticItemDataService()
        {
            Price price = new()
            {
                Currency = "alc",
            };

            await this.priceViewModelFactory.CreateAsync(price);

            this.staticDataServiceMock
                .Received()
                .GetText(price.Currency);
        }

        [Test]
        public async Task CreateAsyncShouldSetCurrencyOnPriceViewModel()
        {
            const string expected = "Orb of Alchemy";
            Price price = new();

            this.staticDataServiceMock.GetText(Arg.Any<string>())
                .Returns(expected);

            PriceViewModel result = await this.priceViewModelFactory.CreateAsync(price);

            Assert.That(result.Currency, Is.EqualTo(expected));
        }

        [Test]
        public async Task CreateAsyncShouldCallGetImageUrlOnStaticItemDataService()
        {
            Price price = new()
            {
                Currency = "alc",
            };

            await this.priceViewModelFactory.CreateAsync(price);

            this.staticDataServiceMock
                .Received()
                .GetImageUrl(price.Currency);
        }

        [Test]
        public async Task CreateAsyncShouldCallGetImageOnImageService()
        {
            // arrange
            Uri expected = new("http://www.google.de");
            Price price = new();

            this.staticDataServiceMock.GetImageUrl(Arg.Any<string>())
                .Returns(expected);

            CancellationTokenSource cancellationTokenSource = new();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            // act
            await this.priceViewModelFactory.CreateAsync(price, cancellationToken);

            await this.imageServiceMock
                .Received()
                .GetImageAsync(expected, cancellationToken);
        }

        [Test]
        public async Task CreateAsyncShouldSetImageOnPriceViewModel()
        {
            IImage expected = new TestBitmap();
            Price price = new();

            this.imageServiceMock.GetImageAsync(Arg.Any<Uri>(), Arg.Any<CancellationToken>())
                .Returns(expected);

            PriceViewModel result = await this.priceViewModelFactory.CreateAsync(price);

            Assert.That(result.Image, Is.EqualTo(expected));
        }

        private class TestBitmap : IImage
        {
            public Size Size { get; } = new(1, 1);

            public void Draw(DrawingContext context, Rect sourceRect, Rect destRect)
            {
            }
        }
    }
}