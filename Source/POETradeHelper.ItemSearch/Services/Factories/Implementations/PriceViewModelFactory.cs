using POETradeHelper.Common.UI;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Services;
using System;
using System.Threading.Tasks;

namespace POETradeHelper.ItemSearch.Services.Factories
{
    public class PriceViewModelFactory : IPriceViewModelFactory
    {
        private readonly IStaticItemDataService staticItemDataService;
        private readonly IImageService imageService;

        public PriceViewModelFactory(IStaticItemDataService staticItemDataService, IImageService imageService)
        {
            this.staticItemDataService = staticItemDataService;
            this.imageService = imageService;
        }

        public async Task<PriceViewModel> CreateAsync(Price price)
        {
            return price != null
                ? new PriceViewModel
                {
                    Amount = price.Amount.ToString("0.##").PadLeft(6),
                    Currency = this.staticItemDataService.GetText(price.Currency),
                    Image = await this.imageService.GetImageAsync(this.staticItemDataService.GetImageUrl(price.Currency))
                }
                : null;
        }
    }
}