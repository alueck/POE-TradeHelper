namespace POETradeHelper.PathOfExileTradeApi.Models.Filters
{
    public class MinMaxFilter : IFilter
    {
        public decimal? Min { get; set; }
        public decimal? Max { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}