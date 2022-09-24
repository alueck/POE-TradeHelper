using Moq;

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
        private readonly Mock<IStatsDataService> statsDataServiceMock;
        private readonly Mock<IPseudoItemStatsParser> pseudoItemStatsParserMock;
        private readonly ItemStatsParser itemStatsParser;
        private readonly ItemStringBuilder itemStringBuilder;

        public ItemStatsParserTests()
        {
            this.statsDataServiceMock = new Mock<IStatsDataService>();
            this.pseudoItemStatsParserMock = new Mock<IPseudoItemStatsParser>();
            this.itemStatsParser = new ItemStatsParser(this.statsDataServiceMock.Object, this.pseudoItemStatsParserMock.Object);
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

            this.statsDataServiceMock.Setup(x => x.GetStatData(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string[]>()))
                .Returns(new StatData());

            ItemStats result = this.itemStatsParser.Parse(itemStringLines, false);

            Assert.That(result.AllStats, Has.Count.EqualTo(1));

            ItemStat itemStat = result.AllStats.First();
            Assert.That(itemStat.Text, Is.EqualTo(expected));
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

            this.statsDataServiceMock.Setup(x => x.GetStatData(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string[]>()))
                .Returns(new StatData { Text = expected });

            ItemStats result = this.itemStatsParser.Parse(itemStringLines, false);

            Assert.That(result.AllStats, Has.Count.EqualTo(1));

            ItemStat itemStat = result.AllStats.First();
            Assert.That(itemStat.TextWithPlaceholders, Is.EqualTo(expected));
        }

        [TestCase(StatCategory.Explicit, "Minions deal 1 to 15 additional Physical Damage")]
        [TestCase(StatCategory.Implicit, "+25% to Cold Resistance (implicit)")]
        [TestCase(StatCategory.Crafted, "+25% to Cold Resistance (crafted)")]
        [TestCase(StatCategory.Enchant, "10% increased Movement Speed if you haven't been Hit Recently (enchant)")]
        [TestCase(StatCategory.Fractured, "Adds 11 to 142 Lightning Damage (fractured)")]
        public void ParseShouldParseStatTextInCorrectCategory(StatCategory expected, string statText)
        {
            string[] itemStringLines = this.itemStringBuilder
                               .WithName("Titan Greaves")
                               .WithItemLevel(75)
                               .WithItemStat(statText, expected)
                               .BuildLines();

            this.statsDataServiceMock.Setup(x => x.GetStatData(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string[]>()))
                .Returns(new StatData { Type = expected.GetDisplayName() });

            ItemStats result = this.itemStatsParser.Parse(itemStringLines, false);

            Assert.That(result.AllStats, Has.Count.EqualTo(1));

            ItemStat itemStat = result.AllStats.First();
            Assert.That(itemStat.StatCategory, Is.EqualTo(expected));
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

            Assert.That(result.AllStats, Is.Empty);
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

            this.statsDataServiceMock.Verify(x => x.GetStatData(It.IsAny<string>(), It.IsAny<bool>(), StatCategory.Explicit.GetDisplayName()));
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

            this.statsDataServiceMock.Verify(x => x.GetStatData(It.IsAny<string>(), It.IsAny<bool>(), statCategory.GetDisplayName()));
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

            this.statsDataServiceMock.Verify(x => x.GetStatData(It.IsAny<string>(), preferLocalStats, It.IsAny<string[]>()));
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

            this.statsDataServiceMock.Setup(x => x.GetStatData(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string[]>()))
                .Returns(new StatData { Id = expected });

            ItemStats result = this.itemStatsParser.Parse(itemStringLines, false);

            Assert.That(result.AllStats, Has.Count.EqualTo(1));

            ItemStat itemStat = result.AllStats.First();
            Assert.That(itemStat.Id, Is.EqualTo(expected));
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

            this.statsDataServiceMock.Setup(x => x.GetStatData(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string[]>()))
                .Returns(new StatData
                {
                    Text = textWithPlaceholders
                });

            ItemStats result = this.itemStatsParser.Parse(itemStringLines, false);

            Assert.That(result.AllStats, Has.Count.EqualTo(1));

            ItemStat itemStat = result.AllStats.First();
            Assert.That(itemStat.GetType(), Is.EqualTo(typeof(ItemStat)));
        }

        [Test]
        public void ParseShouldReturnSingleValueItemStatIfTextWithPlaceholdersContainsOnePlaceholder()
        {
            string[] itemStringLines = this.itemStringBuilder
                    .WithName("Titan Greaves")
                    .WithItemLevel(75)
                    .WithItemStat("+25% to Cold Resistance", StatCategory.Explicit)
                    .BuildLines();

            this.statsDataServiceMock.Setup(x => x.GetStatData(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string[]>()))
                .Returns(new StatData
                {
                    Text = "#% to Cold Resistance"
                });

            ItemStats result = this.itemStatsParser.Parse(itemStringLines, false);

            Assert.That(result.AllStats, Has.Count.EqualTo(1));

            ItemStat itemStat = result.AllStats.First();
            Assert.IsInstanceOf<SingleValueItemStat>(itemStat);
        }

        [Test]
        public void ParseShouldReturnMinMaxValueItemStatIfTextWithPlaceholderContainsTwoPlaceholders()
        {
            string[] itemStringLines = this.itemStringBuilder
                    .WithName("Titan Greaves")
                    .WithItemLevel(75)
                    .WithItemStat("Minions deal 1 to 15 additional Physical Damage", StatCategory.Explicit)
                    .BuildLines();

            this.statsDataServiceMock.Setup(x => x.GetStatData(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string[]>()))
                .Returns(new StatData
                {
                    Text = "Minions deal # to # additional Physical Damage"
                });

            ItemStats result = this.itemStatsParser.Parse(itemStringLines, false);

            Assert.That(result.AllStats, Has.Count.EqualTo(1));

            ItemStat itemStat = result.AllStats.First();
            Assert.IsInstanceOf<MinMaxValueItemStat>(itemStat);
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

            this.statsDataServiceMock.Setup(x => x.GetStatData(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string[]>()))
                .Returns(new StatData
                {
                    Text = textWithPlaceholders
                });

            ItemStats result = this.itemStatsParser.Parse(itemStringLines, false);

            Assert.That(result.AllStats, Has.Count.EqualTo(1));
            SingleValueItemStat itemStat = (SingleValueItemStat)result.AllStats.First();
            Assert.That(itemStat.Value, Is.EqualTo(expected));
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

            this.statsDataServiceMock.Setup(x => x.GetStatData(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string[]>()))
                .Returns(new StatData
                {
                    Text = textWithPlaceholders
                });

            ItemStats result = this.itemStatsParser.Parse(itemStringLines, false);

            Assert.That(result.AllStats, Has.Count.EqualTo(1));
            MinMaxValueItemStat itemStat = (MinMaxValueItemStat)result.AllStats.First();
            Assert.That(itemStat.MinValue, Is.EqualTo(expected));
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

            this.statsDataServiceMock.Setup(x => x.GetStatData(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string[]>()))
                .Returns(new StatData
                {
                    Text = textWithPlaceholders
                });

            ItemStats result = this.itemStatsParser.Parse(itemStringLines, false);

            Assert.That(result.AllStats, Has.Count.EqualTo(1));
            MinMaxValueItemStat itemStat = (MinMaxValueItemStat)result.AllStats.First();
            Assert.That(itemStat.MaxValue, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldCallParseOnPseudoItemStatsParser()
        {
            string[] itemStringLines = this.itemStringBuilder
                .WithName("Titan Greaves")
                .WithItemLevel(75)
                .WithItemStat("statText", StatCategory.Explicit)
                .BuildLines();

            this.statsDataServiceMock.Setup(x => x.GetStatData(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string[]>()))
                .Returns(new StatData());

            this.pseudoItemStatsParserMock.Setup(x => x.Parse(It.IsAny<IEnumerable<ItemStat>>()))
                .Callback(() => this.statsDataServiceMock.Verify(x => x.GetStatData(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string[]>())));

            ItemStats result = this.itemStatsParser.Parse(itemStringLines, false);

            this.pseudoItemStatsParserMock.Verify(x => x.Parse(It.Is<IEnumerable<ItemStat>>(enumerable => enumerable.SequenceEqual(result.AllStats))));
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

            this.statsDataServiceMock.Setup(x => x.GetStatData(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<string[]>()))
                .Returns(new StatData());

            this.pseudoItemStatsParserMock.Setup(x => x.Parse(It.IsAny<IEnumerable<ItemStat>>()))
                .Returns(new List<ItemStat> { expected });

            ItemStats result = this.itemStatsParser.Parse(itemStringLines, false);

            Assert.That(result.AllStats, Contains.Item(expected));
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
                MaxValue = 15
            };

            ItemStat expectedImplicitItemStat = new SingleValueItemStat(StatCategory.Implicit)
            {
                Id = "implicit item stat id",
                Text = "10% increased Movement Speed",
                TextWithPlaceholders = "#% increased Movement Speed",
                Value = 10
            };

            ItemStat expectedCraftedItemStat = new SingleValueItemStat(StatCategory.Crafted)
            {
                Id = "crafted item stat id",
                Text = "+25% to Cold Resistance",
                TextWithPlaceholders = "#% to Cold Resistance",
                Value = 25
            };

            ItemStat expectedEnchantedItemStat = new SingleValueItemStat(StatCategory.Enchant)
            {
                Id = "enchanted item stat id",
                Text = "10% increased Movement Speed if you haven't been Hit Recently",
                TextWithPlaceholders = "#% increased Movement Speed if you haven't been Hit Recently",
                Value = 10
            };

            ItemStat[] itemStats = { expectedExplicitItemStat, expectedImplicitItemStat, expectedCraftedItemStat, expectedEnchantedItemStat };

            foreach (var itemStat in itemStats)
            {
                this.statsDataServiceMock.Setup(x => x.GetStatData(It.Is<string>(s => s == itemStat.Text), It.IsAny<bool>(), It.IsAny<string[]>()))
                    .Returns(new StatData { Id = itemStat.Id, Text = itemStat.TextWithPlaceholders, Type = itemStat.StatCategory.GetDisplayName() });
            }

            string[] itemStringLines = this.itemStringBuilder
                   .WithName("Titan Greaves")
                   .WithItemLevel(75)
                   .WithItemStat(expectedExplicitItemStat.Text, expectedExplicitItemStat.StatCategory)
                   .WithItemStat($"{expectedImplicitItemStat.Text} ({StatCategory.Implicit.GetDisplayName().ToLower()})", expectedImplicitItemStat.StatCategory)
                   .WithItemStat($"{expectedCraftedItemStat.Text} ({StatCategory.Crafted.GetDisplayName().ToLower()})", expectedCraftedItemStat.StatCategory)
                   .WithItemStat($"{expectedEnchantedItemStat.Text} ({StatCategory.Enchant.GetDisplayName().ToLower()})", expectedEnchantedItemStat.StatCategory)
                   .BuildLines();

            ItemStats result = this.itemStatsParser.Parse(itemStringLines, false);

            Assert.That(result.AllStats, Has.Count.EqualTo(4));

            Assert.That(result.ExplicitStats, Has.Count.EqualTo(1));
            AssertEquals(expectedExplicitItemStat, result.ExplicitStats.First());

            Assert.That(result.ImplicitStats, Has.Count.EqualTo(1));
            AssertEquals(expectedImplicitItemStat, result.ImplicitStats.First());

            Assert.That(result.CraftedStats, Has.Count.EqualTo(1));
            AssertEquals(expectedCraftedItemStat, result.CraftedStats.First());

            Assert.That(result.EnchantedStats, Has.Count.EqualTo(1));
            AssertEquals(expectedEnchantedItemStat, result.EnchantedStats.First());
        }

        private static void AssertEquals(ItemStat expectedItemStat, ItemStat actualItemStat)
        {
            Assert.That(actualItemStat.GetType(), Is.EqualTo(expectedItemStat.GetType()));
            Assert.That(actualItemStat.Id, Is.EqualTo(expectedItemStat.Id));
            Assert.That(actualItemStat.StatCategory, Is.EqualTo(expectedItemStat.StatCategory));
            Assert.That(actualItemStat.Text, Is.EqualTo(expectedItemStat.Text));
            Assert.That(actualItemStat.TextWithPlaceholders, Is.EqualTo(expectedItemStat.TextWithPlaceholders));

            if (expectedItemStat is SingleValueItemStat expectedSingleValueItemStat)
            {
                Assert.That(((SingleValueItemStat)actualItemStat).Value, Is.EqualTo(expectedSingleValueItemStat.Value));
            }
            else if (expectedItemStat is MinMaxValueItemStat expectedMinMaxValueItemStat)
            {
                var actualMinMaxValueItemStat = (MinMaxValueItemStat)actualItemStat;
                Assert.That(actualMinMaxValueItemStat.MinValue, Is.EqualTo(expectedMinMaxValueItemStat.MinValue));
                Assert.That(actualMinMaxValueItemStat.MaxValue, Is.EqualTo(expectedMinMaxValueItemStat.MaxValue));
            }
        }
    }
}