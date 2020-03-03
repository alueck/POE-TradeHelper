using Moq;
using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
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
                                .WithDescription(Resources.OrganItemDescriptor)
                                .BuildLines();

            ItemStats result = this.organItemStatsParser.Parse(itemStringLines);

            Assert.That(result.AllStats, Has.Count.EqualTo(1));
            Assert.That(result.MonsterStats, Has.Count.EqualTo(1));

            MonsterItemStat stat = result.MonsterStats.First();
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
                                .WithDescription(Resources.OrganItemDescriptor)
                                .BuildLines();

            ItemStats result = this.organItemStatsParser.Parse(itemStringLines);

            Assert.That(result.AllStats, Has.Count.EqualTo(1));
            Assert.That(result.MonsterStats, Has.Count.EqualTo(1));

            MonsterItemStat stat = result.MonsterStats.First();
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
                                .WithDescription(Resources.OrganItemDescriptor)
                                .BuildLines();

            ItemStats result = this.organItemStatsParser.Parse(itemStringLines);

            Assert.That(result.AllStats, Has.Count.EqualTo(1));
            Assert.That(result.MonsterStats, Has.Count.EqualTo(1));

            MonsterItemStat stat = result.MonsterStats.First();
            Assert.That(stat.Count, Is.EqualTo(3));
        }

        [Test]
        public void ParseShouldCallGetIdOnStatDataService()
        {
            string[] itemStringLines = this.itemStringBuilder
                                .WithType("Oriath's Virtue's Eye")
                                .WithItemLevel(73)
                                .WithExplicitItemStat("Drops additional Currency Items")
                                .WithDescription(Resources.OrganItemDescriptor)
                                .BuildLines();

            ItemStats result = this.organItemStatsParser.Parse(itemStringLines);

            Assert.That(result.AllStats, Has.Count.EqualTo(1));
            Assert.That(result.MonsterStats, Has.Count.EqualTo(1));

            foreach (ItemStat stat in result.MonsterStats)
            {
                this.statsDataServiceMock.Verify(x => x.GetStatData(stat));
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
                    .WithDescription(Resources.OrganItemDescriptor)
                    .BuildLines();

            this.statsDataServiceMock.Setup(x => x.GetStatData(It.IsAny<ItemStat>()))
                .Returns(new StatData { Id = expected });

            ItemStats result = this.organItemStatsParser.Parse(itemStringLines);

            Assert.That(result.AllStats, Has.Count.EqualTo(1));
            Assert.That(result.MonsterStats, Has.Count.EqualTo(1));

            MonsterItemStat stat = result.MonsterStats.First();
            Assert.That(stat.Id, Is.EqualTo(expected));
        }
    }
}