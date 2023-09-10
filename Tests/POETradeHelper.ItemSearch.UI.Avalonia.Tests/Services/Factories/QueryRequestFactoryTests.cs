using System.Collections.Generic;
using System.Linq;

using NSubstitute;

using NUnit.Framework;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services.Mappers;
using POETradeHelper.ItemSearch.UI.Avalonia.Factories.Implementations;
using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Tests.Services.Factories
{
    public class QueryRequestFactoryTests
    {
        private List<IItemSearchQueryRequestMapper> itemSearchQueryRequestMapperMocks;
        private SearchQueryRequestFactory searchQueryRequestFactory;

        [SetUp]
        public void Setup()
        {
            this.itemSearchQueryRequestMapperMocks = new List<IItemSearchQueryRequestMapper>
            {
                Substitute.For<IItemSearchQueryRequestMapper>(),
                Substitute.For<IItemSearchQueryRequestMapper>()
            };

            this.searchQueryRequestFactory = new SearchQueryRequestFactory(this.itemSearchQueryRequestMapperMocks);
        }

        [Test]
        public void CreateShouldCallCanMapOnAllItemSearchQueryRequestMappers()
        {
            var item = new EquippableItem(ItemRarity.Rare) { Name = "TestItem" };
            this.itemSearchQueryRequestMapperMocks[1].CanMap(item)
                .Returns(true);

            this.searchQueryRequestFactory.Create(item);

            this.itemSearchQueryRequestMapperMocks[0]
                .Received()
                .CanMap(item);
            this.itemSearchQueryRequestMapperMocks[1]
                .Received()
                .CanMap(item);
        }

        [Test]
        public void CreateShouldCallMapToQueryRequestOnFirstItemSearchQueryRequestMapper()
        {
            var item = new EquippableItem(ItemRarity.Rare) { Name = "TestItem" };

            this.itemSearchQueryRequestMapperMocks[0].CanMap(item)
                .Returns(true);
            this.itemSearchQueryRequestMapperMocks[1].CanMap(item)
                .Returns(true);

            this.searchQueryRequestFactory.Create(item);

            this.itemSearchQueryRequestMapperMocks[0]
                .Received()
                .MapToQueryRequest(item);
            this.itemSearchQueryRequestMapperMocks[1]
                .DidNotReceive()
                .MapToQueryRequest(item);
        }

        [Test]
        public void CreateShouldReturnResultFromItemSearchQueryRequestMapper()
        {
            var item = new EquippableItem(ItemRarity.Rare) { Name = "TestItem" };
            var expected = new SearchQueryRequest
            {
                League = "Heist"
            };

            this.itemSearchQueryRequestMapperMocks[0].CanMap(item)
                .Returns(true);
            this.itemSearchQueryRequestMapperMocks[0].MapToQueryRequest(item)
                .Returns(expected);

            SearchQueryRequest result = this.searchQueryRequestFactory.Create(item);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void CreateShouldMapEnabledStatFilterToQueryStatFilter()
        {
            MinMaxStatFilterViewModel statFilterViewModel = new()
            {
                Id = "itemStatId",
                Text = "# to Maximum Life",
                Min = 50,
                Max = 65,
                IsEnabled = true
            };
            var advancedFiltersViewModel = GetAdvancedFiltersViewModel(new[] { statFilterViewModel }, null);
            SearchQueryRequest result = this.searchQueryRequestFactory.Create(new SearchQueryRequest(), advancedFiltersViewModel);

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
            var advancedFiltersViewModel = GetAdvancedFiltersViewModel(new[] { statFilterViewModel }, null);

            SearchQueryRequest result = this.searchQueryRequestFactory.Create(new SearchQueryRequest(), advancedFiltersViewModel);

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
            var filters = new StatFilterViewModel[]
            {
                new MinMaxStatFilterViewModel
                {
                    Id = "statId1",
                    IsEnabled = true
                },
                new MinMaxStatFilterViewModel
                {
                    Id = "statId2",
                    IsEnabled = false
                },
                new MinMaxStatFilterViewModel
                {
                    Id = "statId3",
                    IsEnabled = true
                }
            };
            var advancedFiltersViewModel = GetAdvancedFiltersViewModel(filters, null);

            SearchQueryRequest result = this.searchQueryRequestFactory.Create(new SearchQueryRequest(), advancedFiltersViewModel);

            Assert.NotNull(result);

            Assert.That(result.Query.Stats, Has.Count.EqualTo(1));
            StatFilters statFilters = result.Query.Stats.First();

            Assert.That(statFilters.Filters, Has.Count.EqualTo(2));
            Assert.That(statFilters.Filters, Has.One.Matches<StatFilter>(f => f.Id == filters[0].Id));
            Assert.That(statFilters.Filters, Has.One.Matches<StatFilter>(f => f.Id == filters[2].Id));
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
            var advancedFiltersViewModel = GetAdvancedFiltersViewModel(new[] { statFilterViewModel }, null);

            var searchQueryRequest = new SearchQueryRequest
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
            };

            SearchQueryRequest result = this.searchQueryRequestFactory.Create(searchQueryRequest, advancedFiltersViewModel);

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
            var advancedFiltersViewModel = GetAdvancedFiltersViewModel(new[] { statFilterViewModel }, null);

            SearchQueryRequest result = this.searchQueryRequestFactory.Create(new SearchQueryRequest(), advancedFiltersViewModel);

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
            BindableMinMaxFilterViewModel additionalFilter = new(x => x.Query.Filters.MiscFilters.Quality)
            {
                Min = minValue,
                Max = maxValue,
                IsEnabled = true
            };

            var advancedFiltersViewModel = GetAdvancedFiltersViewModel(null, new[] { additionalFilter });

            SearchQueryRequest result = this.searchQueryRequestFactory.Create(new SearchQueryRequest(), advancedFiltersViewModel);

            Assert.NotNull(result);

            MinMaxFilter qualityFilter = result.Query.Filters.MiscFilters.Quality;
            Assert.NotNull(qualityFilter);
            Assert.That(qualityFilter.Min, Is.EqualTo(expectedMinValue));
            Assert.That(qualityFilter.Max, Is.EqualTo(expectedMaxValue));
        }

        [Test]
        public void CreateShouldNotMapDisabledAdditionalFilterToQuery()
        {
            BindableMinMaxFilterViewModel additionalFilter = new(x => x.Query.Filters.MiscFilters.Quality)
            {
                IsEnabled = false
            };
            var advancedFiltersViewModel = GetAdvancedFiltersViewModel(null, new[] { additionalFilter });

            SearchQueryRequest result = this.searchQueryRequestFactory.Create(new SearchQueryRequest(), advancedFiltersViewModel);

            Assert.NotNull(result);
            Assert.Null(result.Query.Filters.MiscFilters.Quality);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CreateShouldMapAdditionalBoolFilterToQuery(bool value)
        {
            BindableFilterViewModel additionalFilter = new(x => x.Query.Filters.MiscFilters.CrusaderItem)
            {
                IsEnabled = value
            };
            var advancedFiltersViewModel = GetAdvancedFiltersViewModel(null, new[] { additionalFilter });

            SearchQueryRequest result = this.searchQueryRequestFactory.Create(new SearchQueryRequest(), advancedFiltersViewModel);

            Assert.NotNull(result);

            BoolOptionFilter crusaderItemFilter = result.Query.Filters.MiscFilters.CrusaderItem;
            Assert.NotNull(crusaderItemFilter);
            Assert.That(crusaderItemFilter.Option, Is.EqualTo(value));
        }

        [Test]
        public void CreateShouldNotMapAdditionalBoolFilterWithoutEnabledValueToQuery()
        {
            BindableFilterViewModel additionalFilter = new(x => x.Query.Filters.MiscFilters.CrusaderItem);
            var advancedFiltersViewModel = GetAdvancedFiltersViewModel(null, new[] { additionalFilter });

            SearchQueryRequest result = this.searchQueryRequestFactory.Create(new SearchQueryRequest(), advancedFiltersViewModel);

            Assert.NotNull(result);
            Assert.IsNull(result.Query.Filters.MiscFilters.CrusaderItem);
        }

        [Test]
        public void CreateShouldClearFiltersBeforeAddingNewOnes()
        {
            BindableFilterViewModel additionalFilter = new(x => x.Query.Filters.MiscFilters.CrusaderItem)
            {
                IsEnabled = true
            };
            var advancedFiltersViewModel = GetAdvancedFiltersViewModel(null, new[] { additionalFilter });

            const string categoryOptionValue = "axe";
            const string rarityOptionValue = "unique";
            var searchQueryRequest = new SearchQueryRequest
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
            };

            SearchQueryRequest result = this.searchQueryRequestFactory.Create(searchQueryRequest, advancedFiltersViewModel);

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
            var searchQueryRequest = new SearchQueryRequest
            {
                Query =
                {
                    Name = expected
                }
            };

            SearchQueryRequest result = this.searchQueryRequestFactory.Create(searchQueryRequest, GetAdvancedFiltersViewModel(null, null));

            Assert.IsNotNull(result);
            Assert.That(result.Query.Name, Is.EqualTo(expected));
        }

        [Test]
        public void CreateShouldMapTerm()
        {
            const string expected = "expected term";
            var searchQueryRequest = new SearchQueryRequest
            {
                Query =
                {
                    Term = expected
                }
            };

            SearchQueryRequest result = this.searchQueryRequestFactory.Create(searchQueryRequest, GetAdvancedFiltersViewModel(null, null));

            Assert.IsNotNull(result);
            Assert.That(result.Query.Term, Is.EqualTo(expected));
        }

        [Test]
        public void CreateShouldMapType()
        {
            const string expected = "expected type";
            var searchQueryRequest = new SearchQueryRequest
            {
                Query =
                {
                    Type = expected
                }
            };

            SearchQueryRequest result = this.searchQueryRequestFactory.Create(searchQueryRequest, GetAdvancedFiltersViewModel(null, null));

            Assert.IsNotNull(result);
            Assert.That(result.Query.Type, Is.EqualTo(expected));
        }

        [Test]
        public void CreateShouldMapLeague()
        {
            const string expected = "expected league";
            var searchQueryRequest = new SearchQueryRequest
            {
                League = expected
            };

            SearchQueryRequest result = this.searchQueryRequestFactory.Create(searchQueryRequest, GetAdvancedFiltersViewModel(null, null));

            Assert.IsNotNull(result);
            Assert.That(result.League, Is.EqualTo(expected));
        }

        [Test]
        public void CreateShouldMapSocketsFilter()
        {
            BindableSocketsFilterViewModel bindableSocketsFilterViewModel = new(x => x.Query.Filters.SocketFilters.Sockets)
            {
                Min = 5,
                Max = 6,
                Red = 1,
                Green = 1,
                Blue = 2,
                White = 2,
                IsEnabled = true
            };
            var advancedFiltersViewModel = GetAdvancedFiltersViewModel(null, new[] { bindableSocketsFilterViewModel });

            SearchQueryRequest result = this.searchQueryRequestFactory.Create(new SearchQueryRequest(), advancedFiltersViewModel);

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

        private static IAdvancedFiltersViewModel GetAdvancedFiltersViewModel(IEnumerable<StatFilterViewModel> statFilters, IEnumerable<FilterViewModelBase> additionalFilters)
        {
            var mock = Substitute.For<IAdvancedFiltersViewModel>();
            mock.AllStatFilters
                .Returns(statFilters ?? new List<StatFilterViewModel>());
            mock.AdditionalFilters
                .Returns(additionalFilters?.ToList() ?? new List<FilterViewModelBase>());

            return mock;
        }
    }
}
