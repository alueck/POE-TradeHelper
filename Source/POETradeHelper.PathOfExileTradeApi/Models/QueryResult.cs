using System.Collections.Generic;

namespace POETradeHelper.PathOfExileTradeApi.Models
{
    public class QueryResult<TResult>
    {
        public List<TResult> Result { get; set; } = new List<TResult>();
    }
}