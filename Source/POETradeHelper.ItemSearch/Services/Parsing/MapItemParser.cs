using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using System.Linq;

namespace POETradeHelper.ItemSearch.Services.Parsing
{
    public class MapItemParser : ItemParserBase
    {
        public override bool CanParse(string[] itemStringLines)
        {
            return itemStringLines.Any(l => l.Contains(Resources.MapTierDescriptor));
        }

        public override Item Parse(string[] itemStringLines)
        {
            ItemRarity rarity = this.GetRarity(itemStringLines);
            var mapItem = new MapItem(rarity)
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

            mapItem.IsBlighted = mapItem.Type.Contains(Resources.BlightedPrefix);
            mapItem.Type = GetTypeWithoutPrefixes(mapItem.Type);

            return mapItem;
        }

        private static void SetNameAndType(MapItem mapItem, string[] itemStringLines)
        {
            if (mapItem.IsIdentified && mapItem.Rarity != ItemRarity.Normal)
            {
                mapItem.Name = itemStringLines[1];
                mapItem.Type = itemStringLines[2];
            }
            else
            {
                mapItem.Type = mapItem.Name = itemStringLines[1];
            }
        }

        private static string GetTypeWithoutPrefixes(string type)
        {
            return type.Replace(Resources.SuperiorPrefix, "").Replace(Resources.BlightedPrefix, "").Trim();
        }
    }
}