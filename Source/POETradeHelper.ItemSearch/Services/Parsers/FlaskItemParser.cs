using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using System.Text.RegularExpressions;

namespace POETradeHelper.ItemSearch.Services.Parsers
{
    public class FlaskItemParser : ItemParserBase
    {
        private const int NameLineIndex = 1;

        public override bool CanParse(string[] itemStringLines)
        {
            return itemStringLines[1].Contains(Resources.FlaskKeyword);
        }

        public override Item Parse(string[] itemStringLines)
        {
            ItemRarity? rarity = this.GetRarity(itemStringLines);
            return new FlaskItem(rarity.Value)
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
            if (name.Contains(Resources.LifeFlaskKeyword) || name.Contains(Resources.ManaFlaskKeyword))
            {
                match = Regex.Match(name, $@"\w+\s{{1}}\w+\s{{1}}{Resources.FlaskKeyword}");
            }
            else
            {
                match = Regex.Match(name, $@"\w+\s{{1}}{Resources.FlaskKeyword}");
            }

            return match.Value;
        }
    }
}