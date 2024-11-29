using System;
using System.Collections.Generic;

using POETradeHelper.PathOfExileTradeApi.Models.Filters;

namespace POETradeHelper.PathOfExileTradeApi.Models
{
    public class Exchange : ICloneable
    {
        public IList<string> Want { get; private set; } = [];

        public IList<string> Have { get; private set; } = [];

        public OptionFilter Status { get; private set; } = new()
        {
            Option = "online",
        };

        public object Clone() =>
            new Exchange
            {
                Want = [..this.Want],
                Have = [..this.Have],
                Status = (OptionFilter)this.Status.Clone(),
            };
    }
}