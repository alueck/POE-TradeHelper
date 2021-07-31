using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using MediatR;
using Microsoft.Extensions.Options;
using POETradeHelper.Common.UI.Services;
using POETradeHelper.ItemSearch.Contract.Configuration;
using POETradeHelper.PathOfExileTradeApi.Services;
using POETradeHelper.PricePrediction.Models;
using POETradeHelper.PricePrediction.ViewModels;

namespace POETradeHelper.PricePrediction.Queries.Handlers
{
    public class GetPricePredictionViewModelQueryHandler : IRequestHandler<GetPricePredictionViewModelQuery, PricePredictionViewModel>
    {
        private readonly IOptionsMonitor<ItemSearchOptions> itemSearchOptions;
        private readonly IStaticDataService staticDataService;
        private readonly IImageService imageService;
        private readonly IMediator mediator;

        public GetPricePredictionViewModelQueryHandler(
            IOptionsMonitor<ItemSearchOptions> itemSearchOptions,
            IStaticDataService staticDataService,
            IImageService imageService,
            IMediator mediator)
        {
            this.itemSearchOptions = itemSearchOptions;
            this.staticDataService = staticDataService;
            this.imageService = imageService;
            this.mediator = mediator;
        }

        public async Task<PricePredictionViewModel> Handle(GetPricePredictionViewModelQuery request, CancellationToken cancellationToken)
        {
            var query = new GetPoePricesInfoPredictionQuery(request.Item, this.itemSearchOptions.CurrentValue.League);
            PoePricesInfoPrediction prediction = await this.mediator.Send(query, cancellationToken).ConfigureAwait(false);

            return await this.CreateViewModelAsync(prediction, cancellationToken);
        }

        private async Task<PricePredictionViewModel> CreateViewModelAsync(PoePricesInfoPrediction prediction, CancellationToken cancellationToken)
        {
            if (prediction is not { ErrorCode: 0 })
            {
                return new PricePredictionViewModel();
            }

            Uri imageUrl = this.staticDataService.GetImageUrl(prediction.Currency);
            IBitmap image = await this.imageService.GetImageAsync(imageUrl, cancellationToken).ConfigureAwait(false);

            return new PricePredictionViewModel
            {
                Prediction = $"{prediction.Min:N}-{prediction.Max:N}",
                ConfidenceScore = $"{prediction.ConfidenceScore:N} %",
                Currency = this.staticDataService.GetText(prediction.Currency),
                CurrencyImage = image
            };
        }
    }
}