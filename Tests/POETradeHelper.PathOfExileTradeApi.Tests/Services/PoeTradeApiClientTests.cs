﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using POETradeHelper.Common.Wrappers;
using POETradeHelper.ItemSearch.Contract;
using POETradeHelper.ItemSearch.Contract.Configuration;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Exceptions;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Properties;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.PathOfExileTradeApi.Tests.Services
{
    public class PoeTradeApiClientTests
    {
        private Mock<IHttpClientWrapper> httpClientWrapperMock;
        private Mock<IHttpClientFactoryWrapper> httpClientFactoryWrapperMock;
        private Mock<IPoeTradeApiJsonSerializer> poeTradeApiJsonSerializerMock;
        private Mock<IItemSearchQueryRequestMapperAggregator> itemToQueryRequestMapperAggregatorMock;
        private Mock<IOptionsMonitor<ItemSearchOptions>> itemSearchOptionsMock;
        private PoeTradeApiClient poeTradeApiClient;

        [SetUp]
        public void Setup()
        {
            this.httpClientWrapperMock = new Mock<IHttpClientWrapper>();
            this.httpClientWrapperMock.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HttpResponseMessage
                {
                    Content = new StringContent("")
                });
            this.httpClientWrapperMock.Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HttpResponseMessage
                {
                    Content = new StringContent("")
                });

            this.httpClientFactoryWrapperMock = new Mock<IHttpClientFactoryWrapper>();
            this.httpClientFactoryWrapperMock.Setup(x => x.CreateClient(Constants.HttpClientNames.PoeTradeApiItemSearchClient))
                .Returns(this.httpClientWrapperMock.Object);

            this.poeTradeApiJsonSerializerMock = new Mock<IPoeTradeApiJsonSerializer>();
            this.poeTradeApiJsonSerializerMock.Setup(x => x.Serialize(It.IsAny<object>()))
                .Returns("");

            this.itemToQueryRequestMapperAggregatorMock = new Mock<IItemSearchQueryRequestMapperAggregator>();
            this.itemToQueryRequestMapperAggregatorMock.Setup(x => x.MapToQueryRequest(It.IsAny<Item>()))
                .Returns(new SearchQueryRequest());

            this.itemSearchOptionsMock = new Mock<IOptionsMonitor<ItemSearchOptions>>();
            this.itemSearchOptionsMock.Setup(x => x.CurrentValue)
                .Returns(new ItemSearchOptions { League = new League() });

            this.poeTradeApiClient = new PoeTradeApiClient(this.httpClientFactoryWrapperMock.Object, this.poeTradeApiJsonSerializerMock.Object, this.itemToQueryRequestMapperAggregatorMock.Object, this.itemSearchOptionsMock.Object);
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

        [TestCaseSource(nameof(QueryRequests))]
        public async Task GetListingssAsyncShouldPostToCorrectSearchEndpoint(IQueryRequest queryRequest)
        {
            var item = new CurrencyItem();

            string leagueId = "Metamorph";
            this.MockItemSearchOptions(leagueId);

            this.itemToQueryRequestMapperAggregatorMock.Setup(x => x.MapToQueryRequest(It.IsAny<Item>()))
                .Returns(queryRequest);

            string expectedUri = queryRequest.Endpoint + "/" + leagueId;

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<SearchQueryResult>(It.IsAny<string>()))
                .Returns(new SearchQueryResult());

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<ItemListingsQueryResult>(It.IsAny<string>()))
                .Returns(new ItemListingsQueryResult());

            await this.poeTradeApiClient.GetListingsAsync(item);

            this.httpClientWrapperMock.Verify(x => x.PostAsync(expectedUri, It.IsAny<HttpContent>(), It.IsAny<CancellationToken>()));
        }

        private static IEnumerable<IQueryRequest> QueryRequests
        {
            get
            {
                yield return new SearchQueryRequest();
                yield return new ExchangeQueryRequest();
            }
        }

        [Test]
        public async Task GetListingssAsyncShouldPostMappedQueryRequest()
        {
            var item = new CurrencyItem();
            AsyncTestDelegate testDelegate = async () => await this.poeTradeApiClient.GetListingsAsync(item);

            await this.GetListingsAsyncShouldPostQueryRequest(testDelegate);
        }

        [Test]
        public async Task GetListingssAsyncShouldPassCancellationTokenToPostSearch()
        {
            var item = new CurrencyItem();
            var cancellationToken = new CancellationToken();

            AsyncTestDelegate testDelegate = async () => await this.poeTradeApiClient.GetListingsAsync(item, cancellationToken);

            await this.GetListingssAsyncShouldPassCancellationTokenToPostSearch(testDelegate, cancellationToken);
        }

        [Test]
        public async Task GetListingsAsyncShouldDeserializeSearchResponseAsSearchQueryResult()
        {
            var item = new CurrencyItem();

            AsyncTestDelegate testDelegate = async () => await this.poeTradeApiClient.GetListingsAsync(item);

            await this.GetListingsAsyncShouldDeserializeSearchResponseAsSearchQueryResult(testDelegate);
        }

        [Test]
        public async Task GetListingsAsyncShouldSetRequestOnSearchQueryResult()
        {
            var item = new CurrencyItem();
            var searchQueryRequest = new SearchQueryRequest();

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<SearchQueryResult>(It.IsAny<string>()))
                .Returns(new SearchQueryResult());
            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<ItemListingsQueryResult>(It.IsAny<string>()))
                .Returns(new ItemListingsQueryResult());
            this.itemToQueryRequestMapperAggregatorMock.Setup(x => x.MapToQueryRequest(It.IsAny<Item>()))
                .Returns(searchQueryRequest);

            ItemListingsQueryResult result = await this.poeTradeApiClient.GetListingsAsync(item);

            Assert.That(result.SearchQueryRequest, Is.EqualTo(searchQueryRequest));
        }

        [Test]
        public async Task GetListingsAsyncShouldFetchItemsWithCorrectUri()
        {
            var item = new CurrencyItem();

            AsyncTestDelegate testDelegate = async () => await this.poeTradeApiClient.GetListingsAsync(item);

            await this.GetListingsAsyncShouldFetchItemsWithCorrectUri(testDelegate);
        }

        [Test]
        public async Task GetListingsAsyncShouldNotFetchItemsIfSearchQueryResultIsEmpty()
        {
            var item = new CurrencyItem();

            AsyncTestDelegate testDelegate = async () => await this.poeTradeApiClient.GetListingsAsync(item);

            await this.GetListingsAsyncShouldNotFetchItemsIfSearchQueryResultIsEmpty(testDelegate);
        }

        [Test]
        public async Task GetListingsAsyncShouldDeserializeFetchResponseAsItemListingQueryResult()
        {
            var item = new CurrencyItem();

            AsyncTestDelegate testDelegate = async () => await this.poeTradeApiClient.GetListingsAsync(item);

            await this.GetListingsAsyncShouldDeserializeFetchResponseAsItemListingQueryResult(testDelegate);
        }

        [Test]
        public async Task GetListingsAsyncShouldReturnFetchResult()
        {
            var item = new CurrencyItem();

            await this.GetListingsAsyncShouldReturnFetchResult(async () => await this.poeTradeApiClient.GetListingsAsync(item));
        }

        [Test]
        public async Task GetListingsAsyncShouldReturnResultWithUri()
        {
            var item = new CurrencyItem();

            await this.GetListingsAsyncShouldReturnResultWithUri(async () => await this.poeTradeApiClient.GetListingsAsync(item));
        }

        [Test]
        public async Task GetListingsAsyncShouldReturnResultWithTotalCount()
        {
            var item = new CurrencyItem();

            await this.GetListingsAsyncShouldReturnResultWithTotalCount(async () => await this.poeTradeApiClient.GetListingsAsync(item));
        }

        [Test]
        public void GetListingsAsyncShouldThrowExceptionIfFetchResponseStatusCodeDoesNotIndicateSuccess()
        {
            var item = new CurrencyItem();

            AsyncTestDelegate asyncTestDelegate = async () => await this.poeTradeApiClient.GetListingsAsync(item);

            this.GetListingsAsyncShouldThrowExceptionIfFetchResponseStatusCodeDoesNotIndicateSuccess(asyncTestDelegate);
        }

        [Test]
        public void GetListingsAsyncShouldThrowExceptionIfSearchRequestStatusCodeDoesNotIndicateSuccess()
        {
            var item = new CurrencyItem();

            AsyncTestDelegate asyncTestDelegate = async () => await this.poeTradeApiClient.GetListingsAsync(item);

            this.GetListingsAsyncShouldThrowExceptionIfSearchRequestStatusCodeDoesNotIndicateSuccess(asyncTestDelegate);
        }

        [Test]
        public void GetListingsAsyncShouldThrowPoeTradeApiCommunicationExceptionIfAnyExceptionOccurs()
        {
            AsyncTestDelegate asyncTestDelegate = async () => await this.poeTradeApiClient.GetListingsAsync((Item)null);

            this.GetListingsAsyncShouldThrowPoeTradeApiCommunicationExceptionIfAnyExceptionOccurs(asyncTestDelegate);
        }

        [Test]
        public async Task GetListingssAsyncWithQueryRequestShouldPostQueryRequest()
        {
            var queryRequest = new SearchQueryRequest();
            AsyncTestDelegate testDelegate = async () => await this.poeTradeApiClient.GetListingsAsync(queryRequest);

            await this.GetListingsAsyncShouldPostQueryRequest(testDelegate);
        }

        [Test]
        public async Task GetListingsAsyncWithQueryReqeustShouldPassCancellationTokenToPostSearch()
        {
            var queryRequest = new SearchQueryRequest();
            var cancellationToken = new CancellationToken();

            AsyncTestDelegate testDelegate = async () => await this.poeTradeApiClient.GetListingsAsync(queryRequest, cancellationToken);

            await this.GetListingssAsyncShouldPassCancellationTokenToPostSearch(testDelegate, cancellationToken);
        }

        [Test]
        public async Task GetListingsAsyncWithQueryRequestShouldDeserializeSearchResponseAsSearchQueryResult()
        {
            var queryRequest = new SearchQueryRequest();

            AsyncTestDelegate testDelegate = async () => await this.poeTradeApiClient.GetListingsAsync(queryRequest);

            await this.GetListingsAsyncShouldDeserializeSearchResponseAsSearchQueryResult(testDelegate);
        }

        [Test]
        public async Task GetListingsAsynchWithQueryRequestShouldSetRequestOnSearchQueryResult()
        {
            var queryRequest = new SearchQueryRequest();

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<SearchQueryResult>(It.IsAny<string>()))
                .Returns(new SearchQueryResult());
            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<ItemListingsQueryResult>(It.IsAny<string>()))
                .Returns(new ItemListingsQueryResult());

            ItemListingsQueryResult result = await this.poeTradeApiClient.GetListingsAsync(queryRequest);

            Assert.That(result.SearchQueryRequest, Is.EqualTo(queryRequest));
        }

        [Test]
        public async Task GetListingsAsyncWithQueryRequestShouldFetchItemsWithCorrectUri()
        {
            var queryRequest = new SearchQueryRequest();

            AsyncTestDelegate testDelegate = async () => await this.poeTradeApiClient.GetListingsAsync(queryRequest);

            await this.GetListingsAsyncShouldFetchItemsWithCorrectUri(testDelegate);
        }

        [Test]
        public async Task GetListingsAsyncWithQueryRequestShouldNotFetchItemsIfSearchQueryResultIsEmpty()
        {
            var queryRequest = new SearchQueryRequest();

            AsyncTestDelegate testDelegate = async () => await this.poeTradeApiClient.GetListingsAsync(queryRequest);

            await this.GetListingsAsyncShouldNotFetchItemsIfSearchQueryResultIsEmpty(testDelegate);
        }

        [Test]
        public async Task GetListingsAsyncWithQueryRequestShouldDeserializeFetchResponseAsItemListingQueryResult()
        {
            var queryRequest = new SearchQueryRequest();

            AsyncTestDelegate testDelegate = async () => await this.poeTradeApiClient.GetListingsAsync(queryRequest);

            await this.GetListingsAsyncShouldDeserializeFetchResponseAsItemListingQueryResult(testDelegate);
        }

        [Test]
        public async Task GetListingsAsyncWithQueryRequestShouldReturnFetchResult()
        {
            var queryRequest = new SearchQueryRequest();

            await this.GetListingsAsyncShouldReturnFetchResult(async () => await this.poeTradeApiClient.GetListingsAsync(queryRequest));
        }

        [Test]
        public async Task GetListingsAsyncWithQueryRequestShouldReturnResultWithUri()
        {
            var queryRequest = new SearchQueryRequest();

            await this.GetListingsAsyncShouldReturnResultWithUri(async () => await this.poeTradeApiClient.GetListingsAsync(queryRequest));
        }

        [Test]
        public async Task GetListingsAsyncWithQueryRequestShouldReturnResultWithTotalCount()
        {
            var queryRequest = new SearchQueryRequest();

            await this.GetListingsAsyncShouldReturnResultWithTotalCount(async () => await this.poeTradeApiClient.GetListingsAsync(queryRequest));
        }

        [Test]
        public void GetListingsAsyncWithQueryRequestShouldThrowExceptionIfFetchResponseStatusCodeDoesNotIndicateSuccess()
        {
            var queryRequest = new SearchQueryRequest();

            AsyncTestDelegate asyncTestDelegate = async () => await this.poeTradeApiClient.GetListingsAsync(queryRequest);

            this.GetListingsAsyncShouldThrowExceptionIfFetchResponseStatusCodeDoesNotIndicateSuccess(asyncTestDelegate);
        }

        [Test]
        public void GetListingsAsyncWithQueryRequestShouldThrowExceptionIfSearchRequestStatusCodeDoesNotIndicateSuccess()
        {
            var queryRequest = new SearchQueryRequest();

            AsyncTestDelegate asyncTestDelegate = async () => await this.poeTradeApiClient.GetListingsAsync(queryRequest);

            this.GetListingsAsyncShouldThrowExceptionIfSearchRequestStatusCodeDoesNotIndicateSuccess(asyncTestDelegate);
        }

        [Test]
        public void GetListingsAsyncWithQueryRequestShouldThrowPoeTradeApiCommunicationExceptionIfAnyExceptionOccurs()
        {
            AsyncTestDelegate asyncTestDelegate = async () => await this.poeTradeApiClient.GetListingsAsync(new SearchQueryRequest());

            this.GetListingsAsyncShouldThrowPoeTradeApiCommunicationExceptionIfAnyExceptionOccurs(asyncTestDelegate);
        }

        private async Task GetListingsAsyncShouldPostQueryRequest(AsyncTestDelegate asyncTestDelegate)
        {
            string expected = "serialized query request";
            StringContent expectedStringContent = new StringContent(expected, Encoding.UTF8, "application/json");

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Serialize(It.IsAny<IQueryRequest>()))
                .Returns(expected);

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<SearchQueryResult>(It.IsAny<string>()))
                .Returns(new SearchQueryResult());

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<ItemListingsQueryResult>(It.IsAny<string>()))
                .Returns(new ItemListingsQueryResult());

            await asyncTestDelegate();

            this.httpClientWrapperMock.Verify(x => x.PostAsync(It.IsAny<string>(), It.Is<StringContent>(s => s.ReadAsStringAsync().GetAwaiter().GetResult() == expected), It.IsAny<CancellationToken>()));
        }

        private async Task GetListingssAsyncShouldPassCancellationTokenToPostSearch(AsyncTestDelegate asyncTestDelegate, CancellationToken cancellationToken)
        {
            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<SearchQueryResult>(It.IsAny<string>()))
                .Returns(new SearchQueryResult());

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<ItemListingsQueryResult>(It.IsAny<string>()))
                .Returns(new ItemListingsQueryResult());

            await asyncTestDelegate();

            this.httpClientWrapperMock.Verify(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>(), cancellationToken));
        }

        private async Task GetListingsAsyncShouldDeserializeSearchResponseAsSearchQueryResult(AsyncTestDelegate asyncTestDelegate)
        {
            string expected = "serialized search query response";
            HttpResponseMessage httpResponse = new HttpResponseMessage
            {
                Content = new StringContent(expected)
            };

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<SearchQueryResult>(It.IsAny<string>()))
                .Returns(new SearchQueryResult());
            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<ItemListingsQueryResult>(It.IsAny<string>()))
                .Returns(new ItemListingsQueryResult());

            this.httpClientWrapperMock.Setup(x => x.PostAsync(It.Is<string>(s => s.StartsWith(Resources.PoeTradeApiSearchEndpoint)), It.IsAny<HttpContent>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(httpResponse);

            await asyncTestDelegate();

            this.poeTradeApiJsonSerializerMock.Verify(x => x.Deserialize<SearchQueryResult>(expected));
        }

        private async Task GetListingsAsyncShouldFetchItemsWithCorrectUri(AsyncTestDelegate asyncTestDelegate)
        {
            var searchQueryResult = new SearchQueryResult
            {
                Result = Enumerable.Range(0, 20).Select(i => i.ToString()).ToList(),
                Total = 20
            };

            string expectedUri = $"{Resources.PoeTradeApiFetchEndpoint}/{string.Join(',', searchQueryResult.Result.Take(10))}";

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<SearchQueryResult>(It.IsAny<string>()))
                .Returns(searchQueryResult);

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<ItemListingsQueryResult>(It.IsAny<string>()))
                .Returns(new ItemListingsQueryResult());

            await asyncTestDelegate();

            this.httpClientWrapperMock.Verify(x => x.GetAsync(expectedUri, It.IsAny<CancellationToken>()));
        }

        private async Task GetListingsAsyncShouldNotFetchItemsIfSearchQueryResultIsEmpty(AsyncTestDelegate asyncTestDelegate)
        {
            var searchQueryResult = new SearchQueryResult();

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<SearchQueryResult>(It.IsAny<string>()))
                .Returns(searchQueryResult);

            await asyncTestDelegate();

            this.httpClientWrapperMock.Verify(x => x.GetAsync(It.Is<string>(s => s.Contains(Resources.PoeTradeApiFetchEndpoint)), It.IsAny<CancellationToken>()), Times.Never);
        }

        private async Task GetListingsAsyncShouldDeserializeFetchResponseAsItemListingQueryResult(AsyncTestDelegate asyncTestDelegate)
        {
            string expected = "serialized item listings";

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<SearchQueryResult>(It.IsAny<string>()))
                .Returns(new SearchQueryResult
                {
                    Result = new List<string> { "123" },
                    Total = 1
                });

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<ItemListingsQueryResult>(It.IsAny<string>()))
                .Returns(new ItemListingsQueryResult());

            this.httpClientWrapperMock.Setup(x => x.GetAsync(It.Is<string>(s => s.StartsWith(Resources.PoeTradeApiFetchEndpoint)), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HttpResponseMessage
                {
                    Content = new StringContent(expected)
                });

            await asyncTestDelegate();

            this.poeTradeApiJsonSerializerMock.Verify(x => x.Deserialize<QueryResult<ListingResult>>(expected));
        }

        private async Task GetListingsAsyncShouldReturnFetchResult(Func<Task<ItemListingsQueryResult>> asyncTestDelegate)
        {
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

            ItemListingsQueryResult result = await asyncTestDelegate();

            Assert.That(result, Is.SameAs(expected));
        }

        private async Task GetListingsAsyncShouldReturnResultWithUri(Func<Task<ItemListingsQueryResult>> asyncTestDelegate)
        {
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

            Uri expectedUri = new Uri($"{Resources.PoeTradeBaseUrl}{Resources.PoeTradeApiSearchEndpoint}/{leagueId}/{expectedId}");

            ItemListingsQueryResult result = await asyncTestDelegate();

            Assert.That(result.Uri, Is.EqualTo(expectedUri));
        }

        private async Task GetListingsAsyncShouldReturnResultWithTotalCount(Func<Task<ItemListingsQueryResult>> asyncTestDelegate)
        {
            int expectedCount = 100;

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<SearchQueryResult>(It.IsAny<string>()))
                .Returns(new SearchQueryResult
                {
                    Total = expectedCount
                });

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<ItemListingsQueryResult>(It.IsAny<string>()))
                .Returns(new ItemListingsQueryResult());

            ItemListingsQueryResult result = await asyncTestDelegate();

            Assert.That(result.TotalCount, Is.EqualTo(expectedCount));
        }

        private void GetListingsAsyncShouldThrowExceptionIfFetchResponseStatusCodeDoesNotIndicateSuccess(AsyncTestDelegate asyncTestDelegate)
        {
            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<SearchQueryResult>(It.IsAny<string>()))
                .Returns(new SearchQueryResult
                {
                    Result = new List<string> { "123" },
                    Total = 1
                });

            this.AssertThrowsPoeTradeApiCommunicationExceptionIfHttpResponseDoesNotReturnSuccessStatusCode(asyncTestDelegate, Resources.PoeTradeApiFetchEndpoint);
        }

        private void GetListingsAsyncShouldThrowExceptionIfSearchRequestStatusCodeDoesNotIndicateSuccess(AsyncTestDelegate asyncTestDelegate)
        {
            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<SearchQueryResult>(It.IsAny<string>()))
                .Returns(new SearchQueryResult());

            this.AssertThrowsPoeTradeApiCommunicationExceptionIfHttpResponseDoesNotReturnSuccessStatusCode(asyncTestDelegate, Resources.PoeTradeApiSearchEndpoint, "posted json content");
        }

        private void GetListingsAsyncShouldThrowPoeTradeApiCommunicationExceptionIfAnyExceptionOccurs(AsyncTestDelegate asyncTestDelegate)
        {
            Exception expectedInnerException = new Exception();

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<SearchQueryResult>(It.IsAny<string>()))
                .Throws(expectedInnerException);

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
            this.httpClientWrapperMock.Setup(x => x.GetAsync(It.Is<string>(s => s.Contains(endpoint)), It.IsAny<CancellationToken>()))
                .ReturnsAsync(httpResponse);
            this.httpClientWrapperMock.Setup(x => x.PostAsync(It.Is<string>(s => s.Contains(endpoint)), It.IsAny<HttpContent>(), It.IsAny<CancellationToken>()))
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

            this.itemSearchOptionsMock.Setup(x => x.CurrentValue)
                .Returns(itemSearchOptions);
        }
    }
}