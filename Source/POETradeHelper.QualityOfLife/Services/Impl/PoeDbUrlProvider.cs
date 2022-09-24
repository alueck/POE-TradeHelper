using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.QualityOfLife.Models;

namespace POETradeHelper.QualityOfLife.Services.Impl
{
    public class PoeDbUrlProvider : IWikiUrlProvider
    {
        public WikiType HandledWikiType => WikiType.PoeDb;

        public Uri GetUrl(Item item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return new Uri($"https://poedb.tw/us/{(item.Rarity == ItemRarity.Unique ? item.Name : item.Type).Replace(" ", "_").Replace("'", "")}");
        }
    }
}