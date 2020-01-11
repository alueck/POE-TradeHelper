namespace POETradeHelper.PathOfExileTradeApi.Models
{
    public class ItemListing
    {
        public bool Verified { get; set; }

        public string Icon { get; set; }

        public string Name { get; set; }

        public string TypeLine { get; set; }

        public bool Identified { get; set; }

        public byte ILvl { get; set; }
    }
}