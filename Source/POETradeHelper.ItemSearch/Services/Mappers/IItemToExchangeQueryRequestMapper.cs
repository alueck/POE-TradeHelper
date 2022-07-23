using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.ItemSearch.Services.Mappers;

public interface IItemToExchangeQueryRequestMapper
{
    ExchangeQueryRequest MapToQueryRequest(Item item);
}