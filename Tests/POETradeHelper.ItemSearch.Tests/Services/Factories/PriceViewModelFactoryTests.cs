using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Utilities;
using Moq;
using NUnit.Framework;
using POETradeHelper.Common.UI.Services;
using POETradeHelper.ItemSearch.Services.Factories;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.ItemSearch.Tests.Services.Factories
{
    public class PriceViewModelFactoryTests
    {
        private Mock<IStaticDataService> staticDataServiceMock;
        private Mock<IImageService> imageServiceMock;
        private PriceViewModelFactory priceViewModelFactory;

        [SetUp]
        public void Setup()
        {
            this.staticDataServiceMock = new Mock<IStaticDataService>();
            this.imageServiceMock = new Mock<IImageService>();
            this.priceViewModelFactory = new PriceViewModelFactory(this.staticDataServiceMock.Object, this.imageServiceMock.Object);
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
            var price = new Price
            {
                Amount = amount
            };

            PriceViewModel result = await this.priceViewModelFactory.CreateAsync(price);

            Assert.That(result.Amount, Is.EqualTo(amount.ToString("0.##").PadLeft(6)));
        }

        [Test]
        public async Task CreateAsyncShouldCallGetTextOnStaticItemDataService()
        {
            var price = new Price
            {
                Currency = "alc"
            };

            await this.priceViewModelFactory.CreateAsync(price);

            this.staticDataServiceMock.Verify(x => x.GetText(price.Currency));
        }

        [Test]
        public async Task CreateAsyncShouldSetCurrencyOnPriceViewModel()
        {
            const string expected = "Orb of Alchemy";
            var price = new Price();

            this.staticDataServiceMock.Setup(x => x.GetText(It.IsAny<string>()))
                .Returns(expected);

            PriceViewModel result = await this.priceViewModelFactory.CreateAsync(price);

            Assert.That(result.Currency, Is.EqualTo(expected));
        }

        [Test]
        public async Task CreateAsyncShouldCallGetImageUrlOnStaticItemDataService()
        {
            var price = new Price
            {
                Currency = "alc"
            };

            await this.priceViewModelFactory.CreateAsync(price);

            this.staticDataServiceMock.Verify(x => x.GetImageUrl(price.Currency));
        }

        [Test]
        public async Task CreateAsyncShouldCallGetImageOnImageService()
        {
            Uri expected = new Uri("http://www.google.de");
            var price = new Price();

            this.staticDataServiceMock.Setup(x => x.GetImageUrl(It.IsAny<string>()))
                .Returns(expected);

            await this.priceViewModelFactory.CreateAsync(price);

            this.imageServiceMock.Verify(x => x.GetImageAsync(expected));
        }

        [Test]
        public async Task CreateAsyncShouldSetImageOnPriceViewModel()
        {
            IBitmap expected = new TestBitmap();
            var price = new Price();

            this.imageServiceMock.Setup(x => x.GetImageAsync(It.IsAny<Uri>()))
                .ReturnsAsync(expected);

            PriceViewModel result = await this.priceViewModelFactory.CreateAsync(price);

            Assert.That(result.Image, Is.EqualTo(expected));
        }

        private class TestBitmap : IBitmap
        {
            public Vector Dpi => throw new NotImplementedException();

            public PixelSize PixelSize => throw new NotImplementedException();

            public IRef<IBitmapImpl> PlatformImpl => throw new NotImplementedException();

            public Size Size => throw new NotImplementedException();

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public void Save(string fileName)
            {
                throw new NotImplementedException();
            }

            public void Save(Stream stream)
            {
                throw new NotImplementedException();
            }
        }
    }
}