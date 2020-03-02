﻿using Moq;
using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;
using POETradeHelper.ItemSearch.Services.Parsers;
using POETradeHelper.ItemSearch.Tests.TestHelpers;

namespace POETradeHelper.ItemSearch.Tests.Services.Parsers
{
    public class FlaskItemParserTests
    {
        private Mock<IFlaskItemStatsParser> flaskItemStatsParserMock;
        private FlaskItemParser flaskItemParser;
        private ItemStringBuilder itemStringBuilder;

        [SetUp]
        public void Setup()
        {
            this.flaskItemStatsParserMock = new Mock<IFlaskItemStatsParser>();
            this.flaskItemParser = new FlaskItemParser(this.flaskItemStatsParserMock.Object);
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

        [TestCase("Divine Life Flask")]
        [TestCase("Divine Mana Flask")]
        [TestCase("Quicksilver Flask")]
        public void ParseShouldParseFlaskType(string expected)
        {
            string name = $"Bubbling {expected} of Staunching";
            string[] itemStringLines = this.itemStringBuilder
                            .WithName(name)
                            .BuildLines();

            FlaskItem result = this.flaskItemParser.Parse(itemStringLines) as FlaskItem;

            Assert.That(result.Type, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseFlaskTypeOfUniqueFlask()
        {
            const string expected = "Silver Flask";
            string[] itemStringLines = this.itemStringBuilder
                            .WithRarity(ItemRarity.Unique)
                            .WithName("Cinderswallow Urn")
                            .WithType(expected)
                            .BuildLines();

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

            this.flaskItemStatsParserMock.Verify(x => x.Parse(itemStringLines));
        }

        [Test]
        public void ParseShouldNotCallParseOnFlaskItemStatsParserIfFlaskIsUnidentified()
        {
            string[] itemStringLines = this.itemStringBuilder
                                        .WithName("Divine Life Flask")
                                        .WithUnidentified()
                                        .BuildLines();

            this.flaskItemParser.Parse(itemStringLines);

            this.flaskItemStatsParserMock.Verify(x => x.Parse(itemStringLines), Times.Never);
        }

        [Test]
        public void ParseShouldSetStatsToStatsFromStatsDataService()
        {
            FlaskItemStats expected = new FlaskItemStats();
            string[] itemStringLines = this.itemStringBuilder
                                        .WithName("Divine Life Flask")
                                        .BuildLines();

            this.flaskItemStatsParserMock.Setup(x => x.Parse(It.IsAny<string[]>()))
                .Returns(expected);

            FlaskItem result = this.flaskItemParser.Parse(itemStringLines) as FlaskItem;

            Assert.That(result.Stats, Is.SameAs(expected));
        }
    }
}