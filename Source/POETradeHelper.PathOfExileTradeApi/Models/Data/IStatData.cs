namespace POETradeHelper.PathOfExileTradeApi.Models;

public interface IStatData
{
    string Id { get; }

    string Text { get; }

    string Type { get; }

    int Lines { get; }
}