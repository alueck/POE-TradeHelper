namespace POETradeHelper.ItemSearch.Contract.Models
{
    public class ItemStat
    {
        public ItemStat(StatCategory statCategory = StatCategory.Unknown)
        {
            this.StatCategory = statCategory;
        }

        public string Id { get; set; }

        public string Text { get; set; }

        public string TextWithPlaceholders { get; set; }

        public StatCategory StatCategory { get; set; }
    }
}