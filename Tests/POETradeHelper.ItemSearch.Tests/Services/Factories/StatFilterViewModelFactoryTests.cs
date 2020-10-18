using System.Collections.Generic;
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

        [Test]
        public void CreateShouldReturnStatFilterViewModelWithCurrentValueForSingleValueItemStat()
        {
            var itemStat = new SingleValueItemStat(StatCategory.Explicit)
            {
                Value = 10
            };
            string expected = itemStat.Value.ToString();

            MinMaxStatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest()) as MinMaxStatFilterViewModel;

            Assert.NotNull(result);
            Assert.That(result.Current, Is.EqualTo(expected));
        }

        [Test]
        public void CreateShouldReturnStatFilterViewModelWithCurrentValueForMinMaxValueItemStat()
        {
            var itemStat = new MinMaxValueItemStat(StatCategory.Explicit)
            {
                MinValue = 10,
                MaxValue = 20
            };
            string expected = $"{itemStat.MinValue} - {itemStat.MaxValue}";

            MinMaxStatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest()) as MinMaxStatFilterViewModel;

            Assert.NotNull(result);
            Assert.That(result.Current, Is.EqualTo(expected));
        }

        [Test]
        public void CreateShouldReturnStatFilterViewModelWithMinValueForSingleValueItemStat()
        {
            const int expected = 15;
            var itemStat = new SingleValueItemStat(StatCategory.Explicit)
            {
                Value = expected
            };

            MinMaxStatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest()) as MinMaxStatFilterViewModel;

            Assert.NotNull(result);
            Assert.That(result.Min, Is.EqualTo(expected));
        }

        [Test]
        public void CreateShouldReturnStatFilterViewModelWithMaxValueForSingleValueItemStat()
        {
            const int expected = 20;
            var itemStat = new SingleValueItemStat(StatCategory.Explicit)
            {
                Value = expected
            };

            MinMaxStatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest()) as MinMaxStatFilterViewModel;

            Assert.NotNull(result);
            Assert.That(result.Max, Is.EqualTo(expected));
        }

        [Test]
        public void CreateShouldReturnStatFilterViewModelWithMinValueForMinMaxValueItemStat()
        {
            const int expected = 15;
            var itemStat = new MinMaxValueItemStat(StatCategory.Explicit)
            {
                MinValue = expected
            };

            MinMaxStatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest()) as MinMaxStatFilterViewModel;

            Assert.NotNull(result);
            Assert.That(result.Min, Is.EqualTo(expected));
        }

        [Test]
        public void CreateShouldReturnStatFilterViewModelWithMaxValueForMinMaxValueItemStat()
        {
            const int expected = 20;
            var itemStat = new MinMaxValueItemStat(StatCategory.Explicit)
            {
                MaxValue = expected
            };

            MinMaxStatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest()) as MinMaxStatFilterViewModel;

            Assert.NotNull(result);
            Assert.That(result.Max, Is.EqualTo(expected));
        }

        [TestCase(-0.1, 10, 9)]
        [TestCase(-0.15, 10, 8)]
        [TestCase(0.1, 10, 11)]
        public void CreateShouldConsiderMinValuePercentageOffsetForSingleValueItemStat(double offset, int value, int expected)
        {
            var itemStat = new SingleValueItemStat(StatCategory.Explicit)
            {
                Value = value
            };

            this.CreateShouldConsiderMinValuePercentageOffsetForItemStat(itemStat, offset, expected);
        }

        [TestCase(-0.1, 10, 9)]
        [TestCase(-0.15, 10, 8)]
        [TestCase(0.1, 10, 11)]
        public void CreateShouldConsiderMinValuePercentageOffsetForMinMaxValueItemStat(double offset, int minValue, int expected)
        {
            var itemStat = new MinMaxValueItemStat(StatCategory.Explicit)
            {
                MinValue = minValue
            };

            this.CreateShouldConsiderMinValuePercentageOffsetForItemStat(itemStat, offset, expected);
        }

        private void CreateShouldConsiderMinValuePercentageOffsetForItemStat(ItemStat itemStat, double percentageOffset, int expected)
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

        [TestCase(0.1, 20, 22)]
        [TestCase(0.15, 20, 23)]
        [TestCase(-0.1, 20, 18)]
        public void CreateShouldConsiderMaxValuePercentageOffsetForSingleValueItemStat(double offset, int value, int expected)
        {
            var itemStat = new SingleValueItemStat(StatCategory.Explicit)
            {
                Value = value
            };

            this.CreateShouldConsiderMaxValuePercentageOffsetForItemStat(itemStat, offset, expected);
        }

        [TestCase(0.1, 20, 22)]
        [TestCase(0.15, 20, 23)]
        [TestCase(-0.1, 20, 18)]
        public void CreateShouldConsiderMaxValuePercentageOffsetForMinMaxValueItemStat(double offset, int maxValue, int expected)
        {
            var itemStat = new MinMaxValueItemStat(StatCategory.Explicit)
            {
                MaxValue = maxValue
            };

            this.CreateShouldConsiderMaxValuePercentageOffsetForItemStat(itemStat, offset, expected);
        }

        private void CreateShouldConsiderMaxValuePercentageOffsetForItemStat(ItemStat itemStat, double percentageOffset, int expected)
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
        public void CreateShouldTakeMinValueFromQueryRequestForSingleValueItemStat(int? expected)
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
        public void CreateShouldTakeMinValueFromQueryRequestForMinMaxValueItemStat(int? expected)
        {
            var itemStat = new MinMaxValueItemStat(StatCategory.Explicit)
            {
                MinValue = 10
            };

            this.CreateShouldTakeMinValueFromQueryRequest(itemStat, expected);
        }

        private void CreateShouldTakeMinValueFromQueryRequest(ItemStat itemStat, int? expected)
        {
            this.itemSearchOptionsMock.Setup(x => x.CurrentValue)
                .Returns(new ItemSearchOptions
                {
                    AdvancedQueryOptions = new AdvancedQueryOptions
                    {
                        MinValuePercentageOffset = -0.1
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
        public void CreateShouldTakeMaxValueFromQueryRequestForSingleValueItemStat(int? expected)
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
        public void CreateShouldTakeMaxValueFromQueryRequestForMinMaxValueItemStat(int? expected)
        {
            var itemStat = new MinMaxValueItemStat(StatCategory.Explicit)
            {
                MaxValue = 20
            };

            this.CreateShouldTakeMaxValueFromQueryRequest(itemStat, expected);
        }

        private void CreateShouldTakeMaxValueFromQueryRequest(ItemStat itemStat, int? expected)
        {
            this.itemSearchOptionsMock.Setup(x => x.CurrentValue)
                .Returns(new ItemSearchOptions
                {
                    AdvancedQueryOptions = new AdvancedQueryOptions
                    {
                        MaxValuePercentageOffset = 0.1
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
                        MinValuePercentageOffset = -0.1,
                        MaxValuePercentageOffset = 0.1
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