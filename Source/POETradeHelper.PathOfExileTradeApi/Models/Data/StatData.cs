using POETradeHelper.Common.Extensions;
using POETradeHelper.ItemSearch.Contract;
using System;

namespace POETradeHelper.PathOfExileTradeApi.Models
{
    internal class StatData
    {
        public string Id { get; set; }

        public string Text { get; set; }

        public string Type { get; set; }

        public StatCategory? StatCategory => Type.ParseToEnumByDisplayName<StatCategory>(StringComparison.OrdinalIgnoreCase);
    }
}