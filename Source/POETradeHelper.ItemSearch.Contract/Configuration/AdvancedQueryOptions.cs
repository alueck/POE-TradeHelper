namespace POETradeHelper.ItemSearch.Contract.Configuration
{
    public class AdvancedQueryOptions
    {
        public double MinValuePercentageOffset { get; set; } = -0.1;

        public double MaxValuePercentageOffset { get; set; } = 0.1;
    }
}