using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using POETradeHelper.ItemSearch.Services.Factories;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;

namespace POETradeHelper.ItemSearch.Tests.Services.Factories
{
    public class QueryRequestFactoryTests
    {
        private QueryRequestFactory queryRequestFactory;

        [SetUp]
        public void Setup()
        {
            this.queryRequestFactory = new QueryRequestFactory();
        }

        [TestCaseSource(nameof(CreateShouldMapEnabledStatFilterToQueryStatFilterTestData))]
        public void CreateShouldMapEnabledStatFilterToQueryStatFilter(AdvancedQueryViewModel advancedQueryViewModel, MinMaxStatFilterViewModel statFilterViewModel)
        {
            SearchQueryRequest result = this.queryRequestFactory.Create(advancedQueryViewModel) as SearchQueryRequest;

            Assert.NotNull(result);

            Assert.That(result.Query.Stats, Has.Count.EqualTo(1));
            StatFilters statFilters = result.Query.Stats.First();

            Assert.That(statFilters.Filters, Has.Count.EqualTo(1));
            StatFilter statFilter = statFilters.Filters.First();

            Assert.That(statFilter.Id, Is.EqualTo(statFilterViewModel.Id));
            Assert.That(statFilter.Text, Is.EqualTo(statFilterViewModel.Text));
            Assert.NotNull(statFilter.Value);
            Assert.That(statFilter.Value.Min, Is.EqualTo(statFilterViewModel.Min));
            Assert.That(statFilter.Value.Max, Is.EqualTo(statFilterViewModel.Max));
        }

        [Test]
        public void CreateShouldSetGreaterValueAsMaxValueForStatFilter()
        {
            const int expectedValue = 65;
            var statFilterViewModel = new MinMaxStatFilterViewModel
            {
                Min = expectedValue,
                Max = 40,
                IsEnabled = true
            };

            var advancedQueryViewModel = new AdvancedQueryViewModel
            {
                ExplicitItemStatFilters =
                {
                    statFilterViewModel
                },
                QueryRequest = new SearchQueryRequest()
            };

            SearchQueryRequest result = this.queryRequestFactory.Create(advancedQueryViewModel) as SearchQueryRequest;

            Assert.NotNull(result);

            Assert.That(result.Query.Stats, Has.Count.EqualTo(1));
            StatFilters statFilters = result.Query.Stats.First();

            Assert.That(statFilters.Filters, Has.Count.EqualTo(1));
            StatFilter statFilter = statFilters.Filters.First();

            Assert.That(statFilter.Value.Min, Is.EqualTo(expectedValue));
            Assert.That(statFilter.Value.Max, Is.EqualTo(expectedValue));
        }

        [Test]
        public void CreateShouldCreateMultipleFilters()
        {
            var advancedQueryViewModel = new AdvancedQueryViewModel
            {
                ExplicitItemStatFilters =
                {
                    new MinMaxStatFilterViewModel
                    {
                        Id = "explicitItemStatId",
                        IsEnabled = false
                    },
                    new MinMaxStatFilterViewModel
                    {
                        Id = "explicitItemStatId2",
                        IsEnabled = true
                    }
                },
                ImplicitItemStatFilters =
                {
                    new MinMaxStatFilterViewModel
                    {
                        Id = "implicitItemStatId",
                        IsEnabled = true
                    }
                },
                QueryRequest = new SearchQueryRequest()
            };

            SearchQueryRequest result = this.queryRequestFactory.Create(advancedQueryViewModel) as SearchQueryRequest;

            Assert.NotNull(result);

            Assert.That(result.Query.Stats, Has.Count.EqualTo(1));
            StatFilters statFilters = result.Query.Stats.First();

            Assert.That(statFilters.Filters, Has.Count.EqualTo(2));
            StatFilter statFilter = statFilters.Filters.First();

            Assert.That(statFilters.Filters, Has.One.Matches<StatFilter>(f => f.Id == advancedQueryViewModel.ExplicitItemStatFilters[1].Id));
            Assert.That(statFilters.Filters, Has.One.Matches<StatFilter>(f => f.Id == advancedQueryViewModel.ImplicitItemStatFilters[0].Id));
        }

        [Test]
        public void CreateShouldClearStatsOnQueryRequestBeforeAddingNewOnes()
        {
            var statFilterViewModel = new MinMaxStatFilterViewModel
            {
                Id = "itemStatId",
                Text = "# to Maximum Life",
                Min = 50,
                Max = 65,
                IsEnabled = true
            };

            var advancedQueryViewModel = new AdvancedQueryViewModel
            {
                ExplicitItemStatFilters =
                {
                    statFilterViewModel
                },
                QueryRequest = new SearchQueryRequest
                {
                    Query =
                    {
                        Stats =
                        {
                            new StatFilters
                            {
                                Filters =
                                {
                                    new StatFilter()
                                }
                            }
                        }
                    }
                }
            };

            SearchQueryRequest result = this.queryRequestFactory.Create(advancedQueryViewModel) as SearchQueryRequest;

            Assert.NotNull(result);
            Assert.That(result.Query.Stats, Has.Count.EqualTo(1));
            Assert.That(result.Query.Stats.First().Filters, Has.Count.EqualTo(1));
        }

        [Test]
        public void CreateShouldCreateStatFilters()
        {
            var statFilterViewModel = new StatFilterViewModel
            {
                Id = "itemStatId",
                Text = "Trigger Edict of Frost on Kill",
                IsEnabled = true
            };

            var advancedQueryViewModel = new AdvancedQueryViewModel
            {
                ExplicitItemStatFilters =
                {
                    statFilterViewModel
                },
                QueryRequest = new SearchQueryRequest()
            };

            SearchQueryRequest result = this.queryRequestFactory.Create(advancedQueryViewModel) as SearchQueryRequest;

            Assert.NotNull(result);

            Assert.That(result.Query.Stats, Has.Count.EqualTo(1));
            StatFilters statFilters = result.Query.Stats.First();

            Assert.That(statFilters.Filters, Has.Count.EqualTo(1));
            StatFilter statFilter = statFilters.Filters.First();

            Assert.That(statFilter.Id, Is.EqualTo(statFilterViewModel.Id));
            Assert.That(statFilter.Text, Is.EqualTo(statFilterViewModel.Text));
        }

        [TestCase(10, 20, 10, 20)]
        [TestCase(105, 117, 105, 117)]
        [TestCase(105, null, 105, null)]
        [TestCase(105, 95, 105, 105)]
        public void CreateShouldMapAdditionalMinMaxFilterToQuery(int? minValue, int? maxValue, int? expectedMinValue, int? expectedMaxValue)
        {
            BindableMinMaxFilterViewModel additionalFilter = new BindableMinMaxFilterViewModel(x => x.Query.Filters.MiscFilters.Quality)
            {
                Min = minValue,
                Max = maxValue,
                IsEnabled = true
            };

            var advancedQueryViewModel = new AdvancedQueryViewModel
            {
                QueryRequest = new SearchQueryRequest(),
                AdditionalFilters =
                {
                    additionalFilter
                }
            };

            SearchQueryRequest result = this.queryRequestFactory.Create(advancedQueryViewModel) as SearchQueryRequest;

            Assert.NotNull(result);

            MinMaxFilter qualityFilter = result.Query.Filters.MiscFilters.Quality;
            Assert.NotNull(qualityFilter);
            Assert.That(qualityFilter.Min, Is.EqualTo(expectedMinValue));
            Assert.That(qualityFilter.Max, Is.EqualTo(expectedMaxValue));
        }

        [Test]
        public void CreateShouldNotMapDisabledAdditionalFilterToQuery()
        {
            BindableMinMaxFilterViewModel additionalFilter = new BindableMinMaxFilterViewModel(x => x.Query.Filters.MiscFilters.Quality)
            {
                IsEnabled = false
            };

            var advancedQueryViewModel = new AdvancedQueryViewModel
            {
                QueryRequest = new SearchQueryRequest(),
                AdditionalFilters =
                {
                    additionalFilter
                }
            };

            SearchQueryRequest result = this.queryRequestFactory.Create(advancedQueryViewModel) as SearchQueryRequest;

            Assert.NotNull(result);
            Assert.Null(result.Query.Filters.MiscFilters.Quality);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CreateShouldMapAdditionalBoolFilterToQuery(bool value)
        {
            BindableFilterViewModel additionalFilter = new BindableFilterViewModel(x => x.Query.Filters.MiscFilters.CrusaderItem)
            {
                IsEnabled = value
            };

            var advancedQueryViewModel = new AdvancedQueryViewModel
            {
                QueryRequest = new SearchQueryRequest(),
                AdditionalFilters =
                {
                    additionalFilter
                }
            };

            SearchQueryRequest result = this.queryRequestFactory.Create(advancedQueryViewModel) as SearchQueryRequest;

            Assert.NotNull(result);

            BoolOptionFilter crusaderItemFilter = result.Query.Filters.MiscFilters.CrusaderItem;
            Assert.NotNull(crusaderItemFilter);
            Assert.That(crusaderItemFilter.Option, Is.EqualTo(value));
        }

        [Test]
        public void CreateShouldClearFiltersBeforeAddingNewOnes()
        {
            BindableFilterViewModel additionalFilter = new BindableFilterViewModel(x => x.Query.Filters.MiscFilters.CrusaderItem)
            {
                IsEnabled = true
            };

            const string categoryOptionValue = "axe";
            const string rarityOptionValue = "unique";
            var advancedQueryViewModel = new AdvancedQueryViewModel
            {
                QueryRequest = new SearchQueryRequest
                {
                    Query =
                    {
                        Filters =
                        {
                            ArmourFilters =
                            {
                                Armour = new MinMaxFilter()
                            },
                            MapFilters =
                            {
                                MapTier = new MinMaxFilter()
                            },
                            MiscFilters =
                            {
                                Corrupted = new BoolOptionFilter()
                            },
                            RequirementsFilters =
                            {
                                Level = new MinMaxFilter()
                            },
                            SocketFilters =
                            {
                                Links = new SocketsFilter()
                            },
                            TypeFilters =
                            {
                                Category = new OptionFilter{ Option = categoryOptionValue },
                                Rarity = new OptionFilter { Option = rarityOptionValue}
                            },
                            WeaponFilters =
                            {
                                Damage = new MinMaxFilter()
                            }
                        }
                    }
                },
                AdditionalFilters =
                {
                    additionalFilter
                }
            };

            SearchQueryRequest result = this.queryRequestFactory.Create(advancedQueryViewModel) as SearchQueryRequest;

            Assert.NotNull(result);
            Assert.IsNull(result.Query.Filters.ArmourFilters.Filters);
            Assert.IsNull(result.Query.Filters.MapFilters.Filters);
            Assert.IsNull(result.Query.Filters.RequirementsFilters.Filters);
            Assert.IsNull(result.Query.Filters.SocketFilters.Filters);
            Assert.IsNull(result.Query.Filters.WeaponFilters.Filters);

            Assert.That(result.Query.Filters.MiscFilters.Filters, Has.Count.EqualTo(1));
            Assert.IsNotNull(result.Query.Filters.MiscFilters.CrusaderItem);

            Assert.That(result.Query.Filters.TypeFilters.Filters, Has.Count.EqualTo(2));
            Assert.That(result.Query.Filters.TypeFilters.Category.Option, Is.EqualTo(categoryOptionValue));
            Assert.That(result.Query.Filters.TypeFilters.Rarity.Option, Is.EqualTo(rarityOptionValue));
        }

        [Test]
        public void CreateShouldMapName()
        {
            const string expected = "expected name";

            var advancedQueryViewModel = new AdvancedQueryViewModel
            {
                QueryRequest = new SearchQueryRequest
                {
                    Query =
                    {
                        Name = expected
                    }
                }
            };

            SearchQueryRequest result = this.queryRequestFactory.Create(advancedQueryViewModel) as SearchQueryRequest;

            Assert.IsNotNull(result);
            Assert.That(result.Query.Name, Is.EqualTo(expected));
        }

        [Test]
        public void CreateShouldMapTerm()
        {
            const string expected = "expected term";

            var advancedQueryViewModel = new AdvancedQueryViewModel
            {
                QueryRequest = new SearchQueryRequest
                {
                    Query =
                    {
                        Term = expected
                    }
                }
            };

            SearchQueryRequest result = this.queryRequestFactory.Create(advancedQueryViewModel) as SearchQueryRequest;

            Assert.IsNotNull(result);
            Assert.That(result.Query.Term, Is.EqualTo(expected));
        }

        [Test]
        public void CreateShouldMapType()
        {
            const string expected = "expected type";

            var advancedQueryViewModel = new AdvancedQueryViewModel
            {
                QueryRequest = new SearchQueryRequest
                {
                    Query =
                    {
                        Type = expected
                    }
                }
            };

            SearchQueryRequest result = this.queryRequestFactory.Create(advancedQueryViewModel) as SearchQueryRequest;

            Assert.IsNotNull(result);
            Assert.That(result.Query.Type, Is.EqualTo(expected));
        }

        [Test]
        public void CreateShouldMapSocketsFilter()
        {
            BindableSocketsFilterViewModel bindableSocketsFilterViewModel = new BindableSocketsFilterViewModel(x => x.Query.Filters.SocketFilters.Sockets)
            {
                Min = 5,
                Max = 6,
                Red = 1,
                Green = 1,
                Blue = 2,
                White = 2,
                IsEnabled = true
            };
            var advancedQueryViewModel = new AdvancedQueryViewModel
            {
                QueryRequest = new SearchQueryRequest(),
                AdditionalFilters =
                {
                    bindableSocketsFilterViewModel
                }
            };

            SearchQueryRequest result = this.queryRequestFactory.Create(advancedQueryViewModel) as SearchQueryRequest;

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Query.Filters.SocketFilters);

            SocketsFilter socketsFilter = result.Query.Filters.SocketFilters.Sockets;
            Assert.IsNotNull(socketsFilter);
            Assert.That(socketsFilter.Min, Is.EqualTo(bindableSocketsFilterViewModel.Min));
            Assert.That(socketsFilter.Max, Is.EqualTo(bindableSocketsFilterViewModel.Max));
            Assert.That(socketsFilter.Red, Is.EqualTo(bindableSocketsFilterViewModel.Red));
            Assert.That(socketsFilter.Green, Is.EqualTo(bindableSocketsFilterViewModel.Green));
            Assert.That(socketsFilter.Blue, Is.EqualTo(bindableSocketsFilterViewModel.Blue));
            Assert.That(socketsFilter.White, Is.EqualTo(bindableSocketsFilterViewModel.White));
        }

        private static IEnumerable<TestCaseData> CreateShouldMapEnabledStatFilterToQueryStatFilterTestData
        {
            get
            {
                MinMaxStatFilterViewModel statFilterViewModel = new MinMaxStatFilterViewModel
                {
                    Id = "itemStatId",
                    Text = "# to Maximum Life",
                    Min = 50,
                    Max = 65,
                    IsEnabled = true
                };

                yield return new TestCaseData(new AdvancedQueryViewModel { EnchantedItemStatFilters = { statFilterViewModel }, QueryRequest = new SearchQueryRequest() }, statFilterViewModel);
                yield return new TestCaseData(new AdvancedQueryViewModel { ImplicitItemStatFilters = { statFilterViewModel }, QueryRequest = new SearchQueryRequest() }, statFilterViewModel);
                yield return new TestCaseData(new AdvancedQueryViewModel { ExplicitItemStatFilters = { statFilterViewModel }, QueryRequest = new SearchQueryRequest() }, statFilterViewModel);
                yield return new TestCaseData(new AdvancedQueryViewModel { CraftedItemStatFilters = { statFilterViewModel }, QueryRequest = new SearchQueryRequest() }, statFilterViewModel);
                yield return new TestCaseData(new AdvancedQueryViewModel { MonsterItemStatFilters = { statFilterViewModel }, QueryRequest = new SearchQueryRequest() }, statFilterViewModel);
                yield return new TestCaseData(new AdvancedQueryViewModel { AdditionalFilters = { statFilterViewModel }, QueryRequest = new SearchQueryRequest() }, statFilterViewModel);
            }
        }
    }
}