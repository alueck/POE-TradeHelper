namespace POETradeHelper.ItemSearch.Contract.Models
{
    public class JewelItem : ItemWithStats, IIdentifiableItem, ICorruptableItem
    {
        public JewelItem(ItemRarity rarity) : base(rarity)
        {
        }

        public bool IsIdentified { get; set; }

        public bool IsCorrupted { get; set; }
    }
}