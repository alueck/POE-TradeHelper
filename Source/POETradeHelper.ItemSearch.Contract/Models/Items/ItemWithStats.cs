namespace POETradeHelper.ItemSearch.Contract.Models
{
    public abstract class ItemWithStats : Item
    {
        protected ItemWithStats(ItemRarity rarity) : base(rarity)
        {
        }

        public ItemStats? Stats { get; set; }
    }
}
