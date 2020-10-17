using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using POETradeHelper.Common.Wrappers;
using POETradeHelper.ItemSearch.Contract.Configuration;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Exceptions;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Properties;

namespace POETradeHelper.PathOfExileTradeApi.Services
{
    public class PoeTradeApiClient : IPoeTradeApiClient
    {
        private readonly IHttpClientWrapper httpClient;
        private readonly IPoeTradeApiJsonSerializer jsonSerializer;
        private readonly IItemSearchQueryRequestMapperAggregator itemToQueryRequestMapperAggregator;
        private readonly IOptionsMonitor<ItemSearchOptions> itemSearchOptions;

        public PoeTradeApiClient(IHttpClientFactoryWrapper httpClientFactory,
            IPoeTradeApiJsonSerializer jsonSerializer,
            IItemSearchQueryRequestMapperAggregator itemSearchQueryRequestMapperAggregator,
            IOptionsMonitor<ItemSearchOptions> itemSearchOptions)
        {
            this.httpClient = httpClientFactory.CreateClient(Constants.HttpClientNames.PoeTradeApiItemSearchClient);
            this.jsonSerializer = jsonSerializer;
            this.itemToQueryRequestMapperAggregator = itemSearchQueryRequestMapperAggregator;
            this.itemSearchOptions = itemSearchOptions;
        }

        public async Task<ItemListingsQueryResult> GetListingsAsync(Item item, CancellationToken cancellationToken = default)
        {
            IQueryRequest queryRequest = this.itemToQueryRequestMapperAggregator.MapToQueryRequest(item);

            return await this.GetItemListingsQueryResult(queryRequest, cancellationToken);
        }

        public Task<ItemListingsQueryResult> GetListingsAsync(IQueryRequest queryRequest, CancellationToken cancellationToken = default)
        {
            return this.GetItemListingsQueryResult(queryRequest, cancellationToken);
        }

        private async Task<ItemListingsQueryResult> GetItemListingsQueryResult(IQueryRequest queryRequest, CancellationToken cancellationToken)
        {
            try
            {
                SearchQueryResult searchQueryResult = await this.GetSearchQueryResult(queryRequest, cancellationToken);

                return await this.GetListingsQueryResult(searchQueryResult, cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return null;
            }
            catch (Exception exception) when (!(exception is PoeTradeApiCommunicationException))
            {
                throw new PoeTradeApiCommunicationException("Retrieving listings for item led to an exception.", exception);
            }
        }

        private async Task<SearchQueryResult> GetSearchQueryResult(IQueryRequest queryRequest, CancellationToken cancellationToken)
        {
            StringContent content = this.GetJsonStringContent(queryRequest);

            string endpoint = $"{queryRequest.Endpoint}/{this.itemSearchOptions.CurrentValue.League.Id}";
            HttpResponseMessage response = await this.httpClient.PostAsync(endpoint, content, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new PoeTradeApiCommunicationException(endpoint, await content.ReadAsStringAsync(), response.StatusCode);
            }

            var searchQueryResult = await this.ReadAsJsonAsync<SearchQueryResult>(response.Content);
            searchQueryResult.Request = queryRequest;

            return searchQueryResult;
        }

        private StringContent GetJsonStringContent(IQueryRequest queryRequest)
        {
            string serializedQueryRequest = this.jsonSerializer.Serialize(queryRequest);

            return new StringContent(serializedQueryRequest, Encoding.UTF8, "application/json");
        }

        private async Task<ItemListingsQueryResult> GetListingsQueryResult(SearchQueryResult searchQueryResult, CancellationToken cancellationToken)
        {
            ItemListingsQueryResult itemListingsQueryResult = null;

            if (searchQueryResult.Total > 0)
            {
                itemListingsQueryResult = await this.GetAsync<ItemListingsQueryResult>($"{Resources.PoeTradeApiFetchEndpoint}/{string.Join(",", searchQueryResult.Result.Take(10))}", cancellationToken);
            }

            itemListingsQueryResult = itemListingsQueryResult ?? new ItemListingsQueryResult();
            itemListingsQueryResult.Uri = new Uri($"{Resources.PoeTradeBaseUrl}{Resources.PoeTradeApiSearchEndpoint}/{this.itemSearchOptions.CurrentValue.League.Id}/{searchQueryResult.Id}");
            itemListingsQueryResult.TotalCount = searchQueryResult.Total;
            itemListingsQueryResult.SearchQueryRequest = searchQueryResult.Request;

            return itemListingsQueryResult;
        }

        private async Task<TResult> GetAsync<TResult>(string endpoint, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await this.httpClient.GetAsync(endpoint, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    throw new PoeTradeApiCommunicationException(endpoint, response.StatusCode);
                }

                return await this.ReadAsJsonAsync<TResult>(response.Content);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception) when (!(exception is PoeTradeApiCommunicationException))
            {
                throw new PoeTradeApiCommunicationException($"Retrieving data from '{endpoint}' led to an exception.", exception);
            }
        }

        private async Task<TResult> ReadAsJsonAsync<TResult>(HttpContent httpContent)
        {
            string json = await httpContent.ReadAsStringAsync();

            return this.jsonSerializer.Deserialize<TResult>(json);
        }
    }
}