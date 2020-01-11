using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using System.Linq;
using System.Text.RegularExpressions;

namespace POETradeHelper.ItemSearch.Services
{
    public class GemItemParser : ItemParserBase
    {
        private const int NameLineIndex = 1;

        public override bool CanParse(string itemString)
        {
            return this.HasRarity(itemString, ItemRarity.Gem);
        }

        public override Item Parse(string itemString)
        {
            string[] lines = GetLines(itemString);

            return new GemItem
            {
                Name = lines[NameLineIndex],
                Type = lines[NameLineIndex],
                IsCorrupted = IsCorrupted(lines),
                Quality = GetQuality(lines),
                Level = GetLevel(lines)
            };
        }

        private bool IsCorrupted(string[] lines)
        {
            return lines.Any(l => l == Resources.CorruptedDescriptor);
        }

        private int GetQuality(string[] lines)
        {
            int quality = 0;
            var qualityLine = lines.FirstOrDefault(l => l.Contains(Resources.QualityDescriptor));

            if (qualityLine != null)
            {
                Match match = Regex.Match(qualityLine, @"\+(?<quality>\d+)%");
                quality = int.Parse(match.Groups["quality"].Value);
            }

            return quality;
        }

        private int GetLevel(string[] lines)
        {
            int level = 0;
            var levelLine = lines.FirstOrDefault(l => l.Contains(Resources.LevelDescriptor));

            if (levelLine != null)
            {
                level = int.Parse(levelLine.Replace(Resources.LevelDescriptor, "").Trim());
            }

            return level;
        }
    }
}