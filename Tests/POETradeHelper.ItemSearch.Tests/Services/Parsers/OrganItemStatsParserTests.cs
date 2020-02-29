using Moq;
using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services.Parsers;
using POETradeHelper.ItemSearch.Tests.TestHelpers;
using System.Linq;

namespace POETradeHelper.ItemSearch.Tests.Services.Parsers
{
    public class OrganItemStatsParserTests
    {
        private Mock<IStatsDataService> statsDataServiceMock;
        private OrganItemStatsParser organItemStatsParser;
        private ItemStringBuilder itemStringBuilder;

        [SetUp]
        public void Setup()
        {
            this.statsDataServiceMock = new Mock<IStatsDataService>();
            this.organItemStatsParser = new OrganItemStatsParser(this.statsDataServiceMock.Object);
            this.itemStringBuilder = new ItemStringBuilder();
        }

        [Test]
        public void ParseShouldParseStatWithCorrectText()
        {
            const string expected = "Drops additional Currency Items";
            string[] itemStringLines = this.itemStringBuilder
                                .WithType("Oriath's Virtue's Eye")
                                .WithItemLevel(73)
                                .WithExplicitItemStat(expected)
                                .BuildLines();

            OrganItemStats result = this.organItemStatsParser.Parse(itemStringLines);

            Assert.That(result.Stats, Has.Count.EqualTo(1));

            MonsterItemStat stat = result.Stats.First();
            Assert.That(stat.Text, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseStatWithCorrectTextWithPlaceholders()
        {
            const string expected = "Drops additional Currency Items (×#)";
            string[] itemStringLines = this.itemStringBuilder
                                .WithType("Oriath's Virtue's Eye")
                                .WithItemLevel(73)
                                .WithExplicitItemStat("Drops additional Currency Items")
                                .BuildLines();

            OrganItemStats result = this.organItemStatsParser.Parse(itemStringLines);

            MonsterItemStat stat = result.Stats.First();
            Assert.That(stat.TextWithPlaceholders, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseStatWithCorrectCount()
        {
            const string statText = "Drops additional Currency Items";
            string[] itemStringLines = this.itemStringBuilder
                                .WithType("Oriath's Virtue's Eye")
                                .WithItemLevel(73)
                                .WithExplicitItemStat(statText)
                                .WithExplicitItemStat(statText)
                                .WithExplicitItemStat(statText)
                                .BuildLines();

            OrganItemStats result = this.organItemStatsParser.Parse(itemStringLines);

            Assert.That(result.Stats, Has.Count.EqualTo(1));

            MonsterItemStat stat = result.Stats.First();
            Assert.That(stat.Count, Is.EqualTo(3));
        }

        [Test]
        public void ParseShouldCallGetIdOnStatDataService()
        {
            string[] itemStringLines = this.itemStringBuilder
                                .WithType("Oriath's Virtue's Eye")
                                .WithItemLevel(73)
                                .WithExplicitItemStat("Drops additional Currency Items")
                                .BuildLines();

            OrganItemStats result = this.organItemStatsParser.Parse(itemStringLines);

            foreach (ItemStat stat in result.Stats)
            {
                this.statsDataServiceMock.Verify(x => x.GetId(stat));
            }
        }

        [Test]
        public void ParseShouldSetStatIdFromStatDataService()
        {
            const string expected = "item stat id";
            string[] itemStringLines = this.itemStringBuilder
                    .WithType("Oriath's Virtue's Eye")
                    .WithItemLevel(73)
                    .WithExplicitItemStat("Drops additional Currency Items")
                    .BuildLines();

            this.statsDataServiceMock.Setup(x => x.GetId(It.IsAny<ItemStat>()))
                .Returns(expected);

            OrganItemStats result = this.organItemStatsParser.Parse(itemStringLines);

            MonsterItemStat stat = result.Stats.First();
            Assert.That(stat.Id, Is.EqualTo(expected));
        }
    }
}