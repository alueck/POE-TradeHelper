using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Services;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;
using System.Threading.Tasks;

namespace POETradeHelper.ItemSearch.Services
{
    public class SearchItemProvider : ISearchItemProvider
    {
        private readonly ICopyCommand copyCommand;
        private readonly IItemParserAggregator itemParserAggregator;

        public SearchItemProvider(ICopyCommand copyCommand, IItemParserAggregator itemParserAggregator)
        {
            this.copyCommand = copyCommand;
            this.itemParserAggregator = itemParserAggregator;
        }

        public async Task<Item> GetItemFromUnderCursorAsync(System.Threading.CancellationToken cancellationToken = default)
        {
            Item item = null;
            string itemString = await this.copyCommand.ExecuteAsync(cancellationToken);

            if (!cancellationToken.IsCancellationRequested && this.itemParserAggregator.IsParseable(itemString))
            {
                item = this.itemParserAggregator.Parse(itemString);
            }

            return item;
        }
    }
}