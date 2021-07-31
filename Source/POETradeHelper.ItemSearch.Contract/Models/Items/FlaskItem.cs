using System;

namespace POETradeHelper.ItemSearch.Contract.Models
{
    public class FlaskItem : ItemWithStats, IIdentifiableItem, IQualityItem
    {
        public FlaskItem(ItemRarity rarity) : base(rarity)
        {
            if (rarity is ItemRarity.Normal or ItemRarity.Magic or ItemRarity.Rare or ItemRarity.Unique)
            {
                return;
            }

            throw new ArgumentException($"Rarity of {nameof(FlaskItem)} cannot be {rarity}.");
        }

        public bool IsIdentified { get; set; }

        public int Quality { get; set; }
    }
}