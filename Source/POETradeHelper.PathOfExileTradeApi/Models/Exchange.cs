using System;
using System.Collections.Generic;
using System.Linq;

using POETradeHelper.PathOfExileTradeApi.Models.Filters;

namespace POETradeHelper.PathOfExileTradeApi.Models
{
    public class Exchange : ICloneable
    {
        public IList<string> Want { get; private set; } = new List<string>();

        public IList<string> Have { get; private set; } = new List<string>();

        public OptionFilter Status { get; private set; } = new()
        {
            Option = "online",
        };

        public object Clone() =>
            new Exchange
            {
                Want = this.Want.ToList(),
                Have = this.Have.ToList(),
                Status = (OptionFilter)this.Status.Clone(),
            };
    }
}