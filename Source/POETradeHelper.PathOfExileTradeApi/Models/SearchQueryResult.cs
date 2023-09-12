namespace POETradeHelper.PathOfExileTradeApi.Models
{
    public class SearchQueryResult : QueryResult<string>
    {
        public string Id { get; set; } = string.Empty;

        public int Total { get; set; }

        public SearchQueryRequest Request { get; set; } = new();
    }
}