using FluentAssertions;

using NSubstitute;

using NUnit.Framework;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services.Parsers.ItemStatsParsers;
using POETradeHelper.PathOfExileTradeApi.Constants;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.ItemSearch.Tests.Services.Parsers.ItemStatsParsers
{
    public class PseudoItemStatsParserTests
    {
        private readonly IPseudoStatDataMappingService pseudoStatsDataMappingServiceMock;
        private readonly PseudoItemStatsParser pseudoItemStatsParser;

        public PseudoItemStatsParserTests()
        {
            this.pseudoStatsDataMappingServiceMock = Substitute.For<IPseudoStatDataMappingService>();
            this.pseudoItemStatsParser = new PseudoItemStatsParser(this.pseudoStatsDataMappingServiceMock);
        }

        [Test]
        public void ParseShouldCallGetPseudoStatDataOnPseudoStatDataMappingService()
        {
            List<ItemStat> itemStats = new List<ItemStat>
            {
                new(StatCategory.Explicit) { Id = "explicit stat id" },
                new(StatCategory.Implicit) { Id = "implicit stat id" },
            };

            this.pseudoItemStatsParser.Parse(itemStats);

            this.pseudoStatsDataMappingServiceMock
                .Received()
                .GetPseudoStatData(itemStats[0].Id);
            this.pseudoStatsDataMappingServiceMock
                .Received()
                .GetPseudoStatData(itemStats[1].Id);
        }

        [TestCase(2)]
        [TestCase(4)]
        public void ParseShouldReturnPseudoItemStatIfTheSamePseudoStatDataIsReturnedTwoOrMoreTimes(int count)
        {
            IList<ItemStat> itemStats = new List<ItemStat>();

            for (int i = 0; i < count; i++)
            {
                itemStats.Add(new SingleValueItemStat(StatCategory.Explicit) { Id = $"{i}" });
            }

            StatData pseudoStatData = new() { Id = "test stat data" };
            this.pseudoStatsDataMappingServiceMock.GetPseudoStatData(Arg.Any<string>())
                .Returns(new[] { pseudoStatData });

            IEnumerable<ItemStat> result = this.pseudoItemStatsParser.Parse(itemStats).ToList();

            result.Should().HaveCount(1);
        }

        [Test]
        public void ParseShouldReturnEmptyListIfNoPseudoStatDataExistsTwoOrMoreTimes()
        {
            IList<ItemStat> itemStats = new List<ItemStat> { new(StatCategory.Explicit) };

            StatData pseudoStatData = new() { Id = "test stat data" };
            this.pseudoStatsDataMappingServiceMock.GetPseudoStatData(Arg.Any<string>())
                .Returns(new[] { pseudoStatData });

            IEnumerable<ItemStat> result = this.pseudoItemStatsParser.Parse(itemStats);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void ParseShouldReturnSingleValueItemStatIfItemStatsWithMatchingPseudoStatDataAreSingleValueItemStats()
        {
            IList<ItemStat> itemStats = new List<ItemStat>
            {
                new SingleValueItemStat(StatCategory.Explicit) { Id = "stat 1" },
                new SingleValueItemStat(StatCategory.Implicit) { Id = "stat 2" },
            };

            StatData pseudoStatData = new() { Id = "test stat data" };
            this.pseudoStatsDataMappingServiceMock.GetPseudoStatData(Arg.Any<string>())
                .Returns(new[] { pseudoStatData });

            IEnumerable<ItemStat> result = this.pseudoItemStatsParser.Parse(itemStats);

            ItemStat itemStat = result.First();
            Assert.IsInstanceOf<SingleValueItemStat>(itemStat);
            Assert.That(itemStat.StatCategory, Is.EqualTo(StatCategory.Pseudo));
        }

        [Test]
        public void ParseShouldReturnSingleValueItemStatWithId()
        {
            const string expected = "single value item stat id";

            IList<ItemStat> itemStats = new List<ItemStat>
            {
                new SingleValueItemStat(StatCategory.Explicit) { Id = "stat 1" },
                new SingleValueItemStat(StatCategory.Implicit) { Id = "stat 2" },
            };

            StatData pseudoStatData = new() { Id = expected };
            this.pseudoStatsDataMappingServiceMock.GetPseudoStatData(Arg.Any<string>())
                .Returns(new[] { pseudoStatData });

            IEnumerable<ItemStat> result = this.pseudoItemStatsParser.Parse(itemStats);

            Assert.That(result.First().Id, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldReturnSingleValueItemStatWithTextWithPlaceholders()
        {
            const string expected = "+#% total to Fire Resistance";

            IList<ItemStat> itemStats = new List<ItemStat>
            {
                new SingleValueItemStat(StatCategory.Explicit) { Id = "stat 1" },
                new SingleValueItemStat(StatCategory.Implicit) { Id = "stat 2" },
            };

            StatData pseudoStatData = new() { Id = "test stat data", Text = expected };
            this.pseudoStatsDataMappingServiceMock.GetPseudoStatData(Arg.Any<string>())
                .Returns(new[] { pseudoStatData });

            IEnumerable<ItemStat> result = this.pseudoItemStatsParser.Parse(itemStats);

            Assert.That(result.First().TextWithPlaceholders, Is.EqualTo(expected));
        }

        [TestCase(10, 12, 22)]
        [TestCase(3, 11, 14)]
        public void ParseShouldReturnSingleValueItemStatWithValueCombinedFromMatchingItemStats(int value1, int value2, int expected)
        {
            IList<ItemStat> itemStats = new List<ItemStat>
            {
                new SingleValueItemStat(StatCategory.Explicit) { Id = "stat 1", Value = value1 },
                new SingleValueItemStat(StatCategory.Implicit) { Id = "stat 2", Value = value2 },
                new SingleValueItemStat(StatCategory.Implicit) { Value = 10 },
            };

            StatData pseudoStatData = new() { Id = "test stat data" };
            this.pseudoStatsDataMappingServiceMock.GetPseudoStatData(Arg.Is<string>(s => !string.IsNullOrEmpty(s)))
                .Returns(new[] { pseudoStatData });

            this.pseudoStatsDataMappingServiceMock.GetPseudoStatData(Arg.Is<string>(s => string.IsNullOrEmpty(s)))
                .Returns(new[] { new StatData { Id = "other stat data " } });

            IEnumerable<ItemStat> result = this.pseudoItemStatsParser.Parse(itemStats);

            SingleValueItemStat itemStat = (SingleValueItemStat)result.First();
            Assert.That(itemStat.Value, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldReturnMinMaxValueItemStatIfItemStatsWithMatchingPseudoStatDataAreMinMaxValueItemStats()
        {
            IList<ItemStat> itemStats = new List<ItemStat>
            {
                new MinMaxValueItemStat(StatCategory.Explicit) { Id = "stat 1" },
                new MinMaxValueItemStat(StatCategory.Implicit) { Id = "stat 2" },
            };

            StatData pseudoStatData = new() { Id = "test stat data" };
            this.pseudoStatsDataMappingServiceMock.GetPseudoStatData(Arg.Any<string>())
                .Returns(new[] { pseudoStatData });

            IEnumerable<ItemStat> result = this.pseudoItemStatsParser.Parse(itemStats);

            ItemStat itemStat = result.First();
            Assert.IsInstanceOf<MinMaxValueItemStat>(itemStat);
            Assert.That(itemStat.StatCategory, Is.EqualTo(StatCategory.Pseudo));
        }

        [Test]
        public void ParseShouldReturnMinmaxValueItemStatWithId()
        {
            const string expected = "min max value item stat id";

            IList<ItemStat> itemStats = new List<ItemStat>
            {
                new MinMaxValueItemStat(StatCategory.Explicit) { Id = "stat 1" },
                new MinMaxValueItemStat(StatCategory.Implicit) { Id = "stat 2" },
            };

            StatData pseudoStatData = new() { Id = expected };
            this.pseudoStatsDataMappingServiceMock.GetPseudoStatData(Arg.Any<string>())
                .Returns(new[] { pseudoStatData });

            IEnumerable<ItemStat> result = this.pseudoItemStatsParser.Parse(itemStats);

            Assert.That(result.First().Id, Is.EqualTo(expected));
        }

        [Test]
        public void ParseShouldReturnMinMaxValueItemStatWithTextWithPlaceholders()
        {
            const string expected = "+#% total to Fire Resistance";

            IList<ItemStat> itemStats = new List<ItemStat>
            {
                new MinMaxValueItemStat(StatCategory.Explicit) { Id = "stat 1" },
                new MinMaxValueItemStat(StatCategory.Implicit) { Id = "stat 2" },
            };

            StatData pseudoStatData = new() { Id = "test stat data", Text = expected };
            this.pseudoStatsDataMappingServiceMock.GetPseudoStatData(Arg.Any<string>())
                .Returns(new[] { pseudoStatData });

            IEnumerable<ItemStat> result = this.pseudoItemStatsParser.Parse(itemStats);

            Assert.That(result.First().TextWithPlaceholders, Is.EqualTo(expected));
        }

        [TestCase(7, 14, 21)]
        [TestCase(10, 14, 24)]
        public void ParseShouldReturnMinMaxValueItemStatWithMinValueCombinedFromMatchingItemStats(int minValue1, int minValue2, int expected)
        {
            IList<ItemStat> itemStats = new List<ItemStat>
            {
                new MinMaxValueItemStat(StatCategory.Explicit) { Id = "stat 1", MinValue = minValue1 },
                new MinMaxValueItemStat(StatCategory.Implicit) { Id = "stat 2", MinValue = minValue2 },
                new MinMaxValueItemStat(StatCategory.Implicit) { MinValue = 10 },
            };

            StatData pseudoStatData = new() { Id = "test stat data" };
            this.pseudoStatsDataMappingServiceMock.GetPseudoStatData(Arg.Is<string>(s => !string.IsNullOrEmpty(s)))
                .Returns(new[] { pseudoStatData });

            this.pseudoStatsDataMappingServiceMock.GetPseudoStatData(Arg.Is<string>(s => string.IsNullOrEmpty(s)))
                .Returns(new[] { new StatData { Id = "other stat data " } });

            IEnumerable<ItemStat> result = this.pseudoItemStatsParser.Parse(itemStats);

            MinMaxValueItemStat itemStat = (MinMaxValueItemStat)result.First();
            Assert.That(itemStat.MinValue, Is.EqualTo(expected));
        }

        [TestCase(45, 64, 109)]
        [TestCase(35, 22, 57)]
        public void ParseShouldReturnMaxMaxValueItemStatWithMaxValueCombinedFromMatchingItemStats(int maxValue1, int maxValue2, int expected)
        {
            IList<ItemStat> itemStats = new List<ItemStat>
            {
                new MinMaxValueItemStat(StatCategory.Explicit) { Id = "stat 1", MaxValue = maxValue1 },
                new MinMaxValueItemStat(StatCategory.Implicit) { Id = "stat 2", MaxValue = maxValue2 },
                new MinMaxValueItemStat(StatCategory.Implicit) { MaxValue = 10 },
            };

            StatData pseudoStatData = new() { Id = "test stat data" };
            this.pseudoStatsDataMappingServiceMock.GetPseudoStatData(Arg.Is<string>(s => !string.IsNullOrEmpty(s)))
                .Returns(new[] { pseudoStatData });

            this.pseudoStatsDataMappingServiceMock.GetPseudoStatData(Arg.Is<string>(s => string.IsNullOrEmpty(s)))
                .Returns(new[] { new StatData { Id = "other stat data " } });

            IEnumerable<ItemStat> result = this.pseudoItemStatsParser.Parse(itemStats);

            MinMaxValueItemStat itemStat = (MinMaxValueItemStat)result.First();
            Assert.That(itemStat.MaxValue, Is.EqualTo(expected));
        }

        [TestCase(StatId.FireAndColdResistance, StatId.FireResistance, PseudoStatId.TotalFireResistance)]
        [TestCase(StatId.FireAndLightningResistance, StatId.LightningResistance, PseudoStatId.TotalLightningResistance)]
        [TestCase(StatId.ColdAndLightningResistance, StatId.ColdResistance, PseudoStatId.TotalColdResistance)]
        public void ParseShouldConsiderDoubleResistanceStats(string doubleResistanceStatId, string resistanceStatId, string expectedPseudoTotalResistanceStatId)
        {
            // arrange
            const int doubleResistanceStatValue = 10;
            const int resistanceStatValue = 17;

            const int expectedTotalElementalResistance = (doubleResistanceStatValue * 2) + resistanceStatValue;
            const int expectedTotalSingleResistanceValue = doubleResistanceStatValue + resistanceStatValue;

            IList<ItemStat> itemStats = new List<ItemStat>
            {
                new SingleValueItemStat(StatCategory.Explicit)
                    { Id = doubleResistanceStatId, Value = doubleResistanceStatValue },
                new SingleValueItemStat(StatCategory.Explicit) { Id = resistanceStatId, Value = resistanceStatValue },
            };

            this.SetupPseudoStatsDataMappingServiceMockForDoubleResistanceStats();

            // act
            IEnumerable<ItemStat> result = this.pseudoItemStatsParser.Parse(itemStats);

            // assert
            SingleValueItemStat itemStat =
                (SingleValueItemStat)result.First(i => i.Id == PseudoStatId.TotalElementalResistance);
            Assert.That(itemStat.Value, Is.EqualTo(expectedTotalElementalResistance));

            itemStat = (SingleValueItemStat)result.First(i => i.Id == PseudoStatId.TotalResistance);
            Assert.That(itemStat.Value, Is.EqualTo(expectedTotalElementalResistance));

            itemStat = (SingleValueItemStat)result.First(i => i.Id == expectedPseudoTotalResistanceStatId);
            Assert.That(itemStat.Value, Is.EqualTo(expectedTotalSingleResistanceValue));
        }

        private void SetupPseudoStatsDataMappingServiceMockForDoubleResistanceStats()
        {
            StatData totalFireResistanceStatData = new() { Id = PseudoStatId.TotalFireResistance };
            StatData totalColdResistanceStatData = new() { Id = PseudoStatId.TotalColdResistance };
            StatData totalLightningResistanceStatData = new() { Id = PseudoStatId.TotalLightningResistance };
            StatData totalElementalResistanceStatData = new() { Id = PseudoStatId.TotalElementalResistance };
            StatData totalResistanceStatData = new() { Id = PseudoStatId.TotalResistance };

            this.pseudoStatsDataMappingServiceMock.GetPseudoStatData(Arg.Is<string>(s => s == StatId.FireAndColdResistance))
                .Returns(new[]
                {
                    totalFireResistanceStatData, totalColdResistanceStatData, totalElementalResistanceStatData,
                    totalResistanceStatData,
                });
            this.pseudoStatsDataMappingServiceMock
                .GetPseudoStatData(Arg.Is<string>(s => s == StatId.FireAndLightningResistance))
                .Returns(new[]
                {
                    totalFireResistanceStatData, totalLightningResistanceStatData, totalElementalResistanceStatData,
                    totalResistanceStatData,
                });
            this.pseudoStatsDataMappingServiceMock
                .GetPseudoStatData(Arg.Is<string>(s => s == StatId.ColdAndLightningResistance))
                .Returns(new[]
                {
                    totalColdResistanceStatData, totalLightningResistanceStatData, totalElementalResistanceStatData,
                    totalResistanceStatData,
                });

            this.pseudoStatsDataMappingServiceMock.GetPseudoStatData(Arg.Is<string>(s => s == StatId.FireResistance))
                .Returns(new[] { totalFireResistanceStatData, totalElementalResistanceStatData, totalResistanceStatData });
            this.pseudoStatsDataMappingServiceMock.GetPseudoStatData(Arg.Is<string>(s => s == StatId.LightningResistance))
                .Returns(new[] { totalLightningResistanceStatData, totalElementalResistanceStatData, totalResistanceStatData });
            this.pseudoStatsDataMappingServiceMock.GetPseudoStatData(Arg.Is<string>(s => s == StatId.ColdResistance))
                .Returns(new[] { totalColdResistanceStatData, totalElementalResistanceStatData, totalResistanceStatData });
        }
    }
}