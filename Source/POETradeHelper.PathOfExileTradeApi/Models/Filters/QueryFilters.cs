using System.Text.Json.Serialization;

namespace POETradeHelper.PathOfExileTradeApi.Models.Filters
{
    public class QueryFilters
    {
        public TypeFilters TypeFilters { get; set; }

        public WeaponFilters WeaponFilters { get; set; }

        public ArmourFilters ArmourFilters { get; set; }

        public SocketFilters SocketFilters { get; set; }

        [JsonPropertyName("req_filters")]
        public RequirementsFilters RequirementsFilters { get; set; }

        public MapFilters MapFilters { get; set; }

        public MiscFilters MiscFilters { get; set; }
    }
}