using Moq;
using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;
using POETradeHelper.ItemSearch.Services.Parsers;
using POETradeHelper.ItemSearch.Tests.TestHelpers;

namespace POETradeHelper.ItemSearch.Tests.Services.Parsers
{
    public class EquippableItemParserTests
    {
        private Mock<ISocketsParser> socketsParserMock;
        private EquippableItemParser equippableItemParser;
        private ItemStringBuilder itemStringBuilder;

        [SetUp]
        public void Setup()
        {
            this.socketsParserMock = new Mock<ISocketsParser>();
            this.equippableItemParser = new EquippableItemParser(this.socketsParserMock.Object);
            this.itemStringBuilder = new ItemStringBuilder();
        }

        [TestCase(ItemRarity.Normal, true)]
        [TestCase(ItemRarity.Magic, true)]
        [TestCase(ItemRarity.Rare, true)]
        [TestCase(ItemRarity.Unique, true)]
        [TestCase(ItemRarity.Currency, false)]
        [TestCase(ItemRarity.DivinationCard, false)]
        [TestCase(ItemRarity.Gem, false)]
        public void CanParseShouldReturnTrueForMatchingRarityItems(ItemRarity rarity, bool expected)
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithRarity(rarity)
                .WithItemLevel(1)
                .BuildLines();

            bool result = this.equippableItemParser.CanParse(itemStringLines);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void CanParseShouldReturnFalseForItemWithoutItemLevel()
        {
            string[] itemStringLines = this.itemStringBuilder
                                            .WithItemLevel(0)
                                            .BuildLines();

            bool result = this.equippableItemParser.CanParse(itemStringLines);

            Assert.IsFalse(result);
        }

        [Test]
        public void CanParseShouldReturnFalseForItemWithMapTier()
        {
            string[] itemStringLines = this.itemStringBuilder
                                .WithItemLevel(1)
                                .WithDescription($"{Resources.MapTierDescriptor} 10")
                                .BuildLines();

            bool result = this.equippableItemParser.CanParse(itemStringLines);

            Assert.IsFalse(result);
        }

        [Test]
        public void CanParseShouldReturnFalseForMetamorphOrganItem()
        {
            string[] itemStringLines = this.itemStringBuilder
                                .WithItemLevel(1)
                                .WithDescription(Resources.OrganItemDescriptor)
                                .BuildLines();

            bool result = this.equippableItemParser.CanParse(itemStringLines);

            Assert.IsFalse(result);
        }

        [TestCase(ItemRarity.Normal)]
        [TestCase(ItemRarity.Magic)]
        [TestCase(ItemRarity.Rare)]
        [TestCase(ItemRarity.Unique)]
        public void ParseShouldParseRarity(ItemRarity expected)
        {
            string[] itemStringLines = this.itemStringBuilder
                                            .WithRarity(expected)
                                            .BuildLines();

            EquippableItem result = this.equippableItemParser.Parse(itemStringLines) as EquippableItem;

            Assert.That(result.Rarity, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseIdentified()
        {
            string[] itemStringLines = this.itemStringBuilder
                                            .BuildLines();

            EquippableItem result = this.equippableItemParser.Parse(itemStringLines) as EquippableItem;

            Assert.IsTrue(result.IsIdentified);
        }

        [Test]
        public void ParseShouldParseUnidentified()
        {
            string[] itemStringLines = this.itemStringBuilder
                                            .WithUnidentified()
                                            .BuildLines();

            EquippableItem result = this.equippableItemParser.Parse(itemStringLines) as EquippableItem;

            Assert.IsFalse(result.IsIdentified);
        }

        [Test]
        public void ParseShouldParseNameOfIdentifiedItem()
        {
            const string expected = "Wrath Salvation";

            string[] itemStringLines = this.itemStringBuilder
                                            .WithName(expected)
                                            .WithType("Cutthroat's Garb")
                                            .BuildLines();

            EquippableItem result = this.equippableItemParser.Parse(itemStringLines) as EquippableItem;

            Assert.That(result.Name, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseNameOfUnidentifiedItem()
        {
            const string expected = "Cutthroat's Garb";

            string[] itemStringLines = this.itemStringBuilder
                                            .WithRarity(ItemRarity.Magic)
                                            .WithUnidentified()
                                            .WithName(expected)
                                            .BuildLines();

            EquippableItem result = this.equippableItemParser.Parse(itemStringLines) as EquippableItem;

            Assert.That(result.Name, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseTypeOfIdentifiedItem()
        {
            const string expected = "Cutthroat's Garb";

            string[] itemStringLines = this.itemStringBuilder
                                            .WithRarity(ItemRarity.Magic)
                                            .WithName("Wrath Salvation")
                                            .WithType(expected)
                                            .BuildLines();

            EquippableItem result = this.equippableItemParser.Parse(itemStringLines) as EquippableItem;

            Assert.That(result.Type, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseTypeOfUnidentifiedItem()
        {
            const string expected = "Cutthroat's Garb";

            string[] itemStringLines = this.itemStringBuilder
                                            .WithRarity(ItemRarity.Magic)
                                            .WithUnidentified()
                                            .WithName(expected)
                                            .BuildLines();

            EquippableItem result = this.equippableItemParser.Parse(itemStringLines) as EquippableItem;

            Assert.That(result.Type, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseTypeOfUnidentifiedSuperiorItem()
        {
            const string expected = "Cutthroat's Garb";

            string[] itemStringLines = this.itemStringBuilder
                                            .WithRarity(ItemRarity.Magic)
                                            .WithUnidentified()
                                            .WithName($"{Resources.SuperiorPrefix} {expected}")
                                            .BuildLines();

            EquippableItem result = this.equippableItemParser.Parse(itemStringLines) as EquippableItem;

            Assert.That(result.Type, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseTypeOfNormalRarityItem()
        {
            const string expected = "Cutthroat's Garb";

            string[] itemStringLines = this.itemStringBuilder
                                            .WithRarity(ItemRarity.Normal)
                                            .WithType(expected)
                                            .BuildLines();

            EquippableItem result = this.equippableItemParser.Parse(itemStringLines) as EquippableItem;

            Assert.That(result.Type, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseTypeOfNormalRaritySuperiorItem()
        {
            const string expected = "Cutthroat's Garb";

            string[] itemStringLines = this.itemStringBuilder
                                            .WithRarity(ItemRarity.Normal)
                                            .WithType($"{Resources.SuperiorPrefix} {expected}")
                                            .BuildLines();

            EquippableItem result = this.equippableItemParser.Parse(itemStringLines) as EquippableItem;

            Assert.That(result.Type, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseTypeOfSynthesisedItem()
        {
            const string expected = "Cutthroat's Garb";

            string[] itemStringLines = this.itemStringBuilder
                                            .WithType($"{Resources.SynthesisedKeyword} {expected}")
                                            .BuildLines();

            EquippableItem result = this.equippableItemParser.Parse(itemStringLines) as EquippableItem;

            Assert.That(result.Type, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseCorrupted()
        {
            string[] itemStringLines = this.itemStringBuilder
                                            .WithCorrupted()
                                            .BuildLines();

            EquippableItem result = this.equippableItemParser.Parse(itemStringLines) as EquippableItem;

            Assert.IsTrue(result.IsCorrupted);
        }

        [Test]
        public void ParseShouldParseNotCorrupted()
        {
            string[] itemStringLines = this.itemStringBuilder
                                            .BuildLines();

            EquippableItem result = this.equippableItemParser.Parse(itemStringLines) as EquippableItem;

            Assert.IsFalse(result.IsCorrupted);
        }

        [TestCase(74)]
        [TestCase(100)]
        public void ParseShouldParseItemLevel(int expected)
        {
            string[] itemStringLines = this.itemStringBuilder
                                            .WithItemLevel(expected)
                                            .BuildLines();

            EquippableItem result = this.equippableItemParser.Parse(itemStringLines) as EquippableItem;

            Assert.That(result.ItemLevel, Is.EqualTo(expected));
        }

        [TestCase(10)]
        [TestCase(20)]
        public void ParseShouldParseQuality(int expected)
        {
            string[] itemStringLines = this.itemStringBuilder
                                            .WithQuality(expected)
                                            .BuildLines();

            EquippableItem result = this.equippableItemParser.Parse(itemStringLines) as EquippableItem;

            Assert.That(result.Quality, Is.EqualTo(expected));
        }

        [TestCase(InfluenceType.None)]
        [TestCase(InfluenceType.Crusader)]
        [TestCase(InfluenceType.Shaper)]
        public void ParseShouldParseInfluence(InfluenceType expected)
        {
            string[] itemStringLines = this.itemStringBuilder
                                            .WithInflucence(expected)
                                            .BuildLines();

            EquippableItem result = this.equippableItemParser.Parse(itemStringLines) as EquippableItem;

            Assert.That(result.Influence, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldCallParseOnSocketsParser()
        {
            const string socketsString = "B-G-G-G";
            string[] itemStringLines = this.itemStringBuilder
                                .WithSockets(socketsString)
                                .BuildLines();

            this.equippableItemParser.Parse(itemStringLines);

            this.socketsParserMock.Verify(x => x.Parse(socketsString));
        }

        [Test]
        public void ParseShouldSetSocketsOnItem()
        {
            string[] itemStringLines = this.itemStringBuilder
                                            .BuildLines();

            ItemSockets expected = new ItemSockets
            {
                SocketGroups =
                {
                    new SocketGroup { Sockets =
                        {
                            new Socket { SocketType = SocketType.Blue },
                        }
                    }
                }
            };

            this.socketsParserMock.Setup(x => x.Parse(It.IsAny<string>()))
                .Returns(expected);

            EquippableItem result = this.equippableItemParser.Parse(itemStringLines) as EquippableItem;

            Assert.That(result.Sockets, Is.EqualTo(expected));
        }
    }
}