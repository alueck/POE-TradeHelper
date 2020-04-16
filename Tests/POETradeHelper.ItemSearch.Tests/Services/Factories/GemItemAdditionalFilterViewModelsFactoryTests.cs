using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Properties;
using POETradeHelper.ItemSearch.Services.Factories;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace POETradeHelper.ItemSearch.Tests.Services.Factories
{
    public class GemItemAdditionalFilterViewModelsFactoryTests
    {
        private GemItemAdditionalFilterViewModelsFactory gemItemAdditionalFilterViewModelsFactory;

        [SetUp]
        public void Setup()
        {
            this.gemItemAdditionalFilterViewModelsFactory = new GemItemAdditionalFilterViewModelsFactory();
        }

        [TestCaseSource(nameof(NonGemItems))]
        public void CreateShouldReturnEmptyEnumerableForNonGemItems(Item item)
        {
            IEnumerable<FilterViewModelBase> result = this.gemItemAdditionalFilterViewModelsFactory.Create(item, new SearchQueryRequest());

            Assert.IsNotNull(result);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void CreateShouldReturnQualityFilterViewModel()
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.Quality;
            var gemItem = new GemItem
            {
                Quality = 11
            };

            var expected = new BindableMinMaxFilterViewModel(expectedBindingExpression)
            {
                Min = gemItem.Quality,
                Max = gemItem.Quality,
                Current = gemItem.Quality.ToString(),
                Text = Resources.QualityColumn,
                IsEnabled = false
            };

            IEnumerable<FilterViewModelBase> result = this.gemItemAdditionalFilterViewModelsFactory.Create(gemItem, new SearchQueryRequest());

            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);

            Assert.That(result, Has.One.Matches<FilterViewModelBase>(x => MatchFilterViewModel(x, expected)));
        }

        [Test]
        public void CreateShouldReturnQualityFilterViewModelWithValuesFromSearchQueryRequest()
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.Quality;
            var gemItem = new GemItem
            {
                Quality = 11
            };

            int? minValue = 10;
            int? maxValue = 17;
            var searchQueryRequest = new SearchQueryRequest
            {
                Query =
                {
                    Filters =
                    {
                        MiscFilters =
                        {
                            Quality = new MinMaxFilter
                            {
                                Min = minValue,
                                Max = maxValue
                            }
                        }
                    }
                }
            };

            var expected = new BindableMinMaxFilterViewModel(expectedBindingExpression)
            {
                Min = minValue,
                Max = maxValue,
                Current = gemItem.Quality.ToString(),
                Text = Resources.QualityColumn,
                IsEnabled = true
            };

            IEnumerable<FilterViewModelBase> result = this.gemItemAdditionalFilterViewModelsFactory.Create(gemItem, searchQueryRequest);

            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);

            Assert.That(result, Has.One.Matches<FilterViewModelBase>(x => MatchFilterViewModel(x, expected)));
        }

        [Test]
        public void CreateShouldReturnGemLevelFilterViewModel()
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.GemLevel;
            var gemItem = new GemItem
            {
                Level = 10
            };

            var expected = new BindableMinMaxFilterViewModel(expectedBindingExpression)
            {
                Min = gemItem.Level,
                Max = gemItem.Level,
                Current = gemItem.Level.ToString(),
                Text = Resources.GemLevelColumn,
                IsEnabled = false
            };

            IEnumerable<FilterViewModelBase> result = this.gemItemAdditionalFilterViewModelsFactory.Create(gemItem, new SearchQueryRequest());

            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);

            Assert.That(result, Has.One.Matches<FilterViewModelBase>(x => MatchFilterViewModel(x, expected)));
        }

        [Test]
        public void CreateShouldReturnGemLevelFilterViewModelWithValuesFromSearchQueryRequest()
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.GemLevel;
            var gemItem = new GemItem
            {
                Level = 11
            };

            int? minValue = 5;
            int? maxValue = 20;
            var searchQueryRequest = new SearchQueryRequest
            {
                Query =
                {
                    Filters =
                    {
                        MiscFilters =
                        {
                            GemLevel = new MinMaxFilter
                            {
                                Min = minValue,
                                Max = maxValue
                            }
                        }
                    }
                }
            };

            var expected = new BindableMinMaxFilterViewModel(expectedBindingExpression)
            {
                Min = minValue,
                Max = maxValue,
                Current = gemItem.Level.ToString(),
                Text = Resources.GemLevelColumn,
                IsEnabled = true
            };

            IEnumerable<FilterViewModelBase> result = this.gemItemAdditionalFilterViewModelsFactory.Create(gemItem, searchQueryRequest);

            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);

            Assert.That(result, Has.One.Matches<FilterViewModelBase>(x => MatchFilterViewModel(x, expected)));
        }

        [Test]
        public void CreateShouldReturnExperiencePercentlFilterViewModel()
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.GemLevelProgress;
            var gemItem = new GemItem
            {
                ExperiencePercent = 27
            };

            var expected = new BindableMinMaxFilterViewModel(expectedBindingExpression)
            {
                Min = gemItem.ExperiencePercent,
                Max = gemItem.ExperiencePercent,
                Current = gemItem.ExperiencePercent.ToString(),
                Text = Resources.GemExperiencePercentColumn,
                IsEnabled = false
            };

            IEnumerable<FilterViewModelBase> result = this.gemItemAdditionalFilterViewModelsFactory.Create(gemItem, new SearchQueryRequest());

            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);

            Assert.That(result, Has.One.Matches<FilterViewModelBase>(x => MatchFilterViewModel(x, expected)));
        }

        [Test]
        public void CreateShouldReturnGemExperiencePercentFilterViewModelWithValuesFromSearchQueryRequest()
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.GemLevelProgress;
            var gemItem = new GemItem
            {
                ExperiencePercent = 52
            };

            int? minValue = 47;
            int? maxValue = 90;
            var searchQueryRequest = new SearchQueryRequest
            {
                Query =
                {
                    Filters =
                    {
                        MiscFilters =
                        {
                            GemLevelProgress = new MinMaxFilter
                            {
                                Min = minValue,
                                Max = maxValue
                            }
                        }
                    }
                }
            };

            var expected = new BindableMinMaxFilterViewModel(expectedBindingExpression)
            {
                Min = minValue,
                Max = maxValue,
                Current = gemItem.ExperiencePercent.ToString(),
                Text = Resources.GemExperiencePercentColumn,
                IsEnabled = true
            };

            IEnumerable<FilterViewModelBase> result = this.gemItemAdditionalFilterViewModelsFactory.Create(gemItem, searchQueryRequest);

            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);

            Assert.That(result, Has.One.Matches<FilterViewModelBase>(x => MatchFilterViewModel(x, expected)));
        }

        private static bool MatchFilterViewModel(FilterViewModelBase actual, BindableMinMaxFilterViewModel expected)
        {
            return actual is BindableMinMaxFilterViewModel bindableMinMaxFilterViewModel
                            && bindableMinMaxFilterViewModel.BindingExpression.ToString() == expected.BindingExpression.ToString()
                            && bindableMinMaxFilterViewModel.Min == expected.Min
                            && bindableMinMaxFilterViewModel.Max == expected.Max
                            && bindableMinMaxFilterViewModel.Current == expected.Current
                            && bindableMinMaxFilterViewModel.Text == expected.Text
                            && bindableMinMaxFilterViewModel.IsEnabled == expected.IsEnabled;
        }

        private static IEnumerable<Item> NonGemItems
        {
            get
            {
                yield return new CurrencyItem();
                yield return new DivinationCardItem();
                yield return new FlaskItem(ItemRarity.Normal);
                yield return new FragmentItem();
                yield return new MapItem(ItemRarity.Normal);
                yield return new OrganItem();
                yield return new ProphecyItem();
                yield return new JewelItem(ItemRarity.Magic);
                yield return new EquippableItem(ItemRarity.Magic);
            }
        }
    }
}