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
    public class JewelItemAdditionalFilterViewModelsFactoryTests : AdditionalFilterViewModelsFactoryTestsBase
    {
        [SetUp]
        public void Setup() => this.AdditionalFilterViewModelsFactory = new JewelItemAdditionalFilterViewModelsFactory();

        [TestCaseSource(nameof(GetNonJewelItems))]
        public void CreateShouldReturnEmptyEnumerableForNonJewelItems(Item item)
        {
            IEnumerable<FilterViewModelBase> result =
                this.AdditionalFilterViewModelsFactory.Create(item, new SearchQueryRequest());

            Assert.That(result, Is.Empty);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CreateShouldReturnIdentifiedFilterViewModel(bool value)
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.Identified;
            JewelItem jewelItem = new(ItemRarity.Rare)
            {
                IsIdentified = value,
            };

            this.CreateShouldReturnBindableFilterViewModel(
                expectedBindingExpression,
                jewelItem,
                null,
                Resources.Identified);
        }

        [Test]
        public void CreateShouldReturnIdentifiedFilterViewModelWithValueFromQueryRequest()
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.Identified;
            JewelItem jewelItem = new(ItemRarity.Rare)
            {
                IsIdentified = true,
            };

            BoolOptionFilter queryRequestFilter = new()
            {
                Option = false,
            };

            this.CreateShouldReturnBindableFilterViewModelWithValueFromQueryRequest(
                expectedBindingExpression,
                jewelItem,
                Resources.Identified,
                queryRequestFilter);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CreateShouldReturnCorruptedFilterViewModel(bool value)
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.Corrupted;
            JewelItem jewelItem = new(ItemRarity.Rare)
            {
                IsCorrupted = value,
            };

            this.CreateShouldReturnBindableFilterViewModel(expectedBindingExpression, jewelItem, null, Resources.Corrupted);
        }

        [Test]
        public void CreateShouldReturnCorruptedFilterViewModelWithValueFromQueryRequest()
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.Corrupted;
            JewelItem jewelItem = new(ItemRarity.Rare)
            {
                IsCorrupted = true,
            };

            BoolOptionFilter queryRequestFilter = new()
            {
                Option = false,
            };

            this.CreateShouldReturnBindableFilterViewModelWithValueFromQueryRequest(
                expectedBindingExpression,
                jewelItem,
                Resources.Corrupted,
                queryRequestFilter);
        }

        private static IEnumerable<Item> GetNonJewelItems()
        {
            yield return new CurrencyItem();
            yield return new DivinationCardItem();
            yield return new FlaskItem(ItemRarity.Normal);
            yield return new FragmentItem();
            yield return new OrganItem();
            yield return new ProphecyItem();
            yield return new MapItem(ItemRarity.Magic);
            yield return new EquippableItem(ItemRarity.Magic);
            yield return new GemItem();
        }
    }
}