﻿using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Services.Parsers;
using System.Collections.Generic;
using System.Text;

namespace POETradeHelper.ItemSearch.Tests.TestHelpers
{
    public class ItemStringBuilder : ItemStringBuilderBase<ItemStringBuilder>
    {
        public ItemStatsGroup ItemStatsGroup { get; private set; } = new ItemStatsGroup();

        public ICollection<string> Descriptions { get; private set; } = new List<string>();

        public int ItemLevel { get; private set; }

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

        public override string Build()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder
                .Append(this.NameAndRarityGroup)
                .AppendLine(ParserConstants.PropertyGroupSeparator)
                .Append(ItemStatsGroup)
                .AppendLine(ParserConstants.PropertyGroupSeparator, () => this.ItemLevel > 0)
                .AppendLine($"{Resources.ItemLevelDescriptor} {this.ItemLevel}", () => this.ItemLevel > 0)
                .AppendLine(ParserConstants.PropertyGroupSeparator, () => !this.IsIdentified)
                .AppendLine(Resources.UnidentifiedKeyword, () => !this.IsIdentified);

            foreach (var description in this.Descriptions)
            {
                stringBuilder
                    .AppendLine(ParserConstants.PropertyGroupSeparator)
                    .AppendLine(description);
            }

            stringBuilder
                .AppendLine(ParserConstants.PropertyGroupSeparator, () => this.IsCorrupted)
                .AppendLine(Resources.CorruptedKeyword, () => this.IsCorrupted);

            return stringBuilder.ToString();
        }
    }
}