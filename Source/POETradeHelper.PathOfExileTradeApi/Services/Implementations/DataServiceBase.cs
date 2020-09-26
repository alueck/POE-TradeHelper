using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using POETradeHelper.Common;
using POETradeHelper.Common.Wrappers;
using POETradeHelper.PathOfExileTradeApi.Exceptions;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Properties;

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
            this.httpClient = httpClientFactory.CreateClient();
            this.poeTradeApiJsonSerializer = poeTradeApiJsonSerializer;
        }

        protected IList<Data<TDataType>> Data { get; private set; } = new List<Data<TDataType>>();

        public virtual async Task OnInitAsync()
        {
            string requestUri = Resources.PoeTradeApiBaseUrl + this.endpoint;

            HttpResponseMessage httpResponse = await this.httpClient.GetAsync(requestUri);

            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new PoeTradeApiCommunicationException(requestUri, httpResponse.StatusCode);
            }

            string content = await httpResponse.Content.ReadAsStringAsync();
            var queryResult = this.poeTradeApiJsonSerializer.Deserialize<QueryResult<Data<TDataType>>>(content);

            if (queryResult != null)
            {
                this.Data = queryResult.Result;
            }
        }
    }
}