using System;

namespace POETradeHelper.ItemSearch.Contract.Models
{
    public class JewelItem : ItemWithStats, IIdentifiableItem, ICorruptableItem
    {
        public JewelItem(ItemRarity rarity) : base(rarity)
        {
            if (rarity is ItemRarity.Normal or ItemRarity.Magic or ItemRarity.Rare or ItemRarity.Unique)
            {
                return;
            }

            throw new ArgumentException($"Rarity of {nameof(JewelItem)} cannot be {rarity}.");
        }

        public bool IsIdentified { get; set; }

        public bool IsCorrupted { get; set; }
    }
}