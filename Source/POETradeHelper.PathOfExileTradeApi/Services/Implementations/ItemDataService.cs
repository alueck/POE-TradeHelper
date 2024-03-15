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
        public ItemDataService(
            IHttpClientFactoryWrapper httpClientFactory,
            IPoeTradeApiJsonSerializer poeTradeApiJsonSerializer)
            : base(Resources.PoeTradeApiItemDataEndpoint, httpClientFactory, poeTradeApiJsonSerializer)
        {
        }

        public ItemType? GetType(string name)
        {
            List<ItemData> matches = new();

            foreach (ItemData? entry in this.Data.SelectMany(x => x.Entries))
            {
                if (entry.Type == name || entry.Text == name)
                {
                    return new ItemType(entry.Type, entry.Disc);
                }

                if (name.Contains(entry.Type))
                {
                    matches.Add(entry);
                }
            }

            ItemData? itemData = matches.MaxBy(x => x.Type.Length);

            return itemData != null
                ? new ItemType(itemData.Type, itemData.Disc)
                : null;
        }

        public string? GetCategory(string type) =>
            this.Data
                .FirstOrDefault(x =>
                    x.Entries.Any(itemEntry => string.Equals(itemEntry.Type, type, StringComparison.Ordinal)))
                ?.Id;
    }
}