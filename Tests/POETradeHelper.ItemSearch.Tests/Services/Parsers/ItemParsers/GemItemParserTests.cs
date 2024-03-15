using FluentAssertions;

using NSubstitute;

using NUnit.Framework;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services.Parsers.ItemParsers;
using POETradeHelper.ItemSearch.Tests.Properties;
using POETradeHelper.ItemSearch.Tests.TestHelpers.ItemStringBuilders;
using POETradeHelper.PathOfExileTradeApi.Models;
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

            result.Should().Be(expected);
        }

        [Test]
        public void ParseShouldReturnGemItem()
        {
            string[] itemStringLines = this.itemStringBuilder.BuildLines();

            Item item = this.ItemParser.Parse(itemStringLines);

            item.Should().BeOfType<GemItem>();
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
        public void ParseShouldSetTypeFromItemDataService()
        {
            const string expected = "Result from ItemDataService";

            string[] itemStringLines = this.itemStringBuilder
                .WithName("Flameblast")
                .BuildLines();

            this.itemDataServiceMock.GetType(Arg.Any<string>())
                .Returns(new ItemType(expected));

            GemItem result = (GemItem)this.ItemParser.Parse(itemStringLines);

            result.Type.Should().Be(expected);
        }

        [Test]
        public void ParseShouldParseCorruptedTrue()
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithCorrupted()
                .BuildLines();

            GemItem result = (GemItem)this.ItemParser.Parse(itemStringLines);

            result.IsCorrupted.Should().BeTrue();
        }

        [Test]
        public void ParseShouldParseCorruptedFalse()
        {
            string[] itemStringLines = this.itemStringBuilder
                .BuildLines();

            GemItem result = (GemItem)this.ItemParser.Parse(itemStringLines);

            result.IsCorrupted.Should().BeFalse();
        }

        [Test]
        public void ParseShouldParseQuality()
        {
            const int expected = 13;
            string[] itemStringLines = this.itemStringBuilder
                .WithQuality(expected)
                .BuildLines();

            GemItem result = (GemItem)this.ItemParser.Parse(itemStringLines);

            result.Quality.Should().Be(expected);
        }

        [Test]
        public void ParseShouldParseZeroQualityIfItemHasNoQuality()
        {
            const int expected = 0;
            string[] itemStringLines = this.itemStringBuilder
                .BuildLines();

            GemItem result = (GemItem)this.ItemParser.Parse(itemStringLines);

            result.Quality.Should().Be(expected);
        }

        [Test]
        public void ParseShouldParseGemLevel()
        {
            const int expected = 17;
            string[] itemStringLines = this.itemStringBuilder
                .WithGemLevel(expected)
                .BuildLines();

            GemItem result = (GemItem)this.ItemParser.Parse(itemStringLines);

            result.Level.Should().Be(expected);
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

            result.ExperiencePercent.Should().Be(expected);
        }

        [Test]
        public void ParseShouldParseVaalGem()
        {
            const string name = "Vaal Animate Weapon (Animate Weapon of Ranged Arms)";
            string[] itemStringLines = Resources.VaalAnimateWeaponOfRangedArms.Split(Environment.NewLine);
            ItemType itemType = new("Vaal Animate Weapon", "alt_y");

            this.itemDataServiceMock
                .GetType(name)
                .Returns(itemType);

            GemItem result = (GemItem)this.ItemParser.Parse(itemStringLines);

            result.Should()
                .BeEquivalentTo(
                    new GemItem
                    {
                        Name = name,
                        Type = itemType.Type,
                        TypeDiscriminator = itemType.Discriminator,
                        IsCorrupted = true,
                        IsVaalVersion = true,
                        Level = 20,
                        Quality = 20,
                    },
                    config => config.Excluding(x => x.PlainItemText).Excluding(x => x.ExtendedItemText));
        }

        [Test]
        public void ParseShouldParseVaalGemWithDifferentNameCorrectly()
        {
            string[] itemStringLines = Resources.VaalImpurityOfLightning.Split(Environment.NewLine);
            const string type = "Vaal Impurity of Lightning";
            this.itemDataServiceMock
                .GetType(type)
                .Returns(new ItemType(type));

            GemItem result = (GemItem)this.ItemParser.Parse(itemStringLines);

            result.Should()
                .BeEquivalentTo(
                    new GemItem
                    {
                        Name = type,
                        Type = type,
                        IsCorrupted = true,
                        IsVaalVersion = true,
                        Level = 1,
                    },
                    config => config.Excluding(x => x.PlainItemText).Excluding(x => x.ExtendedItemText));
        }

        protected override string[] GetValidItemStringLines() =>
            this.itemStringBuilder
                .WithName($"{Contract.Properties.Resources.VaalKeyword} Flameblast")
                .WithTags($"{Contract.Properties.Resources.VaalKeyword}, Spell, AoE, Fire, Channelling")
                .WithCorrupted()
                .BuildLines();
    }
}