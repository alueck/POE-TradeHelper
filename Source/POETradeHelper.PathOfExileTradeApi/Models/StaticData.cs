using System.Collections.Generic;

namespace POETradeHelper.PathOfExileTradeApi.Models
{
    public class StaticData
    {
        public string Id { get; set; }

        public IList<StaticDataEntry> Entries { get; set; }
    }
}