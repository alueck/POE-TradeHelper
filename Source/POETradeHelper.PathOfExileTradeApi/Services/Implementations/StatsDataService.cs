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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace POETradeHelper.PathOfExileTradeApi.Services.Implementations
{
    public class StatsDataService : IStatsDataService, IInitializable
    {
        private const string Placeholder = "#";

        private readonly IHttpClientWrapper httpClient;
        private IPoeTradeApiJsonSerializer poeTradeApiJsonSerializer;
        private IList<Data<StatData>> statsData;

        private static readonly Regex NumberRegex = new Regex(@"[\+\-]?\d+", RegexOptions.Compiled);

        private delegate bool MatchStatTextDelegate(string statDataText, string statText);

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

        public StatData GetStatData(ItemStat itemStat, params StatCategory[] statCategoriesToSearch)
        {
            IEnumerable<Data<StatData>> statDataListsToSearch = this.GetStatDataListsToSearch(statCategoriesToSearch);

            return statDataListsToSearch
                .SelectMany(x => x.Entries)
                .FirstOrDefault(statData => IsMatchingStatData(statData, itemStat));
        }

        private IEnumerable<Data<StatData>> GetStatDataListsToSearch(params StatCategory[] statCategoriesToSearch)
        {
            IEnumerable<Data<StatData>> result = null;

            if (statCategoriesToSearch.Length != 0 && !statCategoriesToSearch.Any(x => x == StatCategory.Unknown))
            {
                result = this.statsData.Where(x => statCategoriesToSearch.Any(statCategory => string.Equals(x.Id, statCategory.GetDisplayName(), StringComparison.OrdinalIgnoreCase)));
            }

            return result ?? this.statsData;
        }

        private static bool IsMatchingStatData(StatData statData, ItemStat itemStat)
        {
            MatchStatTextDelegate matchStatTextDelegate = GetMatchStatTextDelegate(itemStat.Text);
            return matchStatTextDelegate(GetNormalizedStatText(statData.Text), GetNormalizedStatText(itemStat.Text));
        }

        private static MatchStatTextDelegate GetMatchStatTextDelegate(string statText)
        {
            return statText.StartsWith(Resources.MetamorphStatDescriptor, StringComparison.OrdinalIgnoreCase)
                ? (statDataText, text) => statDataText.StartsWith(text, StringComparison.OrdinalIgnoreCase)
                : (MatchStatTextDelegate)((statDataText, text) => string.Equals(statDataText, text, StringComparison.OrdinalIgnoreCase));
        }

        private static string GetNormalizedStatText(string statText)
        {
            statText = NumberRegex.Replace(statText, Placeholder);

            return statText.Replace($"+{Placeholder}", Placeholder);
        }
    }
}