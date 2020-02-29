using System;
using System.Collections.Generic;

namespace POETradeHelper.Common.Extensions
{
    public static class IListExtensions
    {
        public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if (items != null)
            {
                foreach (var item in items)
                {
                    list.Add(item);
                }
            }
        }
    }
}