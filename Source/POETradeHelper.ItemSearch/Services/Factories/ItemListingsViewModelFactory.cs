using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using System.Linq;
using DynamicData;

namespace POETradeHelper.ItemSearch.Services.Factories
{
    public class ItemListingsViewModelFactory : IItemListingsViewModelFactory
    {
        private readonly IListingViewModelFactory listingViewModelFactory;

        public ItemListingsViewModelFactory(IListingViewModelFactory listingViewModelFactory)
        {
            this.listingViewModelFactory = listingViewModelFactory;
        }

        public ItemListingsViewModel Create(ItemListingsQueryResult itemListingsQueryResult)
        {
            var result = new ItemListingsViewModel
            {
                ListingsUri = itemListingsQueryResult.Uri,
                ItemDescription = itemListingsQueryResult.Item.DisplayName,
                ItemRarity = itemListingsQueryResult.Item.Rarity
            };

            result.Listings.AddRange(itemListingsQueryResult.Result.Select(x => this.listingViewModelFactory.Create(x, itemListingsQueryResult.Item)));

            return result;
        }
    }
}