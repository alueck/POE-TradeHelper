﻿namespace POETradeHelper.ItemSearch.Contract.Models
{
    public abstract class Item
    {
        protected Item(ItemRarity rarity)
        {
            this.Rarity = rarity;
        }

        public string ItemText { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

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