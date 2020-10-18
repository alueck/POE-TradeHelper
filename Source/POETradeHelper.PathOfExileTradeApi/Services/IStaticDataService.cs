using System;

namespace POETradeHelper.PathOfExileTradeApi.Services
{
    public interface IStaticDataService
    {
        /// <summary>
        /// Returns the id for the given <paramref name="itemName"/> or null if no match was found.
        /// </summary>
        /// <param name="itemName">name of the item for which to retrieve the id</param>
        string GetId(string itemName);

        Uri GetImageUrl(string id);

        string GetText(string id);
    }
}