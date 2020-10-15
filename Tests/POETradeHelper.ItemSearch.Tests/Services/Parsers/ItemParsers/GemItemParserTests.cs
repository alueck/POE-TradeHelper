using System;
using System.Collections;
using Moq;
using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Services.Parsers;
using POETradeHelper.ItemSearch.Tests.TestHelpers;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.ItemSearch.Tests.Services.Parsers
{
    public class GemItemParserTests
    {
        private Mock<IItemDataService> itemDataServiceMock;
        private GemItemParser gemItemParser;
        private GemItemStringBuilder itemStringBuilder;

        [SetUp]
        public void Setup()
        {
            this.itemDataServiceMock = new Mock<IItemDataService>();
            this.gemItemParser = new GemItemParser(this.itemDataServiceMock.Object);
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
        public void ParseShouldCallGetTypeOnItemDataServiceWithGemName()
        {
            string expected = "Flameblast";
            string[] itemStringLines = this.itemStringBuilder
                .WithName(expected)
                .BuildLines();

            this.gemItemParser.Parse(itemStringLines);

            this.itemDataServiceMock.Verify(x => x.GetType(expected));
        }

        [Test]
        public void ParseShouldSetNameAndTypeFromItemDataService()
        {
            const string expected = "Result from ItemDataService";

            string[] itemStringLines = this.itemStringBuilder
                .WithName("Flameblast")
                .BuildLines();

            this.itemDataServiceMock.Setup(x => x.GetType(It.IsAny<string>()))
                .Returns(expected);

            GemItem result = this.gemItemParser.Parse(itemStringLines) as GemItem;

            Assert.That(result.Name, Is.EqualTo(expected));
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
            string expected = $"{Resources.VaalKeyword} {name}";

            string[] itemStringLines = this.itemStringBuilder
                .WithName(name)
                .WithTags($"{Resources.VaalKeyword}, Spell, AoE, Fire, Channelling")
                .WithCorrupted()
                .BuildLines();

            GemItem result = this.gemItemParser.Parse(itemStringLines) as GemItem;

            Assert.IsTrue(result.IsVaalVersion);
            Assert.That(result.Name, Is.EqualTo(expected));
            Assert.That(result.Type, Is.EqualTo(expected));
        }

        [TestCase("150/1.000", 15)]
        [TestCase("123/1.000", 12)]
        [TestCase("129/1.000", 12)]
        public void ParseShouldParseGemExperiencePercent(string experience, int expected)
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithName("Flameblast")
                .WithExperience(experience)
                .BuildLines();

            GemItem result = this.gemItemParser.Parse(itemStringLines) as GemItem;

            Assert.That(result.ExperiencePercent, Is.EqualTo(expected));
        }

        [TestCaseSource(nameof(GemQualityTypeTestCases))]
        public void ParseShouldParseGemQualityType(string qualityTypePrefix, GemQualityType expectedQualityType)
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithName($"{qualityTypePrefix}Flameblast")
                .BuildLines();

            GemItem result = this.gemItemParser.Parse(itemStringLines) as GemItem;

            Assert.That(result.QualityType, Is.EqualTo(expectedQualityType));
        }

        private static IEnumerable GemQualityTypeTestCases
        {
            get
            {
                yield return new object[] { "", GemQualityType.Default };
                yield return new object[] { $"{Resources.GemQualityType_Anomalous} ", GemQualityType.Anomalous };
                yield return new object[] { $"{Resources.GemQualityType_Divergent} ", GemQualityType.Divergent };
                yield return new object[] { $"{Resources.GemQualityType_Phantasmal} ", GemQualityType.Phantasmal };
            }
        }

        [Test]
        public void ParseShouldParseVaalGem()
        {
            string[] itemStringLines = Properties.Resources.AnomalousVaalFlameblast.Split(Environment.NewLine);

            GemItem result = this.gemItemParser.Parse(itemStringLines) as GemItem;

            Assert.IsNotNull(result);
            Assert.That(result.Name, Is.EqualTo("Vaal Flameblast"));
            Assert.That(result.Type, Is.EqualTo("Vaal Flameblast"));
            Assert.That(result.IsCorrupted);
            Assert.That(result.IsVaalVersion);
            Assert.That(result.Level, Is.EqualTo(20));
            Assert.That(result.Quality, Is.EqualTo(20));
            Assert.That(result.QualityType, Is.EqualTo(GemQualityType.Anomalous));
        }
    }
}