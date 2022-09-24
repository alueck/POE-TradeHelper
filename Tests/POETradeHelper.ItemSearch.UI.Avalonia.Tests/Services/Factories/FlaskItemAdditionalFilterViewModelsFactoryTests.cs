using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using NUnit.Framework;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.UI.Avalonia.Factories.Implementations;
using POETradeHelper.ItemSearch.UI.Avalonia.Properties;
using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Tests.Services.Factories
{
    public class FlaskItemAdditionalFilterViewModelsFactoryTests : AdditionalFilterViewModelsFactoryTestsBase
    {
        [SetUp]
        public void Setup()
        {
            AdditionalFilterViewModelsFactory = new FlaskItemAdditionalFilterViewModelsFactory();
        }

        [TestCaseSource(nameof(NonFlaskItems))]
        public void CreateShouldReturnEmptyEnumerableForNonFlaskItems(Item item)
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
            var flaskItem = new FlaskItem(ItemRarity.Unique)
            {
                Quality = 20
            };

            // act & assert
            CreateShouldReturnBindableMinMaxFilterViewModel(expectedBindingExpression, flaskItem, flaskItem.Quality, Resources.QualityColumn);
        }

        [Test]
        public void CreateShouldReturnQualityFilterViewModelWithValuesFromSearchQueryRequest()
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.Quality;
            var flaskItem = new FlaskItem(ItemRarity.Unique)
            {
                Quality = 19
            };

            var queryRequestFilter = new MinMaxFilter
            {
                Min = 15,
                Max = 20
            };

            CreateShouldReturnBindableMinMaxFilterViewModelWithValuesFromQueryRequest(expectedBindingExpression, flaskItem, flaskItem.Quality, Resources.QualityColumn, queryRequestFilter);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CreateShouldReturnIdentifiedFilterViewModel(bool value)
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.Identified;
            var flaskItem = new FlaskItem(ItemRarity.Rare)
            {
                IsIdentified = value
            };

            CreateShouldReturnBindableFilterViewModel(expectedBindingExpression, flaskItem, null, Resources.Identified);
        }

        [Test]
        public void CreateShouldReturnIdentifiedFilterViewModelWithValueFromQueryRequest()
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.Identified;
            var flaskItem = new FlaskItem(ItemRarity.Rare)
            {
                IsIdentified = true
            };

            var queryRequestFilter = new BoolOptionFilter
            {
                Option = false
            };

            CreateShouldReturnBindableFilterViewModelWithValueFromQueryRequest(expectedBindingExpression, flaskItem, Resources.Identified, queryRequestFilter);
        }

        private static IEnumerable<Item> NonFlaskItems
        {
            get
            {
                yield return new CurrencyItem();
                yield return new DivinationCardItem();
                yield return new MapItem(ItemRarity.Normal);
                yield return new FragmentItem();
                yield return new OrganItem();
                yield return new ProphecyItem();
                yield return new JewelItem(ItemRarity.Magic);
                yield return new EquippableItem(ItemRarity.Magic);
                yield return new GemItem();
            }
        }
    }
}