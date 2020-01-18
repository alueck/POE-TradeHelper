using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using System.Linq;

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
                IsVaalVersion = IsVaalVersion(itemStringLines)
            };

            if (gemItem.IsVaalVersion)
            {
                gemItem.Name = gemItem.Type = $"{Resources.VaalKeyword} {gemItem.Name}";
            }

            return gemItem;
        }

        private bool IsVaalVersion(string[] lines)
        {
            return IsCorrupted(lines) && lines.Any(l => l.Contains(Resources.VaalKeyword));
        }
    }
}