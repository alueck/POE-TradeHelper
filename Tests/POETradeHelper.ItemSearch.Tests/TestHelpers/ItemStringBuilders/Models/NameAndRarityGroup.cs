using System.Text;

using POETradeHelper.ItemSearch.Contract.Properties;

namespace POETradeHelper.ItemSearch.Tests.TestHelpers.ItemStringBuilders.Models
{
    public class NameAndRarityGroup
    {
        public string ItemClass { get; set; } = string.Empty;

        public string Rarity { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public override string ToString()
        {
            StringBuilder stringBuilder = new();

            stringBuilder
                .AppendLine($"{Resources.ItemClassDescriptor} {this.ItemClass}")
                .AppendLine($"{Resources.RarityDescriptor} {this.Rarity}")
                .AppendLineIfNotEmpty(this.Name)
                .AppendLineIfNotEmpty(this.Type);

            return stringBuilder.ToString();
        }
    }
}