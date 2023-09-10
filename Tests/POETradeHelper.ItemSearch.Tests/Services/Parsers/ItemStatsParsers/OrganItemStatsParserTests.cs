using NSubstitute;

using NUnit.Framework;

using POETradeHelper.Common.Extensions;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Services.Parsers.ItemStatsParsers;
using POETradeHelper.ItemSearch.Tests.TestHelpers.ItemStringBuilders;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.ItemSearch.Tests.Services.Parsers.ItemStatsParsers
{
    public class OrganItemStatsParserTests
    {
        private readonly IStatsDataService statsDataServiceMock;
        private readonly OrganItemStatsParser organItemStatsParser;
        private readonly ItemStringBuilder itemStringBuilder;

        public OrganItemStatsParserTests()
        {
            this.statsDataServiceMock = Substitute.For<IStatsDataService>();
            this.statsDataServiceMock.GetStatData(Arg.Any<string>(), Arg.Any<bool>(), StatCategory.Monster.GetDisplayName())
                .Returns(ctx => new StatData
                {
                    Type = StatCategory.Monster.GetDisplayName().ToLower(),
                    Text = ctx.Arg<string>(),
                });

            this.organItemStatsParser = new OrganItemStatsParser(this.statsDataServiceMock);
            this.itemStringBuilder = new ItemStringBuilder();
        }

        [Test]
        public void ParseShouldParseStatWithCorrectText()
        {
            const string expected = "Drops additional Currency Items";
            string[] itemStringLines = this.itemStringBuilder
                                .WithType("Oriath's Virtue's Eye")
                                .WithItemLevel(73)
                                .WithItemStat(expected, StatCategory.Monster)
                                .WithDescription(Resources.OrganItemDescriptor)
                                .BuildLines();

            ItemStats result = this.organItemStatsParser.Parse(itemStringLines, false);

            Assert.That(result.AllStats, Has.Count.EqualTo(1));
            Assert.That(result.MonsterStats, Has.Count.EqualTo(1));

            ItemStat stat = result.MonsterStats.First();
            Assert.That(stat.Text, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldParseStatWithCorrectCount()
        {
            const string statText = "Drops additional Currency Items";
            string[] itemStringLines = this.itemStringBuilder
                                .WithType("Oriath's Virtue's Eye")
                                .WithItemLevel(73)
                                .WithItemStat(statText, StatCategory.Monster)
                                .WithItemStat(statText, StatCategory.Monster)
                                .WithItemStat(statText, StatCategory.Monster)
                                .WithDescription(Resources.OrganItemDescriptor)
                                .BuildLines();

            ItemStats result = this.organItemStatsParser.Parse(itemStringLines, false);

            Assert.That(result.AllStats, Has.Count.EqualTo(1));
            Assert.That(result.MonsterStats, Has.Count.EqualTo(1));

            SingleValueItemStat stat = (SingleValueItemStat)result.MonsterStats.First();
            Assert.NotNull(stat);
            Assert.That(stat.Value, Is.EqualTo(3));
        }

        [Test]
        public void ParseShouldCallGetStatDataOnStatDataService()
        {
            string[] itemStringLines = this.itemStringBuilder
                                .WithType("Oriath's Virtue's Eye")
                                .WithItemLevel(73)
                                .WithItemStat("Drops additional Currency Items", StatCategory.Monster)
                                .WithDescription(Resources.OrganItemDescriptor)
                                .BuildLines();

            ItemStats result = this.organItemStatsParser.Parse(itemStringLines, false);

            Assert.That(result.AllStats, Has.Count.EqualTo(1));
            Assert.That(result.MonsterStats, Has.Count.EqualTo(1));

            foreach (ItemStat stat in result.MonsterStats)
            {
                this.statsDataServiceMock
                .Received()
                .GetStatData(stat.Text, false, StatCategory.Monster.GetDisplayName());
            }
        }

        [Test]
        public void ParseShouldSetStatIdFromStatDataService()
        {
            const string expected = "item stat id";
            string[] itemStringLines = this.itemStringBuilder
                    .WithType("Oriath's Virtue's Eye")
                    .WithItemLevel(73)
                    .WithItemStat("Drops additional Currency Items", StatCategory.Monster)
                    .WithDescription(Resources.OrganItemDescriptor)
                    .BuildLines();

            this.statsDataServiceMock.GetStatData(Arg.Any<string>(), Arg.Any<bool>(), StatCategory.Monster.GetDisplayName())
                .Returns(new StatData
                {
                    Id = expected,
                    Type = StatCategory.Monster.GetDisplayName()
                });

            ItemStats result = this.organItemStatsParser.Parse(itemStringLines, false);

            Assert.That(result.AllStats, Has.Count.EqualTo(1));
            Assert.That(result.MonsterStats, Has.Count.EqualTo(1));

            ItemStat stat = result.MonsterStats.First();
            Assert.That(stat.Id, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldSetTextWithPlaceHoldersFromStatDataService()
        {
            const string expected = "Stat text with placeholders";
            string[] itemStringLines = this.itemStringBuilder
                    .WithType("Oriath's Virtue's Eye")
                    .WithItemLevel(73)
                    .WithItemStat("Drops additional Currency Items", StatCategory.Monster)
                    .WithDescription(Resources.OrganItemDescriptor)
                    .BuildLines();

            this.statsDataServiceMock.GetStatData(Arg.Any<string>(), Arg.Any<bool>(), StatCategory.Monster.GetDisplayName())
                .Returns(new StatData
                {
                    Text = expected,
                    Type = StatCategory.Monster.GetDisplayName()
                });

            ItemStats result = this.organItemStatsParser.Parse(itemStringLines, false);

            Assert.That(result.AllStats, Has.Count.EqualTo(1));
            Assert.That(result.MonsterStats, Has.Count.EqualTo(1));

            ItemStat stat = result.MonsterStats.First();
            Assert.That(stat.TextWithPlaceholders, Is.EqualTo(expected));
        }
    }
}
