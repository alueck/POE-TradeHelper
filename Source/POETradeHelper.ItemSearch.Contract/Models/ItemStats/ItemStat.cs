namespace POETradeHelper.ItemSearch.Contract.Models
{
    public class ItemStat
    {
        public ItemStat(StatCategory statCategory)
        {
            this.StatCategory = statCategory;
        }

        public ItemStat(ItemStat itemStat)
        {
            this.Id = itemStat.Id;
            this.Text = itemStat.Text;
            this.TextWithPlaceholders = itemStat.TextWithPlaceholders;
            this.StatCategory = itemStat.StatCategory;
        }

        public string Id { get; set; }

        public string Text { get; set; }

        public string TextWithPlaceholders { get; set; }

        public StatCategory StatCategory { get; set; }
    }
}