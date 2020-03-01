using Moq;
using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services.Parsers;
using POETradeHelper.ItemSearch.Tests.TestHelpers;
using System.Linq;

namespace POETradeHelper.ItemSearch.Tests.Services.Parsers
{
    public class FlaskItemStatsParserTests
    {
        private Mock<IStatsDataService> statsDataServiceMock;
        private FlaskItemStatsParser flaskItemStatsParser;
        private ItemStringBuilder itemStringBuilder;

        [SetUp]
        public void Setup()
        {
            this.statsDataServiceMock = new Mock<IStatsDataService>();
            this.flaskItemStatsParser = new FlaskItemStatsParser(this.statsDataServiceMock.Object);
            this.itemStringBuilder = new ItemStringBuilder();
        }

        [Test]
        public void ParseShouldParseStatText()
        {
            const string expected = "100% increased Recovery when on Low Life";
            string[] itemStringLines = this.itemStringBuilder
                                           .WithName("Divine Life Flask")
                                           .WithItemLevel(60)
                                           .WithExplicitItemStat(expected)
                                           .BuildLines();

            FlaskItemStats result = this.flaskItemStatsParser.Parse(itemStringLines);

            Assert.That(result.ExplicitStats, Has.Count.EqualTo(1));

            ExplicitItemStat stat = result.ExplicitStats.First();
            Assert.That(stat.Text, Is.EqualTo(expected));
        }

        [TestCase("Grants 58% of Life Recovery to Minions", "Grants #% of Life Recovery to Minions")]
        [TestCase("100% increased Recovery when on Low Life", "#% increased Recovery when on Low Life")]
        public void ParseShouldParseStatTextWithPlaceHolders(string statText, string expected)
        {
            string[] itemStringLines = this.itemStringBuilder
                                           .WithName("Divine Life Flask")
                                           .WithItemLevel(60)
                                           .WithExplicitItemStat(statText)
                                           .BuildLines();

            FlaskItemStats result = this.flaskItemStatsParser.Parse(itemStringLines);

            Assert.That(result.ExplicitStats, Has.Count.EqualTo(1));

            ExplicitItemStat stat = result.ExplicitStats.First();
            Assert.That(stat.TextWithPlaceholders, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldCallGetIdOnStatsDataService()
        {
            string[] itemStringLines = this.itemStringBuilder
                               .WithName("Divine Life Flask")
                               .WithItemLevel(60)
                               .WithExplicitItemStat("Grants 58% of Life Recovery to Minions")
                               .WithExplicitItemStat("100% increased Recovery when on Low Life")
                               .BuildLines();

            FlaskItemStats result = this.flaskItemStatsParser.Parse(itemStringLines);

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
                   .WithName("Divine Life Flask")
                   .WithItemLevel(60)
                   .WithExplicitItemStat("Grants 58% of Life Recovery to Minions")
                   .BuildLines();

            this.statsDataServiceMock.Setup(x => x.GetId(It.IsAny<ItemStat>()))
                .Returns(expected);

            FlaskItemStats result = this.flaskItemStatsParser.Parse(itemStringLines);

            Assert.That(result.ExplicitStats, Has.Count.EqualTo(1));

            ExplicitItemStat stat = result.ExplicitStats.First();
            Assert.That(stat.Id, Is.EqualTo(expected));
        }
    }
}