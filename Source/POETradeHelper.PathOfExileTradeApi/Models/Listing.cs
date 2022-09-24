using POETradeHelper.Common.Extensions;

using System;

namespace POETradeHelper.PathOfExileTradeApi.Models
{
    public class Listing
    {
        public DateTime Indexed { get; set; }

        public Account Account { get; set; } = new();

        public Price Price { get; set; } = new();

        public string AgeText => (DateTime.UtcNow - this.Indexed).ToHumanReadableString();
    }
}