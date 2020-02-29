namespace POETradeHelper.ItemSearch.Contract.Models
{
    public class MonsterItemStat : ItemStat
    {
        public int Count { get; set; }

        public override StatCategory StatCategory => StatCategory.Monster;
    }
}