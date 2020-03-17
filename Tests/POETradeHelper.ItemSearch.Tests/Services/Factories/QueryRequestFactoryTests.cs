using NUnit.Framework;
using POETradeHelper.ItemSearch.Services.Factories;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;
using System.Collections.Generic;
using System.Linq;

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

        [TestCaseSource(nameof(MapShouldEnabledStatFilterToQueryStatFilterTestData))]
        public void MapShouldEnabledStatFilterToQueryStatFilter(AdvancedQueryViewModel advancedQueryViewModel, MinMaxStatFilterViewModel statFilterViewModel)
        {
            SearchQueryRequest result = this.queryRequestFactory.Map(advancedQueryViewModel) as SearchQueryRequest;

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
        public void MapShouldMapMultipleFilters()
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

            SearchQueryRequest result = this.queryRequestFactory.Map(advancedQueryViewModel) as SearchQueryRequest;

            Assert.NotNull(result);

            Assert.That(result.Query.Stats, Has.Count.EqualTo(1));
            StatFilters statFilters = result.Query.Stats.First();

            Assert.That(statFilters.Filters, Has.Count.EqualTo(2));
            StatFilter statFilter = statFilters.Filters.First();

            Assert.That(statFilters.Filters, Has.One.Matches<StatFilter>(f => f.Id == advancedQueryViewModel.ExplicitItemStatFilters[1].Id));
            Assert.That(statFilters.Filters, Has.One.Matches<StatFilter>(f => f.Id == advancedQueryViewModel.ImplicitItemStatFilters[0].Id));
        }

        [Test]
        public void MapShouldClearStatsOnQueryRequestBeforeAddingNewOnes()
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

            SearchQueryRequest result = this.queryRequestFactory.Map(advancedQueryViewModel) as SearchQueryRequest;

            Assert.NotNull(result);
            Assert.That(result.Query.Stats, Has.Count.EqualTo(1));
            Assert.That(result.Query.Stats.First().Filters, Has.Count.EqualTo(1));
        }

        [Test]
        public void MapShouldMapStatFilters()
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

            SearchQueryRequest result = this.queryRequestFactory.Map(advancedQueryViewModel) as SearchQueryRequest;

            Assert.NotNull(result);

            Assert.That(result.Query.Stats, Has.Count.EqualTo(1));
            StatFilters statFilters = result.Query.Stats.First();

            Assert.That(statFilters.Filters, Has.Count.EqualTo(1));
            StatFilter statFilter = statFilters.Filters.First();

            Assert.That(statFilter.Id, Is.EqualTo(statFilterViewModel.Id));
            Assert.That(statFilter.Text, Is.EqualTo(statFilterViewModel.Text));
        }

        private static IEnumerable<TestCaseData> MapShouldEnabledStatFilterToQueryStatFilterTestData
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