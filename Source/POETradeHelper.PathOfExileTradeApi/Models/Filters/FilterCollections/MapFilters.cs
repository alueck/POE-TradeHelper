using System.Text.Json.Serialization;

namespace POETradeHelper.PathOfExileTradeApi.Models.Filters
{
    public class MapFilters : FiltersBase
    {
        private const string MapIncreasedItemQuantityFilterName = "map_iiq";
        private const string MapIncreasedItemRarityFilterName = "map_iir";

        [JsonIgnore]
        public MinMaxFilter MapTier
        {
            get => this.GetFilter<MinMaxFilter>();
            set => this.SetFilter(value);
        }

        [JsonIgnore]
        public MinMaxFilter MapPacksize
        {
            get => this.GetFilter<MinMaxFilter>();
            set => this.SetFilter(value);
        }

        [JsonIgnore]
        public MinMaxFilter MapIncreasedItemQuantity
        {
            get => this.GetFilter<MinMaxFilter>(MapIncreasedItemQuantityFilterName);
            set => this.SetFilter(value, MapIncreasedItemQuantityFilterName);
        }

        [JsonIgnore]
        public MinMaxFilter MapIncreasedItemRarity
        {
            get => this.GetFilter<MinMaxFilter>(MapIncreasedItemRarityFilterName);
            set => this.SetFilter(value, MapIncreasedItemRarityFilterName);
        }

        [JsonIgnore]
        public BoolOptionFilter MapShaped
        {
            get => this.GetFilter<BoolOptionFilter>();
            set => this.SetFilter(value);
        }

        [JsonIgnore]
        public BoolOptionFilter MapElder
        {
            get => this.GetFilter<BoolOptionFilter>();
            set => this.SetFilter(value);
        }

        [JsonIgnore]
        public BoolOptionFilter MapBlighted
        {
            get => this.GetFilter<BoolOptionFilter>();
            set => this.SetFilter(value);
        }

        [JsonIgnore]
        public OptionFilter MapSeries
        {
            get => this.GetFilter<OptionFilter>();
            set => this.SetFilter(value);
        }
    }
}