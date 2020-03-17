using System;
using System.Text.Json.Serialization;

namespace POETradeHelper.PathOfExileTradeApi.Models.Filters
{
    public class QueryFilters : ICloneable
    {
        public TypeFilters TypeFilters { get; private set; } = new TypeFilters();

        public WeaponFilters WeaponFilters { get; private set; } = new WeaponFilters();

        public ArmourFilters ArmourFilters { get; private set; } = new ArmourFilters();

        public SocketFilters SocketFilters { get; private set; } = new SocketFilters();

        [JsonPropertyName("req_filters")]
        public RequirementsFilters RequirementsFilters { get; private set; } = new RequirementsFilters();

        public MapFilters MapFilters { get; private set; } = new MapFilters();

        public MiscFilters MiscFilters { get; private set; } = new MiscFilters();

        public object Clone()
        {
            return new QueryFilters
            {
                TypeFilters = (TypeFilters)this.TypeFilters.Clone(),
                WeaponFilters = (WeaponFilters)this.WeaponFilters.Clone(),
                ArmourFilters = (ArmourFilters)this.ArmourFilters.Clone(),
                SocketFilters = (SocketFilters)this.SocketFilters.Clone(),
                RequirementsFilters = (RequirementsFilters)this.RequirementsFilters.Clone(),
                MapFilters = (MapFilters)this.MapFilters.Clone(),
                MiscFilters = (MiscFilters)this.MiscFilters.Clone()
            };
        }
    }
}