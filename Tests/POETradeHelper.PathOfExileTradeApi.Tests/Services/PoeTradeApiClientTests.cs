﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using DotNext;

using FluentAssertions;
using FluentAssertions.Specialized;

using NSubstitute;
using NSubstitute.ExceptionExtensions;

using NUnit.Framework;

using POETradeHelper.Common.Wrappers;
using POETradeHelper.PathOfExileTradeApi.Constants;
using POETradeHelper.PathOfExileTradeApi.Exceptions;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Properties;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.PathOfExileTradeApi.Tests.Services;

public class PoeTradeApiClientTests
{
    private readonly IHttpClientWrapper httpClientWrapperMock;
    private readonly IPoeTradeApiJsonSerializer poeTradeApiJsonSerializerMock;
    private readonly PoeTradeApiClient poeTradeApiClient;

    public PoeTradeApiClientTests()
    {
        this.httpClientWrapperMock = Substitute.For<IHttpClientWrapper>();
        this.httpClientWrapperMock.GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new HttpResponseMessage
            {
                Content = new StringContent(string.Empty),
            });
        this.httpClientWrapperMock.PostAsync(Arg.Any<string>(), Arg.Any<HttpContent>(), Arg.Any<CancellationToken>())
            .Returns(new HttpResponseMessage
            {
                Content = new StringContent(string.Empty),
            });

        var httpClientFactoryWrapperMock = Substitute.For<IHttpClientFactoryWrapper>();
        httpClientFactoryWrapperMock.CreateClient(HttpClientNames.PoeTradeApiItemSearchClient)
            .Returns(this.httpClientWrapperMock);

        this.poeTradeApiJsonSerializerMock = Substitute.For<IPoeTradeApiJsonSerializer>();
        this.poeTradeApiJsonSerializerMock.Serialize(Arg.Any<object>())
            .Returns(string.Empty);

        this.poeTradeApiClient = new PoeTradeApiClient(httpClientFactoryWrapperMock, this.poeTradeApiJsonSerializerMock);
    }

    [Test]
    public async Task GetListingsAsyncWithSearchQueryRequestShouldPostQueryRequest()
    {
        // arrange
        CancellationTokenSource cts = new();
        const string expected = "serialized query request";

        this.poeTradeApiJsonSerializerMock.Serialize(Arg.Any<object>())
            .Returns(expected);

        this.poeTradeApiJsonSerializerMock.Deserialize<SearchQueryResult>(Arg.Any<string>())
            .Returns(new SearchQueryResult());

        this.poeTradeApiJsonSerializerMock.Deserialize<ItemListingsQueryResult>(Arg.Any<string>())
            .Returns(new ItemListingsQueryResult());

        // act
        await this.poeTradeApiClient.GetListingsAsync(new SearchQueryRequest(), cts.Token);

        // assert
        await this.httpClientWrapperMock
            .Received()
            .PostAsync(
                Arg.Any<string>(),
                Arg.Is<StringContent>(s => s.ReadAsStringAsync().GetAwaiter().GetResult() == expected),
                cts.Token);
    }

    [Test]
    public async Task GetListingsAsyncWithSearchQueryRequestShouldDeserializeSearchResponseAsSearchQueryResult()
    {
        // arrange
        const string expected = "serialized search query response";
        HttpResponseMessage httpResponse = new()
        {
            Content = new StringContent(expected),
        };

        this.poeTradeApiJsonSerializerMock.Deserialize<SearchQueryResult>(Arg.Any<string>())
            .Returns(new SearchQueryResult());
        this.poeTradeApiJsonSerializerMock.Deserialize<ItemListingsQueryResult>(Arg.Any<string>())
            .Returns(new ItemListingsQueryResult());

        this.httpClientWrapperMock
            .PostAsync(
                Arg.Is<string>(s => s.StartsWith(Resources.PoeTradeApiSearchEndpoint)),
                Arg.Any<HttpContent>(),
                Arg.Any<CancellationToken>())
            .Returns(httpResponse);

        // act
        await this.poeTradeApiClient.GetListingsAsync(new SearchQueryRequest());

        // assert
        this.poeTradeApiJsonSerializerMock
            .Received()
            .Deserialize<SearchQueryResult>(expected);
    }

    [Test]
    public async Task GetListingsAsyncWithSearchQueryRequestShouldSetSearchQueryResultOnResult()
    {
        SearchQueryRequest searchQueryRequest = new() { League = "Heist" };
        SearchQueryResult expected = new() { Id = "Test" };
        this.poeTradeApiJsonSerializerMock.Deserialize<SearchQueryResult>(Arg.Any<string>())
            .Returns(expected);
        this.poeTradeApiJsonSerializerMock.Deserialize<ItemListingsQueryResult>(Arg.Any<string>())
            .Returns(new ItemListingsQueryResult());

        ItemListingsQueryResult result = await this.poeTradeApiClient.GetListingsAsync(searchQueryRequest);

        result.SearchQueryResult.Should().Be(expected);
    }

    [Test]
    public async Task GetListingsAsyncWithSearchQueryRequestShouldFetchItemsWithCorrectUri()
    {
        // arrange
        SearchQueryResult searchQueryResult = new()
        {
            Result = Enumerable.Range(0, 20).Select(i => i.ToString()).ToList(),
            Total = 20,
        };

        string expectedUri =
            $"{Resources.PoeTradeApiFetchEndpoint}/{string.Join(',', searchQueryResult.Result.Take(10))}";

        this.poeTradeApiJsonSerializerMock
            .Deserialize<SearchQueryResult>(Arg.Any<string>())
            .Returns(searchQueryResult);
        this.poeTradeApiJsonSerializerMock
            .Deserialize<ItemListingsQueryResult>(Arg.Any<string>())
            .Returns(new ItemListingsQueryResult());

        // act
        await this.poeTradeApiClient.GetListingsAsync(new SearchQueryRequest());

        // assert
        await this.httpClientWrapperMock
            .Received()
            .GetAsync(expectedUri, Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task GetListingsAsyncWithSearchQueryRequestShouldNotFetchItemsIfSearchQueryResultIsEmpty()
    {
        this.poeTradeApiJsonSerializerMock
            .Deserialize<SearchQueryResult>(Arg.Any<string>())
            .Returns(new SearchQueryResult());

        await this.poeTradeApiClient.GetListingsAsync(new SearchQueryRequest());

        await this.httpClientWrapperMock
            .DidNotReceive()
            .GetAsync(
                Arg.Is<string>(s => s.Contains(Resources.PoeTradeApiFetchEndpoint)),
                Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task GetListingsAsyncWithSearchQueryRequestShouldDeserializeFetchResponseAsItemListingQueryResult()
    {
        // arrange
        const string expected = "serialized item listings";
        this.poeTradeApiJsonSerializerMock.Deserialize<SearchQueryResult>(Arg.Any<string>())
            .Returns(new SearchQueryResult
            {
                Result = ["123"],
                Total = 1,
            });

        this.poeTradeApiJsonSerializerMock.Deserialize<ItemListingsQueryResult>(Arg.Any<string>())
            .Returns(new ItemListingsQueryResult());

        this.httpClientWrapperMock
            .GetAsync(
                Arg.Is<string>(s => s.StartsWith(Resources.PoeTradeApiFetchEndpoint)),
                Arg.Any<CancellationToken>())
            .Returns(new HttpResponseMessage
            {
                Content = new StringContent(expected),
            });

        // act
        await this.poeTradeApiClient.GetListingsAsync(new SearchQueryRequest());

        // assert
        this.poeTradeApiJsonSerializerMock
            .Received()
            .Deserialize<ItemListingsQueryResult>(expected);
    }

    [Test]
    public async Task GetListingsAsyncWithSearchQueryRequestShouldReturnFetchResultForFirstPage()
    {
        // arrange
        const string id = "aef21";
        const string league = "League";

        SearchQueryResult searchQueryResult = new()
        {
            Id = id,
            Result = ["123"],
            Total = 1,
        };

        ItemListingsQueryResult expected = new()
        {
            Result =
            [
                new() { Id = "Test" },
            ],
            CurrentPage = 1,
            TotalCount = 1,
            Uri = new Uri($"{Resources.PoeTradeBaseUrl}{Resources.PoeTradeApiSearchEndpoint}/{league}/{id}"),
            SearchQueryResult = searchQueryResult,
        };

        this.poeTradeApiJsonSerializerMock.Deserialize<SearchQueryResult>(Arg.Any<string>())
            .Returns(searchQueryResult);

        this.poeTradeApiJsonSerializerMock.Deserialize<ItemListingsQueryResult>(Arg.Any<string>())
            .Returns(expected);

        // act
        ItemListingsQueryResult result = await this.poeTradeApiClient.GetListingsAsync(new SearchQueryRequest { League = league });

        // assert
        result.Should().BeEquivalentTo(expected);
    }

    [Test]
    public async Task GetListingsAsyncWithSearchQueryRequestShouldReturnFetchResultForFirstTwoPagesIfEnoughDataFound()
    {
        // arrange
        const string id = "aef21";
        const string league = "League";

        SearchQueryResult searchQueryResult = new()
        {
            Id = id,
            Result = Enumerable.Range(0, 100).Select(x => x.ToString()).ToList(),
            Total = 100,
        };

        ItemListingsQueryResult firstListingsResult = new()
        {
            Result = [new ListingResult { Id = "Test" }],
            CurrentPage = 1,
            TotalCount = 100,
            Uri = new Uri($"{Resources.PoeTradeBaseUrl}{Resources.PoeTradeApiSearchEndpoint}/{league}/{id}"),
            SearchQueryResult = searchQueryResult,
        };

        ItemListingsQueryResult secondListingsResult = firstListingsResult with
        {
            Result = [new ListingResult { Id = "Test2" }],
            CurrentPage = 2,
        };

        this.poeTradeApiJsonSerializerMock.Deserialize<SearchQueryResult>(Arg.Any<string>())
            .Returns(searchQueryResult);

        this.poeTradeApiJsonSerializerMock.Deserialize<ItemListingsQueryResult>(Arg.Any<string>())
            .Returns(firstListingsResult, secondListingsResult);

        // act
        ItemListingsQueryResult result = await this.poeTradeApiClient.GetListingsAsync(new SearchQueryRequest { League = league });

        // assert
        result.Should().BeEquivalentTo(firstListingsResult with
        {
            CurrentPage = 2,
            Result = [firstListingsResult.Result[0], secondListingsResult.Result[0]],
        });
    }

    [Test]
    public async Task GetListingsAsyncWithSearchQueryRequestShouldReturnResultWithUri()
    {
        // arrange
        SearchQueryRequest queryRequest = new()
        {
            League = "TestLeague",
        };
        const string expectedId = "abdef";
        this.poeTradeApiJsonSerializerMock.Deserialize<SearchQueryResult>(Arg.Any<string>())
            .Returns(new SearchQueryResult
            {
                Id = expectedId,
                Request = queryRequest,
            });

        this.poeTradeApiJsonSerializerMock.Deserialize<ItemListingsQueryResult>(Arg.Any<string>())
            .Returns(new ItemListingsQueryResult());
        Uri expectedUri =
            new($"{Resources.PoeTradeBaseUrl}{Resources.PoeTradeApiSearchEndpoint}/{queryRequest.League}/{expectedId}");

        // act
        ItemListingsQueryResult result = await this.poeTradeApiClient.GetListingsAsync(queryRequest);

        // assert
        result.Uri.Should().Be(expectedUri);
    }

    [Test]
    public async Task GetListingsAsyncWithSearchQueryRequestShouldReturnResultWithTotalCount()
    {
        const int expectedCount = 100;
        this.poeTradeApiJsonSerializerMock.Deserialize<SearchQueryResult>(Arg.Any<string>())
            .Returns(new SearchQueryResult
            {
                Total = expectedCount,
            });

        this.poeTradeApiJsonSerializerMock.Deserialize<ItemListingsQueryResult>(Arg.Any<string>())
            .Returns(new ItemListingsQueryResult());

        ItemListingsQueryResult result = await this.poeTradeApiClient.GetListingsAsync(new SearchQueryRequest());

        result.TotalCount.Should().Be(expectedCount);
    }

    [Test]
    public async Task
        GetListingsAsyncWithSearchQueryRequestShouldThrowExceptionIfFetchResponseStatusCodeDoesNotIndicateSuccess()
    {
        this.poeTradeApiJsonSerializerMock.Deserialize<SearchQueryResult>(Arg.Any<string>())
            .Returns(new SearchQueryResult
            {
                Result = ["123"],
                Total = 1,
            });

        Func<Task> action = () => this.poeTradeApiClient.GetListingsAsync(new SearchQueryRequest());

        await this.AssertThrowsPoeTradeApiCommunicationExceptionIfHttpResponseDoesNotReturnSuccessStatusCode(
            action,
            Resources.PoeTradeApiFetchEndpoint);
    }

    [Test]
    public async Task
        GetListingsAsyncWithSearchQueryRequestShouldThrowExceptionIfSearchRequestStatusCodeDoesNotIndicateSuccess()
    {
        this.poeTradeApiJsonSerializerMock.Deserialize<SearchQueryResult>(Arg.Any<string>())
            .Returns(new SearchQueryResult());

        Func<Task> action = () => this.poeTradeApiClient.GetListingsAsync(new SearchQueryRequest());

        await this.AssertThrowsPoeTradeApiCommunicationExceptionIfHttpResponseDoesNotReturnSuccessStatusCode(
            action,
            Resources.PoeTradeApiSearchEndpoint,
            "posted json content");
    }

    [Test]
    public async Task
        GetListingsAsyncWithSearchQueryRequestShouldThrowPoeTradeApiCommunicationExceptionIfAnyExceptionOccurs()
    {
        Exception expectedInnerException = new();
        this.poeTradeApiJsonSerializerMock
            .Deserialize<SearchQueryResult>(Arg.Any<string>())
            .Throws(expectedInnerException);

        Func<Task> action = () => this.poeTradeApiClient.GetListingsAsync(new SearchQueryRequest());

        await action.Should().ThrowAsync<PoeTradeApiCommunicationException>()
            .Where(ex => ex.InnerException == expectedInnerException);
    }

    [Test]
    public async Task GetListingsAsyncWithExchangeQueryRequestShouldPostQueryRequest()
    {
        // arrange
        CancellationTokenSource cts = new();
        const string expected = "serialized query request";

        this.poeTradeApiJsonSerializerMock.Serialize(Arg.Any<object>())
            .Returns(expected);

        this.poeTradeApiJsonSerializerMock.Deserialize<ExchangeQueryResult>(Arg.Any<string>())
            .Returns(new ExchangeQueryResult("a", 1, []));

        // act
        await this.poeTradeApiClient.GetListingsAsync(new ExchangeQueryRequest(), cts.Token);

        // assert
        await this.httpClientWrapperMock
            .Received()
            .PostAsync(
                Arg.Any<string>(),
                Arg.Is<StringContent>(s => s.ReadAsStringAsync().GetAwaiter().GetResult() == expected),
                cts.Token);
    }

    [Test]
    public async Task
        GetListingsAsyncWithExchangeQueryRequestShouldThrowExceptionIfResponseStatusCodeDoesNotIndicateSuccess()
    {
        ExchangeQueryRequest exchangeQueryRequest = new();
        const string jsonContent = "request";
        const HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest;
        HttpResponseMessage httpResponse = new()
        {
            Content = new StringContent(string.Empty),
            StatusCode = httpStatusCode,
        };

        this.poeTradeApiJsonSerializerMock
            .Serialize(Arg.Any<ExchangeQueryRequest>())
            .Returns(jsonContent);
        this.httpClientWrapperMock
            .PostAsync(Arg.Any<string>(), Arg.Any<HttpContent>(), Arg.Any<CancellationToken>())
            .Returns(httpResponse);

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
        HttpResponseMessage httpResponse = new()
        {
            Content = new StringContent(expected),
        };

        this.poeTradeApiJsonSerializerMock
            .Deserialize<ExchangeQueryResult>(Arg.Any<string>())
            .Returns(new ExchangeQueryResult("a", 1, []));

        this.httpClientWrapperMock
            .PostAsync(Arg.Any<string>(), Arg.Any<HttpContent>(), Arg.Any<CancellationToken>())
            .Returns(httpResponse);

        // act
        await this.poeTradeApiClient.GetListingsAsync(new ExchangeQueryRequest());

        // assert
        this.poeTradeApiJsonSerializerMock
            .Received()
            .Deserialize<ExchangeQueryResult>(expected);
    }

    [Test]
    public async Task GetListingsAsyncWithExchangeQueryRequestShouldReturnResultWithUri()
    {
        // arrange
        ExchangeQueryRequest queryRequest = new()
        {
            League = "TestLeague",
        };
        const string expectedId = "abdef";
        this.poeTradeApiJsonSerializerMock
            .Deserialize<ExchangeQueryResult>(Arg.Any<string>())
            .Returns(new ExchangeQueryResult(expectedId, 1, []));
        Uri expectedUri =
            new(
                $"{Resources.PoeTradeBaseUrl}{Resources.PoeTradeApiExchangeEndpoint}/{queryRequest.League}/{expectedId}");

        // act
        ExchangeQueryResult result = await this.poeTradeApiClient.GetListingsAsync(queryRequest);

        // assert
        result.Uri.Should().Be(expectedUri);
    }

    [Test]
    public async Task LoadNextPageReturnsNoneIfLastResultHasNoMorePages()
    {
        // arrange
        ItemListingsQueryResult lastResult = new()
        {
            SearchQueryResult = new SearchQueryResult
            {
                Total = 100,
            },
            TotalCount = 100,
            CurrentPage = 10,
        };

        // act
        Optional<ItemListingsQueryResult> result = await this.poeTradeApiClient.LoadNextPage(lastResult);

        // assert
        result.Should().Be(Optional<ItemListingsQueryResult>.None);
    }

    [Test]
    public async Task LoadNextPageReturnsResultIfLastResultHasMorePages()
    {
        // arrange
        const string id = "afe2";
        const string league = "League";
        ItemListingsQueryResult lastResult = new()
        {
            SearchQueryResult = new SearchQueryResult
            {
                Result = Enumerable.Range(0, 100).Select(x => x.ToString()).ToList(),
                Total = 100,
                Id = id,
                Request = new SearchQueryRequest { League = league },
            },
            TotalCount = 100,
            CurrentPage = 9,
        };

        string url = $"{Resources.PoeTradeApiFetchEndpoint}/{string.Join(',', lastResult.SearchQueryResult.Result.Skip(90).Take(10))}";
        const string json = "{ json }";
        this.httpClientWrapperMock.GetAsync(url, Arg.Any<CancellationToken>())
            .Returns(new HttpResponseMessage
            {
                Content = new StringContent(json),
            });

        ItemListingsQueryResult expected = new()
        {
            Uri = new Uri($"{Resources.PoeTradeBaseUrl}{Resources.PoeTradeApiSearchEndpoint}/{league}/{id}"),
            SearchQueryResult = lastResult.SearchQueryResult,
            TotalCount = 100,
            CurrentPage = 10,
        };
        this.poeTradeApiJsonSerializerMock
            .Deserialize<ItemListingsQueryResult>(json)
            .Returns(expected);

        // act
        Optional<ItemListingsQueryResult> result = await this.poeTradeApiClient.LoadNextPage(lastResult);

        // assert
        result.Value.Should().BeEquivalentTo(expected);
    }

    private async Task AssertThrowsPoeTradeApiCommunicationExceptionIfHttpResponseDoesNotReturnSuccessStatusCode(
        Func<Task> action, string endpoint, string jsonContent = "")
    {
        const HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest;

        HttpResponseMessage httpResponse = new()
        {
            Content = new StringContent(string.Empty),
            StatusCode = httpStatusCode,
        };

        this.poeTradeApiJsonSerializerMock.Serialize(Arg.Any<SearchQueryRequest>())
            .Returns(jsonContent);
        this.httpClientWrapperMock.GetAsync(Arg.Is<string>(s => s.Contains(endpoint)), Arg.Any<CancellationToken>())
            .Returns(httpResponse);
        this.httpClientWrapperMock
            .PostAsync(
                Arg.Is<string>(s => s.Contains(endpoint)),
                Arg.Any<HttpContent>(),
                Arg.Any<CancellationToken>())
            .Returns(httpResponse);

        ExceptionAssertions<PoeTradeApiCommunicationException> exceptionAssertions = await action.Should()
            .ThrowAsync<PoeTradeApiCommunicationException>()
            .Where(ex => ex.Message.Contains(httpStatusCode.ToString()) && ex.Message.Contains(endpoint));

        if (!string.IsNullOrEmpty(jsonContent))
        {
            exceptionAssertions.Where(ex => ex.Message.Contains(jsonContent));
        }
    }
}