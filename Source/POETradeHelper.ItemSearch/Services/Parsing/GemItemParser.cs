using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using System.Linq;
using System.Text.RegularExpressions;

namespace POETradeHelper.ItemSearch.Services
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
                IsCorrupted = IsCorrupted(itemStringLines),
                Quality = GetQuality(itemStringLines),
                Level = GetLevel(itemStringLines),
                IsVaalVersion = IsVaalVersion(itemStringLines)
            };

            if (gemItem.IsVaalVersion)
            {
                gemItem.Name = $"{Resources.VaalDescriptor} {gemItem.Name}";
                gemItem.Type = $"{Resources.VaalDescriptor} {gemItem.Type}";
            }

            return gemItem;
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

        private bool IsVaalVersion(string[] lines)
        {
            return IsCorrupted(lines) && lines.Any(l => l.Contains(Resources.VaalDescriptor));
        }
    }
}