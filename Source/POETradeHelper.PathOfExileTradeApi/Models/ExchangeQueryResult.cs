using System;
using System.Collections.Generic;

using POETradeHelper.Common.Extensions;

namespace POETradeHelper.PathOfExileTradeApi.Models;

public record ExchangeQueryResult(string Id, int Total, Dictionary<string, ExchangeQueryResultListing> Result)
{
    public Uri? Uri { get; set; }
}

public record ExchangeQueryResultListing(string Id, ExchangeListing Listing);

public record ExchangeListing(DateTime Indexed, Account Account, List<ExchangeOffer> Offers)
{
    public string AgeText => (DateTime.UtcNow - this.Indexed.ToUniversalTime()).ToHumanReadableString();
}

public record ExchangeOffer(Price Item, Price Exchange);