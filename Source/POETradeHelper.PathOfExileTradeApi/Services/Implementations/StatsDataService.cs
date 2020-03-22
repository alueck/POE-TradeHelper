using Microsoft.Extensions.Logging;
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
        private readonly ILogger<StatsDataService> logger;

        public StatsDataService(IHttpClientFactoryWrapper httpclientFactory, IPoeTradeApiJsonSerializer poeTradeApiJsonSerializer, ILogger<StatsDataService> logger)
        {
            this.httpClient = httpclientFactory.CreateClient();
            this.poeTradeApiJsonSerializer = poeTradeApiJsonSerializer;
            this.logger = logger;
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
            StatData result = null;

            IEnumerable<Data<StatData>> statDataListsToSearch = this.GetStatDataListsToSearch(statCategoriesToSearch);

            if (itemStat is MonsterItemStat monsterItemStat)
            {
                result = GetStatDataPrivate(statDataListsToSearch, monsterItemStat);
            }
            else
            {
                result = this.GetStataDataPrivate(statDataListsToSearch, itemStat);
            }

            return result;
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

        private static StatData GetStatDataPrivate(IEnumerable<Data<StatData>> statDataListsToSearch, MonsterItemStat monsterItemStat)
        {
            return statDataListsToSearch
                .SelectMany(x => x.Entries)
                .FirstOrDefault(statData => statData.Text.StartsWith(monsterItemStat.Text, StringComparison.OrdinalIgnoreCase));
        }

        private StatData GetStataDataPrivate(IEnumerable<Data<StatData>> statDataListsToSearch, ItemStat itemStat)
        {
            StatData result = null;

            IList<StatDataTextMatchResult> statDataMatches = GetStatDataMatches(statDataListsToSearch, itemStat);

            result = statDataMatches.FirstOrDefault(match => match.IsExactMatch)?.StatData;

            if (result == null)
            {
                if (statDataMatches.Count == 1)
                {
                    result = statDataMatches[0].StatData;
                }
                else
                {
                    this.logger.LogWarning("Failed to find matching stat data for {@itemStat}. Found: {@statDataMatches}", itemStat, statDataMatches);
                }
            }

            return result;
        }

        private static IList<StatDataTextMatchResult> GetStatDataMatches(IEnumerable<Data<StatData>> statDataListsToSearch, ItemStat itemStat)
        {
            var statDataTextMatcher = new StatDataTextMatcher(itemStat.Text);

            var statDataMatches = new List<StatDataTextMatchResult>();

            foreach (var statData in statDataListsToSearch.SelectMany(x => x.Entries))
            {
                StatDataTextMatchResult matchResult = statDataTextMatcher.Match(statData);

                if (matchResult.IsMatch)
                {
                    statDataMatches.Add(matchResult);

                    if (matchResult.IsExactMatch)
                    {
                        break;
                    }
                }
            }

            return statDataMatches;
        }

        private class StatDataTextMatcher
        {
            private const string ExactMatchGroupName = "exactMatchGroup";

            private readonly Regex regex;

            public StatDataTextMatcher(string statText)
            {
                this.regex = GetStatDataTextRegex(statText);
            }

            public StatDataTextMatchResult Match(StatData statData)
            {
                var match = this.regex.Match(statData.Text);

                return new StatDataTextMatchResult(statData, match.Success, match.Groups[ExactMatchGroupName].Success);
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

                return new Regex($"(?<{ExactMatchGroupName}>^{regexString}$)|({regexString})");
            }
        }

        private class StatDataTextMatchResult
        {
            public StatDataTextMatchResult(StatData statData, bool isMatch, bool isExactMatch)
            {
                this.IsMatch = isMatch;
                this.IsExactMatch = isExactMatch;
                StatData = statData;
            }

            public StatData StatData { get; }
            public bool IsMatch { get; }

            public bool IsExactMatch { get; }
        }
    }
}