using System;
using System.Linq;
using POETradeHelper.ItemSearch.Contract.Extensions;
using POETradeHelper.ItemSearch.Contract.Properties;

namespace POETradeHelper.ItemSearch.Contract.Models
{
    public abstract class Item
    {
        protected Item(ItemRarity rarity)
        {
            this.Rarity = rarity;
        }

        public string ExtendedItemText { get; set; } = string.Empty;

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

        public string PlainItemText
        {
            get
            {
                string text = string.Join(Environment.NewLine, this.ExtendedItemText
                        .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                        .Where(x => !x.StartsWith('{') && !x.StartsWith('(')))
                    .RemoveStatRanges()
                    .Replace(Resources.UnscalableValueSuffix, string.Empty);

                return text;
            }
        }
    }
}