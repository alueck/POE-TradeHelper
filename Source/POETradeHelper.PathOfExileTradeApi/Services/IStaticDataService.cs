using POETradeHelper.ItemSearch.Contract.Models;
using System;

namespace POETradeHelper.PathOfExileTradeApi.Services
{
    public interface IStaticDataService
    {
        /// <summary>
        /// Returns the id for the given <paramref name="item"/>.
        /// Throws <see cref="NotSupportedException"/> if <paramref name="item"/>
        /// is not <see cref="CurrencyItem"/>, <see cref="DivinationCardItem"/> or <see cref="FragmentItem"/>.
        /// </summary>
        /// <param name="item">an item of type <see cref="CurrencyItem"/>, <see cref="DivinationCardItem"/> or <see cref="FragmentItem"/> for which to retrieve the id</param>
        string GetId(Item item);

        Uri GetImageUrl(string id);

        string GetText(string id);
    }
}