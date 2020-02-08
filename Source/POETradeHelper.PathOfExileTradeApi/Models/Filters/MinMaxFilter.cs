namespace POETradeHelper.PathOfExileTradeApi.Models.Filters
{
    public class MinMaxFilter : IFilter
    {
        public int? Min { get; set; }
        public int? Max { get; set; }
    }
}