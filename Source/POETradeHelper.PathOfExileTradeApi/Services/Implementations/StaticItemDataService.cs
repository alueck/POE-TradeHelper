using POETradeHelper.Common;
using POETradeHelper.Common.Wrappers;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Exceptions;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace POETradeHelper.PathOfExileTradeApi.Services
{
    public class StaticItemDataService : IStaticItemDataService, IInitializable
    {
        private readonly IHttpClientWrapper httpClient;
        private readonly IPoeTradeApiJsonSerializer poeTradeApiJsonSerializer;
        private IDictionary<string, StaticDataEntry> nameToStaticDataMappings;

        public StaticItemDataService(IHttpClientFactoryWrapper httpClientFactory, IPoeTradeApiJsonSerializer poeTradeApiJsonSerializer)
        {
            this.httpClient = httpClientFactory.CreateClient(nameof(StaticItemDataService));
            this.poeTradeApiJsonSerializer = poeTradeApiJsonSerializer;
        }

        public string GetId(Item item)
        {
            if (item is CurrencyItem || item is DivinationCardItem || item is FragmentItem)
            {
                return this.nameToStaticDataMappings[item.Name].Id;
            }

            throw new NotSupportedException($"Item of type '{item?.GetType()}' is not supported.");
        }

        public async Task OnInitAsync()
        {
            string requestUri = Resources.PoeTradeApiBaseUrl + Resources.PoeTradeApiStaticDataEndpoint;
            HttpResponseMessage httpResponse = await this.httpClient.GetAsync(requestUri);

            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new PoeTradeApiCommunicationException(requestUri, httpResponse.StatusCode);
            }

            string content = await httpResponse.Content.ReadAsStringAsync();
            var queryResult = this.poeTradeApiJsonSerializer.Deserialize<QueryResult<StaticData>>(content);

            this.nameToStaticDataMappings = queryResult?.Result?
                                                .Where(x => !x.Id.StartsWith("Map"))
                                                .SelectMany(x => x.Entries)
                                                .ToDictionary(x => x.Text);
        }
    }
}