using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.ItemSearch.Services.Factories
{
    public class ListingViewModelFactory : IListingViewModelFactory
    {
        private const string QualityPropertyName = "Quality";
        private const string GemLevelPropertyName = "Level";
        private const string GemExperiencePropertyName = "Experience";
        private readonly IPriceViewModelFactory priceViewModelFactory;

        public ListingViewModelFactory(IPriceViewModelFactory priceViewModelFactory)
        {
            this.priceViewModelFactory = priceViewModelFactory;
        }

        public async Task<SimpleListingViewModel> CreateAsync(ListingResult listingResult, Item item, CancellationToken cancellationToken = default)
        {
            SimpleListingViewModel result = item switch
            {
                GemItem => this.CreateGemItemListingViewModel(listingResult),
                EquippableItem or OrganItem => this.CreateItemListingViewModelWithItemLevel(listingResult),
                FlaskItem => this.CreateFlaskItemViewModel(listingResult),
                _ => new SimpleListingViewModel()
            };

            await this.SetSimpleListingViewModelProperties(result, listingResult, cancellationToken).ConfigureAwait(false);

            return result;
        }
        
        public async Task<SimpleListingViewModel> CreateAsync(ExchangeListing listing, CancellationToken cancellationToken = default)
        {
            ExchangeOffer offer = listing.Offers.FirstOrDefault();
            Price price = offer != null
                ? offer.Item with { Amount = offer.Item.Amount / offer.Exchange.Amount }
                : null;

            return new SimpleListingViewModel
            {
                AccountName = listing.Account?.Name,
                Age = listing.AgeText,
                Price = await this.priceViewModelFactory.CreateAsync(price, cancellationToken).ConfigureAwait(false),
            };
        }

        private async Task SetSimpleListingViewModelProperties(SimpleListingViewModel result, ListingResult listingResult, CancellationToken cancellationToken)
        {
            result.AccountName = listingResult.Listing.Account?.Name;
            result.Price = await this.priceViewModelFactory.CreateAsync(listingResult.Listing.Price, cancellationToken).ConfigureAwait(false);
            result.Age = listingResult.Listing.AgeText;
        }

        private SimpleListingViewModel CreateGemItemListingViewModel(ListingResult listingResult)
        {
            var result = new GemItemListingViewModel
            {
                Level = GetPropertyStringValue(listingResult.Item, GemLevelPropertyName),
                Quality = GetPropertyStringValue(listingResult.Item, QualityPropertyName)
            };

            var gemExperienceProperty = listingResult.Item.AdditionalProperties?.FirstOrDefault(p => p.Name == GemExperiencePropertyName);
            if (gemExperienceProperty != null)
            {
                result.GemExperiencePercent = Math.Round(gemExperienceProperty.Progress * 100, 2, MidpointRounding.AwayFromZero);
            }
            else
            {
                result.GemExperiencePercent = 0.00m; //otherwise these rows will show 0 instead of 0.00
            }

            return result;
        }

        private SimpleListingViewModel CreateFlaskItemViewModel(ListingResult listingResult)
        {
            return new FlaskItemListingViewModel
            {
                Quality = GetPropertyStringValue(listingResult.Item, QualityPropertyName)
            };
        }

        private static string GetPropertyStringValue(ItemListing itemListing, string propertyName)
        {
            var qualityProperty = itemListing.Properties?.FirstOrDefault(p => p.Name == propertyName);

            return qualityProperty?.Values[0][0].GetString();
        }

        private SimpleListingViewModel CreateItemListingViewModelWithItemLevel(ListingResult listingResult)
        {
            return new ItemListingViewModelWithItemLevel
            {
                ItemLevel = listingResult.Item.ItemLevel
            };
        }
    }
}