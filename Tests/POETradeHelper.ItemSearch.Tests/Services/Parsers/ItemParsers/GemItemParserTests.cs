using System.Collections;

using NSubstitute;

using NUnit.Framework;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Services.Parsers.ItemParsers;
using POETradeHelper.ItemSearch.Tests.TestHelpers.ItemStringBuilders;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.ItemSearch.Tests.Services.Parsers.ItemParsers
{
    public class GemItemParserTests : ItemParserTestsBase
    {
        private readonly IItemDataService itemDataServiceMock;
        private readonly GemItemStringBuilder itemStringBuilder;

        public GemItemParserTests()
        {
            this.itemDataServiceMock = Substitute.For<IItemDataService>();
            this.ItemParser = new GemItemParser(this.itemDataServiceMock);
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

            bool result = this.ItemParser.CanParse(itemStringLines);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldReturnGemItem()
        {
            string[] itemStringLines = this.itemStringBuilder.BuildLines();

            Item item = this.ItemParser.Parse(itemStringLines);

            Assert.That(item, Is.InstanceOf<GemItem>());
        }

        [Test]
        public void ParseShouldCallGetTypeOnItemDataServiceWithGemName()
        {
            const string expected = "Flameblast";
            string[] itemStringLines = this.itemStringBuilder
                .WithName(expected)
                .BuildLines();

            this.ItemParser.Parse(itemStringLines);

            this.itemDataServiceMock
                .Received()
                .GetType(expected);
        }

        [Test]
        public void ParseShouldSetNameAndTypeFromItemDataService()
        {
            const string expected = "Result from ItemDataService";

            string[] itemStringLines = this.itemStringBuilder
                .WithName("Flameblast")
                .BuildLines();

            this.itemDataServiceMock.GetType(Arg.Any<string>())
                .Returns(expected);

            GemItem result = (GemItem)this.ItemParser.Parse(itemStringLines);

            Assert.That(result.Name, Is.EqualTo(expected));
            Assert.That(result.Type, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseCorruptedTrue()
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithCorrupted()
                .BuildLines();

            GemItem result = (GemItem)this.ItemParser.Parse(itemStringLines);

            Assert.IsTrue(result.IsCorrupted);
        }

        [Test]
        public void ParseShouldParseCorruptedFalse()
        {
            string[] itemStringLines = this.itemStringBuilder
                .BuildLines();

            GemItem result = (GemItem)this.ItemParser.Parse(itemStringLines);

            Assert.IsFalse(result.IsCorrupted);
        }

        [Test]
        public void ParseShouldParseQuality()
        {
            const int expected = 13;
            string[] itemStringLines = this.itemStringBuilder
                .WithQuality(expected)
                .BuildLines();

            GemItem result = (GemItem)this.ItemParser.Parse(itemStringLines);

            Assert.That(result.Quality, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseZeroQualityIfItemHasNoQuality()
        {
            const int expected = 0;
            string[] itemStringLines = this.itemStringBuilder
                .BuildLines();

            GemItem result = (GemItem)this.ItemParser.Parse(itemStringLines);

            Assert.That(result.Quality, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseGemLevel()
        {
            const int expected = 17;
            string[] itemStringLines = this.itemStringBuilder
                .WithGemLevel(expected)
                .BuildLines();

            GemItem result = (GemItem)this.ItemParser.Parse(itemStringLines);

            Assert.That(result.Level, Is.EqualTo(expected));
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

            GemItem result = (GemItem)this.ItemParser.Parse(itemStringLines);

            Assert.That(result.ExperiencePercent, Is.EqualTo(expected));
        }

        [TestCaseSource(nameof(GemQualityTypeTestCases))]
        public void ParseShouldParseGemQualityType(string qualityTypePrefix, GemQualityType expectedQualityType)
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithName($"{qualityTypePrefix}Flameblast")
                .BuildLines();

            GemItem result = (GemItem)this.ItemParser.Parse(itemStringLines);

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
            this.itemDataServiceMock
                .GetType("Anomalous Vaal Flameblast")
                .Returns("Vaal Flameblast");

            GemItem result = (GemItem)this.ItemParser.Parse(itemStringLines);

            Assert.IsNotNull(result);
            Assert.That(result.Name, Is.EqualTo("Vaal Flameblast"));
            Assert.That(result.Type, Is.EqualTo("Vaal Flameblast"));
            Assert.That(result.IsCorrupted);
            Assert.That(result.IsVaalVersion);
            Assert.That(result.Level, Is.EqualTo(20));
            Assert.That(result.Quality, Is.EqualTo(20));
            Assert.That(result.QualityType, Is.EqualTo(GemQualityType.Anomalous));
        }

        [Test]
        public void ParseShouldParseVaalGemWithDifferentNameCorrectly()
        {
            string[] itemStringLines = Properties.Resources.VaalImpurityOfLightning.Split(Environment.NewLine);
            this.itemDataServiceMock
                .GetType("Vaal Impurity of Lightning")
                .Returns("Vaal Impurity of Lightning");

            GemItem result = (GemItem)this.ItemParser.Parse(itemStringLines);

            Assert.IsNotNull(result);
            Assert.That(result.Name, Is.EqualTo("Vaal Impurity of Lightning"));
            Assert.That(result.Type, Is.EqualTo("Vaal Impurity of Lightning"));
            Assert.That(result.IsCorrupted);
            Assert.That(result.IsVaalVersion);
        }

        protected override string[] GetValidItemStringLines()
        {
            return this.itemStringBuilder
                        .WithName($"{Resources.VaalKeyword} Flameblast")
                        .WithTags($"{Resources.VaalKeyword}, Spell, AoE, Fire, Channelling")
                        .WithCorrupted()
                        .BuildLines();
        }
    }
}
