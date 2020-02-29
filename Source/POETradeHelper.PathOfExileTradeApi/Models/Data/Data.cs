using System.Collections.Generic;

namespace POETradeHelper.PathOfExileTradeApi.Models
{
    public class Data<TType>
    {
        public string Id { get; set; }

        public IList<TType> Entries { get; set; }
    }
}