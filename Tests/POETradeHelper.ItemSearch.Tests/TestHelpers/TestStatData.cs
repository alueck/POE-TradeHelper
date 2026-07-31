using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.ItemSearch.Tests.TestHelpers;

public record TestStatData : IStatData
{
    public string Id { get; set; } = string.Empty;

    public string Text
    {
        get;
        set
        {
            field = value;
            this.Lines = field.Count(x => x == '\n') + 1;
        }
    } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public int Lines { get; private set; } = 1;
}