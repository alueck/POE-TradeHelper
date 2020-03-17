using NUnit.Framework;
using NUnit.Framework.Internal;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services.Factories;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;

namespace POETradeHelper.ItemSearch.Tests.Services.Factories
{
    public class StatFilterViewModelFactoryTests
    {
        private StatFilterViewModelFactory statFilterViewModelFactory;

        [SetUp]
        public void Setup()
        {
            this.statFilterViewModelFactory = new StatFilterViewModelFactory();
        }

        [Test]
        public void CreateShouldReturnStatFilterViewModelWithId()
        {
            const string expected = "item stat id";
            var itemStat = GetItemStat(expected);

            StatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest(), new StatFilterViewModelFactoryConfiguration());

            Assert.NotNull(result);
            Assert.That(result.Id, Is.EqualTo(expected));
        }

        [Test]
        public void CreateShouldReturnMinMaxStatFilterViewModelIfStatTextContainsValueForPlaceholder()
        {
            const string textWithPlaceholders = "# to Maximum Life";
            const string statText = "+79 to Maximum Life";
            var itemStat = GetItemStat(textWithPlaceholders: textWithPlaceholders, statText: statText);

            StatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest(), new StatFilterViewModelFactoryConfiguration());

            Assert.IsInstanceOf<MinMaxStatFilterViewModel>(result);
        }

        [TestCase("#% chance to Trigger Edict of Frost on Kill", "Trigger Edict of Frost on Kill")]
        [TestCase("Extra gore", "Extra gore")]
        public void CreateShouldNotReturnMinMaxFilterViewModelIfStatTextDoesNotContainValueForPlaceholder(string textWithPlaceholders, string statText)
        {
            var itemStat = GetItemStat(textWithPlaceholders: textWithPlaceholders, statText: statText);

            StatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest(), new StatFilterViewModelFactoryConfiguration());

            Assert.IsNotInstanceOf<MinMaxStatFilterViewModel>(result);
        }

        [Test]
        public void CreateShouldReturnStatFilterViewModelWithText()
        {
            const string expected = "# to Maximum Life";
            const string statText = "+79 to Maximum Life";
            var itemStat = GetItemStat(textWithPlaceholders: expected, statText: statText);

            StatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest(), new StatFilterViewModelFactoryConfiguration());

            Assert.NotNull(result);
            Assert.That(result.Text, Is.EqualTo(expected));
        }

        [TestCase("# to Maximum Life", "+79 to Maximum Life", "79")]
        [TestCase("# to Maximum Mana", "+20 to Maximum Mana", "20")]
        [TestCase("Adds # to # Chaos Damage", "Adds 10 to 20 Chaos Damage", "10 - 20")]
        public void CreateShouldReturnStatFilterViewModelWithCurrentValue(string textWithPlaceholders, string statText, string expected)
        {
            var itemStat = GetItemStat(textWithPlaceholders: textWithPlaceholders, statText: statText);

            MinMaxStatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest(), new StatFilterViewModelFactoryConfiguration()) as MinMaxStatFilterViewModel;

            Assert.NotNull(result);
            Assert.That(result.Current, Is.EqualTo(expected));
        }

        [TestCase("# to Maximum Life", "+79 to Maximum Life", 79)]
        [TestCase("# to Maximum Life", "-20 to Maximum Life", -20)]
        [TestCase("# to Maximum Mana", "+20 to Maximum Mana", 20)]
        [TestCase("#% chance for Poisons inflicted with this Weapon to deal 100% more Damage", "60% chance for Poisons inflicted with this Weapon to deal 100% more Damage", 60)]
        public void CreateShouldReturnStatFilterViewModelWithMinValue(string textWithPlaceholders, string statText, int? expected)
        {
            var itemStat = GetItemStat(textWithPlaceholders: textWithPlaceholders, statText: statText);

            MinMaxStatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest(), new StatFilterViewModelFactoryConfiguration()) as MinMaxStatFilterViewModel;

            Assert.NotNull(result);
            Assert.That(result.Min, Is.EqualTo(expected));
        }

        [TestCase("Adds # to # Chaos Damage", "Adds 10 to 20 Chaos Damage", 20)]
        [TestCase("# to Maximum Life", "+79 to Maximum Life", 79)]
        public void CreateShouldReturnStatFilterViewModelWithMaxValue(string textWithPlaceholders, string statText, int? expected)
        {
            var itemStat = GetItemStat(textWithPlaceholders: textWithPlaceholders, statText: statText);

            MinMaxStatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest(), new StatFilterViewModelFactoryConfiguration()) as MinMaxStatFilterViewModel;

            Assert.NotNull(result);
            Assert.That(result.Max, Is.EqualTo(expected));
        }

        [TestCase(-0.1, 10, 9)]
        [TestCase(-0.15, 10, 8)]
        [TestCase(0.1, 10, 11)]
        public void CreateShouldConsiderMinValuePercentageOffset(double offset, int minValue, int expected)
        {
            const string textWithPlaceholders = "Adds # to # Chaos Damage";
            string statText = $"Adds 10 to {minValue} Chaos Damage";
            var configuration = new StatFilterViewModelFactoryConfiguration
            {
                MinValuePercentageOffset = offset
            };

            var itemStat = GetItemStat(textWithPlaceholders: textWithPlaceholders, statText: statText);

            MinMaxStatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest(), configuration) as MinMaxStatFilterViewModel;

            Assert.NotNull(result);
            Assert.That(result.Min, Is.EqualTo(expected));
        }

        [TestCase(0.1, 20, 22)]
        [TestCase(0.15, 20, 23)]
        [TestCase(-0.1, 20, 18)]
        public void CreateShouldConsiderMaxValuePercentageOffset(double offset, int maxValue, int expected)
        {
            const string textWithPlaceholders = "Adds # to # Chaos Damage";
            string statText = $"Adds 10 to {maxValue} Chaos Damage";
            var configuration = new StatFilterViewModelFactoryConfiguration
            {
                MaxValuePercentageOffset = offset
            };

            var itemStat = GetItemStat(textWithPlaceholders: textWithPlaceholders, statText: statText);

            MinMaxStatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest(), configuration) as MinMaxStatFilterViewModel;

            Assert.NotNull(result);
            Assert.That(result.Max, Is.EqualTo(expected));
        }

        [Test]
        public void CreateShouldUseTextInsteadOfTextWithPlaceholders()
        {
            const string textWithPlaceholders = "#% chance to Trigger Edict of Frost on Kill";
            const string statText = "Trigger Edict of Frost on Kill";

            var itemStat = GetItemStat(textWithPlaceholders: textWithPlaceholders, statText: statText);

            StatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest(), new StatFilterViewModelFactoryConfiguration());

            Assert.NotNull(result);
            Assert.That(result.Text, Is.EqualTo(statText));
        }

        [TestCase(null)]
        [TestCase(15)]
        [TestCase(20)]
        public void CreateShouldTakeMinValueFromQueryRequest(int expected)
        {
            const string id = "item stat id";
            const string textWithPlaceholders = "Adds # to # Chaos Damage";
            string statText = "Adds 10 to 25 Chaos Damage";

            var itemStat = GetItemStat(id, textWithPlaceholders, statText);
            SearchQueryRequest queryRequest = GetQueryRequestWithStatFilter(id, new MinMaxFilter { Min = expected });
            MinMaxStatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, queryRequest, new StatFilterViewModelFactoryConfiguration()) as MinMaxStatFilterViewModel;

            Assert.That(result.Min, Is.EqualTo(expected));
        }

        [TestCase(null)]
        [TestCase(30)]
        [TestCase(40)]
        public void CreateShouldTakeMaxValueFromQueryRequest(int expected)
        {
            const string id = "statId";
            const string textWithPlaceholders = "Adds # to # Chaos Damage";
            string statText = "Adds 10 to 25 Chaos Damage";

            var itemStat = GetItemStat(id, textWithPlaceholders, statText);

            SearchQueryRequest queryRequest = GetQueryRequestWithStatFilter(id, new MinMaxFilter { Max = expected });

            MinMaxStatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, queryRequest, new StatFilterViewModelFactoryConfiguration()) as MinMaxStatFilterViewModel;

            Assert.That(result.Max, Is.EqualTo(expected));
        }

        [Test]
        public void CreateShouldSetIsEnabledTrueIfQueryRequestContainsFilter()
        {
            const string id = "statId";
            const string textWithPlaceholders = "Adds # to # Chaos Damage";
            string statText = "Adds 10 to 25 Chaos Damage";

            var itemStat = GetItemStat(id, textWithPlaceholders, statText);

            SearchQueryRequest queryRequest = GetQueryRequestWithStatFilter(id, new MinMaxFilter());

            MinMaxStatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, queryRequest, new StatFilterViewModelFactoryConfiguration()) as MinMaxStatFilterViewModel;

            Assert.That(result.IsEnabled);
        }

        [Test]
        public void CreateShouldNotSetIsEnabledTrueIfQueryRequestDoesNotContainFilter()
        {
            const string id = "statId";
            const string textWithPlaceholders = "Adds # to # Chaos Damage";
            string statText = "Adds 10 to 25 Chaos Damage";

            var itemStat = GetItemStat(id, textWithPlaceholders, statText);

            SearchQueryRequest queryRequest = new SearchQueryRequest();

            MinMaxStatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, queryRequest, new StatFilterViewModelFactoryConfiguration()) as MinMaxStatFilterViewModel;

            Assert.That(result.IsEnabled, Is.False);
        }

        [Test]
        public void CreateShouldMapCountOfMonsterItemStatToMinValue()
        {
            const int expected = 2;
            var itemStat = new MonsterItemStat { Id = "monsterItemStat", Count = expected };

            MinMaxStatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest(), new StatFilterViewModelFactoryConfiguration()) as MinMaxStatFilterViewModel;

            Assert.That(result.Min, Is.EqualTo(expected));
        }

        [Test]
        public void CreateShouldNotSetMaxValueForMonsterItemStat()
        {
            var itemStat = new MonsterItemStat { Id = "monsterItemStat", Count = 3 };

            MinMaxStatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest(), new StatFilterViewModelFactoryConfiguration()) as MinMaxStatFilterViewModel;

            Assert.That(result.Max, Is.Null);
        }

        [Test]
        public void CreateShouldIgnoreConfigurationForMonsterItemStat()
        {
            const int expected = 3;
            var itemStat = new MonsterItemStat { Id = "monsterItemStat", Count = expected };

            StatFilterViewModelFactoryConfiguration configuration = new StatFilterViewModelFactoryConfiguration
            {
                MinValuePercentageOffset = -0.1
            };

            MinMaxStatFilterViewModel result = this.statFilterViewModelFactory.Create(itemStat, new SearchQueryRequest(), configuration) as MinMaxStatFilterViewModel;

            Assert.That(result.Min, Is.EqualTo(expected));
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
            return new ItemStat { Id = id, TextWithPlaceholders = textWithPlaceholders, Text = statText };
        }
    }
}