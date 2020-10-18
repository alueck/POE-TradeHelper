using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using POETradeHelper.Common.Wrappers;
using POETradeHelper.PathOfExileTradeApi.Exceptions;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Properties;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.PathOfExileTradeApi.Tests.Services
{
    public class StaticDataServiceTests
    {
        private Mock<IPoeTradeApiJsonSerializer> poeTradeApiJsonSerializerMock;
        private StaticDataService staticDataService;
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
            httpClientFactoryWrapperMock.Setup(x => x.CreateClient(Constants.HttpClientNames.PoeTradeApiDataClient))
                .Returns(this.httpClientWrapperMock.Object);

            this.poeTradeApiJsonSerializerMock = new Mock<IPoeTradeApiJsonSerializer>();

            this.staticDataService = new StaticDataService(httpClientFactoryWrapperMock.Object, this.poeTradeApiJsonSerializerMock.Object);
        }

        [Test]
        public async Task OnInitShouldCallGetAsyncOnHttpClientWrapper()
        {
            await this.staticDataService.OnInitAsync();

            this.httpClientWrapperMock.Verify(x => x.GetAsync(Resources.PoeTradeApiStaticDataEndpoint, It.IsAny<CancellationToken>()));
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

            await this.staticDataService.OnInitAsync();

            this.poeTradeApiJsonSerializerMock.Verify(x => x.Deserialize<QueryResult<Data<StaticData>>>(content));
        }

        [Test]
        public void OnInitShouldThrowPoeTradeApiCommunicationExceptionIfStatusCodeIsNotSuccess()
        {
            this.httpClientWrapperMock.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest
                });

            AsyncTestDelegate testDelegate = async () => await this.staticDataService.OnInitAsync();

            var exception = Assert.CatchAsync<PoeTradeApiCommunicationException>(testDelegate);
            Assert.That(exception.Message, Contains.Substring(Resources.PoeTradeApiStaticDataEndpoint));
        }

        [Test]
        public async Task GetIdShouldReturnCorrectIdForItemName()
        {
            const string expected = "wis";
            const string itemName = "Scroll of Wisdom";

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<QueryResult<Data<StaticData>>>(It.IsAny<string>()))
                .Returns(new QueryResult<Data<StaticData>>
                {
                    Result = new List<Data<StaticData>>
                    {
                        new Data<StaticData>
                        {
                            Id = "Currency",
                            Entries = new List<StaticData>
                            {
                                new StaticData { Id = "alt", Text = "Orb of Alteration" },
                                new StaticData { Id = expected, Text = itemName }
                            }
                        }
                    }
                });

            await this.staticDataService.OnInitAsync();

            string result = this.staticDataService.GetId(itemName);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void GetIdShouldReturnNullIfItemIdIsNotFound()
        {
            string result = this.staticDataService.GetId("random name");

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetTextShouldReturnCorrectTextForId()
        {
            const string expected = "Scroll of Wisdom";
            const string id = "wis";

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<QueryResult<Data<StaticData>>>(It.IsAny<string>()))
                .Returns(new QueryResult<Data<StaticData>>
                {
                    Result = new List<Data<StaticData>>
                    {
                        new Data<StaticData>
                        {
                            Id = "Currency",
                            Entries = new List<StaticData>
                            {
                                new StaticData { Id = "alt", Text = "Orb of Alteration" },
                                new StaticData { Id = id, Text = expected }
                            }
                        }
                    }
                });

            await this.staticDataService.OnInitAsync();

            string result = this.staticDataService.GetText(id);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public async Task GetTextShouldThrowKeyNotFoundExceptionIfEntryIsNotFound()
        {
            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<QueryResult<Data<StaticData>>>(It.IsAny<string>()))
                .Returns(new QueryResult<Data<StaticData>>
                {
                    Result = new List<Data<StaticData>>()
                });

            await this.staticDataService.OnInitAsync();

            TestDelegate testDelegate = () => this.staticDataService.GetText("abc");

            Assert.Throws<KeyNotFoundException>(testDelegate);
        }

        [Test]
        public async Task GetImageurlShouldReturnCorrectUrlForId()
        {
            const string entryUrl = "image/Art/2DItems/Currency/CurrencyRerollRare.png";
            string expected = Resources.PoeCdnUrl + entryUrl;
            const string id = "wis";

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<QueryResult<Data<StaticData>>>(It.IsAny<string>()))
                .Returns(new QueryResult<Data<StaticData>>
                {
                    Result = new List<Data<StaticData>>
                    {
                        new Data<StaticData>
                        {
                            Id = "Currency",
                            Entries = new List<StaticData>
                            {
                                new StaticData { Id = "alt",  Image = "abc" },
                                new StaticData { Id = id, Image = entryUrl }
                            }
                        }
                    }
                });

            await this.staticDataService.OnInitAsync();

            Uri result = this.staticDataService.GetImageUrl(id);

            Assert.That(result.ToString(), Is.EqualTo(expected));
        }

        [Test]
        public async Task GetImageUrlShouldThrowKeyNotFoundExceptionIfEntryIsNotFound()
        {
            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<QueryResult<Data<StaticData>>>(It.IsAny<string>()))
                .Returns(new QueryResult<Data<StaticData>>
                {
                    Result = new List<Data<StaticData>>()
                });

            await this.staticDataService.OnInitAsync();

            TestDelegate testDelegate = () => this.staticDataService.GetImageUrl("abc");

            Assert.Throws<KeyNotFoundException>(testDelegate);
        }
    }
}