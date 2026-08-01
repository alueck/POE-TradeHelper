using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;

namespace POETradeHelper.ItemSearch.Services.Parsers.ItemParsers
{
    public class MapItemParser : ItemWithStatsParserBase
    {
        private const int NameLineIndex = 2;

        public MapItemParser(IItemStatsParser<ItemWithStats> itemStatsParser)
            : base(itemStatsParser)
        {
        }

        public override bool CanParse(string[] itemStringLines) => itemStringLines.Any(l => l.Contains(Resources.MapTierDescriptor));

        protected override ItemWithStats ParseItemWithoutStats(string[] itemStringLines)
        {
            ItemRarity? rarity = GetRarity(itemStringLines);
            var mapTierLine = itemStringLines.First(l => l.Contains(Resources.MapTierDescriptor));

            MapItem mapItem = new(rarity!.Value)
            {
                Name = itemStringLines[NameLineIndex],
                IsIdentified = this.IsIdentified(itemStringLines),
                Tier = GetIntegerFromFirstStringContaining(itemStringLines, Resources.MapTierDescriptor),
                ItemQuantity = GetIntegerFromFirstStringContaining(itemStringLines, Resources.ItemQuantityDescriptor),
                ItemRarity = GetIntegerFromFirstStringContaining(itemStringLines, Resources.ItemRarityDescriptor),
                MonsterPackSize = GetIntegerFromFirstStringContaining(itemStringLines, Resources.MonsterPackSizeDescriptor),
                Quality = GetIntegerFromFirstStringContaining(itemStringLines, Resources.QualityDescriptor),
                IsCorrupted = this.IsCorrupted(itemStringLines),
                IsBlighted = mapTierLine.Contains(Resources.BlightedPrefix),
                IsBlightRavaged = mapTierLine.Contains(Resources.BlightRavagedPrefix),
            };

            return mapItem;
        }
    }
}