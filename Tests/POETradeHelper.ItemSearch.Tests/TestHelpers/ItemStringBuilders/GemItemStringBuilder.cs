using POETradeHelper.Common.Extensions;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Services;
using System.Text;

namespace POETradeHelper.ItemSearch.Tests.TestHelpers
{
    public class GemItemStringBuilder : ItemStringBuilderBase<GemItemStringBuilder>
    {
        public GemItemStatsGroup ItemStatsGroup { get; } = new GemItemStatsGroup();

        public GemItemStringBuilder()
        {
            this.NameAndRarityGroup.Rarity = ItemRarity.Gem.GetDisplayName();
        }

        public GemItemStringBuilder WithGemLevel(int gemLevel)
        {
            this.ItemStatsGroup.GemLevel = gemLevel;
            return this;
        }

        public GemItemStringBuilder WithTags(string tags)
        {
            this.ItemStatsGroup.Tags = tags;
            return this;
        }

        public GemItemStringBuilder WithQuality(int quality)
        {
            this.ItemStatsGroup.Quality = quality;
            return this;
        }

        public override string Build()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder
                .Append(NameAndRarityGroup)
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