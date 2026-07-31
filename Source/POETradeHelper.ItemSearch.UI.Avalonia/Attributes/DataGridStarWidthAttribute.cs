using System;
using System.Diagnostics.CodeAnalysis;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Attributes;

[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Property)]
public sealed class DataGridStarWidthAttribute : Attribute
{
    public DataGridStarWidthAttribute(double value)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
        this.Value = value;
    }

    public double Value { get; }
}