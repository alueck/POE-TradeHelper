using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using System.Threading.Tasks;

namespace POETradeHelper.ItemSearch.Services.Factories
{
    public class ItemListingsViewModelFactory : IItemListingsViewModelFactory
    {
        private readonly IListingViewModelFactory listingViewModelFactory;

        public ItemListingsViewModelFactory(IListingViewModelFactory listingViewModelFactory)
        {
            this.listingViewModelFactory = listingViewModelFactory;
        }

        public async Task<ItemListingsViewModel> CreateAsync(Item item, ItemListingsQueryResult itemListingsQueryResult)
        {
            var result = new ItemListingsViewModel
            {
                ListingsUri = itemListingsQueryResult.Uri,
                ItemDescription = item.DisplayName,
                ItemRarity = item.Rarity
            };

            foreach (var listingResult in itemListingsQueryResult.Result)
            {
                SimpleListingViewModel itemListingViewModel = await this.listingViewModelFactory.CreateAsync(listingResult, item);
                result.Listings.Add(itemListingViewModel);
            }

            return result;
        }
    }
}