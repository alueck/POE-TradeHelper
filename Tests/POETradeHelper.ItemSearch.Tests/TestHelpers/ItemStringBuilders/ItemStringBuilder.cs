using POETradeHelper.Common.Extensions;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Services.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POETradeHelper.ItemSearch.Tests.TestHelpers
{
    public class ItemStringBuilder : ItemStringBuilderBase<ItemStringBuilder>
    {
        public ItemStatsGroup ItemStatsGroup { get; private set; } = new ItemStatsGroup();

        public IList<ExplicitItemStat> ExplicitItemStats { get; } = new List<ExplicitItemStat>();

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

        public ItemStringBuilder WithExplicitItemStat(string statText)
        {
            this.ExplicitItemStats.Add(new ExplicitItemStat { Text = statText });
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
                .AppendLine(Resources.UnidentifiedKeyword, () => !this.IsIdentified)
                .AppendLine(ParserConstants.PropertyGroupSeparator, () => this.ExplicitItemStats.Any())
                .AppendLine(string.Join(Environment.NewLine, this.ExplicitItemStats.Select(x => x.Text)));

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
    }
}