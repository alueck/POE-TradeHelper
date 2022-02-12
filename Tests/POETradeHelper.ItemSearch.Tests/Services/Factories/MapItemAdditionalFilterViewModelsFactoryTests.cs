using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using NUnit.Framework;

using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Properties;
using POETradeHelper.ItemSearch.Services.Factories;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;

namespace POETradeHelper.ItemSearch.Tests.Services.Factories
{
    public class MapItemAdditionalFilterViewModelsFactoryTests : AdditionalFilterViewModelsFactoryTestsBase
    {
        [SetUp]
        public void Setup()
        {
            this.AdditionalFilterViewModelsFactory = new MapItemAdditionalFilterViewModelsFactory();
        }

        [TestCaseSource(nameof(NonMapItems))]
        public void CreateShouldReturnEmptyEnumerableForNonMapItems(Item item)
        {
            IEnumerable<FilterViewModelBase> result = this.AdditionalFilterViewModelsFactory.Create(item, new SearchQueryRequest());

            Assert.IsNotNull(result);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void CreateShouldReturnQualityFilterViewModel()
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.Quality;
            var mapItem = new MapItem(ItemRarity.Rare)
            {
                Quality = 10
            };

            this.CreateShouldReturnBindableMinMaxFilterViewModel(expectedBindingExpression, mapItem, mapItem.Quality, Resources.QualityColumn);
        }

        [Test]
        public void CreateShouldReturnQualityFilterViewModelWithValuesFromQueryRequest()
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.Quality;
            var mapItem = new MapItem(ItemRarity.Rare)
            {
                Quality = 10
            };

            var queryRequestFilter = new MinMaxFilter
            {
                Min = 8,
                Max = 15
            };

            this.CreateShouldReturnBindableMinMaxFilterViewModelWithValuesFromQueryRequest(expectedBindingExpression, mapItem, mapItem.Quality, Resources.QualityColumn, queryRequestFilter);
        }

        [Test]
        public void CreateShouldReturnItemQuantityFilterViewModel()
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MapFilters.MapIncreasedItemQuantity;
            var mapItem = new MapItem(ItemRarity.Rare)
            {
                ItemQuantity = 79
            };

            this.CreateShouldReturnBindableMinMaxFilterViewModel(expectedBindingExpression, mapItem, mapItem.ItemQuantity, Resources.MapItemQuantity);
        }

        [Test]
        public void CreateShouldReturnItemQuantityFilterViewModelWithValuesFromQueryRequest()
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MapFilters.MapIncreasedItemQuantity;
            var mapItem = new MapItem(ItemRarity.Rare)
            {
                ItemQuantity = 79
            };

            var queryRequestFilter = new MinMaxFilter
            {
                Min = 65,
                Max = 90
            };

            this.CreateShouldReturnBindableMinMaxFilterViewModel(expectedBindingExpression, mapItem, mapItem.ItemQuantity, Resources.MapItemQuantity);
        }

        [Test]
        public void CreateShouldReturnItemRarityFilterViewModel()
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MapFilters.MapIncreasedItemRarity;
            var mapItem = new MapItem(ItemRarity.Rare)
            {
                ItemRarity = 101
            };

            this.CreateShouldReturnBindableMinMaxFilterViewModel(expectedBindingExpression, mapItem, mapItem.ItemRarity, Resources.MapItemRarity);
        }

        [Test]
        public void CreateShouldReturnItemRarityFilterViewModelWithValuesFromQueryRequest()
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MapFilters.MapIncreasedItemRarity;
            var mapItem = new MapItem(ItemRarity.Rare)
            {
                ItemRarity = 101
            };

            var queryRequestFilter = new MinMaxFilter
            {
                Min = 95,
                Max = 105
            };

            this.CreateShouldReturnBindableMinMaxFilterViewModel(expectedBindingExpression, mapItem, mapItem.ItemRarity, Resources.MapItemRarity);
        }

        [Test]
        public void CreateShouldReturnMonsterPackSizeFilterViewModel()
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MapFilters.MapPacksize;
            var mapItem = new MapItem(ItemRarity.Rare)
            {
                MonsterPackSize = 35
            };

            this.CreateShouldReturnBindableMinMaxFilterViewModel(expectedBindingExpression, mapItem, mapItem.MonsterPackSize, Resources.MapMonsterPacksize);
        }

        [Test]
        public void CreateShouldReturnMonsterPackSizeFilterViewModelWithValuesFromQueryRequest()
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MapFilters.MapPacksize;
            var mapItem = new MapItem(ItemRarity.Rare)
            {
                MonsterPackSize = 35
            };

            var queryRequestFilter = new MinMaxFilter
            {
                Min = 20,
                Max = 40
            };

            this.CreateShouldReturnBindableMinMaxFilterViewModel(expectedBindingExpression, mapItem, mapItem.MonsterPackSize, Resources.MapMonsterPacksize);
        }

        [Test]
        public void CreateShouldReturnMapTierFilterViewModel()
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MapFilters.MapTier;
            var mapItem = new MapItem(ItemRarity.Rare)
            {
                Tier = 6
            };

            this.CreateShouldReturnBindableMinMaxFilterViewModel(expectedBindingExpression, mapItem, mapItem.Tier, Resources.MapTier);
        }

        [Test]
        public void CreateShouldReturnMapTierFilterViewModelWithValuesFromQueryRequest()
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MapFilters.MapTier;
            var mapItem = new MapItem(ItemRarity.Rare)
            {
                Tier = 6
            };

            var queryRequestFilter = new MinMaxFilter
            {
                Min = 4,
                Max = 8
            };

            this.CreateShouldReturnBindableMinMaxFilterViewModel(expectedBindingExpression, mapItem, mapItem.Tier, Resources.MapTier);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CreateShouldReturnIdentifiedFilterViewModel(bool value)
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.Identified;
            var mapItem = new MapItem(ItemRarity.Rare)
            {
                IsIdentified = value
            };

            this.CreateShouldReturnBindableFilterViewModel(expectedBindingExpression, mapItem, null, Resources.Identified);
        }

        [Test]
        public void CreateShouldReturnIdentifiedFilterViewModelWithValueFromQueryRequest()
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.Identified;
            var mapItem = new MapItem(ItemRarity.Rare)
            {
                IsIdentified = true
            };

            var queryRequestFilter = new BoolOptionFilter
            {
                Option = false
            };

            this.CreateShouldReturnBindableFilterViewModelWithValueFromQueryRequest(expectedBindingExpression, mapItem, Resources.Identified, queryRequestFilter);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CreateShouldReturnCorruptedFilterViewModel(bool value)
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.Corrupted;
            var mapItem = new MapItem(ItemRarity.Rare)
            {
                IsCorrupted = value
            };

            this.CreateShouldReturnBindableFilterViewModel(expectedBindingExpression, mapItem, null, Resources.Corrupted);
        }

        [Test]
        public void CreateShouldReturnCorruptedFilterViewModelWithValueFromQueryRequest()
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.Corrupted;
            var mapItem = new MapItem(ItemRarity.Rare)
            {
                IsCorrupted = true
            };

            var queryRequestFilter = new BoolOptionFilter
            {
                Option = false
            };

            this.CreateShouldReturnBindableFilterViewModelWithValueFromQueryRequest(expectedBindingExpression, mapItem, Resources.Corrupted, queryRequestFilter);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CreateShouldReturnBlightedFilterViewModel(bool value)
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MapFilters.MapBlighted;
            var mapItem = new MapItem(ItemRarity.Rare)
            {
                IsBlighted = value
            };

            this.CreateShouldReturnBindableFilterViewModel(expectedBindingExpression, mapItem, null, Resources.MapBlighted);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CreateShouldReturnBlightRavagedFilterViewModel(bool value)
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MapFilters.MapBlightRavaged;
            var mapItem = new MapItem(ItemRarity.Rare)
            {
                IsBlightRavaged = value
            };

            this.CreateShouldReturnBindableFilterViewModel(expectedBindingExpression, mapItem, null, Resources.MapBlightRavaged);
        }

        [Test]
        public void CreateShouldReturnBlightedFilterViewModelWithValueFromQueryRequest()
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MapFilters.MapBlighted;
            var mapItem = new MapItem(ItemRarity.Rare)
            {
                IsBlighted = true
            };

            var queryRequestFilter = new BoolOptionFilter
            {
                Option = false
            };

            this.CreateShouldReturnBindableFilterViewModelWithValueFromQueryRequest(expectedBindingExpression, mapItem, Resources.MapBlighted, queryRequestFilter);
        }

        private static IEnumerable<Item> NonMapItems
        {
            get
            {
                yield return new CurrencyItem();
                yield return new DivinationCardItem();
                yield return new FlaskItem(ItemRarity.Normal);
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
