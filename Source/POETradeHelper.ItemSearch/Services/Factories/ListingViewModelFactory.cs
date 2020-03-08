using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using System;
using System.Linq;

namespace POETradeHelper.ItemSearch.Services.Factories
{
    public class ListingViewModelFactory : IListingViewModelFactory
    {
        private const string QualityPropertyName = "Quality";
        private const string GemLevelPropertyName = "Level";

        public SimpleListingViewModel Create(ListingResult listingResult, Item item)
        {
            SimpleListingViewModel result = new SimpleListingViewModel
            {
                AccountName = listingResult.Listing.Account?.Name,
                Price = listingResult.Listing.Price?.PriceText,
                Age = listingResult.Listing.AgeText,
            };

            if (item is GemItem)
            {
                result = CreateGemItemListingViewModel(listingResult);
            }
            else if (item is EquippableItem || item is OrganItem)
            {
                result = CreateItemListingViewModelWithItemLevel(listingResult);
            }
            else if (item is FlaskItem)
            {
                result = CreateFlaskItemViewModel(listingResult);
            }

            return result;
        }

        private static SimpleListingViewModel CreateGemItemListingViewModel(ListingResult listingResult)
        {
            var result = new GemItemListingViewModel
            {
                AccountName = listingResult.Listing.Account?.Name,
                Price = listingResult.Listing.Price?.PriceText,
                Age = listingResult.Listing.AgeText,
                Level = GetPropertyStringValue(listingResult.Item, GemLevelPropertyName),
                Quality = GetPropertyStringValue(listingResult.Item, QualityPropertyName)
            };

            var gemExperienceProperty = listingResult.Item.AdditionalProperties?.FirstOrDefault(p => p.Name == "Experience");
            if (gemExperienceProperty != null)
            {
                result.GemExperiencePercent = Math.Round(gemExperienceProperty.Progress * 100, 2);
            }

            return result;
        }

        private static SimpleListingViewModel CreateFlaskItemViewModel(ListingResult listingResult)
        {
            return new FlaskItemListingViewModel
            {
                AccountName = listingResult.Listing.Account?.Name,
                Price = listingResult.Listing.Price?.PriceText,
                Age = listingResult.Listing.AgeText,
                Quality = GetPropertyStringValue(listingResult.Item, QualityPropertyName)
            };
        }

        private static string GetPropertyStringValue(ItemListing itemListing, string propertyName)
        {
            var qualityProperty = itemListing.Properties?.FirstOrDefault(p => p.Name == propertyName);

            return qualityProperty?.Values[0][0].GetString();
        }

        private static SimpleListingViewModel CreateItemListingViewModelWithItemLevel(ListingResult listingResult)
        {
            return new ItemListingViewModelWithItemLevel
            {
                AccountName = listingResult.Listing.Account?.Name,
                Price = listingResult.Listing.Price?.PriceText,
                Age = listingResult.Listing.AgeText,
                ItemLevel = listingResult.Item.ItemLevel
            };
        }
    }
}