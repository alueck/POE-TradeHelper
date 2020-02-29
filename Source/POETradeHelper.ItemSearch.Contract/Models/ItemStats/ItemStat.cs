namespace POETradeHelper.ItemSearch.Contract.Models
{
    public abstract class ItemStat
    {
        public string Id { get; set; }

        public string Text { get; set; }

        public string TextWithPlaceholders { get; set; }

        public abstract StatCategory StatCategory { get; }
    }
}