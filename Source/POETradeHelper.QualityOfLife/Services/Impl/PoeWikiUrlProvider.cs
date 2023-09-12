using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.QualityOfLife.Models;

namespace POETradeHelper.QualityOfLife.Services.Impl
{
    public class PoeWikiUrlProvider : IWikiUrlProvider
    {
        public WikiType HandledWikiType => WikiType.PoeWiki;

        public Uri GetUrl(Item item) =>
            new($"https://pathofexile.gamepedia.com/{(item.Rarity == ItemRarity.Unique ? item.Name : item.Type)}");
    }
}