using POETradeHelper.ItemSearch.Contract;

namespace POETradeHelper.ItemSearch
{
    public class ItemSearchOptions
    {
        public League League { get; set; }

        public int ItemLevelThreshold { get; set; } = 86;
    }
}