namespace POETradeHelper.PathOfExileTradeApi.Models
{
    public class ListingResult
    {
        public string Id { get; set; }

        public Listing Listing { get; set; }

        public ItemListing Item { get; set; }
    }
}