using System.Text.Json.Serialization;

namespace POETradeHelper.PathOfExileTradeApi.Models.Filters
{
    public class RequirementsFilters : FiltersBase<RequirementsFilters>
    {
        private const string LevelFilterName = "lvl";
        private const string StrengthFilterName = "str";
        private const string DexterityFilterName = "dex";
        private const string IntelligenceFilterName = "int";

        [JsonIgnore]
        public MinMaxFilter? Level
        {
            get => this.GetFilter<MinMaxFilter>(LevelFilterName);
            set => this.SetFilter(value, LevelFilterName);
        }

        [JsonIgnore]
        public MinMaxFilter? Strength
        {
            get => this.GetFilter<MinMaxFilter>(StrengthFilterName);
            set => this.SetFilter(value, StrengthFilterName);
        }

        [JsonIgnore]
        public MinMaxFilter? Dexterity
        {
            get => this.GetFilter<MinMaxFilter>(DexterityFilterName);
            set => this.SetFilter(value, DexterityFilterName);
        }

        [JsonIgnore]
        public MinMaxFilter? Intelligence
        {
            get => this.GetFilter<MinMaxFilter>(IntelligenceFilterName);
            set => this.SetFilter(value, IntelligenceFilterName);
        }
    }
}