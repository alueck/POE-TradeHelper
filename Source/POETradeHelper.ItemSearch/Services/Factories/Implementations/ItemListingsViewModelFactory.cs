using System.Threading;
using System.Threading.Tasks;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.ItemSearch.Services.Factories
{
    public class ItemListingsViewModelFactory : IItemListingsViewModelFactory
    {
        private readonly IListingViewModelFactory listingViewModelFactory;

        public ItemListingsViewModelFactory(IListingViewModelFactory listingViewModelFactory)
        {
            this.listingViewModelFactory = listingViewModelFactory;
        }

        public async Task<ItemListingsViewModel> CreateAsync(Item item, ItemListingsQueryResult itemListingsQueryResult, CancellationToken cancellationToken = default)
        {
            var result = new ItemListingsViewModel
            {
                ListingsUri = itemListingsQueryResult.Uri,
                ItemDescription = item.DisplayName,
                ItemRarity = item.Rarity
            };

            foreach (var listingResult in itemListingsQueryResult.Result)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                SimpleListingViewModel itemListingViewModel = await this.listingViewModelFactory.CreateAsync(listingResult, item, cancellationToken);
                result.Listings.Add(itemListingViewModel);
            }

            return result;
        }
    }
}