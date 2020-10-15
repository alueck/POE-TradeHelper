using System.Text.RegularExpressions;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;

namespace POETradeHelper.ItemSearch.Services.Parsers
{
    public class FlaskItemParser : ItemWithStatsParserBase
    {
        private const int NameLineIndex = 1;
        private readonly IItemTypeParser itemTypeParser;

        public FlaskItemParser(IItemTypeParser itemTypeParser, IItemStatsParser<ItemWithStats> itemStatsParser) :
            base(itemStatsParser)
        {
            this.itemTypeParser = itemTypeParser;
        }

        public override bool CanParse(string[] itemStringLines)
        {
            int typeLineIndex = this.GetTypeLineIndex(itemStringLines);
            return itemStringLines[typeLineIndex].Contains(Resources.FlaskKeyword);
        }

        protected override ItemWithStats ParseItemWithoutStats(string[] itemStringLines)
        {
            ItemRarity? rarity = this.GetRarity(itemStringLines);
            var flaskItem = new FlaskItem(rarity.Value)
            {
                Name = itemStringLines[NameLineIndex],
                Quality = this.GetIntegerFromFirstStringContaining(itemStringLines, Resources.QualityDescriptor),
                IsIdentified = this.IsIdentified(itemStringLines),
            };

            flaskItem.Type = this.itemTypeParser.ParseType(itemStringLines, flaskItem.Rarity, flaskItem.IsIdentified);

            return flaskItem;
        }

        private string GetFlaskType(string[] itemStringLines)
        {
            int typeLineIndex = this.GetTypeLineIndex(itemStringLines);

            string type = itemStringLines[typeLineIndex];

            Match match;
            if (type.Contains(Resources.LifeFlaskKeyword) || type.Contains(Resources.ManaFlaskKeyword))
            {
                match = Regex.Match(type, $@"\w+\s{{1}}\w+\s{{1}}{Resources.FlaskKeyword}");
            }
            else
            {
                match = Regex.Match(type, $@"\w+\s{{1}}{Resources.FlaskKeyword}");
            }

            return match.Value;
        }

        private int GetTypeLineIndex(string[] itemStringLines)
        {
            return this.HasRarity(itemStringLines, ItemRarity.Unique)
                ? NameLineIndex + 1
                : NameLineIndex;
        }
    }
}