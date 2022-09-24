using MediatR;

using POETradeHelper.ItemSearch.Contract.Models;

namespace POETradeHelper.ItemSearch.Queries
{
    public class GetItemFromCursorQuery : IRequest<Item>
    {
    }
}