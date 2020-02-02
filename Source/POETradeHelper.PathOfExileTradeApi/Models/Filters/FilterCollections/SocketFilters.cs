using System.Text.Json.Serialization;

namespace POETradeHelper.PathOfExileTradeApi.Models.Filters
{
    public class SocketFilters : FiltersBase
    {
        [JsonIgnore]
        public SocketsFilter Sockets
        {
            get => this.GetFilter<SocketsFilter>();
            set => this.SetFilter(value);
        }

        [JsonIgnore]
        public SocketsFilter Links
        {
            get => this.GetFilter<SocketsFilter>();
            set => this.SetFilter(value);
        }
    }
}