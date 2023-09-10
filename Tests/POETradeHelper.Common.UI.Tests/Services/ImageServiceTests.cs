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

using NSubstitute;
using NSubstitute.ExceptionExtensions;

using NUnit.Framework;

using POETradeHelper.Common.UI.Services;
using POETradeHelper.Common.Wrappers;

namespace POETradeHelper.Common.UI.Tests.Services
{
    public class ImageServiceTests
    {
        private IHttpClientWrapper httpClientWrapperMock;
        private IBitmapFactory bitmapFactoryMock;
        private ImageService imageService;

        private static readonly Uri uri = new("http://www.google.de");

        [SetUp]
        public void Setup()
        {
            this.httpClientWrapperMock = Substitute.For<IHttpClientWrapper>();
            this.bitmapFactoryMock = Substitute.For<IBitmapFactory>();

            var httpClientFactoryWrapperMock = Substitute.For<IHttpClientFactoryWrapper>();
            httpClientFactoryWrapperMock.CreateClient()
                .Returns(this.httpClientWrapperMock);

            this.imageService = new ImageService(httpClientFactoryWrapperMock, this.bitmapFactoryMock);
        }

        [Test]
        public async Task GetImageAsyncShouldCallGetAsyncOnHttpClient()
        {
            // arrange
            this.httpClientWrapperMock.GetAsync(Arg.Any<Uri>(), Arg.Any<CancellationToken>())
                .Returns(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest
                });

            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            // act
            await this.imageService.GetImageAsync(uri, cancellationToken);

            // assert
            await this.httpClientWrapperMock
                .Received()
                .GetAsync(uri, cancellationToken);
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
            this.bitmapFactoryMock
                .Received()
                .Create(stream);
        }

        [Test]
        public async Task GetImageAsyncShouldReturnImageIfHttpResponseIndicatesSuccess()
        {
            // arrange
            IBitmap expected = new TestBitmap();
            this.MockHttpClientGetAsyncSuccessResponse();
            this.bitmapFactoryMock.Create(Arg.Any<Stream>())
                .Returns(expected);

            // act
            IBitmap result = await this.imageService.GetImageAsync(uri);

            // assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public async Task GetImageAsyncShouldReturnNullIfHttpResponseDoesNotIndicatesSuccess()
        {
            HttpResponseMessage httpResponse = new()
            {
                StatusCode = HttpStatusCode.BadRequest
            };

            this.httpClientWrapperMock.GetAsync(Arg.Any<Uri>(), Arg.Any<CancellationToken>())
                .Returns(httpResponse);

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
                .GetAsync(Arg.Any<Uri>(), Arg.Any<CancellationToken>())
                .ThrowsAsync((Exception)Activator.CreateInstance(exceptionType));

            // act
            async Task Action() => await this.imageService.GetImageAsync(uri, cancellationToken);

            // assert
            Assert.ThrowsAsync(exceptionType, Action);
        }

        private HttpResponseMessage MockHttpClientGetAsyncSuccessResponse(Stream stream = null)
        {
            HttpResponseMessage httpResponse = new()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StreamContent(stream ?? new MemoryStream())
            };

            this.httpClientWrapperMock.GetAsync(Arg.Any<Uri>(), Arg.Any<CancellationToken>())
                .Returns(httpResponse);
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
