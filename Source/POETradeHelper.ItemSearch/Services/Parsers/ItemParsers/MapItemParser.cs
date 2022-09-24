using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;

namespace POETradeHelper.ItemSearch.Services.Parsers.ItemParsers
{
    public class MapItemParser : ItemWithStatsParserBase
    {
        private const int NameLineIndex = 2;
        private readonly IItemTypeParser itemTypeParser;

        public MapItemParser(IItemTypeParser itemTypeParser, IItemStatsParser<ItemWithStats> itemStatsParser) : base(itemStatsParser)
        {
            this.itemTypeParser = itemTypeParser;
        }

        public override bool CanParse(string[] itemStringLines)
        {
            return itemStringLines.Any(l => l.Contains(Resources.MapTierDescriptor));
        }

        protected override ItemWithStats ParseItemWithoutStats(string[] itemStringLines)
        {
            ItemRarity? rarity = GetRarity(itemStringLines);
            var mapItem = new MapItem(rarity!.Value)
            {
                Name = itemStringLines[NameLineIndex],
                IsIdentified = this.IsIdentified(itemStringLines),
                Tier = GetIntegerFromFirstStringContaining(itemStringLines, Resources.MapTierDescriptor),
                ItemQuantity = GetIntegerFromFirstStringContaining(itemStringLines, Resources.ItemQuantityDescriptor),
                ItemRarity = GetIntegerFromFirstStringContaining(itemStringLines, Resources.ItemRarityDescriptor),
                MonsterPackSize = GetIntegerFromFirstStringContaining(itemStringLines, Resources.MonsterPackSizeDescriptor),
                Quality = GetIntegerFromFirstStringContaining(itemStringLines, Resources.QualityDescriptor),
                IsCorrupted = this.IsCorrupted(itemStringLines)
            };

            mapItem.Type = this.itemTypeParser.ParseType(itemStringLines, mapItem.Rarity, mapItem.IsIdentified);
            mapItem.IsBlighted = mapItem.Name.Contains(Resources.BlightedPrefix);
            mapItem.IsBlightRavaged = mapItem.Name.Contains(Resources.BlightRavagedPrefix);

            return mapItem;
        }
    }
}
