using System.Collections.Generic;

namespace POETradeHelper.PathOfExileTradeApi.Models
{
    public class Data<TType>
    {
        private string? id;

        public string Id
        {
            get => this.id ?? this.Label;
            set => this.id = value;
        }

        public string Label { get; set; } = string.Empty;

        public IList<TType> Entries { get; set; } = [];
    }
}