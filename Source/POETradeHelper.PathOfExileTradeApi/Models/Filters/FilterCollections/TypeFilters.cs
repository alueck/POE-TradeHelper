using POETradeHelper.PathOfExileTradeApi.Models.Filters;
using System.Text.Json.Serialization;

namespace POETradeHelper.PathOfExileTradeApi
{
    public class TypeFilters : FiltersBase
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