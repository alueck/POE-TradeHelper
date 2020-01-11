namespace POETradeHelper.ItemSearch.Contract.Models
{
    public class EquippableItem : Item, ICorruptableItem, IIdentifiableItem
    {
        public string Quality { get; set; }
        public string ItemLevel { get; set; }
        public InfluenceType Influence { get; set; }
        public ItemSockets Sockets { get; set; }
        public bool IsCorrupted { get; set; }
        public bool IsIdentified { get; set; }
    }
}