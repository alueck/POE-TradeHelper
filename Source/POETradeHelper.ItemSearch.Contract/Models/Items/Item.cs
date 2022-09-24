namespace POETradeHelper.ItemSearch.Contract.Models
{
    public abstract class Item
    {
        protected Item(ItemRarity rarity)
        {
            this.Rarity = rarity;
        }

        public string ItemText { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public ItemRarity Rarity { get; }

        public string DisplayName
        {
            get
            {
                string displayName = this.Name;

                if (!string.IsNullOrEmpty(this.Type) && !displayName.Contains(this.Type))
                {
                    displayName += $" {this.Type}";
                }

                return displayName;
            }
        }
    }
}
