namespace POETradeHelper.PathOfExileTradeApi.Models
{
    public class SearchQueryResult : QueryResult<string>
    {
        public string Id { get; set; }
        public int Total { get; set; }
        public SearchQueryRequest Request { get; set; }
    }
}