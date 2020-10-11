using System;
using System.Collections.Generic;
using System.Linq;
using POETradeHelper.Common.Extensions;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.ItemSearch.Services.Parsers
{
    public class GemItemParser : ItemParserBase
    {
        private const int NameLineIndex = 1;
        private readonly IItemDataService itemDataService;

        public GemItemParser(IItemDataService itemDataService)
        {
            this.itemDataService = itemDataService;
        }

        public override bool CanParse(string[] itemStringLines)
        {
            return this.HasRarity(itemStringLines, ItemRarity.Gem);
        }

        protected override Item ParseItem(string[] itemStringLines)
        {
            var gemItem = new GemItem
            {
                IsCorrupted = this.IsCorrupted(itemStringLines),
                Quality = this.GetIntegerFromFirstStringContaining(itemStringLines, Resources.QualityDescriptor),
                Level = this.GetIntegerFromFirstStringContaining(itemStringLines, Resources.LevelDescriptor),
                ExperiencePercent = GetExperiencePercent(itemStringLines),
                IsVaalVersion = IsVaalVersion(itemStringLines),
                QualityType = GetQualityType(itemStringLines)
            };

            gemItem.Name = gemItem.Type = this.itemDataService.GetType(itemStringLines[NameLineIndex]);

            if (gemItem.IsVaalVersion)
            {
                gemItem.Name = gemItem.Type = GetVaalVersionName(itemStringLines[NameLineIndex]);
            }

            return gemItem;
        }

        private static int GetExperiencePercent(string[] itemStringLines)
        {
            int experiencePercent = 0;
            string experienceLine = itemStringLines.FirstOrDefault(l => l.Contains(Resources.ExperienceDescriptor));

            if (experienceLine != null)
            {
                IEnumerable<decimal> experienceNumbers = experienceLine
                    .Replace(Resources.ExperienceDescriptor, "")
                    .Replace(".", "")
                    .Trim()
                    .Split('/')
                    .Select(s => decimal.Parse(s));

                experiencePercent = GetIntegralPercent(experienceNumbers.First(), experienceNumbers.Last());
            }

            return experiencePercent;
        }

        private static int GetIntegralPercent(decimal currentOutOfTotal, decimal total)
        {
            decimal percent = (currentOutOfTotal / total) * 100;

            return (int)percent;
        }

        private bool IsVaalVersion(string[] lines)
        {
            return IsCorrupted(lines) && lines.Any(l => l.Contains(Resources.VaalKeyword));
        }

        private static GemQualityType GetQualityType(string[] itemStringLines)
        {
            GemQualityType result = default;

            foreach (GemQualityType gemQualityType in Enum.GetValues(typeof(GemQualityType)))
            {
                if (itemStringLines.Any(l => l.StartsWith(gemQualityType.GetDisplayName())))
                {
                    result = gemQualityType;
                    break;
                }
            }

            return result;
        }

        private static string GetVaalVersionName(string gemName)
        {
            return $"{Resources.VaalKeyword} {gemName}";
        }
    }
}