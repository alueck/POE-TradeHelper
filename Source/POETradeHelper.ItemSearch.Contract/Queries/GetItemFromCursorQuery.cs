using MediatR;
using POETradeHelper.ItemSearch.Contract.Models;

namespace POETradeHelper.ItemSearch.Contract.Queries
{
    public class GetItemFromCursorQuery : IRequest<Item>
    {
    }
}