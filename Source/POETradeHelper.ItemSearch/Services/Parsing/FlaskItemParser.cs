using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using System.Text.RegularExpressions;

namespace POETradeHelper.ItemSearch.Services.Parsing
{
    public class FlaskItemParser : ItemParserBase
    {
        private const int NameLineIndex = 1;

        public override bool CanParse(string[] itemStringLines)
        {
            return itemStringLines[1].Contains(Resources.LifeFlaskDescriptor);
        }

        public override Item Parse(string[] itemStringLines)
        {
            ItemRarity rarity = this.GetRarity(itemStringLines);
            return new FlaskItem(rarity)
            {
                Name = itemStringLines[NameLineIndex],
                Type = this.GetFlaskTypeFromName(itemStringLines[NameLineIndex]),
                Quality = this.GetIntegerFromFirstStringContaining(itemStringLines, Resources.QualityDescriptor),
                IsIdentified = this.IsIdentified(itemStringLines)
            };
        }

        private string GetFlaskTypeFromName(string name)
        {
            Match match;
            if (name.Contains(Resources.LifeFlaskDescriptor) || name.Contains(Resources.ManaFlaskDescriptor))
            {
                match = Regex.Match(name, $@"\w+\s{{1}}\w+\s{{1}}{Resources.FlaskDescriptor}");
            }
            else
            {
                match = Regex.Match(name, $@"\w+\s{{1}}{Resources.FlaskDescriptor}");
            }

            return match.Value;
        }
    }
}