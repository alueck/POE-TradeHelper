namespace POETradeHelper.ItemSearch.Contract.Models
{
    public class FlaskItem : Item, IIdentifiableItem, IQualityItem
    {
        public FlaskItem(ItemRarity rarity) : base(rarity)
        {
        }

        public bool IsIdentified { get; set; }

        public int Quality { get; set; }

        public FlaskItemStats Stats { get; set; }
    }
}