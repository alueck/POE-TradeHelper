﻿using Moq;
using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;
using POETradeHelper.ItemSearch.Services.Parsers;
using POETradeHelper.ItemSearch.Tests.TestHelpers;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.ItemSearch.Tests.Services.Parsers
{
    public class EquippableItemParserTests
    {
        private Mock<ISocketsParser> socketsParserMock;
        private Mock<IItemDataService> itemDataServiceMock;
        private Mock<IItemStatsParser<ItemWithStats>> itemStatsParserMock;
        private EquippableItemParser equippableItemParser;
        private ItemStringBuilder itemStringBuilder;

        [SetUp]
        public void Setup()
        {
            this.socketsParserMock = new Mock<ISocketsParser>();
            this.itemDataServiceMock = new Mock<IItemDataService>();
            this.itemStatsParserMock = new Mock<IItemStatsParser<ItemWithStats>>();
            this.equippableItemParser = new EquippableItemParser(this.socketsParserMock.Object, this.itemDataServiceMock.Object, this.itemStatsParserMock.Object);
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

        [TestCase("Coralito's Signature")]
        [TestCase(null)]
        public void CanParseShouldReturnFalseForFlaskItem(string name)
        {
            string[] itemStringLines = this.itemStringBuilder
                                .WithItemLevel(1)
                                .WithName(name)
                                .WithType(Resources.FlaskKeyword)
                                .BuildLines();

            bool result = this.equippableItemParser.CanParse(itemStringLines);

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
        public void ParseShouldParseTypeOfIdentifiedRareItem()
        {
            const string expected = "Cutthroat's Garb";

            string[] itemStringLines = this.itemStringBuilder
                                            .WithRarity(ItemRarity.Rare)
                                            .WithName("Wrath Salvation")
                                            .WithType(expected)
                                            .BuildLines();

            EquippableItem result = this.equippableItemParser.Parse(itemStringLines) as EquippableItem;

            Assert.That(result.Type, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldCallGetTypeOnItemDataServiceForIdentifiedMagicItem()
        {
            const string expected = "Cutthroat's Garb";
            string name = $"Sanguine {expected} of the Whelpling";

            string[] itemStringLines = this.itemStringBuilder
                                            .WithRarity(ItemRarity.Magic)
                                            .WithName(name)
                                            .BuildLines();

            EquippableItem result = this.equippableItemParser.Parse(itemStringLines) as EquippableItem;

            this.itemDataServiceMock.Verify(x => x.GetType(name));
        }

        [Test]
        public void ParseShouldNotCallGetTypeOnItemDataServiceForUnidentifiedMagicItem()
        {
            const string expected = "Cutthroat's Garb";
            string name = $"Sanguine {expected} of the Whelpling";

            string[] itemStringLines = this.itemStringBuilder
                                            .WithRarity(ItemRarity.Magic)
                                            .WithName(name)
                                            .WithUnidentified()
                                            .BuildLines();

            EquippableItem result = this.equippableItemParser.Parse(itemStringLines) as EquippableItem;

            this.itemDataServiceMock.Verify(x => x.GetType(name), Times.Never);
        }

        [Test]
        public void ParseShouldSetTypeForIdentifiedMagicItemFromItemDataService()
        {
            const string expected = "Cutthroat's Garb";
            string name = $"Sanguine {expected} of the Whelpling";

            string[] itemStringLines = this.itemStringBuilder
                                            .WithRarity(ItemRarity.Magic)
                                            .WithName(name)
                                            .BuildLines();

            this.itemDataServiceMock.Setup(x => x.GetType(It.IsAny<string>()))
                .Returns(expected);

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

        [Test]
        public void ParseShouldCallParseOnItemStatsParserIfItemIsIdentified()
        {
            string[] itemStringLines = this.itemStringBuilder
                                           .WithType("Thicket Bow")
                                           .BuildLines();

            this.equippableItemParser.Parse(itemStringLines);

            this.itemStatsParserMock.Verify(x => x.Parse(itemStringLines));
        }

        [Test]
        public void ParseShouldNotCallParseOnItemStatsParserIfItemIsUnidentified()
        {
            string[] itemStringLines = this.itemStringBuilder
                               .WithType("Thicket Bow")
                               .WithUnidentified()
                               .BuildLines();

            this.equippableItemParser.Parse(itemStringLines);

            this.itemStatsParserMock.Verify(x => x.Parse(itemStringLines), Times.Never);
        }

        [Test]
        public void ParseShouldSetItemStatsFromItemStatsParserOnItem()
        {
            ItemStats expected = new ItemStats();
            string[] itemStringLines = this.itemStringBuilder
                               .WithType("Thicket Bow")
                               .BuildLines();

            this.itemStatsParserMock.Setup(x => x.Parse(It.IsAny<string[]>()))
                .Returns(expected);

            EquippableItem equippableItem = this.equippableItemParser.Parse(itemStringLines) as EquippableItem;

            Assert.That(equippableItem.Stats, Is.SameAs(expected));
        }
    }
}