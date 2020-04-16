namespace POETradeHelper.ItemSearch.Contract.Configuration
{
    public class ItemSearchOptions
    {
        public League League { get; set; }

        public int ItemLevelThreshold { get; set; } = 86;

        public AdvancedQueryOptions AdvancedQueryOptions { get; set; } = new AdvancedQueryOptions();
    }
}