﻿using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Services.Parsing;
using POETradeHelper.ItemSearch.Tests.TestHelpers;
using SharpDX.Direct2D1;

namespace POETradeHelper.ItemSearch.Tests.Services.Parsing
{
    public class MapItemParserTests
    {
        private MapItemParser mapItemParser;
        private MapItemStringBuilder mapItemStringBuilder;

        [SetUp]
        public void Setup()
        {
            this.mapItemParser = new MapItemParser();
            this.mapItemStringBuilder = new MapItemStringBuilder();
        }

        [Test]
        public void CanParseShouldReturnTrueIfItemContainsMapTier()
        {
            string[] itemStringLines = this.mapItemStringBuilder
                .WithMapTier(10)
                .BuildLines();

            bool result = this.mapItemParser.CanParse(itemStringLines);

            Assert.IsTrue(result);
        }

        [Test]
        public void ParseShouldParseMapRarity()
        {
            const ItemRarity expected = ItemRarity.Magic;
            string[] itemStringLines = this.mapItemStringBuilder
                .WithMapTier(10)
                .WithRarity(expected)
                .BuildLines();

            MapItem result = this.mapItemParser.Parse(itemStringLines) as MapItem;

            Assert.That(result.Rarity, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseMapTier()
        {
            const int expected = 10;
            string[] itemStringLines = this.mapItemStringBuilder
                .WithMapTier(expected)
                .BuildLines();

            MapItem result = this.mapItemParser.Parse(itemStringLines) as MapItem;

            Assert.That(result.Tier, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseItemQuantity()
        {
            const int expected = 42;
            string[] itemStringLines = this.mapItemStringBuilder
                .WithItemQuantity(expected)
                .BuildLines();

            MapItem result = this.mapItemParser.Parse(itemStringLines) as MapItem;

            Assert.That(result.ItemQuantity, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseItemRarity()
        {
            const int expected = 42;
            string[] itemStringLines = this.mapItemStringBuilder
                .WithItemRarity(expected)
                .BuildLines();

            MapItem result = this.mapItemParser.Parse(itemStringLines) as MapItem;

            Assert.That(result.ItemRarity, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseMonsterPackSize()
        {
            const int expected = 42;
            string[] itemStringLines = this.mapItemStringBuilder
                .WithMonsterPackSize(expected)
                .BuildLines();

            MapItem result = this.mapItemParser.Parse(itemStringLines) as MapItem;

            Assert.That(result.MonsterPackSize, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseQuality()
        {
            const int expected = 20;
            string[] itemStringLines = this.mapItemStringBuilder
                .WithQuality(expected)
                .BuildLines();

            MapItem result = this.mapItemParser.Parse(itemStringLines) as MapItem;

            Assert.That(result.Quality, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseIdentified()
        {
            string[] itemStringLines = this.mapItemStringBuilder
                .BuildLines();

            MapItem result = this.mapItemParser.Parse(itemStringLines) as MapItem;

            Assert.IsTrue(result.IsIdentified);
        }

        [Test]
        public void ParseShouldParseUnidentifiedMap()
        {
            const string expectedType = "Dig Map";

            string[] itemStringLines = this.mapItemStringBuilder
                .WithType(expectedType)
                .WithUnidentified()
                .BuildLines();

            MapItem result = this.mapItemParser.Parse(itemStringLines) as MapItem;

            Assert.That(result.Type, Is.EqualTo(expectedType));
            Assert.IsFalse(result.IsIdentified);
        }

        [Test]
        public void ParseShouldParseUnidentifiedSuperiorMap()
        {
            const string expectedType = "Dig Map";
            string expectedName = $"{Resources.SuperiorPrefix} {expectedType}";

            string[] itemStringLines = this.mapItemStringBuilder
                .WithName(expectedName)
                .WithUnidentified()
                .WithQuality(20)
                .BuildLines();

            MapItem result = this.mapItemParser.Parse(itemStringLines) as MapItem;

            Assert.That(result.Name, Is.EqualTo(expectedName));
            Assert.That(result.Type, Is.EqualTo(expectedType));
            Assert.IsFalse(result.IsIdentified);
        }

        [Test]
        public void ParseShouldParseBlightedMap()
        {
            const string expectedType = "Dig Map";

            string[] itemStringLines = this.mapItemStringBuilder
                .WithType($"{Resources.BlightedPrefix} {expectedType}")
                .BuildLines();

            MapItem result = this.mapItemParser.Parse(itemStringLines) as MapItem;

            Assert.That(result.Type, Is.EqualTo(expectedType));
            Assert.IsTrue(result.IsBlighted);
        }

        [Test]
        public void ParseShouldParseSuperiorBlightedMap()
        {
            const string expectedType = "Dig Map";

            string[] itemStringLines = this.mapItemStringBuilder
                .WithType($"{Resources.SuperiorPrefix} {Resources.BlightedPrefix} {expectedType}")
                .BuildLines();

            MapItem result = this.mapItemParser.Parse(itemStringLines) as MapItem;

            Assert.That(result.Type, Is.EqualTo(expectedType));
            Assert.IsTrue(result.IsBlighted);
        }

        [Test]
        public void ParseShouldParseCorruptedMap()
        {
            string[] itemStringLines = this.mapItemStringBuilder
                .WithCorrupted()
                .BuildLines();

            MapItem result = this.mapItemParser.Parse(itemStringLines) as MapItem;

            Assert.IsTrue(result.IsCorrupted);
        }

        [Test]
        public void ParseShouldParseNormalRarityMap()
        {
            const string expectedType = "Dig Map";
            string expectedName = $"{Resources.SuperiorPrefix} {Resources.BlightedPrefix} {expectedType}";

            string[] itemStringLines = this.mapItemStringBuilder
                .WithRarity(ItemRarity.Normal)
                .WithName(expectedName)
                .BuildLines();

            MapItem result = this.mapItemParser.Parse(itemStringLines) as MapItem;

            Assert.That(result.Type, Is.EqualTo(expectedType));
            Assert.That(result.Name, Is.EqualTo(expectedName));
        }
    }
}