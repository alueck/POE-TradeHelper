namespace POETradeHelper.ItemSearch.Contract.Models
{
    public class MonsterItemStat : ItemStat
    {
        public MonsterItemStat() : base(StatCategory.Monster)
        {
        }

        public int Count { get; set; }
    }
}