using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using POETradeHelper.ItemSearch.Contract.Configuration;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services.Factories;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;

namespace POETradeHelper.ItemSearch.Tests.Services.Factories
{
    public class StatFilterViewModelFactoryTests
    {
        private Mock<IOptionsMonitor<ItemSearchOptions>> itemSearchOptionsMock;
        private StatFilterViewModelFactory statFilterViewModelFactory;

        [SetUp]
        public void Setup()
        {
            this.itemSearchOptionsMock = new Mock<IOptionsMonitor<ItemSearchOptions>>();
            this.itemSearchOptionsMock.Setup(x => x.CurrentValue)
                .Returns(new ItemSearchOptions
                {
                    AdvancedQueryOptions = new AdvancedQueryOptions
                    {
                        MinValuePercentageOffset = 0,
                        MaxValuePercentageOffset = 0
                    }
                });

            this.statFilterViewModelFactory = new StatFilterViewModelFactory(this.itemSearchOptionsMock.Object);
        }

        [Test]
        public void CreateShouldReturnStatFilterViewModelWithId()
        {
            const string expected = "item stat id";
            var itemStat = GetItemStat(expected);

            StatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest());

            Assert.NotNull(result);
            Assert.That(result.Id, Is.EqualTo(expected));
        }

        [Test]
        public void CreateShouldReturnMinMaxStatFilterViewModelIfItemStatIsSingleValueItemStat()
        {
            var itemStat = new SingleValueItemStat(StatCategory.Explicit);

            StatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest());

            Assert.IsInstanceOf<MinMaxStatFilterViewModel>(result);
        }

        [Test]
        public void CreateShouldReturnMinMaxStatFilterViewModelIfItemStatIsMinMaxValueItemStat()
        {
            var itemStat = new MinMaxValueItemStat(StatCategory.Explicit);

            StatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest());

            Assert.IsInstanceOf<MinMaxStatFilterViewModel>(result);
        }

        public void CreateShouldNotReturnStatFilterViewModelIfItemStatIsItemStatWithoutValue()
        {
            var itemStat = new ItemStat(StatCategory.Explicit);

            StatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest());

            Assert.IsNotNull(result);
            Assert.IsNotInstanceOf<MinMaxStatFilterViewModel>(result);
        }

        [Test]
        public void CreateShouldReturnStatFilterViewModelWithText()
        {
            const string expected = "# to Maximum Life";
            var itemStat = new SingleValueItemStat(GetItemStat(textWithPlaceholders: expected));

            StatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest());

            Assert.NotNull(result);
            Assert.That(result.Text, Is.EqualTo(expected));
        }

        [SetCulture("")]
        [TestCase(10.00, "10")]
        [TestCase(0.34, "0.34")]
        [TestCase(0.30, "0.3")]
        public void CreateShouldReturnStatFilterViewModelWithCurrentValueForSingleValueItemStat(decimal value, string expected)
        {
            var itemStat = new SingleValueItemStat(StatCategory.Explicit)
            {
                Value = value
            };

            MinMaxStatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest()) as MinMaxStatFilterViewModel;

            Assert.NotNull(result);
            Assert.That(result.Current, Is.EqualTo(expected));
        }

        [SetCulture("")]
        [TestCase(10.00, 20.00, "10", "20")]
        [TestCase(0.23, 0.30, "0.23", "0.3")]
        public void CreateShouldReturnStatFilterViewModelWithCurrentValueForMinMaxValueItemStat(decimal minValue, decimal maxValue, string expectedMinValue, string expectedMaxValue)
        {
            var itemStat = new MinMaxValueItemStat(StatCategory.Explicit)
            {
                MinValue = minValue,
                MaxValue = maxValue
            };
            string expected = $"{expectedMinValue} - {expectedMaxValue}";

            MinMaxStatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest()) as MinMaxStatFilterViewModel;

            Assert.NotNull(result);
            Assert.That(result.Current, Is.EqualTo(expected));
        }
        
        [TestCaseSource(nameof(DecimalValueRoundingTestCases))]
        public void CreateShouldReturnStatFilterViewModelWithMinValueForSingleValueItemStat(decimal value, decimal expected)
        {
            var itemStat = new SingleValueItemStat(StatCategory.Explicit)
            {
                Value = value
            };

            MinMaxStatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest()) as MinMaxStatFilterViewModel;

            Assert.NotNull(result);
            Assert.That(result.Min, Is.EqualTo(expected));
        }

        [TestCaseSource(nameof(DecimalValueRoundingTestCases))]
        public void CreateShouldReturnStatFilterViewModelWithMaxValueForSingleValueItemStat(decimal value, decimal expected)
        {
            var itemStat = new SingleValueItemStat(StatCategory.Explicit)
            {
                Value = value
            };

            MinMaxStatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest()) as MinMaxStatFilterViewModel;

            Assert.NotNull(result);
            Assert.That(result.Max, Is.EqualTo(expected));
        }

        [TestCaseSource(nameof(DecimalValueRoundingTestCases))]
        public void CreateShouldReturnStatFilterViewModelWithMinValueForMinMaxValueItemStat(decimal value, decimal expected)
        {
            var itemStat = new MinMaxValueItemStat(StatCategory.Explicit)
            {
                MinValue = value
            };

            MinMaxStatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest()) as MinMaxStatFilterViewModel;

            Assert.NotNull(result);
            Assert.That(result.Min, Is.EqualTo(expected));
        }

        [TestCaseSource(nameof(DecimalValueRoundingTestCases))]
        public void CreateShouldReturnStatFilterViewModelWithMaxValueForMinMaxValueItemStat(decimal value, decimal expected)
        {
            var itemStat = new MinMaxValueItemStat(StatCategory.Explicit)
            {
                MaxValue = value
            };

            MinMaxStatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest()) as MinMaxStatFilterViewModel;

            Assert.NotNull(result);
            Assert.That(result.Max, Is.EqualTo(expected));
        }
        
        public static IEnumerable<object> DecimalValueRoundingTestCases
        {
            get
            {
                yield return new object[] { 15m, 15m };
                yield return new object[] { 0.123m, 0.12m };
                yield return new object[] { 0.125m, 0.13m };
                yield return new object[] { -0.125m, -0.13m };
            }
        }

        [TestCaseSource(nameof(DecimalValueOffsetTestCases))]
        public void CreateShouldConsiderMinValuePercentageOffsetForSingleValueItemStat(decimal offset, decimal value, decimal expected)
        {
            var itemStat = new SingleValueItemStat(StatCategory.Explicit)
            {
                Value = value
            };

            this.CreateShouldConsiderMinValuePercentageOffsetForItemStat(itemStat, offset, expected);
        }

        [TestCaseSource(nameof(DecimalValueOffsetTestCases))]
        public void CreateShouldConsiderMinValuePercentageOffsetForMinMaxValueItemStat(decimal offset, decimal minValue, decimal expected)
        {
            var itemStat = new MinMaxValueItemStat(StatCategory.Explicit)
            {
                MinValue = minValue
            };

            this.CreateShouldConsiderMinValuePercentageOffsetForItemStat(itemStat, offset, expected);
        }

        private void CreateShouldConsiderMinValuePercentageOffsetForItemStat(ItemStat itemStat, decimal percentageOffset, decimal expected)
        {
            this.itemSearchOptionsMock.Setup(x => x.CurrentValue)
                .Returns(new ItemSearchOptions
                {
                    AdvancedQueryOptions = new AdvancedQueryOptions
                    {
                        MinValuePercentageOffset = percentageOffset,
                        MaxValuePercentageOffset = 0
                    }
                });

            MinMaxStatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest()) as MinMaxStatFilterViewModel;

            Assert.NotNull(result);
            Assert.That(result.Min, Is.EqualTo(expected));
        }

        [TestCaseSource(nameof(DecimalValueOffsetTestCases))]
        public void CreateShouldConsiderMaxValuePercentageOffsetForSingleValueItemStat(decimal offset, decimal value, decimal expected)
        {
            var itemStat = new SingleValueItemStat(StatCategory.Explicit)
            {
                Value = value
            };

            this.CreateShouldConsiderMaxValuePercentageOffsetForItemStat(itemStat, offset, expected);
        }

        [TestCaseSource(nameof(DecimalValueOffsetTestCases))]
        public void CreateShouldConsiderMaxValuePercentageOffsetForMinMaxValueItemStat(decimal offset, decimal maxValue, decimal expected)
        {
            var itemStat = new MinMaxValueItemStat(StatCategory.Explicit)
            {
                MaxValue = maxValue
            };

            this.CreateShouldConsiderMaxValuePercentageOffsetForItemStat(itemStat, offset, expected);
        }

        private void CreateShouldConsiderMaxValuePercentageOffsetForItemStat(ItemStat itemStat, decimal percentageOffset, decimal expected)
        {
            this.itemSearchOptionsMock.Setup(x => x.CurrentValue)
                .Returns(new ItemSearchOptions
                {
                    AdvancedQueryOptions = new AdvancedQueryOptions
                    {
                        MinValuePercentageOffset = 0,
                        MaxValuePercentageOffset = percentageOffset
                    }
                });

            MinMaxStatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest()) as MinMaxStatFilterViewModel;

            Assert.NotNull(result);
            Assert.That(result.Max, Is.EqualTo(expected));
        }
        
        public static IEnumerable<object> DecimalValueOffsetTestCases
        {
            get
            {
                yield return new object[] { -0.1m, 10m, 9m };
                yield return new object[] { -0.15m, 10m, 8m };
                yield return new object[] { 0.1m, 10m, 11m };
                yield return new object[] { 0.1m, 1.235m, 1.36m };
                yield return new object[] { -0.1m, -1.34m, -1.21m };
            }
        }

        [Test]
        public void CreateShouldUseTextInsteadOfTextWithPlaceholdersForItemStatWithoutValue()
        {
            const string textWithPlaceholders = "#% chance to Trigger Edict of Frost on Kill";
            const string statText = "Trigger Edict of Frost on Kill";

            var itemStat = new ItemStat(StatCategory.Enchant)
            {
                Text = statText,
                TextWithPlaceholders = textWithPlaceholders
            };

            StatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest());

            Assert.NotNull(result);
            Assert.That(result.Text, Is.EqualTo(statText));
        }

        [TestCase(null)]
        [TestCase(15)]
        [TestCase(20)]
        [TestCase(10.111)]
        public void CreateShouldTakeMinValueFromQueryRequestForSingleValueItemStat(decimal? expected)
        {
            var itemStat = new SingleValueItemStat(StatCategory.Explicit)
            {
                Value = 10
            };

            this.CreateShouldTakeMinValueFromQueryRequest(itemStat, expected);
        }

        [TestCase(null)]
        [TestCase(15)]
        [TestCase(20)]
        [TestCase(10.111)]
        public void CreateShouldTakeMinValueFromQueryRequestForMinMaxValueItemStat(decimal? expected)
        {
            var itemStat = new MinMaxValueItemStat(StatCategory.Explicit)
            {
                MinValue = 10
            };

            this.CreateShouldTakeMinValueFromQueryRequest(itemStat, expected);
        }

        private void CreateShouldTakeMinValueFromQueryRequest(ItemStat itemStat, decimal? expected)
        {
            this.itemSearchOptionsMock.Setup(x => x.CurrentValue)
                .Returns(new ItemSearchOptions
                {
                    AdvancedQueryOptions = new AdvancedQueryOptions
                    {
                        MinValuePercentageOffset = -0.1m
                    }
                });

            itemStat.Id = "item stat id";
            SearchQueryRequest queryRequest = GetQueryRequestWithStatFilter(itemStat.Id, new MinMaxFilter { Min = expected });
            MinMaxStatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, queryRequest) as MinMaxStatFilterViewModel;

            Assert.That(result.Min, Is.EqualTo(expected));
        }

        [TestCase(null)]
        [TestCase(30)]
        [TestCase(40)]
        [TestCase(20.222)]
        public void CreateShouldTakeMaxValueFromQueryRequestForSingleValueItemStat(decimal? expected)
        {
            var itemStat = new SingleValueItemStat(StatCategory.Explicit)
            {
                Value = 10
            };

            this.CreateShouldTakeMaxValueFromQueryRequest(itemStat, expected);
        }

        [TestCase(null)]
        [TestCase(30)]
        [TestCase(40)]
        [TestCase(20.222)]
        public void CreateShouldTakeMaxValueFromQueryRequestForMinMaxValueItemStat(decimal? expected)
        {
            var itemStat = new MinMaxValueItemStat(StatCategory.Explicit)
            {
                MaxValue = 20
            };

            this.CreateShouldTakeMaxValueFromQueryRequest(itemStat, expected);
        }

        private void CreateShouldTakeMaxValueFromQueryRequest(ItemStat itemStat, decimal? expected)
        {
            this.itemSearchOptionsMock.Setup(x => x.CurrentValue)
                .Returns(new ItemSearchOptions
                {
                    AdvancedQueryOptions = new AdvancedQueryOptions
                    {
                        MaxValuePercentageOffset = 0.1m
                    }
                });

            itemStat.Id = "item stat id";
            SearchQueryRequest queryRequest = GetQueryRequestWithStatFilter(itemStat.Id, new MinMaxFilter { Max = expected });

            MinMaxStatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, queryRequest) as MinMaxStatFilterViewModel;

            Assert.That(result.Max, Is.EqualTo(expected));
        }

        [TestCaseSource(nameof(ItemStatsWithIds))]
        public void CreateShouldSetIsEnabledTrueIfQueryRequestContainsFilter(ItemStat itemStat)
        {
            SearchQueryRequest queryRequest = GetQueryRequestWithStatFilter(itemStat.Id, new MinMaxFilter());

            StatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, queryRequest);

            Assert.That(result.IsEnabled, Is.True);
        }

        [TestCaseSource(nameof(ItemStatsWithIds))]
        public void CreateShouldNotSetIsEnabledTrueIfQueryRequestDoesNotContainFilter(ItemStat itemStat)
        {
            SearchQueryRequest queryRequest = new SearchQueryRequest();

            StatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, queryRequest);

            Assert.That(result.IsEnabled, Is.False);
        }

        [Test]
        public void CreateShouldMapValueOfSingleValueItemStatToMinValue()
        {
            const int expected = 2;
            var itemStat = new SingleValueItemStat(StatCategory.Monster) { Value = expected };

            MinMaxStatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest()) as MinMaxStatFilterViewModel;

            Assert.That(result.Min, Is.EqualTo(expected));
        }

        private static IEnumerable<ItemStat> ItemStatsWithIds
        {
            get
            {
                yield return new ItemStat(StatCategory.Explicit) { Id = "item stat id" };
                yield return new SingleValueItemStat(StatCategory.Explicit) { Id = "single value item stat id" };
                yield return new MinMaxValueItemStat(StatCategory.Enchant) { Id = "min max value item stat id" };
            }
        }

        [Test]
        public void CreateShouldIgnoreConfigurationForMonsterItemStat()
        {
            const int expected = 3;
            var itemStat = new SingleValueItemStat(StatCategory.Monster) { Id = "monsterItemStat", Value = expected };

            this.itemSearchOptionsMock.Setup(x => x.CurrentValue)
                .Returns(new ItemSearchOptions
                {
                    AdvancedQueryOptions = new AdvancedQueryOptions
                    {
                        MinValuePercentageOffset = -0.1m,
                        MaxValuePercentageOffset = 0.1m
                    }
                });

            MinMaxStatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest()) as MinMaxStatFilterViewModel;

            Assert.That(result.Min, Is.EqualTo(expected));
            Assert.That(result.Max, Is.EqualTo(expected));
        }

        private static SearchQueryRequest GetQueryRequestWithStatFilter(string statId, MinMaxFilter value)
        {
            return new SearchQueryRequest
            {
                Query =
                {
                    Stats =
                    {
                        new StatFilters
                        {
                            Filters =
                            {
                                new StatFilter
                                {
                                    Id = statId,
                                    Value = value
                                }
                            }
                        }
                    }
                }
            };
        }

        private static ItemStat GetItemStat(string id = "", string textWithPlaceholders = "", string statText = "")
        {
            return new ItemStat(StatCategory.Unknown) { Id = id, TextWithPlaceholders = textWithPlaceholders, Text = statText };
        }
    }
}