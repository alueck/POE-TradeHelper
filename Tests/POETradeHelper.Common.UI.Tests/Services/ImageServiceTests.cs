using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NUnit.Framework;
using POETradeHelper.Common.UI.Services;
using POETradeHelper.Common.Wrappers;

namespace POETradeHelper.Common.UI.Tests.Services
{
    public class ImageServiceTests
    {
        private Mock<IMemoryCache> memoryCacheMock;
        private Mock<IHttpClientWrapper> httpClientWrapperMock;
        private Mock<IBitmapFactory> bitmapFactoryMock;
        private ImageService imageService;

        private static readonly Uri uri = new Uri("http://www.google.de");

        [SetUp]
        public void Setup()
        {
            this.memoryCacheMock = new Mock<IMemoryCache>();
            this.httpClientWrapperMock = new Mock<IHttpClientWrapper>();
            this.bitmapFactoryMock = new Mock<IBitmapFactory>();

            var httpClientFactoryWrapperMock = new Mock<IHttpClientFactoryWrapper>();
            httpClientFactoryWrapperMock.Setup(x => x.CreateClient())
                .Returns(this.httpClientWrapperMock.Object);

            this.imageService = new ImageService(this.memoryCacheMock.Object, httpClientFactoryWrapperMock.Object, this.bitmapFactoryMock.Object);
        }

        [Test]
        public async Task GetImageAsyncShouldCallGetAsyncOnHttpClientIfImageIsNotCached()
        {
            // arrange
            this.httpClientWrapperMock.Setup(x => x.GetAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new System.Net.Http.HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest
                });

            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            // act
            await this.imageService.GetImageAsync(uri, cancellationToken);

            // assert
            this.httpClientWrapperMock.Verify(x => x.GetAsync(uri, cancellationToken));
        }

        [Test]
        public async Task GetImageAsyncShouldReturnCachedImage()
        {
            // arrange
            object obj = new TestBitmap();
            this.memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out obj))
                .Returns(true);

            // act
            IBitmap result = await this.imageService.GetImageAsync(uri);

            // assert
            Assert.That(result, Is.EqualTo(obj));
            this.httpClientWrapperMock.Verify(x => x.GetAsync(uri, It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task GetImageAsyncShouldCallCreateOnBitmapFactoryIfHttpResponseIndicatesSuccess()
        {
            // arrange
            HttpResponseMessage httpResponse = this.MockHttpClientGetAsyncSuccessResponse();

            this.memoryCacheMock.Setup(x => x.CreateEntry(It.IsAny<object>()))
                .Returns(Mock.Of<ICacheEntry>());

            // act
            await this.imageService.GetImageAsync(uri);

            // assert
            var stream = await httpResponse.Content.ReadAsStreamAsync();
            this.bitmapFactoryMock.Verify(x => x.Create(stream));
        }

        [Test]
        public async Task GetImageAsyncShouldCallCreateEntryOnMemoryCache()
        {
            // arrange
            this.MockHttpClientGetAsyncSuccessResponse();

            this.memoryCacheMock.Setup(x => x.CreateEntry(It.IsAny<object>()))
                .Returns(Mock.Of<ICacheEntry>());

            // act
            await this.imageService.GetImageAsync(uri);

            // assert
            this.memoryCacheMock.Verify(x => x.CreateEntry(uri));
        }

        [Test]
        public async Task GetImageAsyncShouldSetValueOnCacheEntry()
        {
            // arrange
            var expectedValue = new TestBitmap();

            this.MockHttpClientGetAsyncSuccessResponse();

            var cacheEntryMock = new Mock<ICacheEntry>();

            this.memoryCacheMock.Setup(x => x.CreateEntry(It.IsAny<object>()))
                .Returns(cacheEntryMock.Object);

            this.bitmapFactoryMock.Setup(x => x.Create(It.IsAny<Stream>()))
                .Returns(expectedValue);

            // act
            await this.imageService.GetImageAsync(uri);

            // assert
            cacheEntryMock.VerifySet(x => x.Value = expectedValue);
        }

        [Test]
        public async Task GetImageAsyncShouldSetSizeOnCacheEntry()
        {
            // arrange
            var expectedValue = 123;

            var memoryStream = new MemoryStream();
            memoryStream.SetLength(expectedValue);

            this.MockHttpClientGetAsyncSuccessResponse(memoryStream);

            var cacheEntryMock = new Mock<ICacheEntry>();

            this.memoryCacheMock.Setup(x => x.CreateEntry(It.IsAny<object>()))
                .Returns(cacheEntryMock.Object);

            // act
            await this.imageService.GetImageAsync(uri);

            // assert
            cacheEntryMock.VerifySet(x => x.Size = expectedValue);
        }

        [Test]
        public async Task GetImageAsyncShouldCallDisposeOnCacheEntryToWriteEntryToCache()
        {
            // arrange
            this.MockHttpClientGetAsyncSuccessResponse();

            var cacheEntryMock = new Mock<ICacheEntry>();

            this.memoryCacheMock.Setup(x => x.CreateEntry(It.IsAny<object>()))
                .Returns(cacheEntryMock.Object);

            // act
            await this.imageService.GetImageAsync(uri);

            // assert
            cacheEntryMock.Verify(x => x.Dispose());
        }

        [Test]
        public async Task GetImageAsyncShouldReturnImageIfHttpResponseIndicatesSuccess()
        {
            // arrange
            IBitmap expected = new TestBitmap();
            this.MockHttpClientGetAsyncSuccessResponse();

            this.memoryCacheMock.Setup(x => x.CreateEntry(It.IsAny<object>()))
                .Returns(Mock.Of<ICacheEntry>());

            this.bitmapFactoryMock.Setup(x => x.Create(It.IsAny<Stream>()))
                .Returns(expected);

            // act
            IBitmap result = await this.imageService.GetImageAsync(uri);

            // assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public async Task GetImageAsyncShouldReturnNullIfHttpResponseDoesNotIndicatesSuccess()
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest
            };

            this.httpClientWrapperMock.Setup(x => x.GetAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(httpResponse);

            IBitmap result = await this.imageService.GetImageAsync(uri);

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetImageAsyncShouldReturnNullIfCancellationIsRequested()
        {
            // arrange
            this.MockHttpClientGetAsyncSuccessResponse();

            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            cancellationTokenSource.Cancel();

            // act
            IBitmap result = await this.imageService.GetImageAsync(uri, cancellationToken);

            // assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetImageAsyncShouldNotCallBitmapFactoryIfCancellationIsRequested()
        {
            // arrange
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            this.httpClientWrapperMock.Setup(x => x.GetAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HttpResponseMessage())
                .Callback(() => cancellationTokenSource.Cancel());

            // act
            await this.imageService.GetImageAsync(uri, cancellationToken);

            // assert
            this.bitmapFactoryMock.Verify(x => x.Create(It.IsAny<Stream>()), Times.Never);
        }

        [Test]
        public async Task GetImageAsyncShouldNotCreateCacheItemIfCancellationIsRequested()
        {
            // arrange
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            this.httpClientWrapperMock.Setup(x => x.GetAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HttpResponseMessage())
                .Callback(() => cancellationTokenSource.Cancel());

            // act
            await this.imageService.GetImageAsync(uri, cancellationToken);

            // assert
            this.memoryCacheMock.Verify(x => x.CreateEntry(It.IsAny<object>()), Times.Never);
        }

        private HttpResponseMessage MockHttpClientGetAsyncSuccessResponse(Stream stream = null)
        {
            HttpResponseMessage httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StreamContent(stream ?? new MemoryStream())
            };

            this.httpClientWrapperMock.Setup(x => x.GetAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(httpResponse);
            return httpResponse;
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