using Autofac.Extras.DynamicProxy;
using MediatR;
using POETradeHelper.Common.Contract.Attributes;
using POETradeHelper.ItemSearch.Contract;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PricePrediction.Models;
using POETradeHelper.PricePrediction.Services;

namespace POETradeHelper.PricePrediction.Queries;

public class GetPoePricesInfoPredictionQuery : IRequest<PoePricesInfoPrediction>
{
    public GetPoePricesInfoPredictionQuery(Item item, League league)
    {
        this.Item = item;
        this.League = league;
    }

    public Item Item { get; }

    public League League { get; }
}

[Intercept(typeof(CacheResultAttributeInterceptor))]
public class GetPoePricesInfoPredictionQueryHandler : IRequestHandler<GetPoePricesInfoPredictionQuery, PoePricesInfoPrediction?>
{
    private readonly IPoePricesInfoClient poePricesInfoClient;

    public GetPoePricesInfoPredictionQueryHandler(IPoePricesInfoClient poePricesInfoClient)
    {
        this.poePricesInfoClient = poePricesInfoClient;
    }

    [CacheResult(DurationSeconds = 30 * 60)]
    public async Task<PoePricesInfoPrediction?> Handle(GetPoePricesInfoPredictionQuery request, CancellationToken cancellationToken)
    {
        PoePricesInfoPrediction? prediction = null;

        if (request is { League: not null, Item.Rarity: ItemRarity.Rare }
            && !string.IsNullOrEmpty(request.Item.PlainItemText))
        {
            prediction = await this.poePricesInfoClient
                .GetPricePredictionAsync(request.League.Id, request.Item.PlainItemText, cancellationToken)
                .ConfigureAwait(false);
        }

        return prediction;
    }
}