using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Factories.Implementations
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
            var result = new ItemListingsViewModel { ListingsUri = itemListingsQueryResult.Uri };

            foreach (var listingResult in itemListingsQueryResult.Result)
            {
                cancellationToken.ThrowIfCancellationRequested();

                SimpleListingViewModel itemListingViewModel = await this.listingViewModelFactory.CreateAsync(listingResult, item, cancellationToken);
                result.Listings.Add(itemListingViewModel);
            }

            return result;
        }

        public async Task<ItemListingsViewModel> CreateAsync(ExchangeQueryResult exchangeQueryResult, CancellationToken cancellationToken = default)
        {
            var result = new ItemListingsViewModel { ListingsUri = exchangeQueryResult.Uri };

            foreach (var listingResult in exchangeQueryResult.Result.Values.Select(x => x.Listing))
            {
                cancellationToken.ThrowIfCancellationRequested();

                SimpleListingViewModel itemListingViewModel = await this.listingViewModelFactory.CreateAsync(listingResult, cancellationToken);
                result.Listings.Add(itemListingViewModel);
            }

            return result;
        }
    }
}