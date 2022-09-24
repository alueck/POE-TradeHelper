namespace POETradeHelper.PathOfExileTradeApi.Models
{
    public class ListingResult
    {
        public string Id { get; set; } = string.Empty;

        public Listing Listing { get; set; } = new();

        public ItemListing Item { get; set; } = new();
    }
}