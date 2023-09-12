using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using NSubstitute;

using NUnit.Framework;

using POETradeHelper.Common.Wrappers;
using POETradeHelper.PathOfExileTradeApi.Constants;
using POETradeHelper.PathOfExileTradeApi.Exceptions;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Properties;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.PathOfExileTradeApi.Tests.Services
{
    public class StaticDataServiceTests
    {
        private IPoeTradeApiJsonSerializer poeTradeApiJsonSerializerMock;
        private StaticDataService staticDataService;
        private IHttpClientWrapper httpClientWrapperMock;

        [SetUp]
        public void Setup()
        {
            this.httpClientWrapperMock = Substitute.For<IHttpClientWrapper>();
            this.httpClientWrapperMock.GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(new HttpResponseMessage
                {
                    Content = new StringContent(string.Empty),
                });

            IHttpClientFactoryWrapper httpClientFactoryWrapperMock = Substitute.For<IHttpClientFactoryWrapper>();
            httpClientFactoryWrapperMock.CreateClient(HttpClientNames.PoeTradeApiDataClient)
                .Returns(this.httpClientWrapperMock);

            this.poeTradeApiJsonSerializerMock = Substitute.For<IPoeTradeApiJsonSerializer>();

            this.staticDataService =
                new StaticDataService(httpClientFactoryWrapperMock, this.poeTradeApiJsonSerializerMock);
        }

        [Test]
        public async Task OnInitShouldCallGetAsyncOnHttpClientWrapper()
        {
            await this.staticDataService.OnInitAsync();

            await this.httpClientWrapperMock
                .Received()
                .GetAsync(Resources.PoeTradeApiStaticDataEndpoint, Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task OnInitShouldDeserializeGetAsyncResponseAsQueryResult()
        {
            const string content = "serialized content";

            this.httpClientWrapperMock.GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(new HttpResponseMessage
                {
                    Content = new StringContent(content),
                });

            await this.staticDataService.OnInitAsync();

            this.poeTradeApiJsonSerializerMock
                .Received()
                .Deserialize<QueryResult<Data<StaticData>>>(content);
        }

        [Test]
        public void OnInitShouldThrowPoeTradeApiCommunicationExceptionIfStatusCodeIsNotSuccess()
        {
            this.httpClientWrapperMock.GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                });

            AsyncTestDelegate testDelegate = async () => await this.staticDataService.OnInitAsync();

            PoeTradeApiCommunicationException exception = Assert.CatchAsync<PoeTradeApiCommunicationException>(testDelegate);
            Assert.That(exception.Message, Contains.Substring(Resources.PoeTradeApiStaticDataEndpoint));
        }

        [Test]
        public async Task GetIdShouldReturnCorrectIdForItemName()
        {
            const string expected = "wis";
            const string itemName = "Scroll of Wisdom";

            this.poeTradeApiJsonSerializerMock.Deserialize<QueryResult<Data<StaticData>>>(Arg.Any<string>())
                .Returns(new QueryResult<Data<StaticData>>
                {
                    Result = new List<Data<StaticData>>
                    {
                        new()
                        {
                            Id = "Currency",
                            Entries = new List<StaticData>
                            {
                                new() { Id = "alt", Text = "Orb of Alteration" },
                                new() { Id = expected, Text = itemName },
                            },
                        },
                    },
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

            this.poeTradeApiJsonSerializerMock.Deserialize<QueryResult<Data<StaticData>>>(Arg.Any<string>())
                .Returns(new QueryResult<Data<StaticData>>
                {
                    Result = new List<Data<StaticData>>
                    {
                        new()
                        {
                            Id = "Currency",
                            Entries = new List<StaticData>
                            {
                                new() { Id = "alt", Text = "Orb of Alteration" },
                                new() { Id = id, Text = expected },
                            },
                        },
                    },
                });

            await this.staticDataService.OnInitAsync();

            string result = this.staticDataService.GetText(id);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public async Task GetTextShouldThrowKeyNotFoundExceptionIfEntryIsNotFound()
        {
            this.poeTradeApiJsonSerializerMock.Deserialize<QueryResult<Data<StaticData>>>(Arg.Any<string>())
                .Returns(new QueryResult<Data<StaticData>>
                {
                    Result = new List<Data<StaticData>>(),
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

            this.poeTradeApiJsonSerializerMock.Deserialize<QueryResult<Data<StaticData>>>(Arg.Any<string>())
                .Returns(new QueryResult<Data<StaticData>>
                {
                    Result = new List<Data<StaticData>>
                    {
                        new()
                        {
                            Id = "Currency",
                            Entries = new List<StaticData>
                            {
                                new() { Id = "alt", Image = "abc" },
                                new() { Id = id, Image = entryUrl },
                            },
                        },
                    },
                });

            await this.staticDataService.OnInitAsync();

            Uri result = this.staticDataService.GetImageUrl(id);

            Assert.That(result.ToString(), Is.EqualTo(expected));
        }

        [Test]
        public async Task GetImageUrlShouldThrowKeyNotFoundExceptionIfEntryIsNotFound()
        {
            this.poeTradeApiJsonSerializerMock.Deserialize<QueryResult<Data<StaticData>>>(Arg.Any<string>())
                .Returns(new QueryResult<Data<StaticData>>
                {
                    Result = new List<Data<StaticData>>(),
                });

            await this.staticDataService.OnInitAsync();

            TestDelegate testDelegate = () => this.staticDataService.GetImageUrl("abc");

            Assert.Throws<KeyNotFoundException>(testDelegate);
        }
    }
}