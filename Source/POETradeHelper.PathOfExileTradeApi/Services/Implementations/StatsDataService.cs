using POETradeHelper.Common;
using POETradeHelper.Common.Extensions;
using POETradeHelper.Common.Wrappers;
using POETradeHelper.ItemSearch.Contract;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Exceptions;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace POETradeHelper.PathOfExileTradeApi.Services.Implementations
{
    public class StatsDataService : IStatsDataService, IInitializable
    {
        private readonly IHttpClientWrapper httpClient;
        private IPoeTradeApiJsonSerializer poeTradeApiJsonSerializer;
        private IList<Data<StatData>> statsData;

        public StatsDataService(IHttpClientFactoryWrapper httpclientFactory, IPoeTradeApiJsonSerializer poeTradeApiJsonSerializer)
        {
            this.httpClient = httpclientFactory.CreateClient();
            this.poeTradeApiJsonSerializer = poeTradeApiJsonSerializer;
        }

        public async Task OnInitAsync()
        {
            string requestUri = Resources.PoeTradeApiBaseUrl + Resources.PoeTradeApiStatsDataEndpoint;
            HttpResponseMessage httpResponse = await this.httpClient.GetAsync(requestUri);

            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new PoeTradeApiCommunicationException(requestUri, httpResponse.StatusCode);
            }

            string content = await httpResponse.Content.ReadAsStringAsync();
            var queryResult = this.poeTradeApiJsonSerializer.Deserialize<QueryResult<Data<StatData>>>(content);

            this.statsData = queryResult?.Result;
        }

        public string GetId(ItemStat itemStat)
        {
            StatData statData = this.GetStatData(itemStat);

            return statData?.Id;
        }

        private StatData GetStatData(ItemStat itemStat)
        {
            IEnumerable<StatData> statData = this.GetStatDataListToSearch(itemStat.StatCategory.GetDisplayName());
            Predicate<string> predicate = GetSearchPredicate(itemStat.TextWithPlaceholders);

            return statData.FirstOrDefault(s => predicate(s.Text) || predicate(s.Text.Replace("+#", "#")));
        }

        private IEnumerable<StatData> GetStatDataListToSearch(string statCategory)
        {
            IEnumerable<StatData> result;

            Data<StatData> statCategoryData = this.statsData.FirstOrDefault(x => string.Equals(x.Id, statCategory, StringComparison.OrdinalIgnoreCase));
            result = statCategoryData?.Entries;

            return result ?? this.statsData.SelectMany(x => x.Entries);
        }

        private static Predicate<string> GetSearchPredicate(string statText)
        {
            return statText.StartsWith(Resources.MetamorphStatDescriptor, StringComparison.OrdinalIgnoreCase)
                ? text => text.StartsWith(statText, StringComparison.OrdinalIgnoreCase)
                : (Predicate<string>)(text => string.Equals(text, statText, StringComparison.OrdinalIgnoreCase));
        }
    }
}