using System;

namespace POETradeHelper.ItemSearch.Contract.Models
{
    public class MapItem : ItemWithStats, IIdentifiableItem, ICorruptableItem, IQualityItem
    {
        public MapItem(ItemRarity rarity) : base(rarity)
        {
            if (rarity is Models.ItemRarity.Normal or Models.ItemRarity.Magic or Models.ItemRarity.Rare
                or Models.ItemRarity.Unique)
            {
                return;
            }

            throw new ArgumentException($"Rarity of {nameof(MapItem)} cannot be {rarity}.");
        }

        public bool IsCorrupted { get; set; }

        public bool IsIdentified { get; set; }

        public int Tier { get; set; }

        public int ItemQuantity { get; set; }

        public int ItemRarity { get; set; }

        public int MonsterPackSize { get; set; }

        public int Quality { get; set; }

        public bool IsBlighted { get; set; }

        public bool IsBlightRavaged { get; set; }
    }
}