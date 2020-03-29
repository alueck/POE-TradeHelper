﻿using DynamicData;
using POETradeHelper.Common.Extensions;
using POETradeHelper.ItemSearch.Contract;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace POETradeHelper.ItemSearch.Services.Parsers
{
    public class ItemStatsParser : ItemStatsParserBase, IItemStatsParser<ItemWithStats>
    {
        private const char Placeholder = '#';

        public ItemStatsParser(IStatsDataService statsDataService) : base(statsDataService)
        {
        }

        public ItemStats Parse(string[] itemStringLines)
        {
            var result = new ItemStats();

            int statsStartIndex = GetStatsStartIndex(itemStringLines);

            var statTexts = itemStringLines.Skip(statsStartIndex).Where(s => s != ParserConstants.PropertyGroupSeparator);

            var itemStats = statTexts.Select(ParseStatText).Where(x => x != null).ToList();

            result.AllStats.AddRange(itemStats);

            return result;
        }

        private ItemStat ParseStatText(string statText)
        {
            ItemStat result;

            if (!TryGetItemStatForCategoryByMarker(statText, StatCategory.Enchant, out result)
                && !TryGetItemStatForCategoryByMarker(statText, StatCategory.Implicit, out result)
                && !TryGetItemStatForCategoryByMarker(statText, StatCategory.Crafted, out result))
            {
                result = new ItemStat(StatCategory.Unknown)
                {
                    Text = statText,
                };
            }

            result = this.GetCompleteItemStat(result);

            return result;
        }

        private bool TryGetItemStatForCategoryByMarker(string statText, StatCategory statCategory, out ItemStat itemStat)
        {
            itemStat = null;

            int statCategoryMarkerIndex = statText.IndexOf($"({statCategory.GetDisplayName()})", StringComparison.OrdinalIgnoreCase);
            if (statCategoryMarkerIndex >= 0)
            {
                statText = statText.Substring(0, statCategoryMarkerIndex).Trim();

                itemStat = new ItemStat(statCategory)
                {
                    Text = statText
                };

                return true;
            }

            return false;
        }

        protected override ItemStat GetCompleteItemStat(ItemStat itemStat)
        {
            ItemStat result = base.GetCompleteItemStat(itemStat);

            var placeholderCount = result?.TextWithPlaceholders?.Count(c => c == Placeholder);
            if (placeholderCount == 1)
            {
                result = GetSingleValueItemStat(itemStat);
            }
            else if (placeholderCount == 2)
            {
                result = GetMinMaxValueItemStat(itemStat);
            }

            return result;
        }

        private static ItemStat GetSingleValueItemStat(ItemStat itemStat)
        {
            int? value = GetFirstNumericValue(itemStat.Text.Substring(itemStat.TextWithPlaceholders.IndexOf(Placeholder)));

            return value.HasValue
                ? new SingleValueItemStat(itemStat)
                {
                    Value = value.Value
                }
                : itemStat;
        }

        private static ItemStat GetMinMaxValueItemStat(ItemStat itemStat)
        {
            var result = new MinMaxValueItemStat(itemStat);

            int maxValueIndex = itemStat.TextWithPlaceholders.LastIndexOf(Placeholder);
            if (maxValueIndex >= 0)
            {
                result.MaxValue = GetFirstNumericValue(itemStat.Text.Substring(maxValueIndex)).GetValueOrDefault();
            }

            int minValueIndex = GetMinValueIndex(itemStat, maxValueIndex);
            if (minValueIndex >= 0)
            {
                result.MinValue = GetFirstNumericValue(itemStat.Text.Substring(minValueIndex)).GetValueOrDefault();
            }

            return result;
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
    }
}