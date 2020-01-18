namespace POETradeHelper.ItemSearch.Contract.Models
{
    public class GemItem : Item, ICorruptableItem
    {
        public GemItem() : base(ItemRarity.Gem)
        {
        }

        public int Quality { get; set; }

        public int Level { get; set; }

        public bool IsCorrupted { get; set; }

        public bool IsVaalVersion { get; set; }
    }
}