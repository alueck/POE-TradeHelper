﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using POETradeHelper.Common.Wrappers;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Properties;
using POETradeHelper.PathOfExileTradeApi.Services.Implementations;

namespace POETradeHelper.PathOfExileTradeApi.Services
{
    public class StaticDataService : DataServiceBase<StaticData>, IStaticDataService
    {
        private IDictionary<string, StaticData> idToStaticDataMappings;

        public StaticDataService(IHttpClientFactoryWrapper httpClientFactory, IPoeTradeApiJsonSerializer poeTradeApiJsonSerializer)
            : base(Resources.PoeTradeApiStaticDataEndpoint, httpClientFactory, poeTradeApiJsonSerializer)
        {
        }

        public string GetId(Item item)
        {
            if (item is CurrencyItem || item is DivinationCardItem || item is FragmentItem)
            {
                return this.idToStaticDataMappings.Values.FirstOrDefault(entry => string.Equals(entry.Text, item.Name, StringComparison.Ordinal))?.Id;
            }

            throw new NotSupportedException($"Item of type '{item?.GetType()}' is not supported.");
        }

        public Uri GetImageUrl(string id)
        {
            return new Uri(Resources.PoeCdnUrl + this.idToStaticDataMappings[id].Image);
        }

        public string GetText(string id)
        {
            return this.idToStaticDataMappings[id].Text;
        }

        public override async Task OnInitAsync()
        {
            await base.OnInitAsync();

            this.idToStaticDataMappings = this.Data
                                                .Where(x => !x.Id.StartsWith("Map"))
                                                .SelectMany(x => x.Entries)
                                                .ToDictionary(x => x.Id);
        }
    }
}