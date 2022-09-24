using MediatR;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;
using POETradeHelper.ItemSearch.Exceptions;

namespace POETradeHelper.ItemSearch.Queries.Handlers
{
    public class GetItemFromCursorQueryHandler : IRequestHandler<GetItemFromCursorQuery, Item>
    {
        private readonly IMediator mediator;
        private readonly IItemParserAggregator itemParserAggregator;

        public GetItemFromCursorQueryHandler(IMediator mediator, IItemParserAggregator itemParserAggregator)
        {
            this.mediator = mediator;
            this.itemParserAggregator = itemParserAggregator;
        }

        public async Task<Item> Handle(GetItemFromCursorQuery request, CancellationToken cancellationToken)
        {
            string itemString = await this.mediator.Send(new GetItemTextFromCursorQuery(), cancellationToken).ConfigureAwait(false);
            if (!this.itemParserAggregator.IsParseable(itemString))
            {
                throw new InvalidItemStringException(itemString);
            }

            Item item = this.itemParserAggregator.Parse(itemString);

            return item;
        }
    }
}