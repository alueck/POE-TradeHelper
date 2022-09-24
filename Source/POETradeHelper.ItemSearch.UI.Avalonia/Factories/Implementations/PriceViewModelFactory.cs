using System.Threading;
using System.Threading.Tasks;

using POETradeHelper.Common.UI.Services;
using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Factories.Implementations
{
    public class PriceViewModelFactory : IPriceViewModelFactory
    {
        private readonly IStaticDataService staticDataService;
        private readonly IImageService imageService;

        public PriceViewModelFactory(IStaticDataService staticItemDataService, IImageService imageService)
        {
            this.staticDataService = staticItemDataService;
            this.imageService = imageService;
        }

        public async Task<PriceViewModel> CreateAsync(Price price, CancellationToken cancellationToken = default)
        {
            return price != null
                ? new PriceViewModel
                {
                    Amount = price.Amount.ToString("0.##").PadLeft(6),
                    Currency = this.staticDataService.GetText(price.Currency),
                    Image = await this.imageService.GetImageAsync(this.staticDataService.GetImageUrl(price.Currency), cancellationToken).ConfigureAwait(false)
                }
                : null;
        }
    }
}