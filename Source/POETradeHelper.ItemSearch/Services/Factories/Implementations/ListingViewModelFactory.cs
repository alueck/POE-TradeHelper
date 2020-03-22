using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<SimpleListingViewModel> CreateAsync(ListingResult listingResult, Item item)
        {
            SimpleListingViewModel result = new SimpleListingViewModel();

            if (item is GemItem)
            {
                result = this.CreateGemItemListingViewModel(listingResult);
            }
            else if (item is EquippableItem || item is OrganItem)
            {
                result = this.CreateItemListingViewModelWithItemLevel(listingResult);
            }
            else if (item is FlaskItem)
            {
                result = this.CreateFlaskItemViewModel(listingResult);
            }

            await this.SetSimpleListingViewModelProperties(result, listingResult);

            return result;
        }

        private async Task SetSimpleListingViewModelProperties(SimpleListingViewModel result, ListingResult listingResult)
        {
            result.AccountName = listingResult.Listing.Account?.Name;
            result.Price = await this.priceViewModelFactory.CreateAsync(listingResult.Listing.Price);
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