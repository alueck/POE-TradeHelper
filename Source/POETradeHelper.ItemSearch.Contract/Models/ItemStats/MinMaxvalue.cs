namespace POETradeHelper.ItemSearch.Contract.Models;

public sealed record MinMaxValue
{
    public required int Min { get; init; }

    public required int Max { get; init; }

    public decimal Average => (this.Min + this.Max) / 2m;
}