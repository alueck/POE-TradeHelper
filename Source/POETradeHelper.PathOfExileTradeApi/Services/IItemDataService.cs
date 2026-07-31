using POETradeHelper.Common.Contract;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.PathOfExileTradeApi.Services
{
    public interface IItemDataService : IInitializable
    {
        ItemType? GetType(string name);

        string? GetCategory(string type);
    }
}
