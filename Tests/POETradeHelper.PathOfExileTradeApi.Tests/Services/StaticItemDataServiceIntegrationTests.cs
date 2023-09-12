using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using NSubstitute;

using NUnit.Framework;

using POETradeHelper.Common.Wrappers;
using POETradeHelper.Common.Wrappers.Implementations;
using POETradeHelper.PathOfExileTradeApi.Constants;
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
        private IHttpClientWrapper httpClientWrapperMock;

        [SetUp]
        public void Setup()
        {
            this.httpClientWrapperMock = Substitute.For<IHttpClientWrapper>();
            IHttpClientFactoryWrapper httpClientFactoryWrapperMock = Substitute.For<IHttpClientFactoryWrapper>();
            httpClientFactoryWrapperMock.CreateClient(HttpClientNames.PoeTradeApiDataClient)
                .Returns(this.httpClientWrapperMock);

            this.staticDataService = new StaticDataService(
                httpClientFactoryWrapperMock,
                new PoeTradeApiJsonSerializer(new JsonSerializerWrapper()));
        }

        [Test]
        public async Task OnInitShouldNotCauseAnyExceptions()
        {
            this.httpClientWrapperMock.GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(new HttpResponseMessage
                {
                    Content = new StringContent(Resources.StaticDataJson),
                });

            await this.staticDataService.OnInitAsync();
        }
    }
}