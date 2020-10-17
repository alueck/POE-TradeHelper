using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using POETradeHelper.Common.Extensions;
using POETradeHelper.Common.Wrappers;
using POETradeHelper.ItemSearch.Contract;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Properties;

namespace POETradeHelper.PathOfExileTradeApi.Services.Implementations
{
    public class StatsDataService : DataServiceBase<Data<StatData>>, IStatsDataService
    {
        private const string Placeholder = "#";

        private IDictionary<string, StatData> statsDataDictionary = new Dictionary<string, StatData>();

        private static readonly Regex NumberRegex = new Regex(@"[\+\-]?\d+", RegexOptions.Compiled);
        private readonly ILogger<StatsDataService> logger;

        public StatsDataService(IHttpClientFactoryWrapper httpclientFactory, IPoeTradeApiJsonSerializer poeTradeApiJsonSerializer, ILogger<StatsDataService> logger)
            : base(Resources.PoeTradeApiStatsDataEndpoint, httpclientFactory, poeTradeApiJsonSerializer)
        {
            this.logger = logger;
        }

        public override async Task OnInitAsync()
        {
            await base.OnInitAsync();

            this.statsDataDictionary = this.Data.SelectMany(x => x.Entries).ToDictionary(statData => statData.Id);
        }

        public StatData GetStatData(ItemStat itemStat, params StatCategory[] statCategoriesToSearch)
        {
            IEnumerable<Data<StatData>> statDataListsToSearch = this.GetStatDataListsToSearch(statCategoriesToSearch);
            StatData result = this.GetStataDataPrivate(statDataListsToSearch, itemStat);

            return result;
        }

        private IEnumerable<Data<StatData>> GetStatDataListsToSearch(params StatCategory[] statCategoriesToSearch)
        {
            IEnumerable<Data<StatData>> result = null;

            if (statCategoriesToSearch.Length != 0 && !statCategoriesToSearch.Any(x => x == StatCategory.Unknown))
            {
                result = this.Data.Where(x => statCategoriesToSearch.Any(statCategory => string.Equals(x.Id, statCategory.GetDisplayName(), StringComparison.OrdinalIgnoreCase)));
            }

            return result ?? this.Data;
        }

        private StatData GetStataDataPrivate(IEnumerable<Data<StatData>> statDataListsToSearch, ItemStat itemStat)
        {
            StatData result = null;

            StatDataTextMatchResult statDataMatch = GetStatDataMatch(statDataListsToSearch, itemStat);

            if (statDataMatch == null)
            {
                this.logger.LogWarning("Failed to find matching stat data for {@itemStat}.", itemStat);
            }
            else
            {
                result = statDataMatch.StatData;
            }

            return result;
        }

        private static StatDataTextMatchResult GetStatDataMatch(IEnumerable<Data<StatData>> statDataListsToSearch, ItemStat itemStat)
        {
            var statDataTextMatcher = new StatDataTextMatcher(itemStat.Text);

            foreach (var statData in statDataListsToSearch.SelectMany(x => x.Entries))
            {
                StatDataTextMatchResult matchResult = statDataTextMatcher.Match(statData);

                if (matchResult.IsMatch)
                {
                    return matchResult;
                }
            }

            return null;
        }

        public StatData GetStatData(string itemStatId)
        {
            return !string.IsNullOrEmpty(itemStatId) && this.statsDataDictionary.TryGetValue(itemStatId, out StatData statData)
                ? statData
                : null;
        }

        private class StatDataTextMatcher
        {
            private readonly Regex regex;

            public StatDataTextMatcher(string statText)
            {
                this.regex = GetStatDataTextRegex(statText);
            }

            public StatDataTextMatchResult Match(StatData statData)
            {
                var match = this.regex.Match(statData.Text);

                return new StatDataTextMatchResult(statData, match.Success);
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
                const string monsterItemStatSuffix = @" \(×#\)";

                return new Regex($@"^[\+\-]?{regexString}({monsterItemStatSuffix})?$");
            }
        }

        private class StatDataTextMatchResult
        {
            public StatDataTextMatchResult(StatData statData, bool isMatch)
            {
                this.IsMatch = isMatch;
                StatData = statData;
            }

            public StatData StatData { get; }

            public bool IsMatch { get; }
        }
    }
}