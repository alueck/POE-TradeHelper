using System;

namespace POETradeHelper.PathOfExileTradeApi.Models
{
    public class ItemListingsQueryResult : QueryResult<ListingResult>
    {
        public Uri Uri { get; set; }
        public int TotalCount { get; set; }
        public SearchQueryRequest SearchQueryRequest { get; set; }
    }
}