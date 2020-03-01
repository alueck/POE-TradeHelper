using POETradeHelper.ItemSearch.Contract.Models.ItemStats;

namespace POETradeHelper.ItemSearch.Contract.Models
{
    public class MapItem : Item, IIdentifiableItem, ICorruptableItem, IQualityItem
    {
        public MapItem(ItemRarity rarity) : base(rarity)
        {
        }

        public bool IsCorrupted { get; set; }
        public bool IsIdentified { get; set; }
        public int Tier { get; set; }
        public int ItemQuantity { get; set; }
        public int ItemRarity { get; set; }
        public int MonsterPackSize { get; set; }
        public int Quality { get; set; }
        public bool IsBlighted { get; set; }
        public MapItemStats Stats { get; set; }
    }
}