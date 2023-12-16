using FluentAssertions;
using NSubstitute;

using NUnit.Framework;

using POETradeHelper.Common.Extensions;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Services.Parsers;
using POETradeHelper.ItemSearch.Services.Parsers.ItemStatsParsers;
using POETradeHelper.ItemSearch.Tests.TestHelpers.ItemStringBuilders;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.ItemSearch.Tests.Services.Parsers.ItemStatsParsers
{
    public class ItemStatsParserTests
    {
        private readonly IStatsDataService statsDataServiceMock;
        private readonly IPseudoItemStatsParser pseudoItemStatsParserMock;
        private readonly ItemStatsParser itemStatsParser;
        private readonly ItemStringBuilder itemStringBuilder;

        public ItemStatsParserTests()
        {
            this.statsDataServiceMock = Substitute.For<IStatsDataService>();
            this.pseudoItemStatsParserMock = Substitute.For<IPseudoItemStatsParser>();
            this.itemStatsParser = new ItemStatsParser(this.statsDataServiceMock, this.pseudoItemStatsParserMock);
            this.itemStringBuilder = new ItemStringBuilder();
        }

        [TestCase(StatCategory.Explicit, "Minions deal 1 to 15 additional Physical Damage", "Minions deal 1 to 15 additional Physical Damage")]
        [TestCase(StatCategory.Implicit, "+25% to Cold Resistance (implicit)", "+25% to Cold Resistance")]
        [TestCase(StatCategory.Crafted, "+25% to Cold Resistance (crafted)", "+25% to Cold Resistance")]
        [TestCase(StatCategory.Enchant, "10% increased Movement Speed if you haven't been Hit Recently (enchant)", "10% increased Movement Speed if you haven't been Hit Recently")]
        [TestCase(StatCategory.Fractured, "Adds 11 to 142 Lightning Damage (fractured)", "Adds 11 to 142 Lightning Damage")]
        public void ParseShouldParseStatText(StatCategory statCategory, string statText, string expected)
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithName("Titan Greaves")
                .WithItemLevel(75)
                .WithItemStat(statText, statCategory)
                .BuildLines();

            this.statsDataServiceMock.GetStatData(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<string[]>())
                .Returns(new StatData());

            ItemStats result = this.itemStatsParser.Parse(itemStringLines, false);

            result.AllStats.Should().HaveCount(1);
            result.AllStats.First().Text.Should().Be(expected);
        }

        [TestCase(StatCategory.Explicit, "Minions deal 1 to 15 additional Physical Damage", "Minions deal # to # additional Physical Damage")]
        [TestCase(StatCategory.Implicit, "+25% to Cold Resistance (implicit)", "#% to Cold Resistance")]
        [TestCase(StatCategory.Crafted, "+25% to Cold Resistance (crafted)", "#% to Cold Resistance")]
        [TestCase(StatCategory.Enchant, "10% increased Movement Speed if you haven't been Hit Recently (enchant)", "#% increased Movement Speed if you haven't been Hit Recently")]
        [TestCase(StatCategory.Fractured, "Adds 11 to 142 Lightning Damage (fractured)", "Adds # to # Lightning Damage")]
        public void ParseShouldSetTextWithPlaceholdersFromStatData(StatCategory statCategory, string statText, string expected)
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithName("Titan Greaves")
                .WithItemLevel(75)
                .WithItemStat(statText, statCategory)
                .BuildLines();

            this.statsDataServiceMock.GetStatData(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<string[]>())
                .Returns(new StatData { Text = expected });

            ItemStats result = this.itemStatsParser.Parse(itemStringLines, false);

            result.AllStats.Should().HaveCount(1);
            result.AllStats.First().TextWithPlaceholders.Should().Be(expected);
        }

        [TestCase("Minions deal 1 to 15 additional Physical Damage", StatCategory.Explicit)]
        [TestCase("+25% to Cold Resistance (implicit)", StatCategory.Implicit)]
        [TestCase("+25% to Cold Resistance (crafted)", StatCategory.Crafted)]
        [TestCase("10% increased Movement Speed if you haven't been Hit Recently (enchant)", StatCategory.Enchant)]
        [TestCase("Adds 11 to 142 Lightning Damage (fractured)", StatCategory.Fractured)]
        public void ParseShouldParseStatTextInCorrectCategory(string statText, StatCategory expected)
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithName("Titan Greaves")
                .WithItemLevel(75)
                .WithItemStat(statText, expected)
                .BuildLines();

            this.statsDataServiceMock.GetStatData(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<string[]>())
                .Returns(new StatData { Type = expected.GetDisplayName() });

            ItemStats result = this.itemStatsParser.Parse(itemStringLines, false);

            result.AllStats.Should().HaveCount(1);
            result.AllStats.First().StatCategory.Should().Be(expected);
        }

        [Test]
        public void ParseShouldNotAddItemStatIfGetStatDataOnStatDataServiceReturnsNull()
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithName("Titan Greaves")
                .WithItemLevel(75)
                .WithItemStat("Minions deal 1 to 15 additional Physical Damage", StatCategory.Explicit)
                .WithDescription("These boots are made for walkin'")
                .BuildLines();

            ItemStats result = this.itemStatsParser.Parse(itemStringLines, false);

            result.AllStats.Should().BeEmpty();
        }

        [TestCase(StatCategory.Explicit)]
        public void ParseShouldCallGetStatDataWithExpectedStatCategories(StatCategory statCategory)
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithName("Titan Greaves")
                .WithItemLevel(75)
                .WithItemStat("Stat text with unknown category", statCategory)
                .BuildLines();

            this.itemStatsParser.Parse(itemStringLines, false);

            this.statsDataServiceMock
                .Received()
                .GetStatData(Arg.Any<string>(), Arg.Any<bool>(), StatCategory.Explicit.GetDisplayName());
        }

        [TestCase(StatCategory.Implicit, "+25% to Cold Resistance (implicit)")]
        [TestCase(StatCategory.Crafted, "+25% to Cold Resistance (crafted)")]
        [TestCase(StatCategory.Enchant, "10% increased Movement Speed if you haven't been Hit Recently (enchant)")]
        [TestCase(StatCategory.Fractured, "Adds 11 to 142 Lightning Damage (fractured)")]
        public void ParseShouldCallGetStatDataWithStatCategory(StatCategory statCategory, string statText)
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithName("Titan Greaves")
                .WithItemLevel(75)
                .WithItemStat(statText, statCategory)
                .BuildLines();

            this.itemStatsParser.Parse(itemStringLines, false);

            this.statsDataServiceMock
                .Received()
                .GetStatData(Arg.Any<string>(), Arg.Any<bool>(), statCategory.GetDisplayName());
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ParseShouldCallGetStatDataWithPreferLocalStats(bool preferLocalStats)
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithName("Titan Greaves")
                .WithItemLevel(75)
                .WithItemStat("+25% to Cold Resistance (implicit)", StatCategory.Implicit)
                .BuildLines();

            this.itemStatsParser.Parse(itemStringLines, preferLocalStats);

            this.statsDataServiceMock
                .Received()
                .GetStatData(Arg.Any<string>(), preferLocalStats, Arg.Any<string[]>());
        }

        [TestCase(StatCategory.Explicit, "Minions deal 1 to 15 additional Physical Damage")]
        [TestCase(StatCategory.Implicit, "+25% to Cold Resistance (implicit)")]
        [TestCase(StatCategory.Crafted, "+25% to Cold Resistance (crafted)")]
        [TestCase(StatCategory.Enchant, "10% increased Movement Speed if you haven't been Hit Recently (enchant)")]
        [TestCase(StatCategory.Fractured, "Adds 11 to 142 Lightning Damage (fractured)")]
        public void ParseShouldSetIdOnStatFromStatsDataService(StatCategory statCategory, string statText)
        {
            const string expected = "item stat id";
            string[] itemStringLines = this.itemStringBuilder
                .WithName("Titan Greaves")
                .WithItemLevel(75)
                .WithItemStat(statText, statCategory)
                .BuildLines();

            this.statsDataServiceMock.GetStatData(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<string[]>())
                .Returns(new StatData { Id = expected });

            ItemStats result = this.itemStatsParser.Parse(itemStringLines, false);

            result.AllStats.Should().HaveCount(1);
            result.AllStats.First().Id.Should().Be(expected);
        }

        [TestCase(StatCategory.Enchant, "Trigger Edict of Frost on Kill", "#% chance to Trigger Edict of Frost on Kill")]
        [TestCase(StatCategory.Explicit, "Extra gore", "Extra gore")]
        public void ParseShouldReturnItemStatWithoutValueIfTextDoesNotContainPlaceholders(StatCategory statCategory, string statText, string textWithPlaceholders)
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithName("Titan Greaves")
                .WithItemLevel(75)
                .WithItemStat(statText, statCategory)
                .BuildLines();

            this.statsDataServiceMock.GetStatData(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<string[]>())
                .Returns(new StatData
                {
                    Text = textWithPlaceholders,
                });

            ItemStats result = this.itemStatsParser.Parse(itemStringLines, false);

            result.AllStats.Should().HaveCount(1);
            result.AllStats.First().Should().BeOfType<ItemStat>();
        }

        [Test]
        public void ParseShouldReturnSingleValueItemStatIfTextWithPlaceholdersContainsOnePlaceholder()
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithName("Titan Greaves")
                .WithItemLevel(75)
                .WithItemStat("+25% to Cold Resistance", StatCategory.Explicit)
                .BuildLines();

            this.statsDataServiceMock.GetStatData(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<string[]>())
                .Returns(new StatData
                {
                    Text = "#% to Cold Resistance",
                });

            ItemStats result = this.itemStatsParser.Parse(itemStringLines, false);

            Assert.That(result.AllStats, Has.Count.EqualTo(1));

            result.AllStats.Should().HaveCount(1);
            result.AllStats.First().Should().BeOfType<SingleValueItemStat>();
        }

        [Test]
        public void ParseShouldReturnMinMaxValueItemStatIfTextWithPlaceholderContainsTwoPlaceholders()
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithName("Titan Greaves")
                .WithItemLevel(75)
                .WithItemStat("Minions deal 1 to 15 additional Physical Damage", StatCategory.Explicit)
                .BuildLines();

            this.statsDataServiceMock.GetStatData(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<string[]>())
                .Returns(new StatData
                {
                    Text = "Minions deal # to # additional Physical Damage",
                });

            ItemStats result = this.itemStatsParser.Parse(itemStringLines, false);

            result.AllStats.Should().HaveCount(1);
            result.AllStats.First().Should().BeOfType<MinMaxValueItemStat>();
        }

        [TestCase(StatCategory.Explicit, "+25% to Cold Resistance", "#% to Cold Resistance", 25)]
        [TestCase(StatCategory.Explicit, "+75 to Maximum Life", "# to Maximum Life", 75)]
        [TestCase(StatCategory.Explicit, "-10 to Maximum Life", "# to Maximum Life", -10)]
        public void ParseShouldSetValueOfSingleValueItemStat(StatCategory statCategory, string statText, string textWithPlaceholders, int expected)
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithName("Titan Greaves")
                .WithItemLevel(75)
                .WithItemStat(statText, statCategory)
                .BuildLines();

            this.statsDataServiceMock.GetStatData(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<string[]>())
                .Returns(new StatData
                {
                    Text = textWithPlaceholders,
                });

            ItemStats result = this.itemStatsParser.Parse(itemStringLines, false);

            result.AllStats.Should().HaveCount(1);
            SingleValueItemStat itemStat = (SingleValueItemStat)result.AllStats.First();
            itemStat.Value.Should().Be(expected);
        }

        [TestCase(StatCategory.Explicit, "Adds 10 to 23 Chaos Damage", "Adds # to # Chaos Damage", 10)]
        [TestCase(StatCategory.Explicit, "Minions deal 1 to 15 additional Physical Damage", "Minions deal # to # additional Physical Damage", 1)]
        public void ParseShouldSetMinValueOfMinMaxValueItemStat(StatCategory statCategory, string statText, string textWithPlaceholders, int expected)
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithName("Titan Greaves")
                .WithItemLevel(75)
                .WithItemStat(statText, statCategory)
                .BuildLines();

            this.statsDataServiceMock.GetStatData(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<string[]>())
                .Returns(new StatData
                {
                    Text = textWithPlaceholders,
                });

            ItemStats result = this.itemStatsParser.Parse(itemStringLines, false);

            result.AllStats.Should().HaveCount(1);
            MinMaxValueItemStat itemStat = (MinMaxValueItemStat)result.AllStats.First();
            itemStat.MinValue.Should().Be(expected);
        }

        [TestCase(StatCategory.Explicit, "Adds 10 to 23 Chaos Damage", "Adds # to # Chaos Damage", 23)]
        [TestCase(StatCategory.Explicit, "Minions deal 1 to 15 additional Physical Damage", "Minions deal # to # additional Physical Damage", 15)]
        public void ParseShouldSetMaxValueOfMinMaxValueItemStat(StatCategory statCategory, string statText, string textWithPlaceholders, int expected)
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithName("Titan Greaves")
                .WithItemLevel(75)
                .WithItemStat(statText, statCategory)
                .BuildLines();

            this.statsDataServiceMock.GetStatData(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<string[]>())
                .Returns(new StatData
                {
                    Text = textWithPlaceholders,
                });

            ItemStats result = this.itemStatsParser.Parse(itemStringLines, false);

            result.AllStats.Should().HaveCount(1);
            MinMaxValueItemStat itemStat = (MinMaxValueItemStat)result.AllStats.First();
            itemStat.MaxValue.Should().Be(expected);
        }

        [Test]
        public void ParseShouldCallParseOnPseudoItemStatsParser()
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithName("Titan Greaves")
                .WithItemLevel(75)
                .WithItemStat("statText", StatCategory.Explicit)
                .BuildLines();

            this.statsDataServiceMock.GetStatData(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<string[]>())
                .Returns(new StatData());

            this.pseudoItemStatsParserMock
                .When(m => m.Parse(Arg.Any<IEnumerable<ItemStat>>()))
                .Do(_ => this.statsDataServiceMock
                    .Received()
                    .GetStatData(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<string[]>()));

            ItemStats result = this.itemStatsParser.Parse(itemStringLines, false);

            this.pseudoItemStatsParserMock
                .Received()
                .Parse(Arg.Is<IEnumerable<ItemStat>>(enumerable => enumerable.SequenceEqual(result.AllStats)));
        }

        [Test]
        public void ParseShouldAddResultFromPseudoItemStatsParserToResult()
        {
            ItemStat expected = new(StatCategory.Pseudo) { Id = "test id" };

            string[] itemStringLines = this.itemStringBuilder
                .WithName("Titan Greaves")
                .WithItemLevel(75)
                .WithItemStat("statText", StatCategory.Explicit)
                .BuildLines();

            this.statsDataServiceMock.GetStatData(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<string[]>())
                .Returns(new StatData());

            this.pseudoItemStatsParserMock.Parse(Arg.Any<IEnumerable<ItemStat>>())
                .Returns(new List<ItemStat> { expected });

            ItemStats result = this.itemStatsParser.Parse(itemStringLines, false);

            result.AllStats.Should().Contain(expected);
        }

        [Test]
        public void ParseShouldParseStatTier()
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithName("Titan Greaves")
                .WithItemLevel(75)
                .WithItemStat("{ Prefix Modifier \"Hunter's\" (Tier: 2) — Damage, Chaos, Ailment }\n84(80-89)% increased Chaos Damage over Time", StatCategory.Explicit)
                .BuildLines();

            this.statsDataServiceMock.GetStatData(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<string[]>())
                .Returns(new StatData { Type = "Explicit" });

            ItemStats result = this.itemStatsParser.Parse(itemStringLines, false);

            result.ExplicitStats.Should()
                .HaveCount(1)
                .And.ContainEquivalentOf(new ItemStat(StatCategory.Explicit)
                {
                    Text = "84% increased Chaos Damage over Time",
                    Tier = 2,
                });
        }

        [Test]
        public void ParseShouldMultipleDifferentStatsCorrectly()
        {
            ItemStat expectedExplicitItemStat = new MinMaxValueItemStat(StatCategory.Explicit)
            {
                Id = "explicit item stat id",
                Text = "Minions deal 1 to 15 additional Physical Damage",
                TextWithPlaceholders = "Minions deal # to # additional Physical Damage",
                MinValue = 1,
                MaxValue = 15,
            };

            ItemStat expectedImplicitItemStat = new SingleValueItemStat(StatCategory.Implicit)
            {
                Id = "implicit item stat id",
                Text = "10% increased Movement Speed",
                TextWithPlaceholders = "#% increased Movement Speed",
                Value = 10,
            };

            ItemStat expectedCraftedItemStat = new SingleValueItemStat(StatCategory.Crafted)
            {
                Id = "crafted item stat id",
                Text = "+25% to Cold Resistance",
                TextWithPlaceholders = "#% to Cold Resistance",
                Value = 25,
            };

            ItemStat expectedEnchantedItemStat = new SingleValueItemStat(StatCategory.Enchant)
            {
                Id = "enchanted item stat id",
                Text = "10% increased Movement Speed if you haven't been Hit Recently",
                TextWithPlaceholders = "#% increased Movement Speed if you haven't been Hit Recently",
                Value = 10,
            };

            ItemStat[] itemStats = { expectedExplicitItemStat, expectedImplicitItemStat, expectedCraftedItemStat, expectedEnchantedItemStat };

            foreach (ItemStat itemStat in itemStats)
            {
                this.statsDataServiceMock
                    .GetStatData(
                        Arg.Is<string>(s => s == itemStat.Text),
                        Arg.Any<bool>(),
                        Arg.Any<string[]>())
                    .Returns(new StatData
                    {
                        Id = itemStat.Id, Text = itemStat.TextWithPlaceholders,
                        Type = itemStat.StatCategory.GetDisplayName(),
                    });
            }

            string[] itemStringLines = this.itemStringBuilder
                .WithName("Titan Greaves")
                .WithItemLevel(75)
                .WithItemStat(expectedExplicitItemStat.Text, expectedExplicitItemStat.StatCategory)
                .WithItemStat(
                    $"{expectedImplicitItemStat.Text} ({StatCategory.Implicit.GetDisplayName().ToLower()})",
                    expectedImplicitItemStat.StatCategory)
                .WithItemStat(
                    $"{expectedCraftedItemStat.Text} ({StatCategory.Crafted.GetDisplayName().ToLower()})",
                    expectedCraftedItemStat.StatCategory)
                .WithItemStat(
                    $"{expectedEnchantedItemStat.Text} ({StatCategory.Enchant.GetDisplayName().ToLower()})",
                    expectedEnchantedItemStat.StatCategory)
                .BuildLines();

            ItemStats result = this.itemStatsParser.Parse(itemStringLines, false);

            result.AllStats.Should().HaveCount(4);
            result.ExplicitStats.Should().ContainEquivalentOf(expectedExplicitItemStat, opt => opt.RespectingRuntimeTypes());
            result.ImplicitStats.Should().ContainEquivalentOf(expectedImplicitItemStat, opt => opt.RespectingRuntimeTypes());
            result.CraftedStats.Should().ContainEquivalentOf(expectedCraftedItemStat, opt => opt.RespectingRuntimeTypes());
            result.EnchantedStats.Should().ContainEquivalentOf(expectedEnchantedItemStat, opt => opt.RespectingRuntimeTypes());
        }

        [Test]
        public void ParseShouldParseItemWithExtendedItemTextCorrectly()
        {
            // arrange
            string itemText = @"
Item Class: Boots
Rarity: Rare
Honour Stride
Dragonscale Boots
--------
Quality: +20% (augmented)
Armour: 145 (augmented)
Evasion Rating: 145 (augmented)
--------
Requirements:
Level: 70
Str: 130
Dex: 67
Int: 35
--------
Sockets: R-R-R-R 
--------
Item Level: 72
--------
6% increased Movement Speed if you haven't been Hit Recently (enchant)
(Recently refers to the past 4 seconds) (enchant)
--------
{ Prefix Modifier ""Athlete's"" (Tier: 1) — Life }
+87(80-89) to maximum Life
{ Master Crafted Prefix Modifier ""Upgraded"" (Rank: 3) — Speed }
23(20-24)% increased Movement Speed (crafted)
{ Suffix Modifier ""of the Polar Bear"" (Tier: 3) — Elemental, Cold, Resistance }
+40(36-41)% to Cold Resistance
{ Suffix Modifier ""of the Apt"" (Tier: 1) }
32% reduced Attribute Requirements
(Attributes are Strength, Dexterity, and Intelligence)
";
            this.statsDataServiceMock
                .GetStatData(Arg.Is<string>(s => !s.Contains("Movement") && !string.IsNullOrWhiteSpace(s)), Arg.Any<bool>(), Arg.Any<string[]>())
                .Returns(new StatData { Type = StatCategory.Explicit.GetDisplayName() });
            this.statsDataServiceMock
                .GetStatData(Arg.Is<string>(s => s.Contains("6%")), Arg.Any<bool>(), Arg.Any<string[]>())
                .Returns(new StatData { Type = StatCategory.Enchant.GetDisplayName() });
            this.statsDataServiceMock
                .GetStatData(Arg.Is<string>(s => s.Contains("23")), Arg.Any<bool>(), Arg.Any<string[]>())
                .Returns(new StatData { Type = StatCategory.Crafted.GetDisplayName() });

            // act
            ItemStats result = this.itemStatsParser.Parse(itemText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries), false);

            // assert
            result.EnchantedStats.Should()
                .HaveCount(1)
                .And.ContainEquivalentOf(new ItemStat(StatCategory.Enchant)
                {
                    Text = "6% increased Movement Speed if you haven't been Hit Recently",
                });

            result.ExplicitStats.Should()
                .HaveCount(3)
                .And.ContainEquivalentOf(new ItemStat(StatCategory.Explicit)
                {
                    Text = "+87 to maximum Life",
                    Tier = 1,
                })
                .And.ContainEquivalentOf(new ItemStat(StatCategory.Explicit)
                {
                    Text = "+40% to Cold Resistance",
                    Tier = 3,
                })
                .And.ContainEquivalentOf(new ItemStat(StatCategory.Explicit)
                {
                    Text = "32% reduced Attribute Requirements",
                    Tier = 1,
                });

            result.CraftedStats.Should()
                .HaveCount(1)
                .And.ContainEquivalentOf(new ItemStat(StatCategory.Crafted)
                {
                    Text = "23% increased Movement Speed",
                    Tier = 3,
                });
        }

        [Test]
        public void ParseShouldParseUniqueItemWithExtendedItemTextCorrectly()
        {
            // arrange
            string itemText = @"
Item Class: Gloves
Rarity: Unique
The Embalmer
Carnal Mitts
--------
Quality: +20% (augmented)
Evasion Rating: 108 (augmented)
Energy Shield: 23 (augmented)
--------
Requirements:
Level: 69
Dex: 151
Int: 108
--------
Sockets: B-G-G-G 
--------
Item Level: 51
--------
Trigger Edict of Frost on Kill (enchant)
--------
{ Unique Modifier — Gem }
Socketed Gems are Supported by Level 20 Vile Toxins — Unscalable Value
{ Unique Modifier — Chaos, Ailment }
22(20-25)% increased Poison Duration
{ Unique Modifier — Life }
+59(50-70) to maximum Life
{ Unique Modifier — Chaos, Resistance }
+19(17-29)% to Chaos Resistance
{ Unique Modifier — Damage, Chaos }
Adds 13(13-17) to 28(23-29) Chaos Damage
--------";
            this.statsDataServiceMock
                .GetStatData(Arg.Is<string>(s => !string.IsNullOrWhiteSpace(s)), Arg.Any<bool>(), Arg.Any<string[]>())
                .Returns(new StatData());

            // act
            ItemStats result = this.itemStatsParser.Parse(itemText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries), false);

            // assert
            result.AllStats.Should()
                .HaveCount(6)
                .And.ContainEquivalentOf(new ItemStat(StatCategory.Unknown)
                {
                    Text = "Trigger Edict of Frost on Kill",
                })
                .And.ContainEquivalentOf(new ItemStat(StatCategory.Unknown)
                {
                    Text = "Socketed Gems are Supported by Level 20 Vile Toxins",
                })
                .And.ContainEquivalentOf(new ItemStat(StatCategory.Unknown)
                {
                    Text = "22% increased Poison Duration",
                })
                .And.ContainEquivalentOf(new ItemStat(StatCategory.Unknown)
                {
                    Text = "+59 to maximum Life",
                })
                .And.ContainEquivalentOf(new ItemStat(StatCategory.Unknown)
                {
                    Text = "+19% to Chaos Resistance",
                })
                .And.ContainEquivalentOf(new ItemStat(StatCategory.Unknown)
                {
                    Text = "Adds 13 to 28 Chaos Damage",
                });
        }
    }
}