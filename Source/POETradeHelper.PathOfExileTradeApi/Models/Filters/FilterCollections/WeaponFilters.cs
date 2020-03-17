using System.Text.Json.Serialization;

namespace POETradeHelper.PathOfExileTradeApi.Models.Filters
{
    public class WeaponFilters : FiltersBase<WeaponFilters>
    {
        private const string AttacksPerSecondFilterName = "aps";
        private const string CriticalChanceFilterName = "crit";
        private const string DamagePerSecondFilterName = "dps";
        private const string PhysicalDamagePerSecondFilterName = "pdps";
        private const string ElementalDamagePerSecondFilterName = "edps";

        [JsonIgnore]
        public MinMaxFilter Damage
        {
            get => this.GetFilter<MinMaxFilter>();
            set => this.SetFilter(value);
        }

        [JsonIgnore]
        public MinMaxFilter AttacksPerSecond
        {
            get => this.GetFilter<MinMaxFilter>(AttacksPerSecondFilterName);
            set => this.SetFilter(value, AttacksPerSecondFilterName);
        }

        [JsonIgnore]
        public MinMaxFilter CriticalChance
        {
            get => this.GetFilter<MinMaxFilter>(CriticalChanceFilterName);
            set => this.SetFilter(value, CriticalChanceFilterName);
        }

        [JsonIgnore]
        public MinMaxFilter DamagePerSecond
        {
            get => this.GetFilter<MinMaxFilter>(DamagePerSecondFilterName);
            set => this.SetFilter(value, DamagePerSecondFilterName);
        }

        [JsonIgnore]
        public MinMaxFilter PhysicalDamagePerSecond
        {
            get => this.GetFilter<MinMaxFilter>(PhysicalDamagePerSecondFilterName);
            set => this.SetFilter(value, PhysicalDamagePerSecondFilterName);
        }

        [JsonIgnore]
        public MinMaxFilter ElementalDamagePerSecond
        {
            get => this.GetFilter<MinMaxFilter>(ElementalDamagePerSecondFilterName);
            set => this.SetFilter(value, ElementalDamagePerSecondFilterName);
        }
    }
}