using Moq;
using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;
using POETradeHelper.ItemSearch.Services.Parsers;
using POETradeHelper.ItemSearch.Tests.TestHelpers;

namespace POETradeHelper.ItemSearch.Tests.Services.Parsers
{
    public class JewelItemParserTests
    {
        private Mock<IItemTypeParser> itemTypeParserMock;
        private Mock<IItemStatsParser<ItemWithStats>> itemStatsParserMock;
        private JewelItemParser jewelItemParser;
        private ItemStringBuilder itemStringBuilder;

        [SetUp]
        public void Setup()
        {
            this.itemTypeParserMock = new Mock<IItemTypeParser>();
            this.itemStatsParserMock = new Mock<IItemStatsParser<ItemWithStats>>();
            this.jewelItemParser = new JewelItemParser(this.itemTypeParserMock.Object, this.itemStatsParserMock.Object);
            this.itemStringBuilder = new ItemStringBuilder();
        }

        [Test]
        public void CanParseShouldReturnTrueIfTypeContainsJewel()
        {
            string[] itemStringLines = this.itemStringBuilder
                                        .WithRarity(ItemRarity.Magic)
                                        .WithType("Pyromantic Cobalt Jewel")
                                        .BuildLines();

            bool result = this.jewelItemParser.CanParse(itemStringLines);

            Assert.IsTrue(result);
        }

        [Test]
        public void CanParseShouldReturnTrueIfTypeContainsJewelForRareJewel()
        {
            string[] itemStringLines = this.itemStringBuilder
                                        .WithRarity(ItemRarity.Rare)
                                        .WithName("Armageddon Joy")
                                        .WithType("Cobalt Jewel")
                                        .BuildLines();

            bool result = this.jewelItemParser.CanParse(itemStringLines);

            Assert.IsTrue(result);
        }

        [Test]
        public void CanParseShouldReturnTrueIfTypeContainsJewelForUnidentifiedRareJewel()
        {
            string[] itemStringLines = this.itemStringBuilder
                                        .WithRarity(ItemRarity.Rare)
                                        .WithType("Cobalt Jewel")
                                        .WithUnidentified()
                                        .BuildLines();

            bool result = this.jewelItemParser.CanParse(itemStringLines);

            Assert.IsTrue(result);
        }

        [TestCase(ItemRarity.Magic)]
        [TestCase(ItemRarity.Rare)]
        [TestCase(ItemRarity.Unique)]
        public void ParseShouldParseRarity(ItemRarity expected)
        {
            string[] itemStringLines = this.itemStringBuilder
                                            .WithRarity(expected)
                                            .BuildLines();

            JewelItem result = this.jewelItemParser.Parse(itemStringLines) as JewelItem;

            Assert.That(result.Rarity, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseIdentified()
        {
            string[] itemStringLines = this.itemStringBuilder
                                            .BuildLines();

            JewelItem result = this.jewelItemParser.Parse(itemStringLines) as JewelItem;

            Assert.IsTrue(result.IsIdentified);
        }

        [Test]
        public void ParseShouldParseUnidentified()
        {
            string[] itemStringLines = this.itemStringBuilder
                                            .WithUnidentified()
                                            .BuildLines();

            JewelItem result = this.jewelItemParser.Parse(itemStringLines) as JewelItem;

            Assert.IsFalse(result.IsIdentified);
        }

        [Test]
        public void ParseShouldParseNameOfMagicJewel()
        {
            const string expected = "Pyromantic Cobalt Jewel";

            string[] itemStringLines = this.itemStringBuilder
                                            .WithRarity(ItemRarity.Magic)
                                            .WithName(expected)
                                            .BuildLines();

            JewelItem result = this.jewelItemParser.Parse(itemStringLines) as JewelItem;

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
                                .WithName("Cobalt Jewel")
                                .BuildLines();

            this.itemTypeParserMock.Setup(x => x.ParseType(itemStringLines, itemRarity, isIdentified))
                .Returns(expected);

            JewelItem result = this.jewelItemParser.Parse(itemStringLines) as JewelItem;

            Assert.That(result.Type, Is.EqualTo(expected));
        }

        [TestCase(ItemRarity.Rare)]
        [TestCase(ItemRarity.Unique)]
        public void ParseShouldParseNameOfRareIdentifiedJewel(ItemRarity itemRarity)
        {
            const string expected = "Armageddon Joy";

            string[] itemStringLines = this.itemStringBuilder
                                            .WithRarity(itemRarity)
                                            .WithName(expected)
                                            .WithType("Cobalt Jewel")
                                            .BuildLines();

            JewelItem result = this.jewelItemParser.Parse(itemStringLines) as JewelItem;

            Assert.That(result.Name, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseCorrupted()
        {
            string[] itemStringLines = this.itemStringBuilder
                                            .WithCorrupted()
                                            .BuildLines();

            JewelItem result = this.jewelItemParser.Parse(itemStringLines) as JewelItem;

            Assert.IsTrue(result.IsCorrupted);
        }

        [Test]
        public void ParseShouldParseNotCorrupted()
        {
            string[] itemStringLines = this.itemStringBuilder
                                            .BuildLines();

            JewelItem result = this.jewelItemParser.Parse(itemStringLines) as JewelItem;

            Assert.IsFalse(result.IsCorrupted);
        }

        [Test]
        public void ParseShouldCallParseOnItemStatsParserIfItemIsIdentified()
        {
            string[] itemStringLines = this.itemStringBuilder
                                .BuildLines();

            this.jewelItemParser.Parse(itemStringLines);

            this.itemStatsParserMock.Verify(x => x.Parse(itemStringLines));
        }

        [Test]
        public void ParseShouldNotCallParseOnItemStatsParserIfItemIsUnidentified()
        {
            string[] itemStringLines = this.itemStringBuilder
                                .WithUnidentified()
                                .BuildLines();

            this.jewelItemParser.Parse(itemStringLines);

            this.itemStatsParserMock.Verify(x => x.Parse(itemStringLines), Times.Never);
        }
    }
}