using POETradeHelper.Common.Contract;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Services;
using System.Threading.Tasks;

namespace POETradeHelper.ItemSearch.Services
{
    public class SearchItemProvider : ISearchItemProvider
    {
        private ICopyCommand copyCommand;
        private readonly IItemParserAggregator itemParserAggregator;

        public SearchItemProvider(ICopyCommand copyCommand, IItemParserAggregator itemParserAggregator)
        {
            this.copyCommand = copyCommand;
            this.itemParserAggregator = itemParserAggregator;
        }

        public async Task<Item> GetItemFromUnderCursorAsync()
        {
            Item item = null;
            string itemString = await this.copyCommand.ExecuteAsync();

            if (this.itemParserAggregator.CanParse(itemString))
            {
                item = this.itemParserAggregator.Parse(itemString);
            }

            return item;
        }
    }
}