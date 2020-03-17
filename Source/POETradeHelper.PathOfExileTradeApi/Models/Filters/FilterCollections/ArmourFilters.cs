using System.Text.Json.Serialization;

namespace POETradeHelper.PathOfExileTradeApi.Models.Filters
{
    public class ArmourFilters : FiltersBase<ArmourFilters>
    {
        private const string ArmourFilterName = "ar";
        private const string EvasionFilterName = "ev";
        private const string EnergyShieldFilterName = "es";

        [JsonIgnore]
        public MinMaxFilter Armour
        {
            get => this.GetFilter<MinMaxFilter>(ArmourFilterName);
            set => this.SetFilter(value, ArmourFilterName);
        }

        [JsonIgnore]
        public MinMaxFilter Evasion
        {
            get => this.GetFilter<MinMaxFilter>(EvasionFilterName);
            set => this.SetFilter(value, EvasionFilterName);
        }

        [JsonIgnore]
        public MinMaxFilter EnergyShield
        {
            get => this.GetFilter<MinMaxFilter>(EnergyShieldFilterName);
            set => this.SetFilter(value, EnergyShieldFilterName);
        }

        [JsonIgnore]
        public MinMaxFilter Block
        {
            get => this.GetFilter<MinMaxFilter>();
            set => this.SetFilter(value);
        }
    }
}