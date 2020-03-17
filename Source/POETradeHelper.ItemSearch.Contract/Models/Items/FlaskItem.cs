namespace POETradeHelper.ItemSearch.Contract.Models
{
    public class FlaskItem : ItemWithStats, IIdentifiableItem, IQualityItem
    {
        public FlaskItem(ItemRarity rarity) : base(rarity)
        {
        }

        public bool IsIdentified { get; set; }

        public int Quality { get; set; }
    }
}