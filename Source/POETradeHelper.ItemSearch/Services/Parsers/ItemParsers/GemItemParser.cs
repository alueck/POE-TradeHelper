using System.Collections.Generic;
using System.Linq;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;

namespace POETradeHelper.ItemSearch.Services.Parsers
{
    public class GemItemParser : ItemParserBase
    {
        private const int NameLineIndex = 1;

        public override bool CanParse(string[] itemStringLines)
        {
            return this.HasRarity(itemStringLines, ItemRarity.Gem);
        }

        public override Item Parse(string[] itemStringLines)
        {
            var gemItem = new GemItem
            {
                Name = itemStringLines[NameLineIndex],
                Type = itemStringLines[NameLineIndex],
                IsCorrupted = this.IsCorrupted(itemStringLines),
                Quality = this.GetIntegerFromFirstStringContaining(itemStringLines, Resources.QualityDescriptor),
                Level = this.GetIntegerFromFirstStringContaining(itemStringLines, Resources.LevelDescriptor),
                ExperiencePercent = GetExperiencePercent(itemStringLines),
                IsVaalVersion = IsVaalVersion(itemStringLines)
            };

            if (gemItem.IsVaalVersion)
            {
                gemItem.Name = gemItem.Type = $"{Resources.VaalKeyword} {gemItem.Name}";
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
    }
}