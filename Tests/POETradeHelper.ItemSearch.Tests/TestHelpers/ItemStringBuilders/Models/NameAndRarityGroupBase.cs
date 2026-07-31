using System.Text;

using POETradeHelper.ItemSearch.Contract.Properties;

namespace POETradeHelper.ItemSearch.Tests.TestHelpers.ItemStringBuilders.Models
{
    public abstract class NameAndRarityGroupBase
    {
        public string ItemClass { get; set; } = string.Empty;

        public string Rarity { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public override string ToString()
        {
            StringBuilder stringBuilder = new();

            stringBuilder
                .AppendLine($"{Resources.ItemClassDescriptor} {this.ItemClass}")
                .AppendLine($"{Resources.RarityDescriptor} {this.Rarity}")
                .AppendLineIfNotEmpty(this.Name);

            return stringBuilder.ToString();
        }
    }
}