using System;

namespace POETradeHelper.PathOfExileTradeApi.Models
{
    public interface IQueryRequest : ICloneable
    {
        string Endpoint { get; }

        string League { get; set; }
    }
}