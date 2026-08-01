using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using DotNext;

using POETradeHelper.Common.Wrappers;
using POETradeHelper.PathOfExileTradeApi.Constants;
using POETradeHelper.PathOfExileTradeApi.Exceptions;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Properties;

namespace POETradeHelper.PathOfExileTradeApi.Services;

public class PoeTradeApiClient : IPoeTradeApiClient
{
    private readonly IHttpClientWrapper httpClient;
    private readonly IPoeTradeApiJsonSerializer jsonSerializer;

    public PoeTradeApiClient(
        IHttpClientFactoryWrapper httpClientFactory,
        IPoeTradeApiJsonSerializer jsonSerializer)
    {
        this.httpClient = httpClientFactory.CreateClient(HttpClientNames.PoeTradeApiItemSearchClient);
        this.jsonSerializer = jsonSerializer;
    }

    public async Task<ItemListingsQueryResult> GetListingsAsync(SearchQueryRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            SearchQueryResult searchQueryResult = await this.GetSearchQueryResult(request, cancellationToken).ConfigureAwait(false);

            ItemListingsQueryResult result = await this.GetListingsQueryResult(searchQueryResult, 1, request.PageSize, cancellationToken).ConfigureAwait(false);

            return result;
        }
        catch (Exception exception) when (exception is not PoeTradeApiCommunicationException and not OperationCanceledException)
        {
            throw new PoeTradeApiCommunicationException("Retrieving listings for item led to an exception.", exception);
        }
    }

    public async Task<Optional<ItemListingsQueryResult>> LoadNextPage(ItemListingsQueryResult lastResult, CancellationToken cancellationToken = default)
    {
        if (!lastResult.HasMorePages)
        {
            return Optional<ItemListingsQueryResult>.None;
        }

        try
        {
            return await this.GetListingsQueryResult(lastResult.SearchQueryResult, lastResult.CurrentPage + 1, lastResult.SearchQueryResult.Request.PageSize, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception) when (exception is not PoeTradeApiCommunicationException and not OperationCanceledException)
        {
            throw new PoeTradeApiCommunicationException("Retrieving listings for item led to an exception.", exception);
        }
    }

    public async Task<ExchangeQueryResult> GetListingsAsync(ExchangeQueryRequest request, CancellationToken cancellationToken = default)
    {
        StringContent content = this.GetJsonStringContent(request);

        string endpoint = $"{request.Endpoint}/{request.League}";
        HttpResponseMessage response = await this.httpClient.PostAsync(endpoint, content, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            string requestContent = await content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            string responseContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            throw new PoeTradeApiCommunicationException(endpoint, response.StatusCode, requestContent, responseContent);
        }

        ExchangeQueryResult result = await this.ReadAsJsonAsync<ExchangeQueryResult>(response.Content).ConfigureAwait(false);
        result.Uri = new Uri($"{Resources.PoeTradeBaseUrl}{endpoint}/{result.Id}");

        return result;
    }

    private async Task<SearchQueryResult> GetSearchQueryResult(SearchQueryRequest queryRequest, CancellationToken cancellationToken)
    {
        StringContent content = this.GetJsonStringContent(queryRequest);

        string endpoint = $"{queryRequest.Endpoint}/{queryRequest.League}";
        HttpResponseMessage response = await this.httpClient.PostAsync(endpoint, content, cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            string requestContent = await content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            string responseContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            throw new PoeTradeApiCommunicationException(endpoint, response.StatusCode, requestContent, responseContent);
        }

        SearchQueryResult searchQueryResult = await this.ReadAsJsonAsync<SearchQueryResult>(response.Content).ConfigureAwait(false);
        searchQueryResult.Request = queryRequest;

        return searchQueryResult;
    }

    private StringContent GetJsonStringContent(object request)
    {
        string serializedQueryRequest = this.jsonSerializer.Serialize(request);

        return new StringContent(serializedQueryRequest, Encoding.UTF8, "application/json");
    }

    private async Task<ItemListingsQueryResult> GetListingsQueryResult(
        SearchQueryResult searchQueryResult,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        ItemListingsQueryResult? itemListingsQueryResult = null;
        List<string> ids = searchQueryResult.Result.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        if (ids.Any())
        {
            string url = $"{Resources.PoeTradeApiFetchEndpoint}/{string.Join(",", ids)}";
            itemListingsQueryResult = await this.GetAsync<ItemListingsQueryResult>(url, cancellationToken).ConfigureAwait(false);
        }

        itemListingsQueryResult = (itemListingsQueryResult ?? new ItemListingsQueryResult()) with
        {
            Uri = new Uri($"{Resources.PoeTradeBaseUrl}{Resources.PoeTradeApiSearchEndpoint}/{searchQueryResult.Request.League}/{searchQueryResult.Id}"),
            TotalCount = searchQueryResult.Total,
            SearchQueryResult = searchQueryResult,
            CurrentPage = ids.Any() ? page : (int)Math.Ceiling(searchQueryResult.Total / (double)pageSize),
        };

        return itemListingsQueryResult;
    }

    private async Task<TResult> GetAsync<TResult>(string endpoint, CancellationToken cancellationToken = default)
    {
        try
        {
            HttpResponseMessage response = await this.httpClient.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                throw new PoeTradeApiCommunicationException(endpoint, response.StatusCode, responseContent);
            }

            return await this.ReadAsJsonAsync<TResult>(response.Content).ConfigureAwait(false);
        }
        catch (Exception exception) when (exception is not PoeTradeApiCommunicationException and not OperationCanceledException)
        {
            throw new PoeTradeApiCommunicationException($"Retrieving data from '{endpoint}' led to an exception.", exception);
        }
    }

    private async Task<TResult> ReadAsJsonAsync<TResult>(HttpContent httpContent)
    {
        string json = await httpContent.ReadAsStringAsync().ConfigureAwait(false);
        TResult? result = this.jsonSerializer.Deserialize<TResult>(json);

        return result ?? throw new PoeTradeApiCommunicationException("Deserialized null result from POE Trade API response.");
    }
}