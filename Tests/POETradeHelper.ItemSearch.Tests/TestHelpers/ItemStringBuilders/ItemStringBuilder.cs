using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using POETradeHelper.Common.Extensions;
using POETradeHelper.ItemSearch.Contract;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Services.Parsers;

namespace POETradeHelper.ItemSearch.Tests.TestHelpers
{
    public class ItemStringBuilder : ItemStringBuilderBase<ItemStringBuilder>
    {
        public ItemStatsGroup ItemStatsGroup { get; private set; } = new ItemStatsGroup();

        public IList<ItemStat> ItemStats { get; } = new List<ItemStat>();

        public ICollection<string> Descriptions { get; private set; } = new List<string>();

        public int ItemLevel { get; private set; }
        public InfluenceType InfluenceType { get; private set; }
        public string SocketsString { get; private set; }

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

        public ItemStringBuilder WithInflucence(InfluenceType influence)
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
                .Append(ItemStatsGroup)
                .AppendLine(ParserConstants.PropertyGroupSeparator, () => !string.IsNullOrEmpty(this.SocketsString))
                .AppendLine($"{Resources.SocketsDescriptor} {this.SocketsString}", () => !string.IsNullOrEmpty(this.SocketsString))
                .AppendLine(ParserConstants.PropertyGroupSeparator, () => this.ItemLevel > 0)
                .AppendLine($"{Resources.ItemLevelDescriptor} {this.ItemLevel}", () => this.ItemLevel > 0)
                .AppendLine(ParserConstants.PropertyGroupSeparator, () => !this.IsIdentified)
                .AppendLine(Resources.UnidentifiedKeyword, () => !this.IsIdentified);

            this.PrintItemStats(stringBuilder, StatCategory.Enchant);
            this.PrintItemStats(stringBuilder, StatCategory.Implicit);
            this.PrintItemStats(stringBuilder, StatCategory.Fractured);
            this.PrintItemStats(stringBuilder, StatCategory.Monster);
            this.PrintItemStats(stringBuilder, StatCategory.Explicit);
            this.PrintItemStats(stringBuilder, StatCategory.Crafted, false);

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
                .AppendLine(this.InfluenceType.GetDisplayName(), () => this.InfluenceType != InfluenceType.None);

            return stringBuilder.ToString();
        }

        private void PrintItemStats(StringBuilder stringBuilder, StatCategory statCategory, bool printPropertyGroupSeparator = true)
        {
            var implicitItemStats = this.ItemStats.Where(x => x.StatCategory == statCategory);
            if (implicitItemStats.Any())
            {
                stringBuilder.AppendLine(ParserConstants.PropertyGroupSeparator, () => printPropertyGroupSeparator)
                             .AppendLine(string.Join(Environment.NewLine, implicitItemStats.Select(x => x.Text)));
            }
        }
    }
}