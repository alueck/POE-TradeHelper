using FluentAssertions;

using NSubstitute;

using NUnit.Framework;

using POETradeHelper.Common.Extensions;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;
using POETradeHelper.ItemSearch.Services.Parsers;
using POETradeHelper.ItemSearch.Services.Parsers.ItemParsers;
using POETradeHelper.ItemSearch.Tests.TestHelpers.ItemStringBuilders;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.ItemSearch.Tests.Services.Parsers.ItemParsers
{
    public class EquippableItemParserTests : ItemParserTestsBase
    {
        private readonly ISocketsParser socketsParserMock;
        private readonly IItemTypeParser itemTypeParserMock;
        private readonly IItemStatsParser<ItemWithStats> itemStatsParserMock;
        private readonly IItemDataService itemDataServiceMock;
        private readonly ItemStringBuilder itemStringBuilder;

        public EquippableItemParserTests()
        {
            this.socketsParserMock = Substitute.For<ISocketsParser>();
            this.itemTypeParserMock = Substitute.For<IItemTypeParser>();
            this.itemStatsParserMock = Substitute.For<IItemStatsParser<ItemWithStats>>();
            this.itemDataServiceMock = Substitute.For<IItemDataService>();
            this.ItemParser = new EquippableItemParser(
                this.socketsParserMock,
                this.itemTypeParserMock,
                this.itemStatsParserMock,
                this.itemDataServiceMock);
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

            result.Should().Be(expected);
        }

        [Test]
        public void CanParseShouldReturnFalseForItemWithoutItemLevel()
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithItemLevel(0)
                .BuildLines();

            bool result = this.ItemParser.CanParse(itemStringLines);

            result.Should().BeFalse();
        }

        [Test]
        public void CanParseShouldReturnFalseForItemWithMapTier()
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithItemLevel(1)
                .WithDescription($"{Resources.MapTierDescriptor} 10")
                .BuildLines();

            bool result = this.ItemParser.CanParse(itemStringLines);

            result.Should().BeFalse();
        }

        [Test]
        public void CanParseShouldReturnFalseForMetamorphOrganItem()
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithItemLevel(1)
                .WithDescription(Resources.OrganItemDescriptor)
                .BuildLines();

            bool result = this.ItemParser.CanParse(itemStringLines);

            result.Should().BeFalse();
        }

        [TestCase("Coralito's Signature")]
        public void CanParseShouldReturnFalseForFlaskItem(string name)
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithItemLevel(1)
                .WithName(name)
                .WithType(Resources.FlaskKeyword)
                .BuildLines();

            bool result = this.ItemParser.CanParse(itemStringLines);

            result.Should().BeFalse();
        }

        [TestCase("Kitava's Teachings")]
        public void CanParseShouldReturnFalseForJewelItem(string name)
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithItemLevel(1)
                .WithName(name)
                .WithType(Resources.JewelKeyword)
                .BuildLines();

            bool result = this.ItemParser.CanParse(itemStringLines);

            result.Should().BeFalse();
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

            EquippableItem result = (EquippableItem)this.ItemParser.Parse(itemStringLines);

            result.Rarity.Should().Be(expected);
        }

        [Test]
        public void ParseShouldParseIdentified()
        {
            string[] itemStringLines = this.itemStringBuilder
                .BuildLines();

            EquippableItem result = (EquippableItem)this.ItemParser.Parse(itemStringLines);

            result.IsIdentified.Should().BeTrue();
        }

        [Test]
        public void ParseShouldParseUnidentified()
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithUnidentified()
                .BuildLines();

            EquippableItem result = (EquippableItem)this.ItemParser.Parse(itemStringLines);

            result.IsIdentified.Should().BeFalse();
        }

        [Test]
        public void ParseShouldParseNameOfIdentifiedItem()
        {
            const string expected = "Wrath Salvation";

            string[] itemStringLines = this.itemStringBuilder
                .WithName(expected)
                .WithType("Cutthroat's Garb")
                .BuildLines();

            EquippableItem result = (EquippableItem)this.ItemParser.Parse(itemStringLines);

            result.Name.Should().Be(expected);
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

            EquippableItem result = (EquippableItem)this.ItemParser.Parse(itemStringLines);

            result.Name.Should().Be(expected);
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

            this.itemTypeParserMock.ParseType(itemStringLines, itemRarity, isIdentified)
                .Returns(expected);

            EquippableItem result = (EquippableItem)this.ItemParser.Parse(itemStringLines);

            result.Type.Should().Be(expected);
        }

        [Test]
        public void ParseShouldParseCorrupted()
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithCorrupted()
                .BuildLines();

            EquippableItem result = (EquippableItem)this.ItemParser.Parse(itemStringLines);

            result.IsCorrupted.Should().BeTrue();
        }

        [Test]
        public void ParseShouldParseNotCorrupted()
        {
            string[] itemStringLines = this.itemStringBuilder
                .BuildLines();

            EquippableItem result = (EquippableItem)this.ItemParser.Parse(itemStringLines);

            result.IsCorrupted.Should().BeFalse();
        }

        [TestCase(74)]
        [TestCase(100)]
        public void ParseShouldParseItemLevel(int expected)
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithItemLevel(expected)
                .BuildLines();

            EquippableItem result = (EquippableItem)this.ItemParser.Parse(itemStringLines);

            result.ItemLevel.Should().Be(expected);
        }

        [TestCase(10)]
        [TestCase(20)]
        public void ParseShouldParseQuality(int expected)
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithQuality(expected)
                .BuildLines();

            EquippableItem result = (EquippableItem)this.ItemParser.Parse(itemStringLines);

            result.Quality.Should().Be(expected);
        }

        [TestCase(InfluenceType.None)]
        [TestCase(InfluenceType.Crusader)]
        [TestCase(InfluenceType.Shaper)]
        public void ParseShouldParseInfluence(InfluenceType expected)
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithInfluence(expected)
                .BuildLines();

            EquippableItem result = (EquippableItem)this.ItemParser.Parse(itemStringLines);

            result.Influence.Should().Be(expected);
        }

        [Test]
        public void ParseShouldCallParseOnSocketsParser()
        {
            const string socketsString = "B-G-G-G";
            string[] itemStringLines = this.itemStringBuilder
                .WithSockets(socketsString)
                .BuildLines();

            this.ItemParser.Parse(itemStringLines);

            this.socketsParserMock
                .Received()
                .Parse(socketsString);
        }

        [Test]
        public void ParseShouldSetSocketsOnItem()
        {
            string[] itemStringLines = this.itemStringBuilder
                .BuildLines();

            ItemSockets expected = new()
            {
                SocketGroups =
                {
                    new SocketGroup
                    {
                        Sockets =
                        {
                            new Socket { SocketType = SocketType.Blue },
                        },
                    },
                },
            };

            this.socketsParserMock.Parse(Arg.Any<string>())
                .Returns(expected);

            EquippableItem result = (EquippableItem)this.ItemParser.Parse(itemStringLines);

            result.Sockets.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void ParseShouldCallParseOnItemStatsParserIfItemIsIdentified()
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithType("Thicket Bow")
                .BuildLines();

            this.ItemParser.Parse(itemStringLines);

            this.itemStatsParserMock
                .Received()
                .Parse(itemStringLines, Arg.Any<bool>());
        }

        [TestCase(EquippableItemCategory.Unknown, false)]
        [TestCase(EquippableItemCategory.Accessories, false)]
        [TestCase(EquippableItemCategory.Armour, true)]
        [TestCase(EquippableItemCategory.Weapons, true)]
        public void ParseShouldCallParseOnItemStatsParserWithPreferLocalStats(
            EquippableItemCategory itemCategory,
            bool expectedPreferLocalStats)
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithType("Thicket Bow")
                .BuildLines();

            const string itemType = "parsed item type";
            this.itemTypeParserMock.ParseType(Arg.Any<string[]>(), Arg.Any<ItemRarity>(), Arg.Any<bool>())
                .Returns(itemType);
            this.itemDataServiceMock.GetCategory(Arg.Any<string>())
                .Returns(itemCategory.GetDisplayName());

            this.ItemParser.Parse(itemStringLines);

            this.itemStatsParserMock
                .Received()
                .Parse(itemStringLines, expectedPreferLocalStats);
        }

        [Test]
        public void ParseShouldNotCallParseOnItemStatsParserIfItemIsUnidentified()
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithType("Thicket Bow")
                .WithUnidentified()
                .BuildLines();

            this.ItemParser.Parse(itemStringLines);

            this.itemStatsParserMock
                .DidNotReceive()
                .Parse(Arg.Any<string[]>(), Arg.Any<bool>());
        }

        [Test]
        public void ParseShouldSetItemStatsFromItemStatsParserOnItem()
        {
            ItemStats expected = new();
            string[] itemStringLines = this.itemStringBuilder
                .WithType("Thicket Bow")
                .BuildLines();

            this.itemStatsParserMock.Parse(Arg.Any<string[]>(), Arg.Any<bool>())
                .Returns(expected);

            EquippableItem result = (EquippableItem)this.ItemParser.Parse(itemStringLines);

            result.Stats.Should().BeSameAs(expected);
        }

        [Test]
        public void ParseShouldCallGetCategoryOnItemDataService()
        {
            // arrange
            string[] itemStringLines = this.itemStringBuilder.BuildLines();
            const string itemType = "parsed item type";

            this.itemTypeParserMock.ParseType(Arg.Any<string[]>(), Arg.Any<ItemRarity>(), Arg.Any<bool>())
                .Returns(itemType);

            // act
            this.ItemParser.Parse(itemStringLines);

            // assert
            this.itemDataServiceMock
                .Received()
                .GetCategory(itemType);
        }

        [TestCase(null, EquippableItemCategory.Unknown)]
        [TestCase(EquippableItemCategory.Accessories, EquippableItemCategory.Accessories)]
        [TestCase(EquippableItemCategory.Armour, EquippableItemCategory.Armour)]
        [TestCase(EquippableItemCategory.Weapons, EquippableItemCategory.Weapons)]
        public void ParseShouldSetCategoryFromItemDataService(EquippableItemCategory? itemCategory, EquippableItemCategory expectedItemCategory)
        {
            // arrange
            string[] itemStringLines = this.itemStringBuilder.BuildLines();
            const string itemType = "parsed item type";

            this.itemTypeParserMock.ParseType(Arg.Any<string[]>(), Arg.Any<ItemRarity>(), Arg.Any<bool>())
                .Returns(itemType);
            this.itemDataServiceMock.GetCategory(Arg.Any<string>())
                .Returns(itemCategory?.GetDisplayName().ToLower());

            // act
            EquippableItem result = (EquippableItem)this.ItemParser.Parse(itemStringLines);

            // assert
            result.Category.Should().Be(expectedItemCategory);
        }

        [TestCase(EquippableItemCategory.Unknown, true)]
        [TestCase(EquippableItemCategory.Accessories, false)]
        [TestCase(EquippableItemCategory.Armour, true)]
        [TestCase(EquippableItemCategory.Weapons, false)]
        public void ParseSetsArmourValues(EquippableItemCategory itemCategory, bool valuesSet)
        {
            // arrange
            const int armour = 150;
            const int energyShield = 89;
            const int evasionRating = 98;
            const int blockChance = 10;
            const int ward = 99;

            this.itemStringBuilder
                .ItemStatsGroup
                .WithArmour(armour)
                .WithEnergyShield(energyShield)
                .WithEvasionRating(evasionRating)
                .WithBlockChance(blockChance)
                .WithWard(ward);

            string[] itemStringLines = this.itemStringBuilder.BuildLines();

            this.itemTypeParserMock
                .ParseType(Arg.Any<string[]>(), Arg.Any<ItemRarity>(), Arg.Any<bool>())
                .Returns("Coronal Leather");

            this.itemDataServiceMock
                .GetCategory(Arg.Any<string>())
                .Returns(itemCategory.GetDisplayName().ToLower());

            // act
            EquippableItem result = (EquippableItem)this.ItemParser.Parse(itemStringLines);

            // assert
            if (valuesSet)
            {
                result.ArmourValues.Should()
                    .BeEquivalentTo(new ArmourValues
                    {
                        Armour = armour,
                        EnergyShield = energyShield,
                        EvasionRating = evasionRating,
                        BlockChance = blockChance,
                        Ward = ward,
                    });
            }
            else
            {
                result.ArmourValues.Should().BeNull();
            }
        }

        [TestCase(EquippableItemCategory.Unknown, true)]
        [TestCase(EquippableItemCategory.Accessories, false)]
        [TestCase(EquippableItemCategory.Armour, false)]
        [TestCase(EquippableItemCategory.Weapons, true)]
        public void ParseSetsWeaponValues(EquippableItemCategory itemCategory, bool valuesSet)
        {
            // arrange
            MinMaxValue physicalDamage = new() { Min = 10, Max = 30 };
            MinMaxValue elementalDamage1 = new() { Min = 10, Max = 19 };
            MinMaxValue elementalDamage2 = new() { Min = 15, Max = 21 };
            const decimal attacksPerSecond = 1.35m;
            const decimal criticalStrikeChance = 6.5m;

            this.itemStringBuilder
                .ItemStatsGroup
                .WithPhysicalDamage(physicalDamage)
                .WithElementalDamage([elementalDamage1, elementalDamage2])
                .WithAttacksPerSecond(attacksPerSecond)
                .WithCriticalStrikeChance(criticalStrikeChance);

            string[] itemStringLines = this.itemStringBuilder.BuildLines();

            this.itemTypeParserMock
                .ParseType(Arg.Any<string[]>(), Arg.Any<ItemRarity>(), Arg.Any<bool>())
                .Returns("Iron Staff");

            this.itemDataServiceMock
                .GetCategory(Arg.Any<string>())
                .Returns(itemCategory.GetDisplayName().ToLower());

            // act
            EquippableItem result = (EquippableItem)this.ItemParser.Parse(itemStringLines);

            // assert
            if (valuesSet)
            {
                result.WeaponValues.Should()
                    .BeEquivalentTo(new WeaponValues
                    {
                        PhysicalDamage = physicalDamage,
                        ElementalDamage = [elementalDamage1, elementalDamage2],
                        AttacksPerSecond = attacksPerSecond,
                        CriticalStrikeChance = criticalStrikeChance,
                    });
            }
            else
            {
                result.WeaponValues.Should().BeNull();
            }
        }

        protected override string[] GetValidItemStringLines() =>
            this.itemStringBuilder
                .WithRarity(ItemRarity.Rare)
                .WithType("Thicket Bow")
                .WithName("Woe Rain")
                .WithItemLevel(85)
                .WithSockets("G-G-G")
                .BuildLines();
    }
}