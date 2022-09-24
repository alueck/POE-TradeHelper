namespace POETradeHelper.PathOfExileTradeApi.Services
{
    public interface IItemDataService
    {
        string? GetType(string name);

        string? GetCategory(string type);
    }
}