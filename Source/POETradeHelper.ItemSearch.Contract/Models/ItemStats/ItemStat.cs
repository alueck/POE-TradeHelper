using System.Linq;

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
            this.Tier = itemStat.Tier;
        }

        public string Id { get; set; } = string.Empty;

        public string Text
        {
            get;
            init
            {
                field = value;
                this.Lines = field.Count(c => c == '\n') + 1;
            }
        } = string.Empty;

        public string TextWithPlaceholders { get; set; } = string.Empty;

        public StatCategory StatCategory { get; }

        public int? Tier { get; init; }

        public int Lines { get; private set; }
    }
}