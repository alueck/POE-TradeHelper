using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.QualityOfLife.Models;

namespace POETradeHelper.QualityOfLife.Services.Impl
{
    public class PoeDbUrlProvider : IWikiUrlProvider
    {
        public WikiType HandledWikiType => WikiType.PoeDb;

        public Uri GetUrl(Item item) =>
            new($"https://poedb.tw/us/{(item.Rarity == ItemRarity.Unique ? item.Name : item.Type).Replace(" ", "_").Replace("'", string.Empty)}");
    }
}