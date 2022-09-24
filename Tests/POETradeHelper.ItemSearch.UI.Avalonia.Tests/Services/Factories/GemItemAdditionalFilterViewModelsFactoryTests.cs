using NUnit.Framework;

using POETradeHelper.ItemSearch.UI.Avalonia.Properties;
using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.UI.Avalonia.Factories.Implementations;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Tests.Services.Factories
{
    public class GemItemAdditionalFilterViewModelsFactoryTests : AdditionalFilterViewModelsFactoryTestsBase
    {
        [SetUp]
        public void Setup()
        {
            AdditionalFilterViewModelsFactory = new GemItemAdditionalFilterViewModelsFactory();
        }

        [TestCaseSource(nameof(NonGemItems))]
        public void CreateShouldReturnEmptyEnumerableForNonGemItems(Item item)
        {
            IEnumerable<FilterViewModelBase> result = AdditionalFilterViewModelsFactory.Create(item, new SearchQueryRequest());

            Assert.IsNotNull(result);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void CreateShouldReturnQualityFilterViewModel()
        {
            // arrange
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.Quality;
            var gemItem = new GemItem
            {
                Quality = 11
            };

            // act & assert
            CreateShouldReturnBindableMinMaxFilterViewModel(expectedBindingExpression, gemItem, gemItem.Quality, Resources.QualityColumn);
        }

        [Test]
        public void CreateShouldReturnQualityFilterViewModelWithValuesFromSearchQueryRequest()
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.Quality;
            var gemItem = new GemItem
            {
                Quality = 11
            };

            var queryRequestFilter = new MinMaxFilter
            {
                Min = 10,
                Max = 17
            };

            CreateShouldReturnBindableMinMaxFilterViewModelWithValuesFromQueryRequest(expectedBindingExpression, gemItem, gemItem.Quality, Resources.QualityColumn, queryRequestFilter);
        }

        [Test]
        public void CreateShouldReturnGemLevelFilterViewModel()
        {
            // arrange
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.GemLevel;
            var gemItem = new GemItem
            {
                Level = 10
            };

            // act & assert
            CreateShouldReturnBindableMinMaxFilterViewModel(expectedBindingExpression, gemItem, gemItem.Level, Resources.GemLevelColumn);
        }

        [Test]
        public void CreateShouldReturnGemLevelFilterViewModelWithValuesFromSearchQueryRequest()
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.GemLevel;
            var gemItem = new GemItem
            {
                Level = 11
            };

            var queryRequestFilter = new MinMaxFilter
            {
                Min = 5,
                Max = 20
            };

            CreateShouldReturnBindableMinMaxFilterViewModelWithValuesFromQueryRequest(expectedBindingExpression, gemItem, gemItem.Level, Resources.GemLevelColumn, queryRequestFilter);
        }

        [Test]
        public void CreateShouldReturnExperiencePercentlFilterViewModel()
        {
            // arrange
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.GemLevelProgress;
            var gemItem = new GemItem
            {
                ExperiencePercent = 27
            };

            // act & assert
            CreateShouldReturnBindableMinMaxFilterViewModel(expectedBindingExpression, gemItem, gemItem.ExperiencePercent, Resources.GemExperiencePercentColumn);
        }

        [Test]
        public void CreateShouldReturnGemExperiencePercentFilterViewModelWithValuesFromSearchQueryRequest()
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.GemLevelProgress;
            var gemItem = new GemItem
            {
                ExperiencePercent = 52
            };

            var queryRequestFilter = new MinMaxFilter
            {
                Min = 47,
                Max = 90
            };

            CreateShouldReturnBindableMinMaxFilterViewModelWithValuesFromQueryRequest(expectedBindingExpression, gemItem, gemItem.ExperiencePercent, Resources.GemExperiencePercentColumn, queryRequestFilter);
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