using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using POETradeHelper.Common.Wrappers;
using POETradeHelper.ItemSearch;
using POETradeHelper.ItemSearch.Contract;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Exceptions;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Properties;
using POETradeHelper.PathOfExileTradeApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace POETradeHelper.PathOfExileTradeApiTests
{
    public class PoeTradeApiClientTests
    {
        private Mock<IHttpClientWrapper> httpClientWrapperMock;
        private Mock<IHttpClientFactoryWrapper> httpClientFactoryWrapperMock;
        private Mock<IPoeTradeApiJsonSerializer> poeTradeApiJsonSerializerMock;
        private Mock<IItemSearchQueryRequestMapperAggregator> itemToQueryRequestMapperAggregatorMock;
        private Mock<IOptions<ItemSearchOptions>> itemSearchOptionsMock;
        private PoeTradeApiClient poeTradeApiClient;

        [SetUp]
        public void Setup()
        {
            this.httpClientWrapperMock = new Mock<IHttpClientWrapper>();
            this.httpClientWrapperMock.Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new HttpResponseMessage
                {
                    Content = new StringContent("")
                });
            this.httpClientWrapperMock.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .ReturnsAsync(new HttpResponseMessage
                {
                    Content = new StringContent("")
                });

            this.httpClientFactoryWrapperMock = new Mock<IHttpClientFactoryWrapper>();
            this.httpClientFactoryWrapperMock.Setup(x => x.CreateClient(It.IsAny<string>()))
                .Returns(this.httpClientWrapperMock.Object);

            this.poeTradeApiJsonSerializerMock = new Mock<IPoeTradeApiJsonSerializer>();
            this.poeTradeApiJsonSerializerMock.Setup(x => x.Serialize(It.IsAny<object>()))
                .Returns("");

            this.itemToQueryRequestMapperAggregatorMock = new Mock<IItemSearchQueryRequestMapperAggregator>();

            this.itemSearchOptionsMock = new Mock<IOptions<ItemSearchOptions>>();
            this.itemSearchOptionsMock.Setup(x => x.Value)
                .Returns(new ItemSearchOptions { League = new League() });

            this.poeTradeApiClient = new PoeTradeApiClient(this.httpClientFactoryWrapperMock.Object, this.poeTradeApiJsonSerializerMock.Object, this.itemToQueryRequestMapperAggregatorMock.Object, this.itemSearchOptionsMock.Object);
        }

        [Test]
        public async Task GetLeaguesAsyncShouldCallGetAsyncWithCorrectUriOnHttpClient()
        {
            await this.poeTradeApiClient.GetLeaguesAsync();

            this.httpClientWrapperMock.Verify(x => x.GetAsync(Resources.PoeTradeApiBaseUrl + Resources.PoeTradeApiLeaguesEndpoint));
        }

        [Test]
        public async Task GetLeaguesAsyncShouldCallDeserializeOnJsonSerializerWithHttpContent()
        {
            string json = "{ \"result\": [ ] }";

            this.httpClientWrapperMock.Setup(x => x.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new HttpResponseMessage
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                });

            await this.poeTradeApiClient.GetLeaguesAsync();

            this.poeTradeApiJsonSerializerMock.Verify(x => x.Deserialize<QueryResult<League>>(json));
        }

        [Test]
        public async Task GetLeaguesAsyncShouldReturnLeagues()
        {
            QueryResult<League> expected = new QueryResult<League>
            {
                Result = new List<League>
                {
                    new League { Id = "Metamorph", Text = "Metamorph" },
                    new League { Id = "Hardcore Metamorph", Text = "Hardcore Metamorph" }
                }
            };

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<QueryResult<League>>(It.IsAny<string>()))
                .Returns(expected);

            IList<League> result = await this.poeTradeApiClient.GetLeaguesAsync();

            Assert.NotNull(result);
            Assert.That(result, Is.EqualTo(expected.Result));
        }

        [Test]
        public void GetLeaguesShouldThrowExceptionIfHttpResponseStatusCodeDoesNotIndicateSuccess()
        {
            AsyncTestDelegate asyncTestDelegate = async () => await this.poeTradeApiClient.GetLeaguesAsync();

            this.AssertThrowsPoeTradeApiCommunicationExceptionIfHttpResponseDoesNotReturnSuccessStatusCode(asyncTestDelegate, Resources.PoeTradeApiLeaguesEndpoint);
        }

        [Test]
        public void GetLeaguesShouldThrowPoeTradeApiCommunicationExceptionIfAnyExceptionOccurs()
        {
            Exception expectedInnerException = new Exception();

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<QueryResult<League>>(It.IsAny<string>()))
                .Throws(expectedInnerException);

            AsyncTestDelegate asyncTestDelegate = async () => await this.poeTradeApiClient.GetLeaguesAsync();

            PoeTradeApiCommunicationException exception = Assert.CatchAsync<PoeTradeApiCommunicationException>(asyncTestDelegate);
            Assert.That(exception.InnerException, Is.EqualTo(expectedInnerException));
        }

        [Test]
        public async Task GetListingsAsyncShouldCallMapOnItemToQueryRequestMapper()
        {
            var item = new CurrencyItem();

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<SearchQueryResult>(It.IsAny<string>()))
                .Returns(new SearchQueryResult());

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<ItemListingsQueryResult>(It.IsAny<string>()))
                .Returns(new ItemListingsQueryResult());

            await this.poeTradeApiClient.GetListingsAsync(item);

            this.itemToQueryRequestMapperAggregatorMock.Verify(x => x.MapToQueryRequest(item));
        }

        [Test]
        public async Task GetListingssAsyncShouldSerializeMappedQueryRequest()
        {
            var item = new CurrencyItem();
            var queryRequest = new SearchQueryRequest();

            this.itemToQueryRequestMapperAggregatorMock.Setup(x => x.MapToQueryRequest(It.IsAny<Item>()))
                .Returns(queryRequest);

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<SearchQueryResult>(It.IsAny<string>()))
                .Returns(new SearchQueryResult());

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<ItemListingsQueryResult>(It.IsAny<string>()))
                .Returns(new ItemListingsQueryResult());

            await this.poeTradeApiClient.GetListingsAsync(item);

            this.poeTradeApiJsonSerializerMock.Verify(x => x.Serialize(queryRequest));
        }

        [Test]
        public async Task GetListingssAsyncShouldPostToCorrectSearchEndpoint()
        {
            var item = new CurrencyItem();

            string leagueId = "Metamorph";
            this.MockItemSearchOptions(leagueId);

            string expectedUri = Resources.PoeTradeApiBaseUrl + Resources.PoeTradeApiSearchEndpoint + "/" + leagueId;

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<SearchQueryResult>(It.IsAny<string>()))
                .Returns(new SearchQueryResult());

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<ItemListingsQueryResult>(It.IsAny<string>()))
                .Returns(new ItemListingsQueryResult());

            await this.poeTradeApiClient.GetListingsAsync(item);

            this.httpClientWrapperMock.Verify(x => x.PostAsync(expectedUri, It.IsAny<HttpContent>()));
        }

        [Test]
        public async Task GetListingssAsyncShouldPostMappedQueryRequest()
        {
            var item = new CurrencyItem();
            string expected = "serialized query request";
            StringContent expectedStringContent = new StringContent(expected, Encoding.UTF8, "application/json");

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Serialize(It.IsAny<Item>()))
                .Returns(expected);

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<SearchQueryResult>(It.IsAny<string>()))
                .Returns(new SearchQueryResult());

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<ItemListingsQueryResult>(It.IsAny<string>()))
                .Returns(new ItemListingsQueryResult());

            await this.poeTradeApiClient.GetListingsAsync(item);

            this.httpClientWrapperMock.Verify(x => x.PostAsync(It.IsAny<string>(), It.Is<StringContent>(s => s.ReadAsStringAsync().GetAwaiter().GetResult() == expected)));
        }

        [Test]
        public async Task GetListingsAsyncShouldDeserializeSearchResponseAsSearchQueryResult()
        {
            var item = new CurrencyItem();
            string expected = "serialized search query response";
            HttpResponseMessage httpResponse = new HttpResponseMessage
            {
                Content = new StringContent(expected)
            };

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<SearchQueryResult>(It.IsAny<string>()))
                .Returns(new SearchQueryResult());
            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<ItemListingsQueryResult>(It.IsAny<string>()))
                .Returns(new ItemListingsQueryResult());

            this.httpClientWrapperMock.Setup(x => x.PostAsync(It.Is<string>(s => s.StartsWith(Resources.PoeTradeApiBaseUrl + Resources.PoeTradeApiSearchEndpoint)), It.IsAny<HttpContent>()))
                .ReturnsAsync(httpResponse);

            await this.poeTradeApiClient.GetListingsAsync(item);

            this.poeTradeApiJsonSerializerMock.Verify(x => x.Deserialize<SearchQueryResult>(expected));
        }

        [Test]
        public async Task GetListingsAsyncShouldFetchItemsWithCorrectUri()
        {
            var item = new CurrencyItem();
            var searchQueryResult = new SearchQueryResult
            {
                Result = Enumerable.Range(0, 20).Select(i => i.ToString()).ToList(),
                Total = 20
            };

            string expectedUri = $"{Resources.PoeTradeApiBaseUrl}{Resources.PoeTradeApiFetchEndpoint}/{string.Join(',', searchQueryResult.Result.Take(10))}";

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<SearchQueryResult>(It.IsAny<string>()))
                .Returns(searchQueryResult);

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<ItemListingsQueryResult>(It.IsAny<string>()))
                .Returns(new ItemListingsQueryResult());

            await this.poeTradeApiClient.GetListingsAsync(item);

            this.httpClientWrapperMock.Verify(x => x.GetAsync(expectedUri));
        }

        [Test]
        public async Task GetListingsAsyncShouldNotFetchItemsIfSearchQueryResultIsEmpty()
        {
            var item = new CurrencyItem();
            var searchQueryResult = new SearchQueryResult();

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<SearchQueryResult>(It.IsAny<string>()))
                .Returns(searchQueryResult);

            await this.poeTradeApiClient.GetListingsAsync(item);

            this.httpClientWrapperMock.Verify(x => x.GetAsync(It.Is<string>(s => s.Contains(Resources.PoeTradeApiFetchEndpoint))), Times.Never);
        }

        [Test]
        public async Task GetListingsAsyncShouldDeserializeFetchResponseAsItemListingQueryResult()
        {
            var item = new CurrencyItem();
            string expected = "serialized item listings";

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<SearchQueryResult>(It.IsAny<string>()))
                .Returns(new SearchQueryResult
                {
                    Result = new List<string> { "123" },
                    Total = 1
                });

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<ItemListingsQueryResult>(It.IsAny<string>()))
                .Returns(new ItemListingsQueryResult());

            this.httpClientWrapperMock.Setup(x => x.GetAsync(It.Is<string>(s => s.StartsWith(Resources.PoeTradeApiBaseUrl + Resources.PoeTradeApiFetchEndpoint))))
                .ReturnsAsync(new HttpResponseMessage
                {
                    Content = new StringContent(expected)
                });

            await this.poeTradeApiClient.GetListingsAsync(item);

            this.poeTradeApiJsonSerializerMock.Verify(x => x.Deserialize<QueryResult<ListingResult>>(expected));
        }

        [Test]
        public async Task GetListingsAsyncShouldReturnFetchResult()
        {
            var item = new CurrencyItem();
            ItemListingsQueryResult expected = new ItemListingsQueryResult
            {
                Result = new List<ListingResult>
                {
                    new ListingResult { Id = "Test"}
                }
            };

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<SearchQueryResult>(It.IsAny<string>()))
                .Returns(new SearchQueryResult
                {
                    Result = new List<string> { "123" },
                    Total = 1
                });

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<ItemListingsQueryResult>(It.IsAny<string>()))
                .Returns(expected);

            ItemListingsQueryResult result = await this.poeTradeApiClient.GetListingsAsync(item);

            Assert.That(result, Is.SameAs(expected));
        }

        [Test]
        public async Task GetListingsAsyncShouldReturnResultWithUri()
        {
            var item = new CurrencyItem();

            string expectedId = "abdef";

            string leagueId = "Metamorph";
            this.MockItemSearchOptions(leagueId);

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<SearchQueryResult>(It.IsAny<string>()))
                .Returns(new SearchQueryResult
                {
                    Id = expectedId
                });

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<ItemListingsQueryResult>(It.IsAny<string>()))
             .Returns(new ItemListingsQueryResult());

            Uri expectedUri = new Uri($"{Resources.PoeTradeApiBaseUrl}{Resources.PoeTradeApiSearchEndpoint}/{leagueId}/{expectedId}");

            ItemListingsQueryResult result = await this.poeTradeApiClient.GetListingsAsync(item);

            Assert.That(result.Uri, Is.EqualTo(expectedUri));
        }

        [Test]
        public async Task GetListingsAsyncShouldReturnResultWithTotalCount()
        {
            var item = new CurrencyItem();

            int expectedCount = 100;

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<SearchQueryResult>(It.IsAny<string>()))
                .Returns(new SearchQueryResult
                {
                    Total = expectedCount
                });

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<ItemListingsQueryResult>(It.IsAny<string>()))
                .Returns(new ItemListingsQueryResult());

            ItemListingsQueryResult result = await this.poeTradeApiClient.GetListingsAsync(item);

            Assert.That(result.TotalCount, Is.EqualTo(expectedCount));
        }

        [Test]
        public async Task GetListingsAsyncShouldReturnResultWithItem()
        {
            var item = new CurrencyItem();

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<SearchQueryResult>(It.IsAny<string>()))
                .Returns(new SearchQueryResult());

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<ItemListingsQueryResult>(It.IsAny<string>()))
                .Returns(new ItemListingsQueryResult());

            ItemListingsQueryResult result = await this.poeTradeApiClient.GetListingsAsync(item);

            Assert.That(result.Item, Is.SameAs(item));
        }

        [Test]
        public void GetListingsAsyncShouldThrowExceptionIfFetchResponseStatusCodeDoesNotIndicateSuccess()
        {
            var item = new CurrencyItem();

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<SearchQueryResult>(It.IsAny<string>()))
                .Returns(new SearchQueryResult
                {
                    Result = new List<string> { "123" },
                    Total = 1
                });

            AsyncTestDelegate asyncTestDelegate = async () => await this.poeTradeApiClient.GetListingsAsync(item);

            this.AssertThrowsPoeTradeApiCommunicationExceptionIfHttpResponseDoesNotReturnSuccessStatusCode(asyncTestDelegate, Resources.PoeTradeApiFetchEndpoint);
        }

        [Test]
        public void GetListingsAsyncShouldThrowExceptionIfSearchRequestStatusCodeDoesNotIndicateSuccess()
        {
            var item = new CurrencyItem();

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<SearchQueryResult>(It.IsAny<string>()))
                .Returns(new SearchQueryResult());

            AsyncTestDelegate asyncTestDelegate = async () => await this.poeTradeApiClient.GetListingsAsync(item);

            this.AssertThrowsPoeTradeApiCommunicationExceptionIfHttpResponseDoesNotReturnSuccessStatusCode(asyncTestDelegate, Resources.PoeTradeApiSearchEndpoint, "posted json content");
        }

        [Test]
        public void GetListingsAsyncShouldThrowPoeTradeApiCommunicationExceptionIfAnyExceptionOccurs()
        {
            Exception expectedInnerException = new Exception();

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<SearchQueryResult>(It.IsAny<string>()))
                .Throws(expectedInnerException);

            AsyncTestDelegate asyncTestDelegate = async () => await this.poeTradeApiClient.GetListingsAsync(null);

            PoeTradeApiCommunicationException exception = Assert.CatchAsync<PoeTradeApiCommunicationException>(asyncTestDelegate);
            Assert.That(exception.InnerException, Is.EqualTo(expectedInnerException));
        }

        private void AssertThrowsPoeTradeApiCommunicationExceptionIfHttpResponseDoesNotReturnSuccessStatusCode(AsyncTestDelegate asyncTestDelegate, string endpoint, string jsonContent = "")
        {
            HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest;

            HttpResponseMessage httpResponse = new HttpResponseMessage
            {
                Content = new StringContent(""),
                StatusCode = httpStatusCode
            };

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Serialize(It.IsAny<SearchQueryRequest>()))
                .Returns(jsonContent);
            this.httpClientWrapperMock.Setup(x => x.GetAsync(It.Is<string>(s => s.Contains(endpoint))))
                .ReturnsAsync(httpResponse);
            this.httpClientWrapperMock.Setup(x => x.PostAsync(It.Is<string>(s => s.Contains(endpoint)), It.IsAny<HttpContent>()))
                .ReturnsAsync(httpResponse);

            PoeTradeApiCommunicationException exception = Assert.CatchAsync<PoeTradeApiCommunicationException>(asyncTestDelegate);
            Assert.That(exception.Message, Contains.Substring(httpStatusCode.ToString()));
            Assert.That(exception.Message, Contains.Substring(endpoint));

            if (!string.IsNullOrEmpty(jsonContent))
            {
                Assert.That(exception.Message, Contains.Substring(jsonContent));
            }
        }

        private void MockItemSearchOptions(string league)
        {
            ItemSearchOptions itemSearchOptions = new ItemSearchOptions
            {
                League = new League
                {
                    Id = league,
                    Text = league
                }
            };

            this.itemSearchOptionsMock.Setup(x => x.Value)
                .Returns(itemSearchOptions);
        }
    }
}