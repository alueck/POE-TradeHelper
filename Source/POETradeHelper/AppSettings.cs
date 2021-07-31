using POETradeHelper.ItemSearch.Contract.Configuration;
using POETradeHelper.QualityOfLife.Models;

namespace POETradeHelper
{
    public class AppSettings
    {
        public ItemSearchOptions ItemSearchOptions { get; set; } = new ItemSearchOptions();

        public WikiOptions WikiOptions { get; set; } = new WikiOptions();
    }
}