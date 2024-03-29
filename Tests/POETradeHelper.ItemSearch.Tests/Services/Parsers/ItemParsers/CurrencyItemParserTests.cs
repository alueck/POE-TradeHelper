﻿using FluentAssertions;

using NUnit.Framework;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services.Parsers.ItemParsers;
using POETradeHelper.ItemSearch.Tests.TestHelpers.ItemStringBuilders;

namespace POETradeHelper.ItemSearch.Tests.Services.Parsers.ItemParsers
{
    internal class CurrencyItemParserTests : ItemParserTestsBase
    {
        private const string Currency = "Scroll of Wisdom";
        private readonly ItemStringBuilder itemStringBuilder;

        public CurrencyItemParserTests()
        {
            this.ItemParser = new CurrencyItemParser();
            this.itemStringBuilder = new ItemStringBuilder().WithRarity(ItemRarity.Currency);
        }

        [TestCase(ItemRarity.Currency, true)]
        [TestCase(ItemRarity.Gem, false)]
        [TestCase(ItemRarity.Normal, false)]
        [TestCase(ItemRarity.Magic, false)]
        [TestCase(ItemRarity.Rare, false)]
        [TestCase(ItemRarity.Unique, false)]
        [TestCase(ItemRarity.DivinationCard, false)]
        public void CanParseShouldReturnTrueIfRarityIsCurrency(ItemRarity rarity, bool expected)
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithRarity(rarity)
                .BuildLines();

            bool result = this.ItemParser.CanParse(itemStringLines);

            result.Should().Be(expected);
        }

        [Test]
        public void ParseShouldReturnCurrencyItem()
        {
            string[] itemStringLines = this.itemStringBuilder.BuildLines();

            Item item = this.ItemParser.Parse(itemStringLines);

            item.Should().BeOfType<CurrencyItem>();
        }

        [Test]
        public void ParseShouldParseCurrencyName()
        {
            string[] itemStringLines = this.GetValidItemStringLines();

            Item item = this.ItemParser.Parse(itemStringLines);

            item.Name.Should().Be(Currency);
        }

        [Test]
        public void ParseShouldParseCurrencyType()
        {
            string[] itemStringLines = this.GetValidItemStringLines();

            Item item = this.ItemParser.Parse(itemStringLines);

            item.Type.Should().Be(Currency);
        }

        protected override string[] GetValidItemStringLines()
        {
            return this.itemStringBuilder
                .WithName(Currency)
                .BuildLines();
        }
    }
}