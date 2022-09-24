﻿namespace POETradeHelper.PathOfExileTradeApi.Models
{
    public record Price
    {
        public string Type { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public string Currency { get; set; } = string.Empty;

        public string PriceText => $"{Amount:0.##} {Currency}";
    }
}