namespace POETradeHelper.ItemSearch.Contract.Models
{
    public class GemItem : ItemWithStats, ICorruptableItem, IQualityItem
    {
        public GemItem() : base(ItemRarity.Gem)
        {
        }

        public string? TypeDiscriminator { get; set; }

        public int Quality { get; set; }

        public int Level { get; set; }

        public int ExperiencePercent { get; set; }

        public bool IsCorrupted { get; set; }

        public bool IsVaalVersion { get; set; }

        public bool IsImbued { get; set; }

        public bool IsTransfigured { get; set; }
    }
}