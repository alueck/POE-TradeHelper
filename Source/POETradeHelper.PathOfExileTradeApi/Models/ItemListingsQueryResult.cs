using System;

namespace POETradeHelper.PathOfExileTradeApi.Models
{
    public record ItemListingsQueryResult : QueryResult<ListingResult>
    {
        public Uri? Uri { get; init; }

        public int TotalCount { get; init; }

        public SearchQueryResult SearchQueryResult { get; init; } = new();

        public int CurrentPage { get; init; }

        public bool HasMorePages => this.SearchQueryResult.Request.PageSize != 0
                                    && this.CurrentPage < Math.Ceiling(this.TotalCount / (double)this.SearchQueryResult.Request.PageSize);
    }
}