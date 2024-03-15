using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

using NSubstitute;

using NUnit.Framework;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Services.Mappers;
using POETradeHelper.ItemSearch.UI.Avalonia.Factories.Implementations;
using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;

using ReactiveUI;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Tests.Services.Factories
{
    public class QueryRequestFactoryTests
    {
        private readonly List<IItemSearchQueryRequestMapper> itemSearchQueryRequestMapperMocks;
        private readonly SearchQueryRequestFactory searchQueryRequestFactory;

        public QueryRequestFactoryTests()
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

            result.Should().BeEquivalentTo(expected);
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

            result.Should().BeEquivalentTo(new SearchQueryRequest
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
                                    Id = statFilterViewModel.Id,
                                    Text = statFilterViewModel.Text,
                                    Value = new MinMaxFilter { Min = statFilterViewModel.Min, Max = statFilterViewModel.Max },
                                },
                            },
                        },
                    },
                },
            });
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

            result.Should().BeEquivalentTo(new SearchQueryRequest
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
                                    Id = statFilterViewModel.Id,
                                    Text = statFilterViewModel.Text,
                                    Value = new MinMaxFilter { Min = expectedValue, Max = expectedValue },
                                },
                            },
                        },
                    },
                },
            });
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

            result.Should().BeEquivalentTo(new SearchQueryRequest
            {
                Query =
                {
                    Stats =
                    {
                        new StatFilters
                        {
                            Filters =
                            {
                                new StatFilter { Id = filters[0].Id },
                                new StatFilter { Id = filters[2].Id },
                            },
                        },
                    },
                },
            });
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

            result.Query.Should().NotBeNull();
            result.Query.Stats.Should().SatisfyRespectively(x => x.Filters.Should().ContainSingle());
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

            result.Should().BeEquivalentTo(new SearchQueryRequest
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
                                    Id = statFilterViewModel.Id,
                                    Text = statFilterViewModel.Text,
                                },
                            },
                        },
                    },
                },
            });
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

            result.Should().NotBeNull();

            MinMaxFilter? qualityFilter = result.Query.Filters.MiscFilters.Quality;
            qualityFilter.Should()
                .BeEquivalentTo(new MinMaxFilter
                {
                    Min = expectedMinValue,
                    Max = expectedMaxValue,
                });
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

            result.Should().BeEquivalentTo(new SearchQueryRequest());
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CreateShouldMapAdditionalBoolFilterToQuery(bool value)
        {
            BindableFilterViewModel<BoolOptionFilter> additionalFilter = new(x => x.Query.Filters.MiscFilters.CrusaderItem)
            {
                IsEnabled = value,
            };
            IAdvancedFiltersViewModel advancedFiltersViewModel = GetAdvancedFiltersViewModel(null, new[] { additionalFilter });

            SearchQueryRequest result = this.searchQueryRequestFactory.Create(new SearchQueryRequest(), advancedFiltersViewModel);

            result.Should().NotBeNull();

            BoolOptionFilter? crusaderItemFilter = result.Query.Filters.MiscFilters.CrusaderItem;
            crusaderItemFilter.Should().BeEquivalentTo(new BoolOptionFilter { Option = value });
        }

        [Test]
        public void CreateShouldNotMapAdditionalBoolFilterWithoutEnabledValueToQuery()
        {
            BindableFilterViewModel<BoolOptionFilter> additionalFilter = new(x => x.Query.Filters.MiscFilters.CrusaderItem);
            IAdvancedFiltersViewModel advancedFiltersViewModel = GetAdvancedFiltersViewModel(null, new[] { additionalFilter });

            SearchQueryRequest result = this.searchQueryRequestFactory.Create(new SearchQueryRequest(), advancedFiltersViewModel);

            result.Should().BeEquivalentTo(new SearchQueryRequest());
        }

        [Test]
        public void CreateShouldClearFiltersBeforeAddingNewOnes()
        {
            BindableFilterViewModel<BoolOptionFilter> additionalFilter = new(x => x.Query.Filters.MiscFilters.CrusaderItem)
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

            result.Should().BeEquivalentTo(new SearchQueryRequest
            {
                Query =
                {
                    Filters =
                    {
                        TypeFilters =
                        {
                            Category = searchQueryRequest.Query.Filters.TypeFilters.Category,
                            Rarity = searchQueryRequest.Query.Filters.TypeFilters.Rarity,
                        },
                        MiscFilters =
                        {
                            CrusaderItem = new BoolOptionFilter { Option = true },
                        },
                    },
                },
            });
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

            result.Should().BeEquivalentTo(searchQueryRequest);
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

            result.Should().BeEquivalentTo(searchQueryRequest);
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

            result.Should().BeEquivalentTo(searchQueryRequest);
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

            result.Should().BeEquivalentTo(searchQueryRequest);
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

            result.Should().BeEquivalentTo(new SearchQueryRequest
            {
                Query =
                {
                    Filters =
                    {
                        SocketFilters =
                        {
                            Sockets = new SocketsFilter
                            {
                                Min = bindableSocketsFilterViewModel.Min,
                                Max = bindableSocketsFilterViewModel.Max,
                                Red = bindableSocketsFilterViewModel.Red,
                                Green = bindableSocketsFilterViewModel.Green,
                                Blue = bindableSocketsFilterViewModel.Blue,
                                White = bindableSocketsFilterViewModel.White,
                            },
                        },
                    },
                },
            });
        }

        private static IAdvancedFiltersViewModel GetAdvancedFiltersViewModel(
            IEnumerable<StatFilterViewModel>? statFilters,
            IEnumerable<FilterViewModelBase>? additionalFilters)
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