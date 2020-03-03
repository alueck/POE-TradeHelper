using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;
using System.Linq;

namespace POETradeHelper.ItemSearch.Services.Parsers
{
    public class MapItemParser : ItemWithStatsParserBase
    {
        private const int NameLineIndex = 1;

        public MapItemParser(IItemStatsParser<ItemWithStats> itemStatsParser) : base(itemStatsParser)
        {
        }

        public override bool CanParse(string[] itemStringLines)
        {
            return itemStringLines.Any(l => l.Contains(Resources.MapTierDescriptor));
        }

        protected override ItemWithStats ParseItemWithoutStats(string[] itemStringLines)
        {
            ItemRarity? rarity = this.GetRarity(itemStringLines);
            var mapItem = new MapItem(rarity.Value)
            {
                IsIdentified = this.IsIdentified(itemStringLines),
                Tier = this.GetIntegerFromFirstStringContaining(itemStringLines, Resources.MapTierDescriptor),
                ItemQuantity = this.GetIntegerFromFirstStringContaining(itemStringLines, Resources.ItemQuantityDescriptor),
                ItemRarity = this.GetIntegerFromFirstStringContaining(itemStringLines, Resources.ItemRarityDescriptor),
                MonsterPackSize = this.GetIntegerFromFirstStringContaining(itemStringLines, Resources.MonsterPackSizeDescriptor),
                Quality = this.GetIntegerFromFirstStringContaining(itemStringLines, Resources.QualityDescriptor),
                IsCorrupted = this.IsCorrupted(itemStringLines)
            };

            SetNameAndType(mapItem, itemStringLines);

            mapItem.IsBlighted = mapItem.Name.Contains(Resources.BlightedPrefix);

            return mapItem;
        }

        private static void SetNameAndType(MapItem mapItem, string[] itemStringLines)
        {
            int typeLineIndex = HasName(mapItem) ? NameLineIndex + 1 : NameLineIndex;

            mapItem.Name = itemStringLines[NameLineIndex];
            mapItem.Type = GetTypeWithoutPrefixes(itemStringLines[typeLineIndex]);
        }

        private static bool HasName(MapItem mapItem)
        {
            return mapItem.IsIdentified && mapItem.Rarity != ItemRarity.Normal;
        }

        private static string GetTypeWithoutPrefixes(string type)
        {
            return type.Replace(Resources.SuperiorPrefix, "").Replace(Resources.BlightedPrefix, "").Trim();
        }
    }
}