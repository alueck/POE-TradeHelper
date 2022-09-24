using System.Text.Json.Serialization;

namespace POETradeHelper.PathOfExileTradeApi.Models.Filters
{
    public class MiscFilters : FiltersBase<MiscFilters>
    {
        private const string ItemLevelFilterName = "ilvl";

        [JsonIgnore]
        public MinMaxFilter? Quality
        {
            get => this.GetFilter<MinMaxFilter>();
            set => this.SetFilter(value);
        }

        [JsonIgnore]
        public MinMaxFilter? ItemLevel
        {
            get => this.GetFilter<MinMaxFilter>(ItemLevelFilterName);
            set => this.SetFilter(value, ItemLevelFilterName);
        }

        [JsonIgnore]
        public MinMaxFilter? GemLevel
        {
            get => this.GetFilter<MinMaxFilter>();
            set => this.SetFilter(value);
        }

        [JsonIgnore]
        public MinMaxFilter? GemLevelProgress
        {
            get => this.GetFilter<MinMaxFilter>();
            set => this.SetFilter(value);
        }

        [JsonIgnore]
        public OptionFilter? GemAlternateQuality
        {
            get => this.GetFilter<OptionFilter>();
            set => this.SetFilter(value);
        }

        [JsonIgnore]
        public BoolOptionFilter? ShaperItem
        {
            get => this.GetFilter<BoolOptionFilter>();
            set => this.SetFilter(value);
        }

        [JsonIgnore]
        public BoolOptionFilter? ElderItem
        {
            get => this.GetFilter<BoolOptionFilter>();
            set => this.SetFilter(value);
        }

        [JsonIgnore]
        public BoolOptionFilter? CrusaderItem
        {
            get => this.GetFilter<BoolOptionFilter>();
            set => this.SetFilter(value);
        }

        [JsonIgnore]
        public BoolOptionFilter? RedeemerItem
        {
            get => this.GetFilter<BoolOptionFilter>();
            set => this.SetFilter(value);
        }

        [JsonIgnore]
        public BoolOptionFilter? HunterItem
        {
            get => this.GetFilter<BoolOptionFilter>();
            set => this.SetFilter(value);
        }

        [JsonIgnore]
        public BoolOptionFilter? WarlordItem
        {
            get => this.GetFilter<BoolOptionFilter>();
            set => this.SetFilter(value);
        }

        [JsonIgnore]
        public BoolOptionFilter? FracturedItem
        {
            get => this.GetFilter<BoolOptionFilter>();
            set => this.SetFilter(value);
        }

        [JsonIgnore]
        public BoolOptionFilter? SynthesisedItem
        {
            get => this.GetFilter<BoolOptionFilter>();
            set => this.SetFilter(value);
        }

        [JsonIgnore]
        public BoolOptionFilter? AlternateArt
        {
            get => this.GetFilter<BoolOptionFilter>();
            set => this.SetFilter(value);
        }

        [JsonIgnore]
        public BoolOptionFilter? Identified
        {
            get => this.GetFilter<BoolOptionFilter>();
            set => this.SetFilter(value);
        }

        [JsonIgnore]
        public BoolOptionFilter? Corrupted
        {
            get => this.GetFilter<BoolOptionFilter>();
            set => this.SetFilter(value);
        }

        [JsonIgnore]
        public BoolOptionFilter? Mirrored
        {
            get => this.GetFilter<BoolOptionFilter>();
            set => this.SetFilter(value);
        }

        [JsonIgnore]
        public BoolOptionFilter? Crafted
        {
            get => this.GetFilter<BoolOptionFilter>();
            set => this.SetFilter(value);
        }

        [JsonIgnore]
        public BoolOptionFilter? Veiled
        {
            get => this.GetFilter<BoolOptionFilter>();
            set => this.SetFilter(value);
        }

        [JsonIgnore]
        public BoolOptionFilter? Enchanted
        {
            get => this.GetFilter<BoolOptionFilter>();
            set => this.SetFilter(value);
        }

        [JsonIgnore]
        public MinMaxFilter? TalismanTier
        {
            get => this.GetFilter<MinMaxFilter>();
            set => this.SetFilter(value);
        }
    }
}