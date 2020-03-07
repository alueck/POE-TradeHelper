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

            Predicate<string> predicate = GetMatchStatDataPredicate(itemStat.Text);

            return statDataListsToSearch
                .SelectMany(x => x.Entries)
                .FirstOrDefault(statData => predicate(statData.Text));
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

        private static Predicate<string> GetMatchStatDataPredicate(string statText)
        {
            Predicate<string> result = null;

            if (statText.StartsWith(Resources.MetamorphStatDescriptor, StringComparison.OrdinalIgnoreCase))
            {
                result = (statDataText) => statDataText.StartsWith(statText, StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                Regex statDataTextRegex = GetStatDataTextRegex(statText);

                result = (statDataText) => statDataTextRegex.IsMatch(statDataText);
            }

            return result;
        }

        /// <summary>
        /// Replaces all numbers in the given <paramref name="statText"/> with a regex capture group to build a regex pattern for matching
        /// the correct stat data text and returns a regex created from this pattern.
        /// </summary>
        /// <example>
        /// 60% chance for Poisons inflicted with this Weapon to deal 100% more Damage
        /// becomes
        /// (60|#)% chance for Poisons inflicted with this Weapon to deal (100|#)% more Damage
        /// </example>
        private static Regex GetStatDataTextRegex(string statText)
        {
            string regexString = NumberRegex.Replace(statText, match => $"({Regex.Escape(match.Value)}|{Regex.Escape(Placeholder)})");

            return new Regex(regexString);
        }
    }
}