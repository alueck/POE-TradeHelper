using System;
using System.Collections.Generic;
using System.Linq;

using POETradeHelper.Common.Wrappers;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Properties;

namespace POETradeHelper.PathOfExileTradeApi.Services.Implementations
{
    public class ItemDataService : DataServiceBase<Data<ItemData>>, IItemDataService
    {
        public ItemDataService(IHttpClientFactoryWrapper httpClientFactory, IPoeTradeApiJsonSerializer poeTradeApiJsonSerializer)
            : base(Resources.PoeTradeApiItemDataEndpoint, httpClientFactory, poeTradeApiJsonSerializer)
        {
        }

        public string GetType(string name)
        {
            List<ItemData> matches = new();

            foreach (var entry in this.Data.SelectMany(x => x.Entries).Where(x => x.Type != null))
            {
                if (entry.Type == name)
                    return entry.Type;

                if (name.Contains(entry.Type!))
                    matches.Add(entry);
            }

            return matches
                .Select(x => x.Type!)
                .MaxBy(x => x.Length)
                ?? string.Empty;
        }

        public string? GetCategory(string type)
        {
            return this.Data
                        .FirstOrDefault(x => x.Entries.Any(itemEntry => string.Equals(itemEntry.Type, type, StringComparison.Ordinal)))
                        ?.Id;
        }
    }
}
