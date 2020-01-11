using POETradeHelper.Common.Extensions;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Services;
using System.Text;

namespace POETradeHelper.ItemSearch.Tests
{
    public class ItemStringBuilder
    {
        public NameAndRarityGroup NameAndRarityGroup { get; } = new NameAndRarityGroup { Rarity = "Normal", Name = "TestName" };
        public ItemStatsGroup ItemStatsGroup { get; } = new ItemStatsGroup();

        private string corrupted;

        public ItemStringBuilder WithRarity(ItemRarity rarity)
        {
            this.NameAndRarityGroup.Rarity = rarity.GetDisplayName();
            return this;
        }

        public ItemStringBuilder WithRarity(string rarity)
        {
            this.NameAndRarityGroup.Rarity = rarity;
            return this;
        }

        public ItemStringBuilder WithName(string name)
        {
            this.NameAndRarityGroup.Name = name;
            return this;
        }

        public ItemStringBuilder WithType(string type)
        {
            this.NameAndRarityGroup.Type = type;
            return this;
        }

        public ItemStringBuilder WithCorrupted()
        {
            this.corrupted = Resources.CorruptedDescriptor;
            return this;
        }

        public ItemStringBuilder WithQuality(int quality)
        {
            this.ItemStatsGroup.Quality = quality;
            return this;
        }

        public ItemStringBuilder WithGemLevel(int gemLevel)
        {
            this.ItemStatsGroup.GemLevel = gemLevel;
            return this;
        }

        public string Build()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder
                .Append(NameAndRarityGroup)
                .AppendLine(ItemParserAggregator.PropertyGroupSeparator)
                .Append(ItemStatsGroup)
                .AppendLine(ItemParserAggregator.PropertyGroupSeparator)
                .AppendLineIfNotEmpty(corrupted);

            return stringBuilder.ToString();
        }
    }

    public class NameAndRarityGroup
    {
        public string Rarity { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder
                .AppendLine($"{Resources.RarityDescriptor} {Rarity}")
                .AppendLineIfNotEmpty(Name)
                .AppendLineIfNotEmpty(Type);

            return stringBuilder.ToString();
        }
    }

    public class ItemStatsGroup
    {
        public int GemLevel { get; set; }

        public int Quality { get; set; }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder
                .AppendLine($"{Resources.LevelDescriptor} {GemLevel}", () => GemLevel > 0)
                .AppendLine($"{Resources.QualityDescriptor} +{Quality}% {Resources.AugmentedDescriptor}", () => Quality > 0);

            return stringBuilder.ToString();
        }
    }
}