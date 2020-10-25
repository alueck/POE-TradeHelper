using Moq;
using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;
using POETradeHelper.ItemSearch.Services.Parsers;
using POETradeHelper.ItemSearch.Tests.TestHelpers;

namespace POETradeHelper.ItemSearch.Tests.Services.Parsers
{
    public class FlaskItemParserTests
    {
        private Mock<IItemTypeParser> itemTypeParserMock;
        private Mock<IItemStatsParser<ItemWithStats>> itemStatsParserMock;
        private FlaskItemParser flaskItemParser;
        private ItemStringBuilder itemStringBuilder;

        [SetUp]
        public void Setup()
        {
            this.itemTypeParserMock = new Mock<IItemTypeParser>();
            this.itemStatsParserMock = new Mock<IItemStatsParser<ItemWithStats>>();
            this.flaskItemParser = new FlaskItemParser(this.itemTypeParserMock.Object, this.itemStatsParserMock.Object);
            this.itemStringBuilder = new ItemStringBuilder();
        }

        [TestCase("Bubbling Divine Life Flask of Staunching")]
        [TestCase("Flask")]
        public void CanParseShouldReturnTrueIfNameContainsFlask(string name)
        {
            string[] itemStringLines = this.itemStringBuilder
                                        .WithName(name)
                                        .BuildLines();

            bool result = this.flaskItemParser.CanParse(itemStringLines);

            Assert.IsTrue(result);
        }

        [Test]
        public void CanParseShouldReturnTrueIfTypeOfUniqueFlaskContainsFlask()
        {
            string[] itemStringLines = this.itemStringBuilder
                                        .WithRarity(ItemRarity.Unique)
                                        .WithName("Cinderswallow Urn")
                                        .WithType("Silver Flask")
                                        .BuildLines();

            bool result = this.flaskItemParser.CanParse(itemStringLines);

            Assert.IsTrue(result);
        }

        [Test]
        public void CanParseShouldReturnFalseIfNameDoesNotContainFlask()
        {
            string[] itemStringLines = this.itemStringBuilder
                                        .WithName("Scroll of Wisdom")
                                        .BuildLines();

            bool result = this.flaskItemParser.CanParse(itemStringLines);

            Assert.IsFalse(result);
        }

        [Test]
        public void ParseShoudParseFlaskRarity()
        {
            const ItemRarity expected = ItemRarity.Unique;
            string[] itemStringLines = this.itemStringBuilder
                .WithRarity(expected)
                .BuildLines();

            FlaskItem result = this.flaskItemParser.Parse(itemStringLines) as FlaskItem;

            Assert.That(result.Rarity, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseFlaskName()
        {
            const string expected = "Bubbling Divine Life Flask of Staunching";
            string[] itemStringLines = this.itemStringBuilder
                            .WithName(expected)
                            .BuildLines();

            FlaskItem result = this.flaskItemParser.Parse(itemStringLines) as FlaskItem;

            Assert.That(result.Name, Is.EqualTo(expected));
        }

        [TestCase(ItemRarity.Magic, true)]
        [TestCase(ItemRarity.Rare, true)]
        [TestCase(ItemRarity.Unique, true)]
        [TestCase(ItemRarity.Normal, false)]
        [TestCase(ItemRarity.Magic, false)]
        [TestCase(ItemRarity.Rare, false)]
        [TestCase(ItemRarity.Unique, false)]
        public void ParseShouldSetTypeFromItemTypeParser(ItemRarity itemRarity, bool isIdentified)
        {
            const string expected = "Result from ItemTypeParser";
            string[] itemStringLines = this.itemStringBuilder
                            .WithRarity(itemRarity)
                            .WithIdentified(isIdentified)
                            .WithName("Cinderswallow Urn")
                            .BuildLines();

            this.itemTypeParserMock.Setup(x => x.ParseType(itemStringLines, itemRarity, isIdentified))
                .Returns(expected);

            FlaskItem result = this.flaskItemParser.Parse(itemStringLines) as FlaskItem;

            Assert.That(result.Type, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseFlaskQuality()
        {
            const int expected = 20;
            string[] itemStringLines = this.itemStringBuilder
                            .WithName("Bubbling Divine Life Flask of Staunching")
                            .WithQuality(expected)
                            .BuildLines();

            FlaskItem result = this.flaskItemParser.Parse(itemStringLines) as FlaskItem;

            Assert.That(result.Quality, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseFlaskIdentified()
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithName("Bubbling Divine Life Flask of Staunching")
                .BuildLines();

            FlaskItem result = this.flaskItemParser.Parse(itemStringLines) as FlaskItem;

            Assert.IsTrue(result.IsIdentified);
        }

        [Test]
        public void ParseShouldParseFlaskUnidentified()
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithName("Bubbling Divine Life Flask of Staunching")
                .WithUnidentified()
                .BuildLines();

            FlaskItem result = this.flaskItemParser.Parse(itemStringLines) as FlaskItem;

            Assert.IsFalse(result.IsIdentified);
        }

        [Test]
        public void ParseShouldCallParseOnFlaskItemStatsParserIfFlaskIsIdentified()
        {
            string[] itemStringLines = this.itemStringBuilder
                                        .WithName("Divine Life Flask")
                                        .BuildLines();

            this.flaskItemParser.Parse(itemStringLines);

            this.itemStatsParserMock.Verify(x => x.Parse(itemStringLines, false));
        }

        [Test]
        public void ParseShouldNotCallParseOnFlaskItemStatsParserIfFlaskIsUnidentified()
        {
            string[] itemStringLines = this.itemStringBuilder
                                        .WithName("Divine Life Flask")
                                        .WithUnidentified()
                                        .BuildLines();

            this.flaskItemParser.Parse(itemStringLines);

            this.itemStatsParserMock.Verify(x => x.Parse(It.IsAny<string[]>(), It.IsAny<bool>()), Times.Never);
        }

        [Test]
        public void ParseShouldSetStatsToStatsFromStatsDataService()
        {
            ItemStats expected = new ItemStats();
            string[] itemStringLines = this.itemStringBuilder
                                        .WithName("Divine Life Flask")
                                        .BuildLines();

            this.itemStatsParserMock.Setup(x => x.Parse(It.IsAny<string[]>(), false))
                .Returns(expected);

            FlaskItem result = this.flaskItemParser.Parse(itemStringLines) as FlaskItem;

            Assert.That(result.Stats, Is.SameAs(expected));
        }
    }
}