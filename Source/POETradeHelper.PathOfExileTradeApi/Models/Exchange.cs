using POETradeHelper.PathOfExileTradeApi.Models.Filters;
using System.Collections.Generic;

namespace POETradeHelper.PathOfExileTradeApi.Models
{
    public class Exchange
    {
        public IList<string> Want { get; } = new List<string>();

        public IList<string> Have { get; } = new List<string>();

        public OptionFilter Status { get; } = new OptionFilter
        {
            Option = "online"
        };
    }
}