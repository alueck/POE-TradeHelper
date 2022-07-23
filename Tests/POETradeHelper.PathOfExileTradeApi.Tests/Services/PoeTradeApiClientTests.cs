using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using Moq;
using NUnit.Framework;
using POETradeHelper.Common.Wrappers;
using POETradeHelper.PathOfExileTradeApi.Exceptions;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Properties;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.PathOfExileTradeApi.Tests.Services;

public class PoeTradeApiClientTests
{
    private Mock<IHttpClientWrapper> httpClientWrapperMock;
    private Mock<IHttpClientFactoryWrapper> httpClientFactoryWrapperMock;
    private Mock<IPoeTradeApiJsonSerializer> poeTradeApiJsonSerializerMock;
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

        this.poeTradeApiClient = new PoeTradeApiClient(this.httpClientFactoryWrapperMock.Object, this.poeTradeApiJsonSerializerMock.Object);
    }

    [Test]
    public void GetListingsAsyncWithSearchQueryRequestShouldThrowArgumentNullExceptionIfQueryRequestIsNull()
    {
        Func<Task> action = () => this.poeTradeApiClient.GetListingsAsync((SearchQueryRequest)null);

        action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task GetListingsAsyncWithSearchQueryRequestShouldPostQueryRequest()
    {
        // arrange
        CancellationTokenSource cts = new();
        const string expected = "serialized query request";

        this.poeTradeApiJsonSerializerMock.Setup(x => x.Serialize(It.IsAny<object>()))
            .Returns(expected);

        this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<SearchQueryResult>(It.IsAny<string>()))
            .Returns(new SearchQueryResult());

        this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<ItemListingsQueryResult>(It.IsAny<string>()))
            .Returns(new ItemListingsQueryResult());

        // act
        await this.poeTradeApiClient.GetListingsAsync(new SearchQueryRequest(), cts.Token);

        // assert
        this.httpClientWrapperMock.Verify(x => x.PostAsync(
            It.IsAny<string>(),
            It.Is<StringContent>(s => s.ReadAsStringAsync().GetAwaiter().GetResult() == expected),
            cts.Token));
    }

    [Test]
    public async Task GetListingsAsyncWithSearchQueryRequestShouldDeserializeSearchResponseAsSearchQueryResult()
    {
        // arrange
        const string expected = "serialized search query response";
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

        // act
        await this.poeTradeApiClient.GetListingsAsync(new SearchQueryRequest());

        // assert
        this.poeTradeApiJsonSerializerMock.Verify(x => x.Deserialize<SearchQueryResult>(expected));
    }

    [Test]
    public async Task GetListingsAsyncWithSearchQueryRequestShouldSetRequestOnResult()
    {
        SearchQueryRequest searchQueryRequest = new() { League = "Heist" };
        this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<SearchQueryResult>(It.IsAny<string>()))
            .Returns(new SearchQueryResult());
        this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<ItemListingsQueryResult>(It.IsAny<string>()))
            .Returns(new ItemListingsQueryResult());

        ItemListingsQueryResult result = await this.poeTradeApiClient.GetListingsAsync(searchQueryRequest);
            
        result.SearchQueryRequest.Should().Be(searchQueryRequest);
    }

    [Test]
    public async Task GetListingsAsyncWithSearchQueryRequestShouldFetchItemsWithCorrectUri()
    {
        // arrange
        var searchQueryResult = new SearchQueryResult
        {
            Result = Enumerable.Range(0, 20).Select(i => i.ToString()).ToList(),
            Total = 20
        };

        string expectedUri = $"{Resources.PoeTradeApiFetchEndpoint}/{string.Join(',', searchQueryResult.Result.Take(10))}";

        this.poeTradeApiJsonSerializerMock
            .Setup(x => x.Deserialize<SearchQueryResult>(It.IsAny<string>()))
            .Returns(searchQueryResult);
        this.poeTradeApiJsonSerializerMock
            .Setup(x => x.Deserialize<ItemListingsQueryResult>(It.IsAny<string>()))
            .Returns(new ItemListingsQueryResult());

        // act
        await this.poeTradeApiClient.GetListingsAsync(new SearchQueryRequest());

        // assert
        this.httpClientWrapperMock.Verify(x => x.GetAsync(expectedUri, It.IsAny<CancellationToken>()));
    }

    [Test]
    public async Task GetListingsAsyncWithSearchQueryRequestShouldNotFetchItemsIfSearchQueryResultIsEmpty()
    {
        this.poeTradeApiJsonSerializerMock
            .Setup(x => x.Deserialize<SearchQueryResult>(It.IsAny<string>()))
            .Returns(new SearchQueryResult());

        await this.poeTradeApiClient.GetListingsAsync(new SearchQueryRequest());

        this.httpClientWrapperMock.Verify(x => x.GetAsync(It.Is<string>(s => s.Contains(Resources.PoeTradeApiFetchEndpoint)), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task GetListingsAsyncWithSearchQueryRequestShouldDeserializeFetchResponseAsItemListingQueryResult()
    {
        // arrange
        const string expected = "serialized item listings";
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

        // act
        await this.poeTradeApiClient.GetListingsAsync(new SearchQueryRequest());

        // assert
        this.poeTradeApiJsonSerializerMock.Verify(x => x.Deserialize<QueryResult<ListingResult>>(expected));
    }

    [Test]
    public async Task GetListingsAsyncWithSearchQueryRequestShouldReturnFetchResult()
    {
        // arrange
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

        // act
        ItemListingsQueryResult result = await this.poeTradeApiClient.GetListingsAsync(new SearchQueryRequest());

        // assert
        result.Should().Be(expected);
    }

    [Test]
    public async Task GetListingsAsyncWithSearchQueryRequestShouldReturnResultWithUri()
    {
        // arrange
        var queryRequest = new SearchQueryRequest
        {
            League = "TestLeague"
        };
        const string expectedId = "abdef";
        this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<SearchQueryResult>(It.IsAny<string>()))
            .Returns(new SearchQueryResult
            {
                Id = expectedId,
                Request = queryRequest
            });

        this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<ItemListingsQueryResult>(It.IsAny<string>()))
            .Returns(new ItemListingsQueryResult());
        Uri expectedUri = new Uri($"{Resources.PoeTradeBaseUrl}{Resources.PoeTradeApiSearchEndpoint}/{queryRequest.League}/{expectedId}");

        // act
        ItemListingsQueryResult result = await this.poeTradeApiClient.GetListingsAsync(queryRequest);

        // assert
        result.Uri.Should().Be(expectedUri);
    }

    [Test]
    public async Task GetListingsAsyncWithSearchQueryRequestShouldReturnResultWithTotalCount()
    {
        int expectedCount = 100;
        this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<SearchQueryResult>(It.IsAny<string>()))
            .Returns(new SearchQueryResult
            {
                Total = expectedCount
            });

        this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<ItemListingsQueryResult>(It.IsAny<string>()))
            .Returns(new ItemListingsQueryResult());

        ItemListingsQueryResult result = await this.poeTradeApiClient.GetListingsAsync(new SearchQueryRequest());

        result.TotalCount.Should().Be(expectedCount);
    }

    [Test]
    public async Task GetListingsAsyncWithSearchQueryRequestShouldThrowExceptionIfFetchResponseStatusCodeDoesNotIndicateSuccess()
    {
        this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<SearchQueryResult>(It.IsAny<string>()))
            .Returns(new SearchQueryResult
            {
                Result = new List<string> { "123" },
                Total = 1
            });

        Func<Task> action = () => this.poeTradeApiClient.GetListingsAsync(new SearchQueryRequest());

        await this.AssertThrowsPoeTradeApiCommunicationExceptionIfHttpResponseDoesNotReturnSuccessStatusCode(action, Resources.PoeTradeApiFetchEndpoint);
    }

    [Test]
    public async Task GetListingsAsyncWithSearchQueryRequestShouldThrowExceptionIfSearchRequestStatusCodeDoesNotIndicateSuccess()
    {
        this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<SearchQueryResult>(It.IsAny<string>()))
            .Returns(new SearchQueryResult());
            
        Func<Task> action = () => this.poeTradeApiClient.GetListingsAsync(new SearchQueryRequest());

        await this.AssertThrowsPoeTradeApiCommunicationExceptionIfHttpResponseDoesNotReturnSuccessStatusCode(action, Resources.PoeTradeApiSearchEndpoint, "posted json content");
    }

    [Test]
    public async Task GetListingsAsyncWithSearchQueryRequestShouldThrowPoeTradeApiCommunicationExceptionIfAnyExceptionOccurs()
    {
        Exception expectedInnerException = new Exception();
        this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<SearchQueryResult>(It.IsAny<string>()))
            .Throws(expectedInnerException);
            
        Func<Task> action = () => this.poeTradeApiClient.GetListingsAsync(new SearchQueryRequest());

        await action.Should().ThrowAsync<PoeTradeApiCommunicationException>()
            .Where(ex => ex.InnerException == expectedInnerException);
    }

    [Test]
    public async Task GetListingsAsyncWithExchangeQueryRequestShouldThrowArgumentNullExceptionWithoutRequest()
    {
        Func<Task> action = () => this.poeTradeApiClient.GetListingsAsync((ExchangeQueryRequest)null);

        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task GetListingsAsyncWithExchangeQueryRequestShouldPostQueryRequest()
    {
        // arrange
        CancellationTokenSource cts = new();
        const string expected = "serialized query request";

        this.poeTradeApiJsonSerializerMock.Setup(x => x.Serialize(It.IsAny<object>()))
            .Returns(expected);

        this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<ExchangeQueryResult>(It.IsAny<string>()))
            .Returns(new ExchangeQueryResult("a", 1, new Dictionary<string, ExchangeQueryResultListing>()));

        // act
        await this.poeTradeApiClient.GetListingsAsync(new ExchangeQueryRequest(), cts.Token);

        // assert
        this.httpClientWrapperMock.Verify(x => x.PostAsync(
            It.IsAny<string>(),
            It.Is<StringContent>(s => s.ReadAsStringAsync().GetAwaiter().GetResult() == expected),
            cts.Token));
    }
        
    [Test]
    public async Task GetListingsAsyncWithExchangeQueryRequestShouldThrowExceptionIfResponseStatusCodeDoesNotIndicateSuccess()
    {
        ExchangeQueryRequest exchangeQueryRequest = new();
        const string jsonContent = "request";
        const HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest;
        HttpResponseMessage httpResponse = new HttpResponseMessage
        {
            Content = new StringContent(""),
            StatusCode = httpStatusCode
        };

        this.poeTradeApiJsonSerializerMock
            .Setup(x => x.Serialize(It.IsAny<ExchangeQueryRequest>()))
            .Returns(jsonContent);
        this.httpClientWrapperMock
            .Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(httpResponse);
            
        Func<Task> action = () => this.poeTradeApiClient.GetListingsAsync(exchangeQueryRequest);
            
        await action.Should().ThrowAsync<PoeTradeApiCommunicationException>()
            .Where(ex =>
                ex.Message.Contains(httpStatusCode.ToString())
                && ex.Message.Contains(exchangeQueryRequest.Endpoint)
                && ex.Message.Contains(jsonContent));
    }
        
    [Test]
    public async Task GetListingsAsyncWithExchangeQueryRequestShouldDeserializeResponse()
    {
        // arrange
        const string expected = "serialized search query response";
        HttpResponseMessage httpResponse = new HttpResponseMessage
        {
            Content = new StringContent(expected)
        };

        this.poeTradeApiJsonSerializerMock
            .Setup(x => x.Deserialize<ExchangeQueryResult>(It.IsAny<string>()))
            .Returns(new ExchangeQueryResult("a", 1, new Dictionary<string, ExchangeQueryResultListing>()));

        this.httpClientWrapperMock
            .Setup(x => x.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(httpResponse);

        // act
        await this.poeTradeApiClient.GetListingsAsync(new ExchangeQueryRequest());

        // assert
        this.poeTradeApiJsonSerializerMock.Verify(x => x.Deserialize<ExchangeQueryResult>(expected));
    }
        
    [Test]
    public async Task GetListingsAsyncWithExchangeQueryRequestShouldReturnResultWithUri()
    {
        // arrange
        var queryRequest = new ExchangeQueryRequest();
        const string expectedId = "abdef";
        this.poeTradeApiJsonSerializerMock
            .Setup(x => x.Deserialize<ExchangeQueryResult>(It.IsAny<string>()))
            .Returns(new ExchangeQueryResult(expectedId, 1, new Dictionary<string, ExchangeQueryResultListing>()));
        Uri expectedUri = new Uri($"{Resources.PoeTradeBaseUrl}{Resources.PoeTradeApiExchangeEndpoint}{queryRequest.League}/{expectedId}");

        // act
        ExchangeQueryResult result = await this.poeTradeApiClient.GetListingsAsync(queryRequest);

        // assert
        result.Uri.Should().Be(expectedUri);
    }

    private async Task AssertThrowsPoeTradeApiCommunicationExceptionIfHttpResponseDoesNotReturnSuccessStatusCode(Func<Task> action, string endpoint, string jsonContent = "")
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

        var exceptionAssertions = await action.Should().ThrowAsync<PoeTradeApiCommunicationException>()
            .Where(ex => ex.Message.Contains(httpStatusCode.ToString()) && ex.Message.Contains(endpoint));

        if (!string.IsNullOrEmpty(jsonContent))
        {
            exceptionAssertions.Where(ex => ex.Message.Contains(jsonContent));
        }
    }
}