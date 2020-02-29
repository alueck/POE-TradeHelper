namespace POETradeHelper.ItemSearch.Contract.Models
{
    public class EquippableItem : Item, ICorruptableItem, IIdentifiableItem, IQualityItem
    {
        public EquippableItem(ItemRarity rarity) : base(rarity)
        {
        }

        public int Quality { get; set; }
        public int ItemLevel { get; set; }
        public InfluenceType Influence { get; set; }
        public ItemSockets Sockets { get; set; }
        public bool IsCorrupted { get; set; }
        public bool IsIdentified { get; set; }
    }
}