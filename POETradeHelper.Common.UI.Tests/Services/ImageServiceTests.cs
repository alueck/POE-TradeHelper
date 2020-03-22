using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NUnit.Framework;
using POETradeHelper.Common.UI.Services;
using POETradeHelper.Common.Wrappers;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace POETradeHelper.Common.UI.Tests.Services
{
    public class ImageServiceTests
    {
        private Mock<IMemoryCache> memoryCacheMock;
        private Mock<IHttpClientWrapper> httpClientWrapperMock;
        private Mock<IImageFactory> imageFactoryMock;
        private ImageService imageService;

        [SetUp]
        public void Setup()
        {
            this.memoryCacheMock = new Mock<IMemoryCache>();
            this.httpClientWrapperMock = new Mock<IHttpClientWrapper>();
            this.imageFactoryMock = new Mock<IImageFactory>();

            var httpClientFactoryWrapperMock = new Mock<IHttpClientFactoryWrapper>();
            httpClientFactoryWrapperMock.Setup(x => x.CreateClient())
                .Returns(this.httpClientWrapperMock.Object);

            this.imageService = new ImageService(this.memoryCacheMock.Object, httpClientFactoryWrapperMock.Object, this.imageFactoryMock.Object);
        }

        [Test]
        public async Task GetImageAsyncShouldCallGetAsyncOnHttpClientIfImageIsNotCached()
        {
            Uri uri = new Uri("http://www.google.de");

            this.httpClientWrapperMock.Setup(x => x.GetAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new System.Net.Http.HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest
                });

            await this.imageService.GetImageAsync(uri);

            this.httpClientWrapperMock.Verify(x => x.GetAsync(uri, It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task GetImageAsyncShouldReturnCachedImage()
        {
            Uri uri = new Uri("http://www.google.de");

            this.httpClientWrapperMock.Setup(x => x.GetAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new System.Net.Http.HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest
                });

            object obj = new TestBitmap();
            this.memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out obj))
                .Returns(true);

            IBitmap result = await this.imageService.GetImageAsync(uri);

            Assert.That(result, Is.EqualTo(obj));
            this.httpClientWrapperMock.Verify(x => x.GetAsync(uri, It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task GetImageAsyncShouldCallCreateOnImageFactoryIfHttpResponseIndicatesSuccess()
        {
            Uri uri = new Uri("http://www.google.de");

            var memoryStream = new MemoryStream();

            HttpResponseMessage httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StreamContent(memoryStream)
            };

            this.httpClientWrapperMock.Setup(x => x.GetAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(httpResponse);

            this.memoryCacheMock.Setup(x => x.CreateEntry(It.IsAny<object>()))
                .Returns(Mock.Of<ICacheEntry>());

            await this.imageService.GetImageAsync(uri);

            var stream = await httpResponse.Content.ReadAsStreamAsync();
            this.imageFactoryMock.Verify(x => x.Create(stream));
        }

        [Test]
        public async Task GetImageAsyncShouldCallCreateEntryOnMemoryCache()
        {
            Uri uri = new Uri("http://www.google.de");

            var memoryStream = new MemoryStream();

            HttpResponseMessage httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StreamContent(memoryStream)
            };

            this.httpClientWrapperMock.Setup(x => x.GetAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(httpResponse);

            this.memoryCacheMock.Setup(x => x.CreateEntry(It.IsAny<object>()))
                .Returns(Mock.Of<ICacheEntry>());

            await this.imageService.GetImageAsync(uri);

            this.memoryCacheMock.Verify(x => x.CreateEntry(uri));
        }

        [Test]
        public async Task GetImageAsyncShouldSetValueOnCacheEntry()
        {
            Uri uri = new Uri("http://www.google.de");
            var expectedValue = new TestBitmap();

            var memoryStream = new MemoryStream();

            HttpResponseMessage httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StreamContent(memoryStream)
            };

            this.httpClientWrapperMock.Setup(x => x.GetAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(httpResponse);

            var cacheEntryMock = new Mock<ICacheEntry>();

            this.memoryCacheMock.Setup(x => x.CreateEntry(It.IsAny<object>()))
                .Returns(cacheEntryMock.Object);

            this.imageFactoryMock.Setup(x => x.Create(It.IsAny<Stream>()))
                .Returns(expectedValue);

            await this.imageService.GetImageAsync(uri);

            cacheEntryMock.VerifySet(x => x.Value = expectedValue);
        }

        [Test]
        public async Task GetImageAsyncShouldSetSizeOnCacheEntry()
        {
            Uri uri = new Uri("http://www.google.de");
            var expectedValue = 123;

            var memoryStream = new MemoryStream();
            memoryStream.SetLength(expectedValue);

            HttpResponseMessage httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StreamContent(memoryStream)
            };

            this.httpClientWrapperMock.Setup(x => x.GetAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(httpResponse);

            var cacheEntryMock = new Mock<ICacheEntry>();

            this.memoryCacheMock.Setup(x => x.CreateEntry(It.IsAny<object>()))
                .Returns(cacheEntryMock.Object);

            await this.imageService.GetImageAsync(uri);

            cacheEntryMock.VerifySet(x => x.Size = expectedValue);
        }

        [Test]
        public async Task GetImageAsyncShouldReturnImageIfHttpResponseIndicatesSuccess()
        {
            IBitmap expected = new TestBitmap();
            Uri uri = new Uri("http://www.google.de");

            var memoryStream = new MemoryStream();

            HttpResponseMessage httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StreamContent(memoryStream)
            };

            this.httpClientWrapperMock.Setup(x => x.GetAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(httpResponse);

            this.memoryCacheMock.Setup(x => x.CreateEntry(It.IsAny<object>()))
                .Returns(Mock.Of<ICacheEntry>());

            this.imageFactoryMock.Setup(x => x.Create(It.IsAny<Stream>()))
                .Returns(expected);

            IBitmap result = await this.imageService.GetImageAsync(uri);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public async Task GetImageAsyncShouldReturnNullIfHttpResponseDoesNotIndicatesSuccess()
        {
            Uri uri = new Uri("http://www.google.de");

            HttpResponseMessage httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest
            };

            this.httpClientWrapperMock.Setup(x => x.GetAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(httpResponse);

            IBitmap result = await this.imageService.GetImageAsync(uri);

            Assert.IsNull(result);
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