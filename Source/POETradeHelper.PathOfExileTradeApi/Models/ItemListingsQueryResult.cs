using System;

namespace POETradeHelper.PathOfExileTradeApi.Models
{
    public record ItemListingsQueryResult : QueryResult<ListingResult>
    {
        public const int PageSize = 10;

        public Uri? Uri { get; init; }

        public int TotalCount { get; init; }

        public SearchQueryResult SearchQueryResult { get; init; } = new();

        public int CurrentPage { get; init; }

        public bool HasMorePages => this.CurrentPage < Math.Ceiling(this.TotalCount / (double)PageSize);
    }
}