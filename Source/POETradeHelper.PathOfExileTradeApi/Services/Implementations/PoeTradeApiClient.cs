using Microsoft.Extensions.Options;
using POETradeHelper.Common.Wrappers;
using POETradeHelper.ItemSearch;
using POETradeHelper.ItemSearch.Contract;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Exceptions;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace POETradeHelper.PathOfExileTradeApi.Services
{
    public class PoeTradeApiClient : IPoeTradeApiClient
    {
        private readonly IHttpClientWrapper httpClient;
        private readonly IPoeTradeApiJsonSerializer jsonSerializer;
        private readonly IItemSearchQueryRequestMapperAggregator itemToQueryRequestMapperAggregator;
        private readonly IOptions<ItemSearchOptions> itemSearchOptions;

        public PoeTradeApiClient(IHttpClientFactoryWrapper httpClientFactory,
            IPoeTradeApiJsonSerializer jsonSerializer,
            IItemSearchQueryRequestMapperAggregator itemSearchQueryRequestMapperAggregator,
            IOptions<ItemSearchOptions> itemSearchOptions)
        {
            this.httpClient = httpClientFactory.CreateClient(nameof(PoeTradeApiClient));
            this.jsonSerializer = jsonSerializer;
            this.itemToQueryRequestMapperAggregator = itemSearchQueryRequestMapperAggregator;
            this.itemSearchOptions = itemSearchOptions;
        }

        public async Task<ItemListingsQueryResult> GetListingsAsync(Item item)
        {
            try
            {
                SearchQueryRequest queryRequest = this.itemToQueryRequestMapperAggregator.MapToQueryRequest(item);

                SearchQueryResult searchQueryResult = await this.GetSearchQueryResult(queryRequest);

                return await this.GetListingsQueryResult(item, searchQueryResult);
            }
            catch (Exception exception) when (!(exception is PoeTradeApiCommunicationException))
            {
                throw new PoeTradeApiCommunicationException("Retrieving listings for item led to an exception.", exception);
            }
        }

        private async Task<SearchQueryResult> GetSearchQueryResult(SearchQueryRequest queryRequest)
        {
            StringContent content = this.GetJsonStringContent(queryRequest);
            HttpResponseMessage response = await this.httpClient.PostAsync($"{Resources.PoeTradeApiBaseUrl}{Resources.PoeTradeApiSearchEndpoint}/{this.itemSearchOptions.Value.League.Id}", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new PoeTradeApiCommunicationException(Resources.PoeTradeApiSearchEndpoint, await content.ReadAsStringAsync(), response.StatusCode);
            }

            return await this.ReadAsJsonAsync<SearchQueryResult>(response.Content);
        }

        private StringContent GetJsonStringContent(SearchQueryRequest queryRequest)
        {
            string serializedQueryRequest = this.jsonSerializer.Serialize(queryRequest);

            return new StringContent(serializedQueryRequest, Encoding.UTF8, "application/json");
        }

        private async Task<ItemListingsQueryResult> GetListingsQueryResult(Item item, SearchQueryResult searchQueryResult)
        {
            ItemListingsQueryResult itemListingsQueryResult = new ItemListingsQueryResult();

            if (searchQueryResult.Total > 0)
            {
                itemListingsQueryResult = await this.GetAsync<ItemListingsQueryResult>($"{Resources.PoeTradeApiFetchEndpoint}/{string.Join(",", searchQueryResult.Result.Take(10))}");
            }

            itemListingsQueryResult.Uri = new Uri($"{Resources.PoeTradeApiBaseUrl}{Resources.PoeTradeApiSearchEndpoint}/{this.itemSearchOptions.Value.League.Id}/{searchQueryResult.Id}");
            itemListingsQueryResult.TotalCount = searchQueryResult.Total;
            itemListingsQueryResult.Item = item;

            return itemListingsQueryResult;
        }

        public async Task<IList<League>> GetLeaguesAsync()
        {
            var queryResult = await this.GetAsync<QueryResult<League>>(Resources.PoeTradeApiLeaguesEndpoint);

            return queryResult?.Result;
        }

        private async Task<TResult> GetAsync<TResult>(string endpoint)
        {
            try
            {
                var response = await this.httpClient.GetAsync(Resources.PoeTradeApiBaseUrl + endpoint);

                if (!response.IsSuccessStatusCode)
                {
                    throw new PoeTradeApiCommunicationException(endpoint, response.StatusCode);
                }

                return await this.ReadAsJsonAsync<TResult>(response.Content);
            }
            catch (Exception exception) when (!(exception is PoeTradeApiCommunicationException))
            {
                throw new PoeTradeApiCommunicationException($"Retrieving data from endpoint '{endpoint}' of Path of Exile Trade API led to an exception.", exception);
            }
        }

        private async Task<TResult> ReadAsJsonAsync<TResult>(HttpContent httpContent)
        {
            string json = await httpContent.ReadAsStringAsync();

            return this.jsonSerializer.Deserialize<TResult>(json);
        }
    }
}