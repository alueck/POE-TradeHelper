namespace POETradeHelper.PathOfExileTradeApi.Models
{
    public record Price
    {
        public string Type { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public string PriceText => $"{Amount:0.##} {Currency}";
    }
}