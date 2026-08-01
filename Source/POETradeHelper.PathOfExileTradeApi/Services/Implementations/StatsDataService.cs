using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using POETradeHelper.Common.Wrappers;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Properties;
using POETradeHelper.RePoE.Services;

namespace POETradeHelper.PathOfExileTradeApi.Services.Implementations
{
    internal partial class StatsDataService : DataServiceBase<Data<StatData>>, IStatsDataService
    {
        private const string ExplicitStatsId = "explicit";
        private const string PseudoStatsId = "pseudo";

        private readonly IAlternativeStatTextsService alternativeStatTextsService;
        private readonly ILogger<StatsDataService> logger;

        private Dictionary<string, StatData> statsDataDictionary = [];

        public StatsDataService(
            IHttpClientFactoryWrapper httpclientFactory,
            IPoeTradeApiJsonSerializer poeTradeApiJsonSerializer,
            IAlternativeStatTextsService alternativeStatTextsService,
            ILogger<StatsDataService> logger)
            : base(Resources.PoeTradeApiStatsDataEndpoint, httpclientFactory, poeTradeApiJsonSerializer)
        {
            this.alternativeStatTextsService = alternativeStatTextsService;
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

            await this.FetchAlternativeStatTexts(this.statsDataDictionary);
        }

        public IStatData? TryGetStatData(IReadOnlyCollection<string> itemStatLines, bool preferLocalStat, params string[] statCategoriesToSearch)
        {
            List<string> toSearch = [];
            if (statCategoriesToSearch.Length == 0)
            {
                toSearch.Add(this.GetStatCategory(itemStatLines.First()));
            }
            else if (statCategoriesToSearch.Length > 0)
            {
                toSearch.AddRange(statCategoriesToSearch);
            }

            IEnumerable<Data<StatData>> statDataListsToSearch = this.GetStatDataListsToSearch(toSearch);
            StatData? result = GetStatDataMatch(statDataListsToSearch, itemStatLines, preferLocalStat);

            if (result == null && toSearch.Count == 1 && string.Equals(toSearch[0], ExplicitStatsId, StringComparison.OrdinalIgnoreCase) && this.Data.Count > 1)
            {
                return this.TryGetStatData(
                    itemStatLines,
                    preferLocalStat,
                    this.Data
                        .Select(x => x.Id)
                        .Where(category => !string.Equals(category, ExplicitStatsId, StringComparison.OrdinalIgnoreCase)
                                           && !string.Equals(category, PseudoStatsId, StringComparison.OrdinalIgnoreCase))
                        .ToArray());
            }

            return result;
        }

        public IStatData? GetStatDataById(string itemStatId)
        {
            return !string.IsNullOrEmpty(itemStatId) && this.statsDataDictionary.TryGetValue(itemStatId, out StatData? statData)
                ? statData
                : null;
        }

        private async Task FetchAlternativeStatTexts(Dictionary<string, StatData> statDataDictionary)
        {
            try
            {
                await foreach (var group in this.alternativeStatTextsService.GetAlternativeStatTexts())
                {
                    if (!statDataDictionary.TryGetValue(group.Id, out StatData? statData))
                    {
                        continue;
                    }

                    foreach (var statText in group.StatTexts.Where(x => !string.Equals(x, statData.Text, StringComparison.OrdinalIgnoreCase)))
                    {
                        statData.Alternatives.Add(new StatData
                        {
                            Id = statData.Id,
                            Text = statText,
                            Type = statData.Type,
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger.LogWarning(ex, "Failed to get alternative stat text data");
            }
        }

        private string GetStatCategory(string itemStatText)
        {
            var match = GetStatCategoryRegex().Match(itemStatText);

            if (match.Success && this.Data.Any(x => string.Equals(x.Id, match.Groups["StatCategory"].Value, StringComparison.OrdinalIgnoreCase)))
            {
                return match.Groups["StatCategory"].Value;
            }

            return ExplicitStatsId;
        }

        private IEnumerable<Data<StatData>> GetStatDataListsToSearch(params ICollection<string> statCategoriesToSearch)
        {
            IEnumerable<Data<StatData>>? result = null;

            if (statCategoriesToSearch.Count != 0)
            {
                result = this.Data.Where(x =>
                    statCategoriesToSearch.Any(statCategory => string.Equals(x.Id, statCategory, StringComparison.OrdinalIgnoreCase)));
            }

            return result ?? this.Data;
        }

        [GeneratedRegex(@"\((?<StatCategory>[^\)]+)\)$")]
        private static partial Regex GetStatCategoryRegex();

        private static StatData? GetStatDataMatch(
            IEnumerable<Data<StatData>> statDataListsToSearch,
            IReadOnlyCollection<string> itemStatLines,
            bool preferLocalStat)
        {
            StatData? result = null;
            string joinedText = string.Join('\n', itemStatLines);

            foreach (var statData in statDataListsToSearch.SelectMany(x => x.Entries))
            {
                if (IsMatch(statData))
                {
                    if ((preferLocalStat && statData.IsLocal) || (!preferLocalStat && !statData.IsLocal))
                    {
                        result = statData;
                        break;
                    }

                    result = statData;
                }
            }

            return result;

            bool IsMatch(StatData statData)
            {
                var match = statData.Lines > 1
                    ? statData.Regex.Match(joinedText)
                    : statData.Regex.Match(itemStatLines.First());

                return match.Success || statData.Alternatives.Any(IsMatch);
            }
        }
    }
}