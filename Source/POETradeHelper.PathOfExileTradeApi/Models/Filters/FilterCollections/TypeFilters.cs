using System.Text.Json.Serialization;

namespace POETradeHelper.PathOfExileTradeApi.Models.Filters
{
    public class TypeFilters : FiltersBase<TypeFilters>
    {
        [JsonIgnore]
        public OptionFilter Category
        {
            get => this.GetFilter<OptionFilter>();
            set => this.SetFilter(value);
        }

        [JsonIgnore]
        public OptionFilter Rarity
        {
            get => this.GetFilter<OptionFilter>();
            set => this.SetFilter(value);
        }
    }
}