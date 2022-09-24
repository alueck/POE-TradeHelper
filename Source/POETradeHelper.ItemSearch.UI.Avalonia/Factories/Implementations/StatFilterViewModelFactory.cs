using System;
using System.Linq;

using Microsoft.Extensions.Options;

using POETradeHelper.ItemSearch.Contract.Configuration;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Factories.Implementations
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
            var matchingFilter = FindMatchingFilter(itemStat, queryRequest);

            (decimal minValue, decimal maxValue)? minMaxTuple = GetMinMaxTuple(itemStat);

            return minMaxTuple.HasValue
                ? this.GetMinMaxStatFilterViewModel(itemStat, matchingFilter, minMaxTuple.Value)
                : GetStatFilterViewModel(itemStat, matchingFilter);
        }

        private static StatFilter? FindMatchingFilter(ItemStat itemStat, SearchQueryRequest queryRequest)
        {
            return queryRequest.Query.Stats.SelectMany(s => s.Filters).FirstOrDefault(filter => string.Equals(filter.Id, itemStat.Id, StringComparison.Ordinal));
        }

        private static (decimal minValue, decimal maxValue)? GetMinMaxTuple(ItemStat itemStat)
        {
            (decimal minValue, decimal maxValue)? minMaxTuple = null;

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

        private StatFilterViewModel GetMinMaxStatFilterViewModel(ItemStat itemStat, StatFilter? matchingFilter, (decimal minValue, decimal maxValue) minMaxTuple)
        {
            decimal minValuePercentageOffset = this.itemSearchOptions.CurrentValue.AdvancedQueryOptions.MinValuePercentageOffset;
            decimal maxValuePercentageOffset = this.itemSearchOptions.CurrentValue.AdvancedQueryOptions.MaxValuePercentageOffset;

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

        private static decimal? GetValueWithOffset(decimal? value, decimal offsetPercentage)
        {
            if (!value.HasValue)
            {
                return default;
            }

            var valueIsInteger = value % 1 == 0;
            var offsetValue = value.Value * (1 + offsetPercentage);

            return valueIsInteger
                ? Math.Truncate(offsetValue)
                : Math.Round(offsetValue, 2, MidpointRounding.AwayFromZero);
        }

        private static string GetCurrent(decimal? currentMinValue, decimal? currentMaxValue)
        {
            var current = $"{currentMinValue:0.##}";

            if (currentMaxValue.HasValue && currentMaxValue != currentMinValue)
            {
                current += $" - {currentMaxValue:0.##}";
            }

            return current;
        }

        private static StatFilterViewModel GetStatFilterViewModel(ItemStat itemStat, StatFilter? matchingFilter)
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
