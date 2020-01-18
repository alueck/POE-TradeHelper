namespace POETradeHelper.ItemSearch.Contract.Models
{
    public abstract class Item
    {
        protected Item(ItemRarity rarity)
        {
            this.Rarity = rarity;
        }

        public string Name { get; set; }

        public string Type { get; set; }

        public ItemRarity Rarity { get; }
    }
}