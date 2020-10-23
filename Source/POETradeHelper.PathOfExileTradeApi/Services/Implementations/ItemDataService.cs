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
            ItemData matchingItemData = this.Data
                .SelectMany(x => x.Entries)
                .Where(x => x.Type != null && name.Contains(x.Type))
                .OrderBy(x => x.Type.Length)
                .FirstOrDefault();

            return matchingItemData?.Type;
        }
    }
}