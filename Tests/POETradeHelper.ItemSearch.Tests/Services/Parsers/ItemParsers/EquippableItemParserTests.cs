using Moq;
using NUnit.Framework;
using POETradeHelper.Common.Extensions;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;
using POETradeHelper.ItemSearch.Services.Parsers;
using POETradeHelper.ItemSearch.Tests.TestHelpers;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.ItemSearch.Tests.Services.Parsers
{
    public class EquippableItemParserTests : ItemParserTestsBase
    {
        private Mock<ISocketsParser> socketsParserMock;
        private Mock<IItemTypeParser> itemTypeParserMock;
        private Mock<IItemStatsParser<ItemWithStats>> itemStatsParserMock;
        private Mock<IItemDataService> itemDataServiceMock;
        private ItemStringBuilder itemStringBuilder;

        [SetUp]
        public void Setup()
        {
            this.socketsParserMock = new Mock<ISocketsParser>();
            this.itemTypeParserMock = new Mock<IItemTypeParser>();
            this.itemStatsParserMock = new Mock<IItemStatsParser<ItemWithStats>>();
            this.itemDataServiceMock = new Mock<IItemDataService>();
            this.ItemParser = new EquippableItemParser(
                this.socketsParserMock.Object,
                this.itemTypeParserMock.Object,
                this.itemStatsParserMock.Object,
                this.itemDataServiceMock.Object);
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

            bool result = this.ItemParser.CanParse(itemStringLines);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void CanParseShouldReturnFalseForItemWithoutItemLevel()
        {
            string[] itemStringLines = this.itemStringBuilder
                                            .WithItemLevel(0)
                                            .BuildLines();

            bool result = this.ItemParser.CanParse(itemStringLines);

            Assert.IsFalse(result);
        }

        [Test]
        public void CanParseShouldReturnFalseForItemWithMapTier()
        {
            string[] itemStringLines = this.itemStringBuilder
                                .WithItemLevel(1)
                                .WithDescription($"{Resources.MapTierDescriptor} 10")
                                .BuildLines();

            bool result = this.ItemParser.CanParse(itemStringLines);

            Assert.IsFalse(result);
        }

        [Test]
        public void CanParseShouldReturnFalseForMetamorphOrganItem()
        {
            string[] itemStringLines = this.itemStringBuilder
                                .WithItemLevel(1)
                                .WithDescription(Resources.OrganItemDescriptor)
                                .BuildLines();

            bool result = this.ItemParser.CanParse(itemStringLines);

            Assert.IsFalse(result);
        }

        [TestCase("Coralito's Signature")]
        [TestCase(null)]
        public void CanParseShouldReturnFalseForFlaskItem(string name)
        {
            string[] itemStringLines = this.itemStringBuilder
                                .WithItemLevel(1)
                                .WithName(name)
                                .WithType(Resources.FlaskKeyword)
                                .BuildLines();

            bool result = this.ItemParser.CanParse(itemStringLines);

            Assert.IsFalse(result);
        }

        [TestCase("Kitava's Teachings")]
        [TestCase(null)]
        public void CanParseShouldReturnFalseForJewelItem(string name)
        {
            string[] itemStringLines = this.itemStringBuilder
                                .WithItemLevel(1)
                                .WithName(name)
                                .WithType(Resources.JewelKeyword)
                                .BuildLines();

            bool result = this.ItemParser.CanParse(itemStringLines);

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

            EquippableItem result = this.ItemParser.Parse(itemStringLines) as EquippableItem;

            Assert.That(result.Rarity, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseIdentified()
        {
            string[] itemStringLines = this.itemStringBuilder
                                            .BuildLines();

            EquippableItem result = this.ItemParser.Parse(itemStringLines) as EquippableItem;

            Assert.IsTrue(result.IsIdentified);
        }

        [Test]
        public void ParseShouldParseUnidentified()
        {
            string[] itemStringLines = this.itemStringBuilder
                                            .WithUnidentified()
                                            .BuildLines();

            EquippableItem result = this.ItemParser.Parse(itemStringLines) as EquippableItem;

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

            EquippableItem result = this.ItemParser.Parse(itemStringLines) as EquippableItem;

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

            EquippableItem result = this.ItemParser.Parse(itemStringLines) as EquippableItem;

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
                                .WithName("Cutthroat's Garb")
                                .BuildLines();

            this.itemTypeParserMock.Setup(x => x.ParseType(itemStringLines, itemRarity, isIdentified))
                .Returns(expected);

            var result = this.ItemParser.Parse(itemStringLines) as EquippableItem;

            Assert.That(result.Type, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseCorrupted()
        {
            string[] itemStringLines = this.itemStringBuilder
                                            .WithCorrupted()
                                            .BuildLines();

            EquippableItem result = this.ItemParser.Parse(itemStringLines) as EquippableItem;

            Assert.IsTrue(result.IsCorrupted);
        }

        [Test]
        public void ParseShouldParseNotCorrupted()
        {
            string[] itemStringLines = this.itemStringBuilder
                                            .BuildLines();

            EquippableItem result = this.ItemParser.Parse(itemStringLines) as EquippableItem;

            Assert.IsFalse(result.IsCorrupted);
        }

        [TestCase(74)]
        [TestCase(100)]
        public void ParseShouldParseItemLevel(int expected)
        {
            string[] itemStringLines = this.itemStringBuilder
                                            .WithItemLevel(expected)
                                            .BuildLines();

            EquippableItem result = this.ItemParser.Parse(itemStringLines) as EquippableItem;

            Assert.That(result.ItemLevel, Is.EqualTo(expected));
        }

        [TestCase(10)]
        [TestCase(20)]
        public void ParseShouldParseQuality(int expected)
        {
            string[] itemStringLines = this.itemStringBuilder
                                            .WithQuality(expected)
                                            .BuildLines();

            EquippableItem result = this.ItemParser.Parse(itemStringLines) as EquippableItem;

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

            EquippableItem result = this.ItemParser.Parse(itemStringLines) as EquippableItem;

            Assert.That(result.Influence, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldCallParseOnSocketsParser()
        {
            const string socketsString = "B-G-G-G";
            string[] itemStringLines = this.itemStringBuilder
                                .WithSockets(socketsString)
                                .BuildLines();

            this.ItemParser.Parse(itemStringLines);

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

            EquippableItem result = this.ItemParser.Parse(itemStringLines) as EquippableItem;

            Assert.That(result.Sockets, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldCallParseOnItemStatsParserIfItemIsIdentified()
        {
            string[] itemStringLines = this.itemStringBuilder
                                           .WithType("Thicket Bow")
                                           .BuildLines();

            this.ItemParser.Parse(itemStringLines);

            this.itemStatsParserMock.Verify(x => x.Parse(itemStringLines, It.IsAny<bool>()));
        }

        [TestCase(EquippableItemCategory.Unknown, false)]
        [TestCase(EquippableItemCategory.Accessories, false)]
        [TestCase(EquippableItemCategory.Armour, true)]
        [TestCase(EquippableItemCategory.Weapons, true)]
        public void ParseShouldCallParseOnItemStatsParserWithPreferLocalStats(EquippableItemCategory itemCategory, bool expectedPreferLocalStats)
        {
            string[] itemStringLines = this.itemStringBuilder
                                           .WithType("Thicket Bow")
                                           .BuildLines();

            this.itemDataServiceMock.Setup(x => x.GetCategory(It.IsAny<string>()))
                .Returns(itemCategory.GetDisplayName());

            this.ItemParser.Parse(itemStringLines);

            this.itemStatsParserMock.Verify(x => x.Parse(itemStringLines, expectedPreferLocalStats));
        }

        [Test]
        public void ParseShouldNotCallParseOnItemStatsParserIfItemIsUnidentified()
        {
            string[] itemStringLines = this.itemStringBuilder
                               .WithType("Thicket Bow")
                               .WithUnidentified()
                               .BuildLines();

            this.ItemParser.Parse(itemStringLines);

            this.itemStatsParserMock.Verify(x => x.Parse(It.IsAny<string[]>(), It.IsAny<bool>()), Times.Never);
        }

        [Test]
        public void ParseShouldSetItemStatsFromItemStatsParserOnItem()
        {
            ItemStats expected = new ItemStats();
            string[] itemStringLines = this.itemStringBuilder
                               .WithType("Thicket Bow")
                               .BuildLines();

            this.itemStatsParserMock.Setup(x => x.Parse(It.IsAny<string[]>(), It.IsAny<bool>()))
                .Returns(expected);

            EquippableItem equippableItem = this.ItemParser.Parse(itemStringLines) as EquippableItem;

            Assert.That(equippableItem.Stats, Is.SameAs(expected));
        }

        [Test]
        public void ParseShouldCallGetCategoryOnItemDataService()
        {
            // arrange
            string[] itemStringLines = this.itemStringBuilder.BuildLines();
            const string itemType = "parsed item type";

            this.itemTypeParserMock.Setup(x => x.ParseType(It.IsAny<string[]>(), It.IsAny<ItemRarity>(), It.IsAny<bool>()))
                .Returns(itemType);

            // act
            this.ItemParser.Parse(itemStringLines);

            // assert
            this.itemDataServiceMock.Verify(x => x.GetCategory(itemType));
        }

        [TestCase(null, EquippableItemCategory.Unknown)]
        [TestCase(EquippableItemCategory.Accessories, EquippableItemCategory.Accessories)]
        [TestCase(EquippableItemCategory.Armour, EquippableItemCategory.Armour)]
        [TestCase(EquippableItemCategory.Weapons, EquippableItemCategory.Weapons)]
        public void ParseShouldSetCategoryFromItemDataService(EquippableItemCategory? itemCategory, EquippableItemCategory expectedItemCategory)
        {
            // arrange
            string[] itemStringLines = this.itemStringBuilder.BuildLines();

            this.itemDataServiceMock.Setup(x => x.GetCategory(It.IsAny<string>()))
                .Returns(itemCategory?.GetDisplayName());

            // act
            EquippableItem result = this.ItemParser.Parse(itemStringLines) as EquippableItem;

            // assert
            Assert.That(result.Category, Is.EqualTo(expectedItemCategory));
        }

        protected override string[] GetValidItemStringLines()
        {
            return this.itemStringBuilder
                        .WithRarity(ItemRarity.Rare)
                        .WithType("Thicket Bow")
                        .WithName("Woe Rain")
                        .WithItemLevel(85)
                        .WithSockets("G-G-G")
                        .BuildLines();
        }
    }
}