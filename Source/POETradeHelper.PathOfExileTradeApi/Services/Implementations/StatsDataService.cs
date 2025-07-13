using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using POETradeHelper.Common.Wrappers;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Properties;

namespace POETradeHelper.PathOfExileTradeApi.Services.Implementations
{
    public class StatsDataService : DataServiceBase<Data<StatData>>, IStatsDataService
    {
        private const string Placeholder = "#";

        private IDictionary<string, StatData> statsDataDictionary = new Dictionary<string, StatData>();

        private static readonly Regex NumberRegex = new(@"[\+\-]?\d+(\.\d+)?", RegexOptions.Compiled);
        private readonly ILogger<StatsDataService> logger;

        public StatsDataService(
            IHttpClientFactoryWrapper httpclientFactory,
            IPoeTradeApiJsonSerializer poeTradeApiJsonSerializer,
            ILogger<StatsDataService> logger)
            : base(Resources.PoeTradeApiStatsDataEndpoint, httpclientFactory, poeTradeApiJsonSerializer)
        {
            this.logger = logger;
        }

        public override async Task OnInitAsync()
        {
            await base.OnInitAsync();

            // This is a bit hacky, but it seems that if there are multiple entries with the same ID
            // the last one is the relevant one.
            this.statsDataDictionary = this.Data
                .SelectMany(x => x.Entries)
                .GroupBy(statData => statData.Id)
                .ToDictionary(group => group.Key, group => group.Last());
        }

        public StatData? GetStatData(string itemStatText, bool preferLocalStat, params string[] statCategoriesToSearch)
        {
            IEnumerable<Data<StatData>> statDataListsToSearch = this.GetStatDataListsToSearch(statCategoriesToSearch);
            StatData? result = this.GetStataDataPrivate(statDataListsToSearch, itemStatText, preferLocalStat);

            return result;
        }

        public StatData? GetStatDataById(string itemStatId)
        {
            return !string.IsNullOrEmpty(itemStatId) && this.statsDataDictionary.TryGetValue(itemStatId, out StatData? statData)
                ? statData
                : null;
        }

        private IEnumerable<Data<StatData>> GetStatDataListsToSearch(params string[] statCategoriesToSearch)
        {
            IEnumerable<Data<StatData>>? result = null;

            if (statCategoriesToSearch.Length != 0)
            {
                result = this.Data.Where(x =>
                    statCategoriesToSearch.Any(statCategory => string.Equals(x.Id, statCategory, StringComparison.OrdinalIgnoreCase)));
            }

            return result ?? this.Data;
        }

        private StatData? GetStataDataPrivate(IEnumerable<Data<StatData>> statDataListsToSearch, string itemStatText, bool preferLocalStat)
        {
            StatData? result = null;

            StatDataTextMatchResult? statDataMatch = GetStatDataMatch(statDataListsToSearch, itemStatText, preferLocalStat);

            if (statDataMatch == null)
            {
                this.logger.LogWarning("Failed to find matching stat data for {@itemStatText}.", itemStatText);
            }
            else
            {
                result = statDataMatch.StatData;
            }

            return result;
        }

        private static StatDataTextMatchResult? GetStatDataMatch(IEnumerable<Data<StatData>> statDataListsToSearch, string itemStatText, bool preferLocalStat)
        {
            StatDataTextMatchResult? result = null;
            var statDataTextMatcher = new StatDataTextMatcher(itemStatText);

            foreach (var statData in statDataListsToSearch.SelectMany(x => x.Entries))
            {
                StatDataTextMatchResult matchResult = statDataTextMatcher.Match(statData);

                if (matchResult.IsMatch)
                {
                    if ((preferLocalStat && matchResult.IsLocalStat) || (!preferLocalStat && !matchResult.IsLocalStat))
                    {
                        result = matchResult;
                        break;
                    }

                    result = matchResult;
                }
            }

            return result;
        }

        private sealed class StatDataTextMatcher
        {
            private const string LocalStatMatchGroupName = "localStat";
            private readonly Regex regex;

            public StatDataTextMatcher(string statText)
            {
                this.regex = GetStatDataTextRegex(statText);
            }

            public StatDataTextMatchResult Match(StatData statData)
            {
                var match = this.regex.Match(statData.Text);

                return new StatDataTextMatchResult(statData, match.Success, match.Groups[LocalStatMatchGroupName].Success);
            }

            /// <summary>
            /// Replaces all numbers in the given <paramref name="statText"/> with a regex capture group to build a regex pattern for matching
            /// the correct stat data text and returns a regex created from this pattern.
            /// </summary>
            /// <example>
            /// 60% chance for Poisons inflicted with this Weapon to deal 100% more Damage
            /// becomes
            /// (60|#)% chance for Poisons inflicted with this Weapon to deal (100|#)% more Damage
            /// .
            /// </example>
            private static Regex GetStatDataTextRegex(string statText)
            {
                string regexString = NumberRegex.Replace(statText, match => @$"({Regex.Escape(match.Value)}|[\+\-]?{Regex.Escape(Placeholder)})");
                const string monsterItemStatSuffix = @" \(×#\)";
                string localSuffix = $@" \({Resources.LocalKeyword}\)";

                return new Regex($@"^({regexString}({monsterItemStatSuffix}|(?<{LocalStatMatchGroupName}>{localSuffix}))?)$");
            }
        }

        private sealed class StatDataTextMatchResult
        {
            public StatDataTextMatchResult(StatData statData, bool isMatch, bool isLocalStat)
            {
                this.StatData = statData;
                this.IsMatch = isMatch;
                this.IsLocalStat = isLocalStat;
            }

            public StatData StatData { get; }

            public bool IsMatch { get; }

            public bool IsLocalStat { get; }
        }
    }
}