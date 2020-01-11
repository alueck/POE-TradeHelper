namespace POETradeHelper.ItemSearch.Contract.Models
{
    public class GemItem : Item, ICorruptableItem
    {
        public GemItem()
        {
            base.Rarity = ItemRarity.Gem;
        }

        public override ItemRarity Rarity
        {
            get => base.Rarity;
            set { }
        }

        public int Quality { get; set; }

        public int Level { get; set; }

        public bool IsCorrupted { get; set; }
    }
}