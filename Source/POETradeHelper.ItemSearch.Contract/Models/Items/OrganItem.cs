namespace POETradeHelper.ItemSearch.Contract.Models
{
    public class OrganItem : Item
    {
        public OrganItem() : base(ItemRarity.Unique)
        {
        }

        public OrganItemStats Stats { get; set; }
    }
}