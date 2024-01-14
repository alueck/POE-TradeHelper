﻿using System.Collections.Generic;
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
                Substitute.For<IItemSearchQueryRequestMapper>(),
            };

            this.searchQueryRequestFactory = new SearchQueryRequestFactory(this.itemSearchQueryRequestMapperMocks);
        }

        [Test]
        public void CreateShouldCallCanMapOnAllItemSearchQueryRequestMappers()
        {
            EquippableItem item = new(ItemRarity.Rare) { Name = "TestItem" };
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
            EquippableItem item = new(ItemRarity.Rare) { Name = "TestItem" };

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
            EquippableItem item = new(ItemRarity.Rare) { Name = "TestItem" };
            SearchQueryRequest expected = new()
            {
                League = "Heist",
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
                IsEnabled = true,
            };
            IAdvancedFiltersViewModel advancedFiltersViewModel = GetAdvancedFiltersViewModel(new[] { statFilterViewModel }, null);
            SearchQueryRequest result = this.searchQueryRequestFactory.Create(new SearchQueryRequest(), advancedFiltersViewModel);

            Assert.That(result, Is.Not.Null);

            Assert.That(result.Query.Stats, Has.Count.EqualTo(1));
            StatFilters statFilters = result.Query.Stats.First();

            Assert.That(statFilters.Filters, Has.Count.EqualTo(1));
            StatFilter statFilter = statFilters.Filters.First();

            Assert.That(statFilter.Id, Is.EqualTo(statFilterViewModel.Id));
            Assert.That(statFilter.Text, Is.EqualTo(statFilterViewModel.Text));
            Assert.That(statFilter.Value, Is.Not.Null);
            Assert.That(statFilter.Value.Min, Is.EqualTo(statFilterViewModel.Min));
            Assert.That(statFilter.Value.Max, Is.EqualTo(statFilterViewModel.Max));
        }

        [Test]
        public void CreateShouldSetGreaterValueAsMaxValueForStatFilter()
        {
            const int expectedValue = 65;
            MinMaxStatFilterViewModel statFilterViewModel = new()
            {
                Min = expectedValue,
                Max = 40,
                IsEnabled = true,
            };
            IAdvancedFiltersViewModel advancedFiltersViewModel = GetAdvancedFiltersViewModel(new[] { statFilterViewModel }, null);

            SearchQueryRequest result = this.searchQueryRequestFactory.Create(new SearchQueryRequest(), advancedFiltersViewModel);

            Assert.That(result, Is.Not.Null);

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
            StatFilterViewModel[] filters =
            {
                new MinMaxStatFilterViewModel
                {
                    Id = "statId1",
                    IsEnabled = true,
                },
                new MinMaxStatFilterViewModel
                {
                    Id = "statId2",
                    IsEnabled = false,
                },
                new MinMaxStatFilterViewModel
                {
                    Id = "statId3",
                    IsEnabled = true,
                },
            };
            IAdvancedFiltersViewModel advancedFiltersViewModel = GetAdvancedFiltersViewModel(filters, null);

            SearchQueryRequest result = this.searchQueryRequestFactory.Create(new SearchQueryRequest(), advancedFiltersViewModel);

            Assert.That(result, Is.Not.Null);

            Assert.That(result.Query.Stats, Has.Count.EqualTo(1));
            StatFilters statFilters = result.Query.Stats.First();

            Assert.That(statFilters.Filters, Has.Count.EqualTo(2));
            Assert.That(statFilters.Filters, Has.One.Matches<StatFilter>(f => f.Id == filters[0].Id));
            Assert.That(statFilters.Filters, Has.One.Matches<StatFilter>(f => f.Id == filters[2].Id));
        }

        [Test]
        public void CreateShouldClearStatsOnQueryRequestBeforeAddingNewOnes()
        {
            MinMaxStatFilterViewModel statFilterViewModel = new()
            {
                Id = "itemStatId",
                Text = "# to Maximum Life",
                Min = 50,
                Max = 65,
                IsEnabled = true,
            };
            IAdvancedFiltersViewModel advancedFiltersViewModel = GetAdvancedFiltersViewModel(new[] { statFilterViewModel }, null);

            SearchQueryRequest searchQueryRequest = new()
            {
                Query =
                {
                    Stats =
                    {
                        new StatFilters
                        {
                            Filters =
                            {
                                new StatFilter(),
                            },
                        },
                    },
                },
            };

            SearchQueryRequest result = this.searchQueryRequestFactory.Create(searchQueryRequest, advancedFiltersViewModel);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Query.Stats, Has.Count.EqualTo(1));
            Assert.That(result.Query.Stats.First().Filters, Has.Count.EqualTo(1));
        }

        [Test]
        public void CreateShouldCreateStatFilters()
        {
            StatFilterViewModel statFilterViewModel = new()
            {
                Id = "itemStatId",
                Text = "Trigger Edict of Frost on Kill",
                IsEnabled = true,
            };
            IAdvancedFiltersViewModel advancedFiltersViewModel = GetAdvancedFiltersViewModel(new[] { statFilterViewModel }, null);

            SearchQueryRequest result = this.searchQueryRequestFactory.Create(new SearchQueryRequest(), advancedFiltersViewModel);

            Assert.That(result, Is.Not.Null);

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
                IsEnabled = true,
            };

            IAdvancedFiltersViewModel advancedFiltersViewModel = GetAdvancedFiltersViewModel(null, new[] { additionalFilter });

            SearchQueryRequest result = this.searchQueryRequestFactory.Create(new SearchQueryRequest(), advancedFiltersViewModel);

            Assert.That(result, Is.Not.Null);

            MinMaxFilter qualityFilter = result.Query.Filters.MiscFilters.Quality;
            Assert.That(qualityFilter, Is.Not.Null);
            Assert.That(qualityFilter.Min, Is.EqualTo(expectedMinValue));
            Assert.That(qualityFilter.Max, Is.EqualTo(expectedMaxValue));
        }

        [Test]
        public void CreateShouldNotMapDisabledAdditionalFilterToQuery()
        {
            BindableMinMaxFilterViewModel additionalFilter = new(x => x.Query.Filters.MiscFilters.Quality)
            {
                IsEnabled = false,
            };
            IAdvancedFiltersViewModel advancedFiltersViewModel = GetAdvancedFiltersViewModel(null, new[] { additionalFilter });

            SearchQueryRequest result = this.searchQueryRequestFactory.Create(new SearchQueryRequest(), advancedFiltersViewModel);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Query.Filters.MiscFilters.Quality, Is.Null);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CreateShouldMapAdditionalBoolFilterToQuery(bool value)
        {
            BindableFilterViewModel additionalFilter = new(x => x.Query.Filters.MiscFilters.CrusaderItem)
            {
                IsEnabled = value,
            };
            IAdvancedFiltersViewModel advancedFiltersViewModel = GetAdvancedFiltersViewModel(null, new[] { additionalFilter });

            SearchQueryRequest result = this.searchQueryRequestFactory.Create(new SearchQueryRequest(), advancedFiltersViewModel);

            Assert.That(result, Is.Not.Null);

            BoolOptionFilter crusaderItemFilter = result.Query.Filters.MiscFilters.CrusaderItem;
            Assert.That(crusaderItemFilter, Is.Not.Null);
            Assert.That(crusaderItemFilter.Option, Is.EqualTo(value));
        }

        [Test]
        public void CreateShouldNotMapAdditionalBoolFilterWithoutEnabledValueToQuery()
        {
            BindableFilterViewModel additionalFilter = new(x => x.Query.Filters.MiscFilters.CrusaderItem);
            IAdvancedFiltersViewModel advancedFiltersViewModel = GetAdvancedFiltersViewModel(null, new[] { additionalFilter });

            SearchQueryRequest result = this.searchQueryRequestFactory.Create(new SearchQueryRequest(), advancedFiltersViewModel);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Query.Filters.MiscFilters.CrusaderItem, Is.Null);
        }

        [Test]
        public void CreateShouldClearFiltersBeforeAddingNewOnes()
        {
            BindableFilterViewModel additionalFilter = new(x => x.Query.Filters.MiscFilters.CrusaderItem)
            {
                IsEnabled = true,
            };
            IAdvancedFiltersViewModel advancedFiltersViewModel = GetAdvancedFiltersViewModel(null, new[] { additionalFilter });

            const string categoryOptionValue = "axe";
            const string rarityOptionValue = "unique";
            SearchQueryRequest searchQueryRequest = new()
            {
                Query =
                {
                    Filters =
                    {
                        ArmourFilters =
                        {
                            Armour = new MinMaxFilter(),
                        },
                        MapFilters =
                        {
                            MapTier = new MinMaxFilter(),
                        },
                        MiscFilters =
                        {
                            Corrupted = new BoolOptionFilter(),
                        },
                        RequirementsFilters =
                        {
                            Level = new MinMaxFilter(),
                        },
                        SocketFilters =
                        {
                            Links = new SocketsFilter(),
                        },
                        TypeFilters =
                        {
                            Category = new OptionFilter { Option = categoryOptionValue },
                            Rarity = new OptionFilter { Option = rarityOptionValue },
                        },
                        WeaponFilters =
                        {
                            Damage = new MinMaxFilter(),
                        },
                    },
                },
            };

            SearchQueryRequest result = this.searchQueryRequestFactory.Create(searchQueryRequest, advancedFiltersViewModel);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Query.Filters.ArmourFilters.Filters, Is.Null);
            Assert.That(result.Query.Filters.MapFilters.Filters, Is.Null);
            Assert.That(result.Query.Filters.RequirementsFilters.Filters, Is.Null);
            Assert.That(result.Query.Filters.SocketFilters.Filters, Is.Null);
            Assert.That(result.Query.Filters.WeaponFilters.Filters, Is.Null);

            Assert.That(result.Query.Filters.MiscFilters.Filters, Has.Count.EqualTo(1));
            Assert.That(result.Query.Filters.MiscFilters.CrusaderItem, Is.Not.Null);

            Assert.That(result.Query.Filters.TypeFilters.Filters, Has.Count.EqualTo(2));
            Assert.That(result.Query.Filters.TypeFilters.Category.Option, Is.EqualTo(categoryOptionValue));
            Assert.That(result.Query.Filters.TypeFilters.Rarity.Option, Is.EqualTo(rarityOptionValue));
        }

        [Test]
        public void CreateShouldMapName()
        {
            const string expected = "expected name";
            SearchQueryRequest searchQueryRequest = new()
            {
                Query =
                {
                    Name = expected,
                },
            };

            SearchQueryRequest result = this.searchQueryRequestFactory.Create(searchQueryRequest, GetAdvancedFiltersViewModel(null, null));

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Query.Name, Is.EqualTo(expected));
        }

        [Test]
        public void CreateShouldMapTerm()
        {
            const string expected = "expected term";
            SearchQueryRequest searchQueryRequest = new()
            {
                Query =
                {
                    Term = expected,
                },
            };

            SearchQueryRequest result = this.searchQueryRequestFactory.Create(searchQueryRequest, GetAdvancedFiltersViewModel(null, null));

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Query.Term, Is.EqualTo(expected));
        }

        [Test]
        public void CreateShouldMapType()
        {
            const string expected = "expected type";
            SearchQueryRequest searchQueryRequest = new()
            {
                Query =
                {
                    Type = new TypeFilter { Option = expected },
                },
            };

            SearchQueryRequest result = this.searchQueryRequestFactory.Create(searchQueryRequest, GetAdvancedFiltersViewModel(null, null));

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Query.Type, Is.EqualTo(searchQueryRequest.Query.Type));
        }

        [Test]
        public void CreateShouldMapLeague()
        {
            const string expected = "expected league";
            SearchQueryRequest searchQueryRequest = new()
            {
                League = expected,
            };

            SearchQueryRequest result = this.searchQueryRequestFactory.Create(searchQueryRequest, GetAdvancedFiltersViewModel(null, null));

            Assert.That(result, Is.Not.Null);
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
                IsEnabled = true,
            };
            IAdvancedFiltersViewModel advancedFiltersViewModel = GetAdvancedFiltersViewModel(null, new[] { bindableSocketsFilterViewModel });

            SearchQueryRequest result = this.searchQueryRequestFactory.Create(new SearchQueryRequest(), advancedFiltersViewModel);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Query.Filters.SocketFilters, Is.Not.Null);

            SocketsFilter socketsFilter = result.Query.Filters.SocketFilters.Sockets;
            Assert.That(socketsFilter, Is.Not.Null);
            Assert.That(socketsFilter.Min, Is.EqualTo(bindableSocketsFilterViewModel.Min));
            Assert.That(socketsFilter.Max, Is.EqualTo(bindableSocketsFilterViewModel.Max));
            Assert.That(socketsFilter.Red, Is.EqualTo(bindableSocketsFilterViewModel.Red));
            Assert.That(socketsFilter.Green, Is.EqualTo(bindableSocketsFilterViewModel.Green));
            Assert.That(socketsFilter.Blue, Is.EqualTo(bindableSocketsFilterViewModel.Blue));
            Assert.That(socketsFilter.White, Is.EqualTo(bindableSocketsFilterViewModel.White));
        }

        private static IAdvancedFiltersViewModel GetAdvancedFiltersViewModel(
            IEnumerable<StatFilterViewModel> statFilters,
            IEnumerable<FilterViewModelBase> additionalFilters)
        {
            IAdvancedFiltersViewModel mock = Substitute.For<IAdvancedFiltersViewModel>();
            mock.AllStatFilters
                .Returns(statFilters ?? new List<StatFilterViewModel>());
            mock.AdditionalFilters
                .Returns(additionalFilters?.ToList() ?? new List<FilterViewModelBase>());

            return mock;
        }
    }
}