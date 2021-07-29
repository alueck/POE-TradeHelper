namespace POETradeHelper.ItemSearch.Contract.Models
{
    public class SingleValueItemStat : ItemStat
    {
        public SingleValueItemStat(StatCategory statCategory) : base(statCategory)
        {
        }

        public SingleValueItemStat(ItemStat itemStat) : base(itemStat)
        {
            if (itemStat is SingleValueItemStat singleValueItemStat)
            {
                this.Value = singleValueItemStat.Value;
            }
        }

        public decimal Value { get; set; }
    }
}