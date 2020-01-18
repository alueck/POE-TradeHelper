using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Services;
using POETradeHelper.ItemSearch.Tests.TestHelpers;

namespace POETradeHelper.ItemSearch.Tests.Services
{
    public class GemItemParserTests
    {
        private GemItemParser gemItemParser;
        private GemItemStringBuilder itemStringBuilder;

        [SetUp]
        public void Setup()
        {
            this.gemItemParser = new GemItemParser();
            this.itemStringBuilder = new GemItemStringBuilder().WithRarity(ItemRarity.Gem);
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
            string[] itemStringLines = this.itemStringBuilder
                .WithRarity(rarity)
                .BuildLines();

            bool result = this.gemItemParser.CanParse(itemStringLines);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldReturnGemItem()
        {
            string[] itemStringLines = this.itemStringBuilder.BuildLines();

            Item item = this.gemItemParser.Parse(itemStringLines);

            Assert.That(item, Is.InstanceOf<GemItem>());
        }

        [Test]
        public void ParseShouldParseGemName()
        {
            string expected = "Flameblast";
            string[] itemStringLines = this.itemStringBuilder
                .WithName(expected)
                .BuildLines();

            Item result = this.gemItemParser.Parse(itemStringLines);

            Assert.That(result.Name, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldReturnGemType()
        {
            string expected = "Flameblast";
            string[] itemStringLines = this.itemStringBuilder
                .WithName(expected)
                .BuildLines();

            Item result = this.gemItemParser.Parse(itemStringLines);

            Assert.That(result.Type, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseCorruptedTrue()
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithCorrupted()
                .BuildLines();

            GemItem result = this.gemItemParser.Parse(itemStringLines) as GemItem;

            Assert.IsTrue(result.IsCorrupted);
        }

        [Test]
        public void ParseShouldParseCorruptedFalse()
        {
            string[] itemStringLines = this.itemStringBuilder
                .BuildLines();

            GemItem result = this.gemItemParser.Parse(itemStringLines) as GemItem;

            Assert.IsFalse(result.IsCorrupted);
        }

        [Test]
        public void ParseShouldParseQuality()
        {
            int expected = 13;
            string[] itemStringLines = this.itemStringBuilder
                .WithQuality(expected)
                .BuildLines();

            GemItem result = this.gemItemParser.Parse(itemStringLines) as GemItem;

            Assert.That(result.Quality, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseZeroQualityIfItemHasNoQuality()
        {
            int expected = 0;
            string[] itemStringLines = this.itemStringBuilder
                .BuildLines();

            GemItem result = this.gemItemParser.Parse(itemStringLines) as GemItem;

            Assert.That(result.Quality, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseGemLevel()
        {
            int expected = 17;
            string[] itemStringLines = this.itemStringBuilder
                .WithGemLevel(expected)
                .BuildLines();

            GemItem result = this.gemItemParser.Parse(itemStringLines) as GemItem;

            Assert.That(result.Level, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseVaalGems()
        {
            const string name = "Flameblast";
            string expected = $"{Resources.VaalDescriptor} {name}";

            string[] itemStringLines = this.itemStringBuilder
                .WithName(name)
                .WithTags($"{Resources.VaalDescriptor}, Spell, AoE, Fire, Channelling")
                .WithCorrupted()
                .BuildLines();

            GemItem result = this.gemItemParser.Parse(itemStringLines) as GemItem;

            Assert.IsTrue(result.IsVaalVersion);
            Assert.That(result.Name, Is.EqualTo(expected));
            Assert.That(result.Type, Is.EqualTo(expected));
        }
    }
}