namespace POETradeHelper.PathOfExileTradeApi.Models
{
    public class Price
    {
        public string Type { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public string PriceText => $"{Amount:0.##} {Currency}";
    }
}