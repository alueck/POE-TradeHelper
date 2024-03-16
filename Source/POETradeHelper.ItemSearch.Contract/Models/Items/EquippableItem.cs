using System;

namespace POETradeHelper.ItemSearch.Contract.Models
{
    public class EquippableItem : ItemWithStats, ICorruptableItem, IIdentifiableItem, IQualityItem, ISynthesisableItem
    {
        public EquippableItem(ItemRarity rarity) : base(rarity)
        {
            if (rarity is ItemRarity.Normal or ItemRarity.Magic or ItemRarity.Rare or ItemRarity.Unique)
            {
                return;
            }

            throw new ArgumentException($"Rarity of {nameof(EquippableItem)} cannot be {rarity}.");
        }

        public int Quality { get; set; }

        public int ItemLevel { get; set; }

        public InfluenceType Influence { get; set; }

        public ItemSockets? Sockets { get; set; }

        public bool IsCorrupted { get; set; }

        public bool IsIdentified { get; set; }

        public bool IsSynthesised { get; set; }

        public EquippableItemCategory Category { get; set; }

        public ArmourValues? ArmourValues { get; set; }

        public WeaponValues? WeaponValues { get; set; }
    }
}