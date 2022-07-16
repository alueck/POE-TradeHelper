using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Utilities;
using Avalonia.Visuals.Media.Imaging;

using Moq;

using NUnit.Framework;

using POETradeHelper.Common.UI.Services;
using POETradeHelper.Common.Wrappers;

namespace POETradeHelper.Common.UI.Tests.Services
{
    public class ImageServiceTests
    {
        private Mock<IHttpClientWrapper> httpClientWrapperMock;
        private Mock<IBitmapFactory> bitmapFactoryMock;
        private ImageService imageService;

        private static readonly Uri uri = new Uri("http://www.google.de");

        [SetUp]
        public void Setup()
        {
            this.httpClientWrapperMock = new Mock<IHttpClientWrapper>();
            this.bitmapFactoryMock = new Mock<IBitmapFactory>();

            var httpClientFactoryWrapperMock = new Mock<IHttpClientFactoryWrapper>();
            httpClientFactoryWrapperMock.Setup(x => x.CreateClient())
                .Returns(this.httpClientWrapperMock.Object);

            this.imageService = new ImageService(httpClientFactoryWrapperMock.Object, this.bitmapFactoryMock.Object);
        }

        [Test]
        public async Task GetImageAsyncShouldCallGetAsyncOnHttpClient()
        {
            // arrange
            this.httpClientWrapperMock.Setup(x => x.GetAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HttpResponseMessage
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
        public async Task GetImageAsyncShouldCallCreateOnBitmapFactoryIfHttpResponseIndicatesSuccess()
        {
            // arrange
            HttpResponseMessage httpResponse = this.MockHttpClientGetAsyncSuccessResponse();

            // act
            await this.imageService.GetImageAsync(uri);

            // assert
            var stream = await httpResponse.Content.ReadAsStreamAsync();
            this.bitmapFactoryMock.Verify(x => x.Create(stream));
        }
        
        [Test]
        public async Task GetImageAsyncShouldReturnImageIfHttpResponseIndicatesSuccess()
        {
            // arrange
            IBitmap expected = new TestBitmap();
            this.MockHttpClientGetAsyncSuccessResponse();
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

        [TestCase(typeof(OperationCanceledException))]
        [TestCase(typeof(TaskCanceledException))]
        public void GetImageAsyncShouldNotCatchException(Type exceptionType)
        {
            // arrange
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            this.httpClientWrapperMock
                .Setup(x => x.GetAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync((Exception)Activator.CreateInstance(exceptionType));

            // act
            async Task Action() => await this.imageService.GetImageAsync(uri, cancellationToken);

            // assert
            Assert.ThrowsAsync(exceptionType, Action);
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

            public void Draw(DrawingContext context, Rect sourceRect, Rect destRect, BitmapInterpolationMode bitmapInterpolationMode)
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