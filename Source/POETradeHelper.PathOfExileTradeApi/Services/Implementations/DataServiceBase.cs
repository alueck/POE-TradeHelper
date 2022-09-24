using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using POETradeHelper.Common.Contract;
using POETradeHelper.Common.Wrappers;
using POETradeHelper.PathOfExileTradeApi.Exceptions;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.PathOfExileTradeApi.Services.Implementations
{
    public abstract class DataServiceBase<TDataType> : IInitializable
    {
        private readonly string endpoint;
        private readonly IHttpClientWrapper httpClient;
        private readonly IPoeTradeApiJsonSerializer poeTradeApiJsonSerializer;

        protected DataServiceBase(string endpoint, IHttpClientFactoryWrapper httpClientFactory, IPoeTradeApiJsonSerializer poeTradeApiJsonSerializer)
        {
            this.endpoint = endpoint;
            this.httpClient = httpClientFactory.CreateClient(Constants.HttpClientNames.PoeTradeApiDataClient);
            this.poeTradeApiJsonSerializer = poeTradeApiJsonSerializer;
        }

        protected IList<TDataType> Data { get; private set; } = new List<TDataType>();

        public virtual async Task OnInitAsync()
        {
            HttpResponseMessage httpResponse = await this.httpClient.GetAsync(this.endpoint);

            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new PoeTradeApiCommunicationException(this.endpoint, httpResponse.StatusCode);
            }

            string content = await httpResponse.Content.ReadAsStringAsync();
            var queryResult = this.poeTradeApiJsonSerializer.Deserialize<QueryResult<TDataType>>(content);

            if (queryResult != null)
            {
                this.Data = queryResult.Result;
            }
        }
    }
}