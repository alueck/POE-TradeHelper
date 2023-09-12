﻿using POETradeHelper.Common.Extensions;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.ItemSearch.Services.Parsers.ItemParsers
{
    public class GemItemParser : ItemParserBase
    {
        private const int NameLineIndex = 2;
        private readonly IItemDataService itemDataService;

        public GemItemParser(IItemDataService itemDataService)
        {
            this.itemDataService = itemDataService;
        }

        public override bool CanParse(string[] itemStringLines) => this.HasRarity(itemStringLines, ItemRarity.Gem);

        protected override Item ParseItem(string[] itemStringLines)
        {
            string? vaalName = Array.FindLast(itemStringLines, l => l.Contains(Resources.VaalKeyword));
            string name = this.itemDataService.GetType(vaalName ?? itemStringLines[NameLineIndex]);

            GemItem gemItem = new GemItem
            {
                Name = name,
                Type = name,
                IsCorrupted = this.IsCorrupted(itemStringLines),
                Quality = GetIntegerFromFirstStringContaining(itemStringLines, Resources.QualityDescriptor),
                Level = GetIntegerFromFirstStringContaining(itemStringLines, Resources.LevelDescriptor),
                ExperiencePercent = GetExperiencePercent(itemStringLines),
                IsVaalVersion = !string.IsNullOrEmpty(vaalName),
                QualityType = GetQualityType(itemStringLines),
            };

            return gemItem;
        }

        private static int GetExperiencePercent(string[] itemStringLines)
        {
            int experiencePercent = 0;
            string? experienceLine = itemStringLines.FirstOrDefault(l => l.Contains(Resources.ExperienceDescriptor));

            if (experienceLine != null)
            {
                IEnumerable<decimal> experienceNumbers = experienceLine
                    .Replace(Resources.ExperienceDescriptor, string.Empty)
                    .Replace(".", string.Empty)
                    .Trim()
                    .Split('/')
                    .Select(decimal.Parse)
                    .ToArray();

                experiencePercent = GetIntegralPercent(experienceNumbers.First(), experienceNumbers.Last());
            }

            return experiencePercent;
        }

        private static int GetIntegralPercent(decimal currentOutOfTotal, decimal total)
        {
            decimal percent = currentOutOfTotal / total * 100;

            return (int)percent;
        }

        private static GemQualityType GetQualityType(string[] itemStringLines) =>
            Enum.GetValues<GemQualityType>()
                .FirstOrDefault(gemQualityType => itemStringLines.Any(l => l.StartsWith(gemQualityType.GetDisplayName())));
    }
}