using System;
using System.Text.Json.Serialization;

namespace POETradeHelper.PathOfExileTradeApi.Models.Filters
{
    public class QueryFilters : ICloneable
    {
        public TypeFilters TypeFilters { get; private set; } = new();

        public WeaponFilters WeaponFilters { get; private set; } = new();

        public ArmourFilters ArmourFilters { get; private set; } = new();

        public SocketFilters SocketFilters { get; private set; } = new();

        [JsonPropertyName("req_filters")]
        public RequirementsFilters RequirementsFilters { get; private set; } = new();

        public MapFilters MapFilters { get; private set; } = new();

        public MiscFilters MiscFilters { get; private set; } = new();

        public object Clone() =>
            new QueryFilters
            {
                TypeFilters = (TypeFilters)this.TypeFilters.Clone(),
                WeaponFilters = (WeaponFilters)this.WeaponFilters.Clone(),
                ArmourFilters = (ArmourFilters)this.ArmourFilters.Clone(),
                SocketFilters = (SocketFilters)this.SocketFilters.Clone(),
                RequirementsFilters = (RequirementsFilters)this.RequirementsFilters.Clone(),
                MapFilters = (MapFilters)this.MapFilters.Clone(),
                MiscFilters = (MiscFilters)this.MiscFilters.Clone(),
            };
    }
}