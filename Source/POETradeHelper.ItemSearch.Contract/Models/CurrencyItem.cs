namespace POETradeHelper.ItemSearch.Contract.Models
{
    public class CurrencyItem : Item
    {
        public CurrencyItem()
        {
            base.Rarity = ItemRarity.Currency;
        }

        public override ItemRarity Rarity
        {
            get => base.Rarity;
            set { }
        }
    }
}