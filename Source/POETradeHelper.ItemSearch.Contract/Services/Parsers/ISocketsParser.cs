using POETradeHelper.ItemSearch.Contract.Models;

namespace POETradeHelper.ItemSearch.Contract.Services.Parsers
{
    public interface ISocketsParser
    {
        ItemSockets Parse(string socketsString);
    }
}