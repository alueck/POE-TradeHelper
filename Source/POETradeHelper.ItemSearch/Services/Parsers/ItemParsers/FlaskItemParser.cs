using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;

namespace POETradeHelper.ItemSearch.Services.Parsers.ItemParsers
{
    public class FlaskItemParser : ItemWithStatsParserBase
    {
        private const int NameLineIndex = 2;
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
            ItemRarity? rarity = GetRarity(itemStringLines);
            var flaskItem = new FlaskItem(rarity!.Value)
            {
                Name = itemStringLines[NameLineIndex],
                Quality = GetIntegerFromFirstStringContaining(itemStringLines, Resources.QualityDescriptor),
                IsIdentified = this.IsIdentified(itemStringLines),
            };

            flaskItem.Type = this.itemTypeParser.ParseType(itemStringLines, flaskItem.Rarity, flaskItem.IsIdentified);

            return flaskItem;
        }

        private int GetTypeLineIndex(string[] itemStringLines)
        {
            return this.HasRarity(itemStringLines, ItemRarity.Unique)
                ? NameLineIndex + 1
                : NameLineIndex;
        }
    }
}