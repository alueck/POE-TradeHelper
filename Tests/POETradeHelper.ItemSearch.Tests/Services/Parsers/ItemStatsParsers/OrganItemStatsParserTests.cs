using AwesomeAssertions;

using Microsoft.Extensions.Logging;

using NSubstitute;

using NUnit.Framework;

using POETradeHelper.Common.Extensions;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Properties;
using POETradeHelper.ItemSearch.Services.Parsers.ItemStatsParsers;
using POETradeHelper.ItemSearch.Tests.TestHelpers;
using POETradeHelper.ItemSearch.Tests.TestHelpers.ItemStringBuilders;
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
            this.statsDataServiceMock.TryGetStatData(Arg.Any<IReadOnlyCollection<string>>(), Arg.Any<bool>(), StatCategory.Monster.GetDisplayName())
                .Returns(
                    ctx => new TestStatData
                    {
                        Type = StatCategory.Monster.GetDisplayName().ToLower(),
                        Text = ctx.Arg<IReadOnlyCollection<string>>()!.FirstOrDefault()!,
                    });

            this.organItemStatsParser = new OrganItemStatsParser(this.statsDataServiceMock, Substitute.For<ILogger<OrganItemStatsParser>>());
            this.itemStringBuilder = new ItemStringBuilder();
        }

        [Test]
        public void ParseShouldParseStatWithCorrectText()
        {
            const string expected = "Drops additional Currency Items";
            string[] itemStringLines = this.itemStringBuilder
                .WithType("Oriath's Virtue's Eye")
                .WithItemLevel(73)
                .WithItemStat($"{expected}{Resources.UnscalableValueSuffix}", StatCategory.Monster)
                .WithDescription(Resources.OrganItemDescriptor)
                .BuildLines();

            ItemStats result = this.organItemStatsParser.Parse(itemStringLines, false);

            result.AllStats.Should().ContainSingle();
            result.MonsterStats.Should().ContainSingle(x => x.Text == expected);
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

            result.AllStats.Should().ContainSingle();
            result.MonsterStats.Should().SatisfyRespectively(x =>
            {
                x.Should().BeOfType<SingleValueItemStat>();
                ((SingleValueItemStat)x).Value.Should().Be(3);
            });
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

            result.MonsterStats.Should().HaveCount(1);

            foreach (ItemStat stat in result.MonsterStats)
            {
                this.statsDataServiceMock
                    .Received()
                    .TryGetStatData(Arg.Is<IReadOnlyCollection<string>>(s => s!.Contains(stat.Text)), false, StatCategory.Monster.GetDisplayName());
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

            this.statsDataServiceMock.TryGetStatData(Arg.Any<IReadOnlyCollection<string>>(), Arg.Any<bool>(), StatCategory.Monster.GetDisplayName())
                .Returns(
                    new TestStatData
                    {
                        Id = expected,
                        Type = StatCategory.Monster.GetDisplayName(),
                    });

            ItemStats result = this.organItemStatsParser.Parse(itemStringLines, false);

            result.AllStats.Should().ContainSingle();
            result.MonsterStats.Should().ContainSingle(x => x.Id == expected);
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

            this.statsDataServiceMock.TryGetStatData(Arg.Any<IReadOnlyCollection<string>>(), Arg.Any<bool>(), StatCategory.Monster.GetDisplayName())
                .Returns(
                    new TestStatData
                    {
                        Text = expected,
                        Type = StatCategory.Monster.GetDisplayName(),
                    });

            ItemStats result = this.organItemStatsParser.Parse(itemStringLines, false);

            result.AllStats.Should().ContainSingle();
            result.MonsterStats.Should().ContainSingle(x => x.TextWithPlaceholders == expected);
        }
    }
}