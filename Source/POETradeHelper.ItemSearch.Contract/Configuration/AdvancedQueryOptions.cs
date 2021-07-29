namespace POETradeHelper.ItemSearch.Contract.Configuration
{
    public class AdvancedQueryOptions
    {
        public decimal MinValuePercentageOffset { get; set; } = -0.1m;

        public decimal MaxValuePercentageOffset { get; set; } = 0.1m;
    }
}