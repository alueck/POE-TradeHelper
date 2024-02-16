using System.Collections.Generic;

namespace POETradeHelper.PathOfExileTradeApi.Models
{
    public record QueryResult<TResult>
    {
        public List<TResult> Result { get; set; } = new();
    }
}