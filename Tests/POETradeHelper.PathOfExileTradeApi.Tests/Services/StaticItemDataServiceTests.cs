using Moq;
using NUnit.Framework;
using POETradeHelper.Common.Wrappers;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Exceptions;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Properties;
using POETradeHelper.PathOfExileTradeApi.Services;
using POETradeHelper.PathOfExileTradeApi.Services.Implementations;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace POETradeHelper.PathOfExileTradeApi.Tests.Services
{
    public class StaticItemDataServiceTests
    {
        private Mock<IPoeTradeApiJsonSerializer> poeTradeApiJsonSerializerMock;
        private StaticItemDataService staticItemDataService;
        private Mock<IHttpClientWrapper> httpClientWrapperMock;

        [SetUp]
        public void Setup()
        {
            this.httpClientWrapperMock = new Mock<IHttpClientWrapper>();
            this.httpClientWrapperMock.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HttpResponseMessage
                {
                    Content = new StringContent("")
                });

            var httpClientFactoryWrapperMock = new Mock<IHttpClientFactoryWrapper>();
            httpClientFactoryWrapperMock.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(this.httpClientWrapperMock.Object);

            this.poeTradeApiJsonSerializerMock = new Mock<IPoeTradeApiJsonSerializer>();

            this.staticItemDataService = new StaticItemDataService(httpClientFactoryWrapperMock.Object, this.poeTradeApiJsonSerializerMock.Object);
        }

        [Test]
        public async Task OnInitShouldCallGetAsyncOnHttpClientWrapper()
        {
            await this.staticItemDataService.OnInitAsync();

            this.httpClientWrapperMock.Verify(x => x.GetAsync(Resources.PoeTradeApiBaseUrl + Resources.PoeTradeApiStaticDataEndpoint, It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task OnInitShouldDeserializeGetAsyncResponseAsQueryResult()
        {
            string content = "serialized content";

            this.httpClientWrapperMock.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HttpResponseMessage
                {
                    Content = new StringContent(content)
                });

            await this.staticItemDataService.OnInitAsync();

            this.poeTradeApiJsonSerializerMock.Verify(x => x.Deserialize<QueryResult<StaticData>>(content));
        }

        [Test]
        public void OnInitShouldThrowPoeTradeApiCommunicationExceptionIfStatusCodeIsNotSuccess()
        {
            this.httpClientWrapperMock.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest
                });

            AsyncTestDelegate testDelegate = async () => await this.staticItemDataService.OnInitAsync();

            var exception = Assert.CatchAsync<PoeTradeApiCommunicationException>(testDelegate);
            Assert.That(exception.Message, Contains.Substring(Resources.PoeTradeApiBaseUrl + Resources.PoeTradeApiStaticDataEndpoint));
        }

        [Test]
        public async Task GetIdShouldReturnCorrectIdForItem()
        {
            const string expected = "wis";
            var item = new CurrencyItem
            {
                Name = "Scroll of Wisdom"
            };

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<QueryResult<StaticData>>(It.IsAny<string>()))
                .Returns(new QueryResult<StaticData>
                {
                    Result = new List<StaticData>
                    {
                        new StaticData
                        {
                            Entries = new List<StaticDataEntry>
                            {
                                new StaticDataEntry { Id = "alt", Text = "Orb of Alteration" },
                                new StaticDataEntry { Id = expected, Text = item.Name }
                            }
                        }
                    }
                });

            await this.staticItemDataService.OnInitAsync();

            string result = this.staticItemDataService.GetId(item);

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}