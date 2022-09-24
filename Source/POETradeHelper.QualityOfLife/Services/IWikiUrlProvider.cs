using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.QualityOfLife.Models;

namespace POETradeHelper.QualityOfLife.Services
{
    public interface IWikiUrlProvider
    {
        WikiType HandledWikiType { get; }

        Uri GetUrl(Item item);
    }
}