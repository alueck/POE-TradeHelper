using System.Text;
using POETradeHelper.ItemSearch.Contract.Properties;

namespace POETradeHelper.ItemSearch.Tests.TestHelpers
{
    public class NameAndRarityGroup
    {
        public string ItemClass { get; set; }
        public string Rarity { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder
                .AppendLine($"{Resources.ItemClassDescriptor} {ItemClass}")
                .AppendLine($"{Resources.RarityDescriptor} {Rarity}")
                .AppendLineIfNotEmpty(Name)
                .AppendLineIfNotEmpty(Type);

            return stringBuilder.ToString();
        }
    }
}