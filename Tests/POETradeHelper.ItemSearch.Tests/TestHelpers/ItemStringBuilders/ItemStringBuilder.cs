using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Services;
using System.Text;

namespace POETradeHelper.ItemSearch.Tests.TestHelpers
{
    public class ItemStringBuilder : ItemStringBuilderBase<ItemStringBuilder>
    {
        public ItemStatsGroup ItemStatsGroup { get; private set; } = new ItemStatsGroup();

        public ItemStringBuilder WithQuality(int quality)
        {
            this.ItemStatsGroup.Quality = quality;
            return this;
        }

        public override string Build()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder
                .Append(this.NameAndRarityGroup)
                .AppendLine(ItemParserAggregator.PropertyGroupSeparator)
                .Append(ItemStatsGroup)
                .AppendLine(ItemParserAggregator.PropertyGroupSeparator, () => !this.IsIdentified)
                .AppendLine(Resources.UnidentifiedDescriptor, () => !this.IsIdentified)
                .AppendLine(ItemParserAggregator.PropertyGroupSeparator, () => this.IsCorrupted)
                .AppendLine(Resources.CorruptedDescriptor, () => this.IsCorrupted);

            return stringBuilder.ToString();
        }
    }
}