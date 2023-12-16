using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

using NUnit.Framework;

using POETradeHelper.Common.Extensions;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.UI.Avalonia.Factories.Implementations;
using POETradeHelper.ItemSearch.UI.Avalonia.Properties;
using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Tests.Services.Factories
{
    public class EquippableItemAdditionalFilterViewModelsFactoryTests : AdditionalFilterViewModelsFactoryTestsBase
    {
        [SetUp]
        public void Setup() =>
            this.AdditionalFilterViewModelsFactory = new EquippableItemAdditionalFilterViewModelsFactory();

        [TestCaseSource(nameof(GetNonEquippableItems))]
        public void CreateShouldReturnEmptyEnumerableForNonEquippableItems(Item item)
        {
            IEnumerable<FilterViewModelBase> result =
                this.AdditionalFilterViewModelsFactory.Create(item, new SearchQueryRequest());

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void CreateShouldReturnQualityFilterViewModel()
        {
            // arrange
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression =
                x => x.Query.Filters.MiscFilters.Quality;
            EquippableItem equippableItem = new(ItemRarity.Unique)
            {
                Quality = 7,
            };

            // act & assert
            this.CreateShouldReturnBindableMinMaxFilterViewModel(
                expectedBindingExpression,
                equippableItem,
                equippableItem.Quality,
                Resources.QualityColumn);
        }

        [Test]
        public void CreateShouldReturnQualityFilterViewModelWithValuesFromSearchQueryRequest()
        {
            // arrange
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.Quality;
            EquippableItem equippableItem = new(ItemRarity.Unique)
            {
                Quality = 7,
            };

            MinMaxFilter queryRequestFilter = new()
            {
                Min = 4,
                Max = 10,
            };

            // act & assert
            this.CreateShouldReturnBindableMinMaxFilterViewModelWithValuesFromQueryRequest(
                expectedBindingExpression,
                equippableItem,
                equippableItem.Quality,
                Resources.QualityColumn,
                queryRequestFilter);
        }

        [Test]
        public void CreateShouldReturnItemLevelFilterViewModel()
        {
            // arrange
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression =
                x => x.Query.Filters.MiscFilters.ItemLevel;
            EquippableItem equippableItem = new(ItemRarity.Unique)
            {
                ItemLevel = 84,
            };

            // act & assert
            this.CreateShouldReturnBindableMinMaxFilterViewModel(
                expectedBindingExpression,
                equippableItem,
                equippableItem.ItemLevel,
                Resources.ItemLevelColumn);
        }

        [Test]
        public void CreateShouldReturnItemLevelFilterViewModelWithValuesFromSearchQueryRequest()
        {
            // arrange
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.ItemLevel;
            EquippableItem equippableItem = new(ItemRarity.Unique)
            {
                ItemLevel = 84,
            };

            MinMaxFilter queryRequestFilter = new()
            {
                Min = 75,
                Max = 90,
            };

            // act & assert
            this.CreateShouldReturnBindableMinMaxFilterViewModelWithValuesFromQueryRequest(
                expectedBindingExpression,
                equippableItem,
                equippableItem.ItemLevel,
                Resources.ItemLevelColumn,
                queryRequestFilter);
        }

        [TestCaseSource(nameof(GetInfluenceFilterViewModelTestData))]
        public void CreateShouldReturnInfluenceFilterViewModel(
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression,
            InfluenceType influenceType)
        {
            // arrange
            EquippableItem equippableItem = new(ItemRarity.Unique)
            {
                Influence = influenceType,
            };

            // act & assert
            this.CreateShouldReturnBindableFilterViewModel(
                expectedBindingExpression,
                equippableItem,
                null,
                equippableItem.Influence.GetDisplayName());
        }

        [TestCaseSource(nameof(GetInfluenceFilterViewModelTestData))]
        public void CreateShouldReturnInfluenceFilterViewModelWithValueFromSearchQueryRequest(
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression,
            InfluenceType influenceType)
        {
            // arrange
            EquippableItem equippableItem = new(ItemRarity.Unique)
            {
                Influence = influenceType,
            };

            BoolOptionFilter queryRequestFilter = new()
            {
                Option = true,
            };

            // act & assert
            this.CreateShouldReturnBindableFilterViewModelWithValueFromQueryRequest(
                expectedBindingExpression,
                equippableItem,
                equippableItem.Influence.GetDisplayName(),
                queryRequestFilter);
        }

        [Test]
        public void CreateShouldReturnSocketFilterViewModelIfItemHasAtLeastOneSocket()
        {
            // arrange
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression =
                x => x.Query.Filters.SocketFilters.Sockets;
            EquippableItem equippableItem = new(ItemRarity.Rare)
            {
                Sockets = new ItemSockets
                {
                    SocketGroups =
                    {
                        new SocketGroup
                        {
                            Sockets =
                            {
                                new Socket(),
                            },
                        },
                    },
                },
            };

            // act & assert
            this.CreateShouldReturnBindableSocketsFilterViewModel(
                expectedBindingExpression,
                equippableItem,
                equippableItem.Sockets.Count,
                Resources.Sockets);
        }

        [Test]
        public void CreateShouldReturnSocketFilterViewModelWithValuesFromSearchQueryRequest()
        {
            // arrange
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression =
                x => x.Query.Filters.SocketFilters.Sockets;
            EquippableItem equippableItem = new(ItemRarity.Rare)
            {
                Sockets = new ItemSockets
                {
                    SocketGroups =
                    {
                        new SocketGroup
                        {
                            Sockets =
                            {
                                new Socket(),
                            },
                        },
                    },
                },
            };

            SocketsFilter queryRequestFilter = new()
            {
                Min = 1,
                Max = 3,
                Red = 1,
                Green = 2,
                Blue = 1,
                White = 2,
            };

            // act & assert
            this.CreateShouldReturnBindableSocketsFilterViewModelWithValuesFromQueryRequest(
                expectedBindingExpression,
                equippableItem,
                equippableItem.Sockets.Count,
                Resources.Sockets,
                queryRequestFilter);
        }

        [Test]
        public void CreateShouldNotReturnSocketFilterViewModelIfItemNoSockets()
        {
            EquippableItem equippableItem = new(ItemRarity.Rare);

            IEnumerable<FilterViewModelBase> result =
                this.AdditionalFilterViewModelsFactory.Create(equippableItem, new SearchQueryRequest());

            Assert.That(result, Has.None.Matches<FilterViewModelBase>(x => x.Text == Resources.Sockets));
        }

        [Test]
        public void CreateShouldReturnLinksFilterViewModelIfItemHasAtLeastOneSocket()
        {
            // arrange
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression =
                x => x.Query.Filters.SocketFilters.Links;
            EquippableItem equippableItem = new(ItemRarity.Rare)
            {
                Sockets = new ItemSockets
                {
                    SocketGroups =
                    {
                        new SocketGroup
                        {
                            Sockets =
                            {
                                new Socket(),
                            },
                        },
                    },
                },
            };

            // act & assert
            this.CreateShouldReturnBindableMinMaxFilterViewModel(
                expectedBindingExpression,
                equippableItem,
                0,
                Resources.Links);
        }

        [Test]
        public void CreateShouldReturnLinksFilterViewModelWithMaxLinkCount()
        {
            // arrange
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression =
                x => x.Query.Filters.SocketFilters.Links;
            EquippableItem equippableItem = new(ItemRarity.Rare)
            {
                Sockets = new ItemSockets
                {
                    SocketGroups =
                    {
                        new SocketGroup
                        {
                            Sockets =
                            {
                                new Socket(),
                            },
                        },
                        new SocketGroup
                        {
                            Sockets =
                            {
                                new Socket(),
                                new Socket(),
                            },
                        },
                        new SocketGroup
                        {
                            Sockets =
                            {
                                new Socket(),
                                new Socket(),
                                new Socket(),
                            },
                        },
                    },
                },
            };

            // act & assert
            this.CreateShouldReturnBindableMinMaxFilterViewModel(
                expectedBindingExpression,
                equippableItem,
                3,
                Resources.Links);
        }

        [Test]
        public void CreateShouldReturnLinkFilterViewModelWithValuesFromSearchQueryRequest()
        {
            // arrange
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression =
                x => x.Query.Filters.SocketFilters.Links;
            EquippableItem equippableItem = new(ItemRarity.Rare)
            {
                Sockets = new ItemSockets
                {
                    SocketGroups =
                    {
                        new SocketGroup
                        {
                            Sockets =
                            {
                                new Socket(),
                            },
                        },
                    },
                },
            };

            SocketsFilter queryRequestFilter = new()
            {
                Min = 1,
                Max = 3,
            };

            // act & assert
            this.CreateShouldReturnBindableMinMaxFilterViewModelWithValuesFromQueryRequest(
                expectedBindingExpression,
                equippableItem,
                0,
                Resources.Links,
                queryRequestFilter);
        }

        [Test]
        public void CreateShouldNotReturnLinkFilterViewModelIfItemNoSockets()
        {
            EquippableItem equippableItem = new(ItemRarity.Rare);

            IEnumerable<FilterViewModelBase> result =
                this.AdditionalFilterViewModelsFactory.Create(equippableItem, new SearchQueryRequest());

            Assert.That(result, Has.None.Matches<FilterViewModelBase>(x => x.Text == Resources.Links));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CreateShouldReturnIdentifiedFilterViewModel(bool value)
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression =
                x => x.Query.Filters.MiscFilters.Identified;
            EquippableItem equippableItem = new(ItemRarity.Rare)
            {
                IsIdentified = value,
            };

            this.CreateShouldReturnBindableFilterViewModel(
                expectedBindingExpression,
                equippableItem,
                null,
                Resources.Identified);
        }

        [Test]
        public void CreateShouldReturnIdentifiedFilterViewModelWithValueFromQueryRequest()
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression =
                x => x.Query.Filters.MiscFilters.Identified;
            EquippableItem equippableItem = new(ItemRarity.Rare)
            {
                IsIdentified = true,
            };

            BoolOptionFilter queryRequestFilter = new()
            {
                Option = false,
            };

            this.CreateShouldReturnBindableFilterViewModelWithValueFromQueryRequest(
                expectedBindingExpression,
                equippableItem,
                Resources.Identified,
                queryRequestFilter);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CreateShouldReturnCorruptedFilterViewModel(bool value)
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression =
                x => x.Query.Filters.MiscFilters.Corrupted;
            EquippableItem equippableItem = new(ItemRarity.Rare)
            {
                IsCorrupted = value,
            };

            this.CreateShouldReturnBindableFilterViewModel(
                expectedBindingExpression,
                equippableItem,
                null,
                Resources.Corrupted);
        }

        [Test]
        public void CreateShouldReturnCorruptedFilterViewModelWithValueFromQueryRequest()
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression =
                x => x.Query.Filters.MiscFilters.Corrupted;
            EquippableItem equippableItem = new(ItemRarity.Rare)
            {
                IsCorrupted = true,
            };

            BoolOptionFilter queryRequestFilter = new()
            {
                Option = false,
            };

            this.CreateShouldReturnBindableFilterViewModelWithValueFromQueryRequest(
                expectedBindingExpression,
                equippableItem,
                Resources.Corrupted,
                queryRequestFilter);
        }

        private static IEnumerable<Item> GetNonEquippableItems()
        {
            yield return new CurrencyItem();
            yield return new DivinationCardItem();
            yield return new MapItem(ItemRarity.Normal);
            yield return new FragmentItem();
            yield return new OrganItem();
            yield return new ProphecyItem();
            yield return new JewelItem(ItemRarity.Magic);
            yield return new FlaskItem(ItemRarity.Magic);
            yield return new GemItem();
        }

        private static IEnumerable GetInfluenceFilterViewModelTestData()
        {
            yield return new TestCaseData(
                (Expression<Func<SearchQueryRequest, IFilter>>)(x => x.Query.Filters.MiscFilters.CrusaderItem),
                InfluenceType.Crusader);
            yield return new TestCaseData(
                (Expression<Func<SearchQueryRequest, IFilter>>)(x => x.Query.Filters.MiscFilters.ElderItem),
                InfluenceType.Elder);
            yield return new TestCaseData(
                (Expression<Func<SearchQueryRequest, IFilter>>)(x => x.Query.Filters.MiscFilters.HunterItem),
                InfluenceType.Hunter);
            yield return new TestCaseData(
                (Expression<Func<SearchQueryRequest, IFilter>>)(x => x.Query.Filters.MiscFilters.RedeemerItem),
                InfluenceType.Redeemer);
            yield return new TestCaseData(
                (Expression<Func<SearchQueryRequest, IFilter>>)(x => x.Query.Filters.MiscFilters.ShaperItem),
                InfluenceType.Shaper);
            yield return new TestCaseData(
                (Expression<Func<SearchQueryRequest, IFilter>>)(x => x.Query.Filters.MiscFilters.WarlordItem),
                InfluenceType.Warlord);
        }
    }
}