using System.Collections.Generic;

using FluentAssertions;

using Microsoft.Extensions.Options;

using NSubstitute;

using NUnit.Framework;

using POETradeHelper.ItemSearch.Contract.Configuration;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.UI.Avalonia.Factories.Implementations;
using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;
using POETradeHelper.PathOfExileTradeApi.Properties;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Tests.Services.Factories
{
    public class StatFilterViewModelFactoryTests
    {
        private readonly IOptionsMonitor<ItemSearchOptions> itemSearchOptionsMock;
        private readonly StatFilterViewModelFactory statFilterViewModelFactory;

        public StatFilterViewModelFactoryTests()
        {
            this.itemSearchOptionsMock = Substitute.For<IOptionsMonitor<ItemSearchOptions>>();
            this.itemSearchOptionsMock.CurrentValue
                .Returns(
                    new ItemSearchOptions
                    {
                        AdvancedQueryOptions = new AdvancedQueryOptions
                        {
                            MinValuePercentageOffset = 0,
                            MaxValuePercentageOffset = 0,
                        },
                    });

            this.statFilterViewModelFactory = new StatFilterViewModelFactory(this.itemSearchOptionsMock);
        }

        [Test]
        public void CreateShouldReturnStatFilterViewModelWithId()
        {
            const string expected = "item stat id";
            ItemStat itemStat = GetItemStat(expected);

            StatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest());

            result.Id.Should().Be(expected);
        }

        [Test]
        public void CreateShouldReturnMinMaxStatFilterViewModelIfItemStatIsSingleValueItemStat()
        {
            SingleValueItemStat itemStat = new(StatCategory.Explicit);

            StatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest());

            result.Should().BeOfType<MinMaxStatFilterViewModel>();
        }

        [Test]
        public void CreateShouldReturnMinMaxStatFilterViewModelIfItemStatIsMinMaxValueItemStat()
        {
            MinMaxValueItemStat itemStat = new(StatCategory.Explicit);

            StatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest());

            result.Should().BeOfType<MinMaxStatFilterViewModel>();
        }

        [Test]
        public void CreateShouldNotReturnStatFilterViewModelIfItemStatIsItemStatWithoutValue()
        {
            ItemStat itemStat = new(StatCategory.Explicit);

            StatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest());

            result.Should()
                .NotBeNull()
                .And.NotBeOfType<MinMaxStatFilterViewModel>();
        }

        [Test]
        public void CreateShouldReturnStatFilterViewModelWithText()
        {
            const string expected = "# to Maximum Life";
            SingleValueItemStat itemStat = new(GetItemStat(textWithPlaceholders: expected));

            StatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest());

            result.Text.Should().Be(expected);
        }

        [SetCulture("")]
        [TestCase(10.00, "10")]
        [TestCase(0.34, "0.34")]
        [TestCase(0.30, "0.3")]
        public void CreateShouldReturnStatFilterViewModelWithCurrentValueForSingleValueItemStat(decimal value, string expected)
        {
            SingleValueItemStat itemStat = new(StatCategory.Explicit)
            {
                Value = value,
            };

            MinMaxStatFilterViewModel result = (MinMaxStatFilterViewModel)this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest());

            result.Current.Should().Be(expected);
        }

        [SetCulture("")]
        [TestCase(10.00, 20.00, "10", "20")]
        [TestCase(0.23, 0.30, "0.23", "0.3")]
        public void CreateShouldReturnStatFilterViewModelWithCurrentValueForMinMaxValueItemStat(
            decimal minValue,
            decimal maxValue,
            string expectedMinValue,
            string expectedMaxValue)
        {
            MinMaxValueItemStat itemStat = new(StatCategory.Explicit)
            {
                MinValue = minValue,
                MaxValue = maxValue,
            };
            string expected = $"{expectedMinValue} - {expectedMaxValue}";

            MinMaxStatFilterViewModel result = (MinMaxStatFilterViewModel)this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest());

            result.Current.Should().Be(expected);
        }

        [TestCaseSource(nameof(GetDecimalValueRoundingTestCases))]
        public void CreateShouldReturnStatFilterViewModelWithMinValueForSingleValueItemStat(decimal value, decimal expected)
        {
            SingleValueItemStat itemStat = new(StatCategory.Explicit)
            {
                Value = value,
            };

            MinMaxStatFilterViewModel result = (MinMaxStatFilterViewModel)this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest());

            result.Min.Should().Be(expected);
        }

        [TestCaseSource(nameof(GetDecimalValueRoundingTestCases))]
        public void CreateShouldReturnStatFilterViewModelWithMaxValueForSingleValueItemStat(decimal value, decimal expected)
        {
            SingleValueItemStat itemStat = new(StatCategory.Explicit)
            {
                Value = value,
            };

            MinMaxStatFilterViewModel result = (MinMaxStatFilterViewModel)this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest());

            result.Max.Should().Be(expected);
        }

        [TestCaseSource(nameof(GetDecimalValueRoundingTestCases))]
        public void CreateShouldReturnStatFilterViewModelWithMinValueForMinMaxValueItemStat(decimal value, decimal expected)
        {
            MinMaxValueItemStat itemStat = new(StatCategory.Explicit)
            {
                MinValue = value,
            };

            MinMaxStatFilterViewModel result = (MinMaxStatFilterViewModel)this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest());

            result.Min.Should().Be(expected);
        }

        [TestCaseSource(nameof(GetDecimalValueRoundingTestCases))]
        public void CreateShouldReturnStatFilterViewModelWithMaxValueForMinMaxValueItemStat(decimal value, decimal expected)
        {
            MinMaxValueItemStat itemStat = new(StatCategory.Explicit)
            {
                MaxValue = value,
            };

            MinMaxStatFilterViewModel result = (MinMaxStatFilterViewModel)this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest());

            result.Max.Should().Be(expected);
        }

        [TestCaseSource(nameof(GetDecimalValueOffsetTestCases))]
        public void CreateShouldConsiderMinValuePercentageOffsetForSingleValueItemStat(decimal offset, decimal value, decimal expected)
        {
            SingleValueItemStat itemStat = new(StatCategory.Explicit)
            {
                Value = value,
            };

            this.CreateShouldConsiderMinValuePercentageOffsetForItemStat(itemStat, offset, expected);
        }

        [TestCaseSource(nameof(GetDecimalValueOffsetTestCases))]
        public void CreateShouldConsiderMinValuePercentageOffsetForMinMaxValueItemStat(decimal offset, decimal minValue, decimal expected)
        {
            MinMaxValueItemStat itemStat = new(StatCategory.Explicit)
            {
                MinValue = minValue,
            };

            this.CreateShouldConsiderMinValuePercentageOffsetForItemStat(itemStat, offset, expected);
        }

        [TestCaseSource(nameof(GetDecimalValueOffsetTestCases))]
        public void CreateShouldConsiderMaxValuePercentageOffsetForSingleValueItemStat(decimal offset, decimal value, decimal expected)
        {
            SingleValueItemStat itemStat = new(StatCategory.Explicit)
            {
                Value = value,
            };

            this.CreateShouldConsiderMaxValuePercentageOffsetForItemStat(itemStat, offset, expected);
        }

        [TestCaseSource(nameof(GetDecimalValueOffsetTestCases))]
        public void CreateShouldConsiderMaxValuePercentageOffsetForMinMaxValueItemStat(decimal offset, decimal maxValue, decimal expected)
        {
            MinMaxValueItemStat itemStat = new(StatCategory.Explicit)
            {
                MaxValue = maxValue,
            };

            this.CreateShouldConsiderMaxValuePercentageOffsetForItemStat(itemStat, offset, expected);
        }

        [Test]
        public void CreateShouldUseTextInsteadOfTextWithPlaceholdersForItemStatWithoutValue()
        {
            const string textWithPlaceholders = "#% chance to Trigger Edict of Frost on Kill";
            const string statText = "Trigger Edict of Frost on Kill";

            ItemStat itemStat = new(StatCategory.Enchant)
            {
                Text = statText,
                TextWithPlaceholders = textWithPlaceholders,
            };

            StatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest());

            result.Text.Should().Be(statText);
        }

        [TestCase(null)]
        [TestCase(15)]
        [TestCase(20)]
        [TestCase(10.111)]
        public void CreateShouldTakeMinValueFromQueryRequestForSingleValueItemStat(decimal? expected)
        {
            SingleValueItemStat itemStat = new(StatCategory.Explicit)
            {
                Value = 10,
            };

            this.CreateShouldTakeMinValueFromQueryRequest(itemStat, expected);
        }

        [TestCase(null)]
        [TestCase(15)]
        [TestCase(20)]
        [TestCase(10.111)]
        public void CreateShouldTakeMinValueFromQueryRequestForMinMaxValueItemStat(decimal? expected)
        {
            MinMaxValueItemStat itemStat = new(StatCategory.Explicit)
            {
                MinValue = 10,
            };

            this.CreateShouldTakeMinValueFromQueryRequest(itemStat, expected);
        }

        [TestCase(null)]
        [TestCase(30)]
        [TestCase(40)]
        [TestCase(20.222)]
        public void CreateShouldTakeMaxValueFromQueryRequestForSingleValueItemStat(decimal? expected)
        {
            SingleValueItemStat itemStat = new(StatCategory.Explicit)
            {
                Value = 10,
            };

            this.CreateShouldTakeMaxValueFromQueryRequest(itemStat, expected);
        }

        [TestCase(null)]
        [TestCase(30)]
        [TestCase(40)]
        [TestCase(20.222)]
        public void CreateShouldTakeMaxValueFromQueryRequestForMinMaxValueItemStat(decimal? expected)
        {
            MinMaxValueItemStat itemStat = new(StatCategory.Explicit)
            {
                MaxValue = 20,
            };

            this.CreateShouldTakeMaxValueFromQueryRequest(itemStat, expected);
        }

        [TestCaseSource(nameof(GetItemStatsWithIds))]
        public void CreateShouldSetIsEnabledTrueIfQueryRequestContainsFilter(ItemStat itemStat)
        {
            SearchQueryRequest queryRequest = GetQueryRequestWithStatFilter(itemStat.Id, new MinMaxFilter());

            StatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, queryRequest);

            result.IsEnabled.Should().BeTrue();
        }

        [TestCaseSource(nameof(GetItemStatsWithIds))]
        public void CreateShouldNotSetIsEnabledTrueIfQueryRequestDoesNotContainFilter(ItemStat itemStat)
        {
            SearchQueryRequest queryRequest = new();

            StatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, queryRequest);

            result.IsEnabled.Should().BeFalse();
        }

        [Test]
        public void CreateShouldMapValueOfSingleValueItemStatToMinValue()
        {
            const int expected = 2;
            SingleValueItemStat itemStat = new(StatCategory.Monster) { Value = expected };

            MinMaxStatFilterViewModel result = (MinMaxStatFilterViewModel)this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest());

            result.Min.Should().Be(expected);
        }

        [Test]
        public void CreateShouldIgnoreConfigurationForMonsterItemStat()
        {
            const int expected = 3;
            SingleValueItemStat itemStat = new(StatCategory.Monster) { Value = expected };

            this.itemSearchOptionsMock.CurrentValue
                .Returns(
                    new ItemSearchOptions
                    {
                        AdvancedQueryOptions = new AdvancedQueryOptions
                        {
                            MinValuePercentageOffset = -0.1m,
                            MaxValuePercentageOffset = 0.1m,
                        },
                    });

            MinMaxStatFilterViewModel result = (MinMaxStatFilterViewModel)this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest());

            result.Min.Should().Be(expected);
            result.Max.Should().Be(expected);
        }

        private void CreateShouldConsiderMinValuePercentageOffsetForItemStat(ItemStat itemStat, decimal percentageOffset, decimal expected)
        {
            this.itemSearchOptionsMock.CurrentValue
                .Returns(
                    new ItemSearchOptions
                    {
                        AdvancedQueryOptions = new AdvancedQueryOptions
                        {
                            MinValuePercentageOffset = percentageOffset,
                            MaxValuePercentageOffset = 0,
                        },
                    });

            MinMaxStatFilterViewModel result = (MinMaxStatFilterViewModel)this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest());

            result.Min.Should().Be(expected);
        }

        private void CreateShouldTakeMinValueFromQueryRequest(ItemStat itemStat, decimal? expected)
        {
            this.itemSearchOptionsMock.CurrentValue
                .Returns(
                    new ItemSearchOptions
                    {
                        AdvancedQueryOptions = new AdvancedQueryOptions
                        {
                            MinValuePercentageOffset = -0.1m,
                        },
                    });

            itemStat.Id = "item stat id";
            SearchQueryRequest queryRequest = GetQueryRequestWithStatFilter(itemStat.Id, new MinMaxFilter { Min = expected });

            MinMaxStatFilterViewModel result = (MinMaxStatFilterViewModel)this.statFilterViewModelFactory.Create(itemStat, queryRequest);

            result.Min.Should().Be(expected);
        }

        private void CreateShouldConsiderMaxValuePercentageOffsetForItemStat(ItemStat itemStat, decimal percentageOffset, decimal expected)
        {
            this.itemSearchOptionsMock.CurrentValue
                .Returns(
                    new ItemSearchOptions
                    {
                        AdvancedQueryOptions = new AdvancedQueryOptions
                        {
                            MinValuePercentageOffset = 0,
                            MaxValuePercentageOffset = percentageOffset,
                        },
                    });

            MinMaxStatFilterViewModel result = (MinMaxStatFilterViewModel)this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest());

            result.Max.Should().Be(expected);
        }

        private void CreateShouldTakeMaxValueFromQueryRequest(ItemStat itemStat, decimal? expected)
        {
            this.itemSearchOptionsMock.CurrentValue
                .Returns(
                    new ItemSearchOptions
                    {
                        AdvancedQueryOptions = new AdvancedQueryOptions
                        {
                            MaxValuePercentageOffset = 0.1m,
                        },
                    });

            itemStat.Id = "item stat id";
            SearchQueryRequest queryRequest = GetQueryRequestWithStatFilter(itemStat.Id, new MinMaxFilter { Max = expected });

            MinMaxStatFilterViewModel result = (MinMaxStatFilterViewModel)this.statFilterViewModelFactory.Create(itemStat, queryRequest);

            result.Max.Should().Be(expected);
        }

        private static IEnumerable<object> GetDecimalValueRoundingTestCases()
        {
            yield return new object[] { 15m, 15m };
            yield return new object[] { 0.123m, 0.12m };
            yield return new object[] { 0.125m, 0.13m };
            yield return new object[] { -0.125m, -0.13m };
        }

        private static IEnumerable<object> GetDecimalValueOffsetTestCases()
        {
            yield return new object[] { -0.1m, 10m, 9m };
            yield return new object[] { -0.15m, 10m, 8m };
            yield return new object[] { 0.1m, 10m, 11m };
            yield return new object[] { 0.1m, 1.235m, 1.36m };
            yield return new object[] { -0.1m, -1.34m, -1.21m };
        }

        private static IEnumerable<ItemStat> GetItemStatsWithIds()
        {
            yield return new ItemStat(StatCategory.Explicit) { Id = "item stat id" };
            yield return new SingleValueItemStat(StatCategory.Explicit) { Id = "single value item stat id" };
            yield return new MinMaxValueItemStat(StatCategory.Enchant) { Id = "min max value item stat id" };
        }

        private static SearchQueryRequest GetQueryRequestWithStatFilter(string statId, MinMaxFilter value) =>
            new()
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
                                    Value = value,
                                },
                            },
                        },
                    },
                },
            };

        private static ItemStat GetItemStat(string id = "", string textWithPlaceholders = "", string statText = "") => new(StatCategory.Unknown)
            { Id = id, TextWithPlaceholders = textWithPlaceholders, Text = statText };
    }
}