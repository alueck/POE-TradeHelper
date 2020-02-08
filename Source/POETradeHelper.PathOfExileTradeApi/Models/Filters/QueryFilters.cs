using System.Text.Json.Serialization;

namespace POETradeHelper.PathOfExileTradeApi.Models.Filters
{
    public class QueryFilters
    {
        public TypeFilters TypeFilters { get; } = new TypeFilters();

        public WeaponFilters WeaponFilters { get; } = new WeaponFilters();

        public ArmourFilters ArmourFilters { get; } = new ArmourFilters();

        public SocketFilters SocketFilters { get; } = new SocketFilters();

        [JsonPropertyName("req_filters")]
        public RequirementsFilters RequirementsFilters { get; } = new RequirementsFilters();

        public MapFilters MapFilters { get; } = new MapFilters();

        public MiscFilters MiscFilters { get; } = new MiscFilters();
    }
}