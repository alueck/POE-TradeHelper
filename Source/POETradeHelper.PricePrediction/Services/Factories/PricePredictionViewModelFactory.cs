using System.Threading;
using System.Threading.Tasks;
using POETradeHelper.Common.UI.Services;
using POETradeHelper.PathOfExileTradeApi.Services;
using POETradeHelper.PricePrediction.Models;
using POETradeHelper.PricePrediction.ViewModels;

namespace POETradeHelper.PricePrediction.Services.Factories
{
    public class PricePredictionViewModelFactory : IPricePredictionViewModelFactory
    {
        private readonly IImageService imageService;
        private readonly IStaticDataService staticDataService;

        public PricePredictionViewModelFactory(IImageService imageService, IStaticDataService staticDataService)
        {
            this.imageService = imageService;
            this.staticDataService = staticDataService;
        }

        public async Task<PricePredictionViewModel> CreateAsync(PoePricesInfoItem poePricesInfoItem, CancellationToken cancellationToken = default)
        {
            if (poePricesInfoItem == null || poePricesInfoItem.ErrorCode != 0)
            {
                return new PricePredictionViewModel();
            }

            System.Uri imageUrl = this.staticDataService.GetImageUrl(poePricesInfoItem.Currency);
            Avalonia.Media.Imaging.IBitmap image = await this.imageService.GetImageAsync(imageUrl, cancellationToken).ConfigureAwait(false);

            return new PricePredictionViewModel
            {
                Prediction = $"{poePricesInfoItem.Min:N}-{poePricesInfoItem.Max:N}",
                ConfidenceScore = $"{poePricesInfoItem.ConfidenceScore:N} %",
                Currency = this.staticDataService.GetText(poePricesInfoItem.Currency),
                CurrencyImage = image
            };
        }
    }
}