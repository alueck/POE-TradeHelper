using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace POETradeHelper.ItemSearch.Services.Factories
{
    public class StatFilterViewModelFactory : IStatFilterViewModelFactory
    {
        private const char Placeholder = '#';

        public StatFilterViewModel Create(ItemStat itemStat, SearchQueryRequest queryRequest, StatFilterViewModelFactoryConfiguration configuration)
        {
            StatFilterViewModel result;

            var matchingFilter = FindMatchingFilter(itemStat, queryRequest);

            (int? minValue, int? maxValue) minMaxTuple;

            if (itemStat is MonsterItemStat monsterItemStat)
            {
                configuration = new StatFilterViewModelFactoryConfiguration();
                minMaxTuple = GetItemStatMinAndMaxValue(monsterItemStat);
            }
            else
            {
                minMaxTuple = GetItemStatMinAndMaxValue(itemStat);
            }

            result = minMaxTuple.minValue.HasValue
                ? GetMinMaxStatFilterViewModel(itemStat, configuration, matchingFilter, minMaxTuple)
                : GetStatFilterViewModel(itemStat, matchingFilter);

            return result;
        }

        private static StatFilter FindMatchingFilter(ItemStat itemStat, SearchQueryRequest queryRequest)
        {
            return queryRequest.Query.Stats.SelectMany(s => s.Filters).FirstOrDefault(filter => string.Equals(filter.Id, itemStat.Id, StringComparison.Ordinal));
        }

        private (int? minValue, int? maxValue) GetItemStatMinAndMaxValue(MonsterItemStat monsterItemStat)
        {
            return (monsterItemStat.Count, null);
        }

        private (int? minValue, int? maxValue) GetItemStatMinAndMaxValue(ItemStat itemStat)
        {
            int? maxValue = null;
            int maxValueIndex = itemStat.TextWithPlaceholders.LastIndexOf(Placeholder);
            if (maxValueIndex >= 0)
            {
                maxValue = GetFirstNumericValue(itemStat.Text.Substring(maxValueIndex));
            }

            int? minValue = null;
            int minValueIndex = GetMinValueIndex(itemStat, maxValueIndex);
            if (minValueIndex >= 0)
            {
                minValue = GetFirstNumericValue(itemStat.Text.Substring(minValueIndex));
            }

            return (minValue, maxValue);
        }

        private static int? GetFirstNumericValue(string text)
        {
            Match match = Regex.Match(text, @"[\+\-]?\d+");

            return match.Success
                ? int.Parse(match.Value)
                : (int?)null;
        }

        private static int GetMinValueIndex(ItemStat itemStat, int maxValueIndex)
        {
            int minValueIndex = maxValueIndex;

            if (maxValueIndex >= 0)
            {
                minValueIndex = itemStat.TextWithPlaceholders.Substring(0, maxValueIndex).IndexOf(Placeholder);

                if (minValueIndex < 0)
                {
                    minValueIndex = maxValueIndex;
                }
            }

            return minValueIndex;
        }

        private static StatFilterViewModel GetMinMaxStatFilterViewModel(ItemStat itemStat, StatFilterViewModelFactoryConfiguration configuration, StatFilter matchingFilter, (int? minValue, int? maxValue) minMaxTuple)
        {
            return new MinMaxStatFilterViewModel
            {
                Id = itemStat.Id,
                IsEnabled = matchingFilter != null,
                Min = matchingFilter != null ? matchingFilter.Value.Min : GetValueWithOffset(minMaxTuple.minValue, configuration.MinValuePercentageOffset),
                Max = matchingFilter != null ? matchingFilter.Value.Max : GetValueWithOffset(minMaxTuple.maxValue, configuration.MaxValuePercentageOffset),
                Current = GetCurrent(minMaxTuple.minValue, minMaxTuple.maxValue),
                Text = itemStat.TextWithPlaceholders
            };
        }

        private static int? GetValueWithOffset(int? value, double offsetPercentage)
        {
            return value.HasValue
                ? (int?)(value * (1 + offsetPercentage))
                : null;
        }

        private static string GetCurrent(int? currentMinValue, int? currentMaxValue)
        {
            var current = $"{currentMinValue}";

            if (currentMaxValue.HasValue && currentMaxValue != currentMinValue)
            {
                current += $" - {currentMaxValue}";
            }

            return current;
        }

        private static StatFilterViewModel GetStatFilterViewModel(ItemStat itemStat, StatFilter matchingFilter)
        {
            return new StatFilterViewModel
            {
                Id = itemStat.Id,
                IsEnabled = matchingFilter != null,
                Text = itemStat.Text
            };
        }
    }
}