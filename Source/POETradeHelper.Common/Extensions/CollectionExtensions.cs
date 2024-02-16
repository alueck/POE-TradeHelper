using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace POETradeHelper.Common.Extensions;

[ExcludeFromCodeCoverage]
public static class CollectionExtensions
{
    public static void Add<T>(this ICollection<T> collection, IEnumerable<T> items)
    {
        foreach (T item in items)
        {
            collection.Add(item);
        }
    }
}