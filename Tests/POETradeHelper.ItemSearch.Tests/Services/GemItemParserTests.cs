using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services;

namespace POETradeHelper.ItemSearch.Tests.Services
{
    public class GemItemParserTests
    {
        private GemItemParser gemItemParser;
        private ItemStringBuilder itemStringBuilder;

        [SetUp]
        public void Setup()
        {
            this.gemItemParser = new GemItemParser();
            this.itemStringBuilder = new ItemStringBuilder();
        }

        [TestCase(ItemRarity.Gem, true)]
        [TestCase(ItemRarity.Normal, false)]
        [TestCase(ItemRarity.Magic, false)]
        [TestCase(ItemRarity.Rare, false)]
        [TestCase(ItemRarity.Unique, false)]
        [TestCase(ItemRarity.Currency, false)]
        [TestCase(ItemRarity.DivinationCard, false)]
        public void CanParseShouldReturnTrueIfRarityIsGem(ItemRarity rarity, bool expected)
        {
            string itemString = this.itemStringBuilder
                .WithRarity(rarity)
                .Build();

            bool result = this.gemItemParser.CanParse(itemString);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseGemName()
        {
            string expected = "Flameblast";
            string itemString = this.itemStringBuilder
                .WithRarity(ItemRarity.Gem)
                .WithName(expected)
                .Build();

            Item result = this.gemItemParser.Parse(itemString);

            Assert.That(result.Name, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldReturnGemType()
        {
            string expected = "Flameblast";
            string itemString = this.itemStringBuilder
                .WithRarity(ItemRarity.Gem)
                .WithName(expected)
                .Build();

            Item result = this.gemItemParser.Parse(itemString);

            Assert.That(result.Type, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseCorruptedTrue()
        {
            string itemString = this.itemStringBuilder
                .WithRarity(ItemRarity.Gem)
                .WithCorrupted()
                .Build();

            GemItem result = this.gemItemParser.Parse(itemString) as GemItem;

            Assert.IsTrue(result.IsCorrupted);
        }

        [Test]
        public void ParseShouldParseCorruptedFalse()
        {
            string itemString = this.itemStringBuilder
                .WithRarity(ItemRarity.Gem)
                .Build();

            GemItem result = this.gemItemParser.Parse(itemString) as GemItem;

            Assert.IsFalse(result.IsCorrupted);
        }

        [Test]
        public void ParseShouldParseQuality()
        {
            int expected = 13;
            string itemString = this.itemStringBuilder
                .WithRarity(ItemRarity.Gem)
                .WithQuality(expected)
                .Build();

            GemItem result = this.gemItemParser.Parse(itemString) as GemItem;

            Assert.That(result.Quality, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseZeroQualityIfItemHasNoQuality()
        {
            int expected = 0;
            string itemString = this.itemStringBuilder
                .WithRarity(ItemRarity.Gem)
                .Build();

            GemItem result = this.gemItemParser.Parse(itemString) as GemItem;

            Assert.That(result.Quality, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseGemLevel()
        {
            int expected = 17;
            string itemString = this.itemStringBuilder
                .WithRarity(ItemRarity.Gem)
                .WithGemLevel(expected)
                .Build();

            GemItem result = this.gemItemParser.Parse(itemString) as GemItem;

            Assert.That(result.Level, Is.EqualTo(expected));
        }
    }
}