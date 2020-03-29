namespace POETradeHelper.ItemSearch.Contract.Models
{
    public class MinMaxValueItemStat : ItemStat
    {
        public MinMaxValueItemStat(StatCategory statCategory) : base(statCategory)
        {
        }

        public MinMaxValueItemStat(ItemStat itemStat) : base(itemStat)
        {
            if (itemStat is MinMaxValueItemStat minMaxValueItemStat)
            {
                this.MinValue = minMaxValueItemStat.MinValue;
                this.MaxValue = minMaxValueItemStat.MaxValue;
            }
        }

        public int MinValue { get; set; }

        public int MaxValue { get; set; }
    }
}