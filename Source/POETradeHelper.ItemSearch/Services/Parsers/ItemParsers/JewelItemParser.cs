using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;

namespace POETradeHelper.ItemSearch.Services.Parsers.ItemParsers
{
    public class JewelItemParser : ItemWithStatsParserBase
    {
        private const int NameLineIndex = 2;
        private readonly IItemTypeParser itemTypeParser;

        public JewelItemParser(IItemTypeParser itemTypeParser, IItemStatsParser<ItemWithStats> itemStatsParser) : base(itemStatsParser)
        {
            this.itemTypeParser = itemTypeParser;
        }

        public override bool CanParse(string[] itemStringLines)
        {
            return itemStringLines.Skip(2).Take(2).Any(line => line.Contains(Resources.JewelKeyword));
        }

        protected override ItemWithStats ParseItemWithoutStats(string[] itemStringLines)
        {
            ItemRarity? rarity = this.GetRarity(itemStringLines);
            var jewelItem = new JewelItem(rarity!.Value)
            {
                Name = itemStringLines[NameLineIndex],
                IsIdentified = this.IsIdentified(itemStringLines),
                IsCorrupted = this.IsCorrupted(itemStringLines)
            };

            jewelItem.Type = this.itemTypeParser.ParseType(itemStringLines, jewelItem.Rarity, jewelItem.IsIdentified);

            return jewelItem;
        }
    }
}