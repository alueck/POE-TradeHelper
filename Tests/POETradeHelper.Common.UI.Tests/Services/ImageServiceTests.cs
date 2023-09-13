using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Media;

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

        private static readonly Uri Uri = new("http://www.google.de");

        [SetUp]
        public void Setup()
        {
            this.httpClientWrapperMock = Substitute.For<IHttpClientWrapper>();
            this.bitmapFactoryMock = Substitute.For<IBitmapFactory>();

            IHttpClientFactoryWrapper httpClientFactoryWrapperMock = Substitute.For<IHttpClientFactoryWrapper>();
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
                    StatusCode = HttpStatusCode.BadRequest,
                });

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            // act
            await this.imageService.GetImageAsync(Uri, cancellationToken);

            // assert
            await this.httpClientWrapperMock
                .Received()
                .GetAsync(Uri, cancellationToken);
        }

        [Test]
        public async Task GetImageAsyncShouldCallCreateOnBitmapFactoryIfHttpResponseIndicatesSuccess()
        {
            // arrange
            HttpResponseMessage httpResponse = this.MockHttpClientGetAsyncSuccessResponse();

            // act
            await this.imageService.GetImageAsync(Uri);

            // assert
            Stream stream = await httpResponse.Content.ReadAsStreamAsync();
            this.bitmapFactoryMock
                .Received()
                .Create(stream);
        }

        [Test]
        public async Task GetImageAsyncShouldReturnImageIfHttpResponseIndicatesSuccess()
        {
            // arrange
            IImage expected = new TestBitmap();
            this.MockHttpClientGetAsyncSuccessResponse();
            this.bitmapFactoryMock.Create(Arg.Any<Stream>())
                .Returns(expected);

            // act
            IImage result = await this.imageService.GetImageAsync(Uri);

            // assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public async Task GetImageAsyncShouldReturnNullIfHttpResponseDoesNotIndicatesSuccess()
        {
            HttpResponseMessage httpResponse = new()
            {
                StatusCode = HttpStatusCode.BadRequest,
            };

            this.httpClientWrapperMock.GetAsync(Arg.Any<Uri>(), Arg.Any<CancellationToken>())
                .Returns(httpResponse);

            IImage result = await this.imageService.GetImageAsync(Uri);

            Assert.IsNull(result);
        }

        [TestCase(typeof(OperationCanceledException))]
        [TestCase(typeof(TaskCanceledException))]
        public void GetImageAsyncShouldNotCatchException(Type exceptionType)
        {
            // arrange
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            this.httpClientWrapperMock
                .GetAsync(Arg.Any<Uri>(), Arg.Any<CancellationToken>())
                .ThrowsAsync((Exception)Activator.CreateInstance(exceptionType));

            // act
            async Task Action()
            {
                await this.imageService.GetImageAsync(Uri, cancellationToken);
            }

            // assert
            Assert.ThrowsAsync(exceptionType, Action);
        }

        private HttpResponseMessage MockHttpClientGetAsyncSuccessResponse(Stream stream = null)
        {
            HttpResponseMessage httpResponse = new()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StreamContent(stream ?? new MemoryStream()),
            };

            this.httpClientWrapperMock.GetAsync(Arg.Any<Uri>(), Arg.Any<CancellationToken>())
                .Returns(httpResponse);
            return httpResponse;
        }

        private class TestBitmap : IImage
        {
            public Size Size { get; } = new Size(1, 1);

            public void Draw(DrawingContext context, Rect sourceRect, Rect destRect)
            {
            }
        }
    }
}