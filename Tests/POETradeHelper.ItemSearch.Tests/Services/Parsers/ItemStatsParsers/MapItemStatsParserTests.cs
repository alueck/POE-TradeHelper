using Moq;
using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services.Parsers;
using POETradeHelper.ItemSearch.Tests.TestHelpers;
using System.Linq;

namespace POETradeHelper.ItemSearch.Tests.Services.Parsers
{
    public class MapItemStatsParserTests
    {
        private Mock<IStatsDataService> statsDataServiceMock;
        private MapItemStatsParser mapItemStatsParser;
        private ItemStringBuilder itemStringBuilder;

        [SetUp]
        public void Setup()
        {
            this.statsDataServiceMock = new Mock<IStatsDataService>();
            this.mapItemStatsParser = new MapItemStatsParser(this.statsDataServiceMock.Object);
            this.itemStringBuilder = new ItemStringBuilder();
        }

        [Test]
        public void ParseShouldParseStatText()
        {
            const string expected = "Monsters deal 100% extra Damage as Fire";
            string[] itemStringLines = this.itemStringBuilder
                                           .WithType("Thicket Map")
                                           .WithItemLevel(60)
                                           .WithExplicitItemStat(expected)
                                           .BuildLines();

            MapItemStats result = this.mapItemStatsParser.Parse(itemStringLines);

            Assert.That(result.ExplicitStats, Has.Count.EqualTo(1));

            ExplicitItemStat stat = result.ExplicitStats.First();
            Assert.That(stat.Text, Is.EqualTo(expected));
        }

        [TestCase("Monsters deal 100% extra Damage as Fire", "Monsters deal #% extra Damage as Fire")]
        [TestCase("Monsters deal 59% extra Damage as Cold", "Monsters deal #% extra Damage as Cold")]
        public void ParseShouldParseStatTextWithPlaceHolders(string statText, string expected)
        {
            string[] itemStringLines = this.itemStringBuilder
                                           .WithType("Thicket Map")
                                           .WithItemLevel(60)
                                           .WithExplicitItemStat(statText)
                                           .BuildLines();

            MapItemStats result = this.mapItemStatsParser.Parse(itemStringLines);

            Assert.That(result.ExplicitStats, Has.Count.EqualTo(1));

            ExplicitItemStat stat = result.ExplicitStats.First();
            Assert.That(stat.TextWithPlaceholders, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldCallGetIdOnStatsDataService()
        {
            string[] itemStringLines = this.itemStringBuilder
                               .WithType("Thicket Map")
                               .WithItemLevel(60)
                               .WithExplicitItemStat("Monsters deal 100% extra Damage as Fire")
                               .WithExplicitItemStat("Monsters deal 59% extra Damage as Cold")
                               .BuildLines();

            MapItemStats result = this.mapItemStatsParser.Parse(itemStringLines);

            Assert.That(result.ExplicitStats, Has.Count.EqualTo(2));

            foreach (ExplicitItemStat explicitItemStat in result.ExplicitStats)
            {
                this.statsDataServiceMock.Verify(x => x.GetId(explicitItemStat));
            }
        }

        [Test]
        public void ParseShouldSetIdOnStatFromStatsDataService()
        {
            const string expected = "item stat id";
            string[] itemStringLines = this.itemStringBuilder
                   .WithType("Thicket Map")
                   .WithItemLevel(60)
                   .WithExplicitItemStat("Monsters deal 100% extra Damage as Fire")
                   .BuildLines();

            this.statsDataServiceMock.Setup(x => x.GetId(It.IsAny<ItemStat>()))
                .Returns(expected);

            MapItemStats result = this.mapItemStatsParser.Parse(itemStringLines);

            Assert.That(result.ExplicitStats, Has.Count.EqualTo(1));

            ExplicitItemStat stat = result.ExplicitStats.First();
            Assert.That(stat.Id, Is.EqualTo(expected));
        }
    }
}