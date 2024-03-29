﻿using FluentAssertions;

using NUnit.Framework;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Services.Parsers.ItemParsers;
using POETradeHelper.ItemSearch.Tests.TestHelpers.ItemStringBuilders;

namespace POETradeHelper.ItemSearch.Tests.Services.Parsers.ItemParsers
{
    public class FragmentItemParserTests : ItemParserTestsBase
    {
        private const string Fragment = "Offering to the Goddess";
        private readonly ItemStringBuilder itemStringBuilder;

        public FragmentItemParserTests()
        {
            this.ItemParser = new FragmentItemParser();
            this.itemStringBuilder = new ItemStringBuilder();
        }

        [Test]
        public void CanParseShouldReturnTrueIfItemIsNormalRarityAndHasNoItemLevelOrProphecyDescription()
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithRarity(ItemRarity.Normal)
                .BuildLines();

            bool result = this.ItemParser.CanParse(itemStringLines);

            result.Should().BeTrue();
        }

        [Test]
        public void CanParseShouldReturnFalseIfItemIsNormalRarityButHasItemLevel()
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithRarity(ItemRarity.Normal)
                .WithItemLevel(10)
                .BuildLines();

            bool result = this.ItemParser.CanParse(itemStringLines);

            result.Should().BeFalse();
        }

        [Test]
        public void CanParseShouldReturnFalseIfItemIsNormalRarityButProphecyDescription()
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithRarity(ItemRarity.Normal)
                .WithDescription($"Right-click to add this {Resources.ProphecyKeyword} to your character.")
                .BuildLines();

            bool result = this.ItemParser.CanParse(itemStringLines);

            result.Should().BeFalse();
        }

        [Test]
        public void ParseShouldParseName()
        {
            string[] itemStringLines = this.GetValidItemStringLines();

            FragmentItem result = (FragmentItem)this.ItemParser.Parse(itemStringLines);

            result.Name.Should().Be(Fragment);
        }

        [Test]
        public void ParseShouldParseType()
        {
            string[] itemStringLines = this.GetValidItemStringLines();

            FragmentItem result = (FragmentItem)this.ItemParser.Parse(itemStringLines);

            result.Type.Should().Be(Fragment);
        }

        protected override string[] GetValidItemStringLines()
        {
            return this.itemStringBuilder
                .WithName(Fragment)
                .BuildLines();
        }
    }
}