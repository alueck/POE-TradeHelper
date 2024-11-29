using System.Text;

using POETradeHelper.Common.Extensions;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Services.Parsers;
using POETradeHelper.ItemSearch.Tests.TestHelpers.ItemStringBuilders.Models;

namespace POETradeHelper.ItemSearch.Tests.TestHelpers.ItemStringBuilders
{
    public class ItemStringBuilder : ItemStringBuilderBase<ItemStringBuilder>
    {
        public ItemStatsGroup ItemStatsGroup { get; private set; } = new ItemStatsGroup();

        public IList<ItemStat> ItemStats { get; } = [];

        public ICollection<string> Descriptions { get; private set; } = [];

        public int ItemLevel { get; private set; }

        public InfluenceType InfluenceType { get; private set; }

        public string SocketsString { get; private set; } = string.Empty;

        public ItemStringBuilder WithQuality(int quality)
        {
            this.ItemStatsGroup.Quality = quality;
            return this;
        }

        public ItemStringBuilder WithDescription(string description)
        {
            if (!string.IsNullOrEmpty(description))
            {
                this.Descriptions.Add(description);
            }

            return this;
        }

        public ItemStringBuilder WithItemLevel(int itemLevel)
        {
            this.ItemLevel = itemLevel;
            return this;
        }

        public ItemStringBuilder WithInfluence(InfluenceType influence)
        {
            this.InfluenceType = influence;
            return this;
        }

        public ItemStringBuilder WithSockets(string socketsString)
        {
            this.SocketsString = socketsString;
            return this;
        }

        public ItemStringBuilder WithItemStat(string statText, StatCategory statCategory)
        {
            this.ItemStats.Add(new ItemStat(statCategory) { Text = statText });
            return this;
        }

        public override string Build()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder
                .Append(this.NameAndRarityGroup)
                .AppendLine(ParserConstants.PropertyGroupSeparator)
                .Append(this.ItemStatsGroup)
                .AppendLine(ParserConstants.PropertyGroupSeparator, () => !string.IsNullOrEmpty(this.SocketsString))
                .AppendLine($"{Resources.SocketsDescriptor} {this.SocketsString}", () => !string.IsNullOrEmpty(this.SocketsString))
                .AppendLine(ParserConstants.PropertyGroupSeparator, () => this.ItemLevel > 0)
                .AppendLine($"{Resources.ItemLevelDescriptor} {this.ItemLevel}", () => this.ItemLevel > 0)
                .AppendLine(ParserConstants.PropertyGroupSeparator, () => !this.IsIdentified)
                .AppendLine(Resources.UnidentifiedKeyword, () => !this.IsIdentified);

            this.PrintItemStats(stringBuilder, StatCategory.Enchant);
            this.PrintItemStats(stringBuilder, StatCategory.Implicit);
            this.PrintItemStats(stringBuilder, StatCategory.Monster);
            this.PrintItemStats(stringBuilder, StatCategory.Fractured, StatCategory.Explicit, StatCategory.Crafted);

            foreach (var description in this.Descriptions)
            {
                stringBuilder
                    .AppendLine(ParserConstants.PropertyGroupSeparator)
                    .AppendLine(description);
            }

            stringBuilder
                .AppendLine(ParserConstants.PropertyGroupSeparator, () => this.IsCorrupted)
                .AppendLine(Resources.CorruptedKeyword, () => this.IsCorrupted)
                .AppendLine(ParserConstants.PropertyGroupSeparator, () => this.InfluenceType != InfluenceType.None)
                .AppendLine(this.InfluenceType.GetDisplayName(), () => this.InfluenceType != InfluenceType.None)
                .AppendLine(ParserConstants.PropertyGroupSeparator, () => this.IsSynthesised)
                .AppendLine($"{Resources.SynthesisedKeyword} Item", () => this.IsSynthesised);

            return stringBuilder.ToString();
        }

        private void PrintItemStats(StringBuilder stringBuilder, params StatCategory[] statCategories)
        {
            var groupedItemStats = this.ItemStats.GroupBy(x => x.StatCategory).ToArray();

            var sb = new StringBuilder();

            foreach (var statCategory in statCategories)
            {
                var itemStats = groupedItemStats.FirstOrDefault(x => x.Key == statCategory);

                if (itemStats != null)
                {
                    sb.AppendLine(string.Join(Environment.NewLine, itemStats.Select(x => x.Text)));
                }
            }

            if (sb.Length > 0)
            {
                stringBuilder.AppendLine(ParserConstants.PropertyGroupSeparator)
                    .AppendLine(sb.ToString());
            }
        }
    }
}