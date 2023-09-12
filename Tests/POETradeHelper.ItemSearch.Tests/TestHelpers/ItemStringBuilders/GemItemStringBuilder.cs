using System.Text;

using POETradeHelper.Common.Extensions;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Services.Parsers;
using POETradeHelper.ItemSearch.Tests.TestHelpers.ItemStringBuilders.Models;

namespace POETradeHelper.ItemSearch.Tests.TestHelpers.ItemStringBuilders
{
    public class GemItemStringBuilder : ItemStringBuilderBase<GemItemStringBuilder>
    {
        public GemItemStringBuilder()
        {
            this.NameAndRarityGroup.Rarity = ItemRarity.Gem.GetDisplayName();
        }

        public GemItemStatsGroup ItemStatsGroup { get; } = new();

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

        public GemItemStringBuilder WithExperience(string experience)
        {
            this.ItemStatsGroup.Experience = experience;
            return this;
        }

        public override string Build()
        {
            StringBuilder stringBuilder = new();

            stringBuilder
                .Append(this.NameAndRarityGroup)
                .AppendLine(ParserConstants.PropertyGroupSeparator)
                .Append(this.ItemStatsGroup)
                .AppendLine(ParserConstants.PropertyGroupSeparator, () => !this.IsIdentified)
                .AppendLine(Resources.UnidentifiedKeyword, () => !this.IsIdentified)
                .AppendLine(ParserConstants.PropertyGroupSeparator, () => this.IsCorrupted)
                .AppendLine(Resources.CorruptedKeyword, () => this.IsCorrupted);

            return stringBuilder.ToString();
        }
    }
}