using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using POETradeHelper.Common.Wrappers;
using POETradeHelper.Common.Wrappers.Implementations;
using POETradeHelper.PathOfExileTradeApi.Services;
using POETradeHelper.PathOfExileTradeApi.Tests.Properties;

namespace POETradeHelper.PathOfExileTradeApi.Tests.Services
{
    /// <summary>
    /// Tests without actual HTTP request but with original data.
    /// </summary>
    [Category("Integration")]
    public class StaticItemDataServiceIntegrationTests
    {
        private StaticDataService staticDataService;
        private Mock<IHttpClientWrapper> httpClientWrapperMock;

        [SetUp]
        public void Setup()
        {
            this.httpClientWrapperMock = new Mock<IHttpClientWrapper>();
            var httpClientFactoryWrapperMock = new Mock<IHttpClientFactoryWrapper>();
            httpClientFactoryWrapperMock.Setup(x => x.CreateClient(Constants.HttpClientNames.PoeTradeApiDataClient))
                .Returns(this.httpClientWrapperMock.Object);

            this.staticDataService = new StaticDataService(httpClientFactoryWrapperMock.Object, new PoeTradeApiJsonSerializer(new JsonSerializerWrapper()));
        }

        [Test]
        public async Task OnInitShouldNotCauseAnyExceptions()
        {
            this.httpClientWrapperMock.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HttpResponseMessage
                {
                    Content = new StringContent(Resources.StaticDataJson)
                });

            await this.staticDataService.OnInitAsync();
        }
    }
}