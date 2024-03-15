using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Microsoft.Extensions.Options;

using NSubstitute;

using NUnit.Framework;

using POETradeHelper.ItemSearch.Contract.Configuration;
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
        public void Setup() => this.AdditionalFilterViewModelsFactory = new JewelItemAdditionalFilterViewModelsFactory(Substitute.For<IOptionsMonitor<ItemSearchOptions>>());

        [TestCaseSource(nameof(GetNonJewelItems))]
        public void CreateShouldReturnEmptyEnumerableForNonJewelItems(Item item)
        {
            IEnumerable<FilterViewModelBase> result =
                this.AdditionalFilterViewModelsFactory.Create(item, new SearchQueryRequest());

            Assert.That(result, Is.Empty);
        }

        [TestCaseSource(nameof(GetBoolOptionFilterTestCases))]
        public void CreateShouldReturnIdentifiedFilterViewModel(BoolOptionFilter queryRequestFilter)
        {
            Expression<Func<SearchQueryRequest, BoolOptionFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.Identified;
            JewelItem jewelItem = new(ItemRarity.Rare)
            {
                IsIdentified = !queryRequestFilter.Option,
            };

            this.CreateShouldReturnBindableBoolOptionFilterViewModel(
                expectedBindingExpression,
                jewelItem,
                Resources.Identified,
                queryRequestFilter);
        }

        [TestCaseSource(nameof(GetBoolOptionFilterTestCases))]
        public void CreateShouldReturnCorruptedFilterViewModel(BoolOptionFilter queryRequestFilter)
        {
            Expression<Func<SearchQueryRequest, BoolOptionFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.Corrupted;
            JewelItem jewelItem = new(ItemRarity.Rare)
            {
                IsCorrupted = !queryRequestFilter.Option,
            };

            this.CreateShouldReturnBindableBoolOptionFilterViewModel(
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