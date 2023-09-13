using System;
using System.Threading;
using System.Threading.Tasks;

using Avalonia.Media;

using MediatR;

using Microsoft.Extensions.Options;

using POETradeHelper.Common.UI.Services;
using POETradeHelper.ItemSearch.Contract.Configuration;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Services;
using POETradeHelper.PricePrediction.Models;
using POETradeHelper.PricePrediction.Queries;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Splat;

namespace POETradeHelper.PricePrediction.UI.Avalonia.ViewModels
{
    public class PricePredictionViewModel : ReactiveObject, IPricePredictionViewModel
    {
        private readonly IOptionsMonitor<ItemSearchOptions> itemSearchOptions;
        private readonly IMediator mediator;
        private readonly IStaticDataService staticDataService;
        private readonly IImageService imageService;

        private Item? item;

        public PricePredictionViewModel(
            IOptionsMonitor<ItemSearchOptions> itemSearchOptions,
            IMediator mediator,
            IStaticDataService staticDataService,
            IImageService imageService)
        {
            this.itemSearchOptions = itemSearchOptions;
            this.mediator = mediator;
            this.staticDataService = staticDataService;
            this.imageService = imageService;

            this.itemSearchOptions.OnChange(newValue =>
            {
                if (!newValue.PricePredictionEnabled)
                {
                    this.Clear();
                }
            });

            this.WhenAnyValue(
                    x => x.Prediction,
                    x => x.Currency,
                    (prediction, currency) => !string.IsNullOrEmpty(prediction) && !string.IsNullOrEmpty(currency))
                .ToPropertyEx(this, x => x.HasValue);
        }

        [Reactive]
        public string Prediction { get; set; } = string.Empty;

        [Reactive]
        public string Currency { get; set; } = string.Empty;

        [Reactive]
        public IImage? CurrencyImage { get; set; }

        [Reactive]
        public string ConfidenceScore { get; set; } = string.Empty;

        [ObservableAsProperty]
        public bool HasValue => !string.IsNullOrEmpty(this.Prediction) && !string.IsNullOrEmpty(this.Currency);

        public async Task LoadAsync(Item item, CancellationToken cancellationToken)
        {
            try
            {
                if (this.itemSearchOptions.CurrentValue.PricePredictionEnabled &&
                    !string.Equals(this.item?.ItemText, item.ItemText))
                {
                    this.item = item;
                    this.Clear();

                    GetPoePricesInfoPredictionQuery request = new(item, this.itemSearchOptions.CurrentValue.League);
                    PoePricesInfoPrediction poePricesInfoPrediction =
                        await this.mediator.Send(request, cancellationToken).ConfigureAwait(true);

                    await this.MapPrediction(poePricesInfoPrediction, cancellationToken);
                }
            }
            catch (Exception exception)
            {
                if (exception is not OperationCanceledException and not TaskCanceledException)
                {
                    this.Log().Error(exception);
                }
            }
        }

        private void Clear()
        {
            this.Prediction = string.Empty;
            this.Currency = string.Empty;
            this.CurrencyImage = null;
            this.ConfidenceScore = string.Empty;
        }

        private async Task MapPrediction(PoePricesInfoPrediction prediction, CancellationToken cancellationToken)
        {
            if (prediction is not { ErrorCode: 0 })
            {
                return;
            }

            this.Prediction = $"{prediction.Min:N}-{prediction.Max:N}";
            this.ConfidenceScore = $"{prediction.ConfidenceScore:N} %";
            this.Currency = this.staticDataService.GetText(prediction.Currency);
            this.CurrencyImage = await this.GetCurrencyImage(prediction.Currency, cancellationToken).ConfigureAwait(false);
        }

        private async Task<IImage?> GetCurrencyImage(string currency, CancellationToken cancellationToken)
        {
            Uri imageUrl = this.staticDataService.GetImageUrl(currency);

            return await this.imageService.GetImageAsync(imageUrl, cancellationToken).ConfigureAwait(false);
        }
    }
}