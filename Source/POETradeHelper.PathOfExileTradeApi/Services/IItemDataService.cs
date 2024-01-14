using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.PathOfExileTradeApi.Services
{
    public interface IItemDataService
    {
        ItemType? GetType(string name);

        string? GetCategory(string type);
    }
}
