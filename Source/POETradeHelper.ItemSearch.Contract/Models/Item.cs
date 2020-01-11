namespace POETradeHelper.ItemSearch.Contract.Models
{
    public abstract class Item
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public virtual ItemRarity Rarity { get; set; }
    }
}