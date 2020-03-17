using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using System.Linq;
using DynamicData;
using POETradeHelper.ItemSearch.Contract.Models;

namespace POETradeHelper.ItemSearch.Services.Factories
{
    public class ItemListingsViewModelFactory : IItemListingsViewModelFactory
    {
        private readonly IListingViewModelFactory listingViewModelFactory;

        public ItemListingsViewModelFactory(IListingViewModelFactory listingViewModelFactory)
        {
            this.listingViewModelFactory = listingViewModelFactory;
        }

        public ItemListingsViewModel Create(Item item, ItemListingsQueryResult itemListingsQueryResult)
        {
            var result = new ItemListingsViewModel
            {
                ListingsUri = itemListingsQueryResult.Uri,
                ItemDescription = item.DisplayName,
                ItemRarity = item.Rarity
            };

            result.Listings.AddRange(itemListingsQueryResult.Result.Select(x => this.listingViewModelFactory.Create(x, item)));

            return result;
        }
    }
}