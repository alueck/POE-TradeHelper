using System.Collections.Generic;

namespace POETradeHelper.Common.Extensions;

public static class ListExtensions
{
    public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
    {
        foreach (T item in items)
        {
            list.Add(item);
        }
    }

    public static void AddIfNotNull<T>(this IList<T> list, T? item)
    {
        if (item != null)
        {
            list.Add(item);
        }
    }
}
