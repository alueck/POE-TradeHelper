﻿using System.Collections.Generic;
using System.Text.Json;

namespace POETradeHelper.PathOfExileTradeApi.Models
{
    public class Property
    {
        public string Name { get; set; } = string.Empty;

        public IList<IList<JsonElement>> Values { get; set; } = new List<IList<JsonElement>>();

        public decimal Progress { get; set; }
    }
}