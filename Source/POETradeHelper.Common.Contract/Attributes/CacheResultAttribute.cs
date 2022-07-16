using System;

namespace POETradeHelper.Common.Contract.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class CacheResultAttribute : Attribute
{
    public int DurationSeconds { get; set; }
}
