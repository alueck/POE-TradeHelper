using Microsoft.Extensions.Options;
using POETradeHelper.ItemSearch.Contract.Configuration;
using POETradeHelper.ItemSearch.Contract;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;
using System;
using System.Linq;

namespace POETradeHelper.ItemSearch.Services.Factories
{
    public class StatFilterViewModelFactory : IStatFilterViewModelFactory
    {
        private readonly IOptionsMonitor<ItemSearchOptions> itemSearchOptions;

        public StatFilterViewModelFactory(IOptionsMonitor<ItemSearchOptions> itemSearchOptions)
        {
            this.itemSearchOptions = itemSearchOptions;
        }

        public StatFilterViewModel Create(ItemStat itemStat, SearchQueryRequest queryRequest)
        {
            StatFilterViewModel result;

            var matchingFilter = FindMatchingFilter(itemStat, queryRequest);

            (int minValue, int maxValue)? minMaxTuple = GetMinMaxTuple(itemStat);

            result = minMaxTuple.HasValue
                ? GetMinMaxStatFilterViewModel(itemStat, matchingFilter, minMaxTuple.Value)
                : GetStatFilterViewModel(itemStat, matchingFilter);

            return result;
        }

        private static StatFilter FindMatchingFilter(ItemStat itemStat, SearchQueryRequest queryRequest)
        {
            return queryRequest.Query.Stats.SelectMany(s => s.Filters).FirstOrDefault(filter => string.Equals(filter.Id, itemStat.Id, StringComparison.Ordinal));
        }

        private static (int minValue, int maxValue)? GetMinMaxTuple(ItemStat itemStat)
        {
            (int minValue, int maxValue)? minMaxTuple = null;

            if (itemStat is SingleValueItemStat singleValueItemStat)
            {
                minMaxTuple = (singleValueItemStat.Value, singleValueItemStat.Value);
            }
            else if (itemStat is MinMaxValueItemStat minMaxValueItemStat)
            {
                minMaxTuple = (minMaxValueItemStat.MinValue, minMaxValueItemStat.MaxValue);
            }

            return minMaxTuple;
        }

        private StatFilterViewModel GetMinMaxStatFilterViewModel(ItemStat itemStat, StatFilter matchingFilter, (int minValue, int maxValue) minMaxTuple)
        {
            double minValuePercentageOffset = this.itemSearchOptions.CurrentValue.AdvancedQueryOptions.MinValuePercentageOffset;
            double maxValuePercentageOffset = this.itemSearchOptions.CurrentValue.AdvancedQueryOptions.MaxValuePercentageOffset;

            if (itemStat.StatCategory == StatCategory.Monster)
            {
                minValuePercentageOffset = 0;
                maxValuePercentageOffset = 0;
            }

            return new MinMaxStatFilterViewModel
            {
                Id = itemStat.Id,
                IsEnabled = matchingFilter != null,
                Min = matchingFilter != null ? matchingFilter.Value.Min : GetValueWithOffset(minMaxTuple.minValue, minValuePercentageOffset),
                Max = matchingFilter != null ? matchingFilter.Value.Max : GetValueWithOffset(minMaxTuple.maxValue, maxValuePercentageOffset),
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