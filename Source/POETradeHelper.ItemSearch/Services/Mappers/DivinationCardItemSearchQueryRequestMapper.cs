using Microsoft.Extensions.Options;

using POETradeHelper.ItemSearch.Contract.Configuration;
using POETradeHelper.ItemSearch.Contract.Models;

namespace POETradeHelper.ItemSearch.Services.Mappers;

public sealed class DivinationCardItemSearchQueryRequestMapper : ItemSearchRequestMapperBase
{
    public DivinationCardItemSearchQueryRequestMapper(IOptionsMonitor<ItemSearchOptions> itemSearchOptions) : base(itemSearchOptions)
    {
    }

    public override bool CanMap(Item item) => item is DivinationCardItem;
}