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
        public void Setup()
        {
            AdditionalFilterViewModelsFactory = new EquippableItemAdditionalFilterViewModelsFactory();
        }

        [TestCaseSource(nameof(NonEquippableItems))]
        public void CreateShouldReturnEmptyEnumerableForNonEquippableItems(Item item)
        {
            IEnumerable<FilterViewModelBase> result = AdditionalFilterViewModelsFactory.Create(item, new SearchQueryRequest());

            Assert.IsNotNull(result);
            Assert.That(result, Is.Empty);
        }

        private static IEnumerable<Item> NonEquippableItems
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
                yield return new FlaskItem(ItemRarity.Magic);
                yield return new GemItem();
            }
        }

        [Test]
        public void CreateShouldReturnQualityFilterViewModel()
        {
            // arrange
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.Quality;
            var equippableItem = new EquippableItem(ItemRarity.Unique)
            {
                Quality = 7
            };

            // act & assert
            CreateShouldReturnBindableMinMaxFilterViewModel(expectedBindingExpression, equippableItem, equippableItem.Quality, Resources.QualityColumn);
        }

        [Test]
        public void CreateShouldReturnQualityFilterViewModelWithValuesFromSearchQueryRequest()
        {
            // arrange
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.Quality;
            var equippableItem = new EquippableItem(ItemRarity.Unique)
            {
                Quality = 7
            };

            var queryRequestFilter = new MinMaxFilter
            {
                Min = 4,
                Max = 10
            };

            // act & assert
            CreateShouldReturnBindableMinMaxFilterViewModelWithValuesFromQueryRequest(expectedBindingExpression, equippableItem, equippableItem.Quality, Resources.QualityColumn, queryRequestFilter);
        }

        [Test]
        public void CreateShouldReturnItemLevelFilterViewModel()
        {
            // arrange
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.ItemLevel;
            var equippableItem = new EquippableItem(ItemRarity.Unique)
            {
                ItemLevel = 84
            };

            // act & assert
            CreateShouldReturnBindableMinMaxFilterViewModel(expectedBindingExpression, equippableItem, equippableItem.ItemLevel, Resources.ItemLevelColumn);
        }

        [Test]
        public void CreateShouldReturnItemLevelFilterViewModelWithValuesFromSearchQueryRequest()
        {
            // arrange
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.ItemLevel;
            var equippableItem = new EquippableItem(ItemRarity.Unique)
            {
                ItemLevel = 84
            };

            var queryRequestFilter = new MinMaxFilter
            {
                Min = 75,
                Max = 90
            };

            // act & assert
            CreateShouldReturnBindableMinMaxFilterViewModelWithValuesFromQueryRequest(expectedBindingExpression, equippableItem, equippableItem.ItemLevel, Resources.ItemLevelColumn, queryRequestFilter);
        }

        [TestCaseSource(nameof(InfluenceFilterViewModelTestData))]
        public void CreateShouldReturnInfluenceFilterViewModel(Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression, InfluenceType influenceType)
        {
            // arrange
            var equippableItem = new EquippableItem(ItemRarity.Unique)
            {
                Influence = influenceType
            };

            // act & assert
            CreateShouldReturnBindableFilterViewModel(expectedBindingExpression, equippableItem, null, equippableItem.Influence.GetDisplayName());
        }

        [TestCaseSource(nameof(InfluenceFilterViewModelTestData))]
        public void CreateShouldReturnInfluenceFilterViewModelWithValueFromSearchQueryRequest(Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression, InfluenceType influenceType)
        {
            // arrange
            var equippableItem = new EquippableItem(ItemRarity.Unique)
            {
                Influence = influenceType
            };

            var queryRequestFilter = new BoolOptionFilter
            {
                Option = true
            };

            // act & assert
            CreateShouldReturnBindableFilterViewModelWithValueFromQueryRequest(expectedBindingExpression, equippableItem, equippableItem.Influence.GetDisplayName(), queryRequestFilter);
        }

        private static IEnumerable InfluenceFilterViewModelTestData
        {
            get
            {
                yield return new TestCaseData((Expression<Func<SearchQueryRequest, IFilter>>)(x => x.Query.Filters.MiscFilters.CrusaderItem), InfluenceType.Crusader);
                yield return new TestCaseData((Expression<Func<SearchQueryRequest, IFilter>>)(x => x.Query.Filters.MiscFilters.ElderItem), InfluenceType.Elder);
                yield return new TestCaseData((Expression<Func<SearchQueryRequest, IFilter>>)(x => x.Query.Filters.MiscFilters.HunterItem), InfluenceType.Hunter);
                yield return new TestCaseData((Expression<Func<SearchQueryRequest, IFilter>>)(x => x.Query.Filters.MiscFilters.RedeemerItem), InfluenceType.Redeemer);
                yield return new TestCaseData((Expression<Func<SearchQueryRequest, IFilter>>)(x => x.Query.Filters.MiscFilters.ShaperItem), InfluenceType.Shaper);
                yield return new TestCaseData((Expression<Func<SearchQueryRequest, IFilter>>)(x => x.Query.Filters.MiscFilters.WarlordItem), InfluenceType.Warlord);
            }
        }

        [Test]
        public void CreateShouldReturnSocketFilterViewModelIfItemHasAtLeastOneSocket()
        {
            // arrange
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.SocketFilters.Sockets;
            var equippableItem = new EquippableItem(ItemRarity.Rare)
            {
                Sockets = new ItemSockets
                {
                    SocketGroups =
                    {
                        new SocketGroup
                        {
                            Sockets =
                            {
                                new Socket()
                            }
                        }
                    }
                }
            };

            // act & assert
            CreateShouldReturnBindableSocketsFilterViewModel(expectedBindingExpression, equippableItem, equippableItem.Sockets.Count, Resources.Sockets);
        }

        [Test]
        public void CreateShouldReturnSocketFilterViewModelWithValuesFromSearchQueryRequest()
        {
            // arrange
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.SocketFilters.Sockets;
            var equippableItem = new EquippableItem(ItemRarity.Rare)
            {
                Sockets = new ItemSockets
                {
                    SocketGroups =
                    {
                        new SocketGroup
                        {
                            Sockets =
                            {
                                new Socket()
                            }
                        }
                    }
                }
            };

            var queryRequestFilter = new SocketsFilter
            {
                Min = 1,
                Max = 3,
                Red = 1,
                Green = 2,
                Blue = 1,
                White = 2
            };

            // act & assert
            CreateShouldReturnBindableSocketsFilterViewModelWithValuesFromQueryRequest(expectedBindingExpression, equippableItem, equippableItem.Sockets.Count, Resources.Sockets, queryRequestFilter);
        }

        [Test]
        public void CreateShouldNotReturnSocketFilterViewModelIfItemNoSockets()
        {
            // arrange
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.SocketFilters.Sockets;
            var equippableItem = new EquippableItem(ItemRarity.Rare);

            var result = AdditionalFilterViewModelsFactory.Create(equippableItem, new SearchQueryRequest());

            Assert.That(result, Has.None.Matches<FilterViewModelBase>(x => x.Text == Resources.Sockets));
        }

        [Test]
        public void CreateShouldReturnLinksFilterViewModelIfItemHasAtLeastOneSocket()
        {
            // arrange
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.SocketFilters.Links;
            var equippableItem = new EquippableItem(ItemRarity.Rare)
            {
                Sockets = new ItemSockets
                {
                    SocketGroups =
                    {
                        new SocketGroup
                        {
                            Sockets =
                            {
                                new Socket()
                            }
                        }
                    }
                }
            };

            // act & assert
            CreateShouldReturnBindableMinMaxFilterViewModel(expectedBindingExpression, equippableItem, 0, Resources.Links);
        }

        [Test]
        public void CreateShouldReturnLinksFilterViewModelWithMaxLinkCount()
        {
            // arrange
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.SocketFilters.Links;
            var equippableItem = new EquippableItem(ItemRarity.Rare)
            {
                Sockets = new ItemSockets
                {
                    SocketGroups =
                    {
                        new SocketGroup
                        {
                            Sockets =
                            {
                                new Socket()
                            }
                        },
                        new SocketGroup
                        {
                            Sockets =
                            {
                                new Socket(),
                                new Socket()
                            }
                        },
                        new SocketGroup
                        {
                            Sockets =
                            {
                                new Socket(),
                                new Socket(),
                                new Socket()
                            }
                        }
                    }
                }
            };

            // act & assert
            CreateShouldReturnBindableMinMaxFilterViewModel(expectedBindingExpression, equippableItem, 3, Resources.Links);
        }

        [Test]
        public void CreateShouldReturnLinkFilterViewModelWithValuesFromSearchQueryRequest()
        {
            // arrange
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.SocketFilters.Links;
            var equippableItem = new EquippableItem(ItemRarity.Rare)
            {
                Sockets = new ItemSockets
                {
                    SocketGroups =
                    {
                        new SocketGroup
                        {
                            Sockets =
                            {
                                new Socket()
                            }
                        }
                    }
                }
            };

            var queryRequestFilter = new SocketsFilter
            {
                Min = 1,
                Max = 3
            };

            // act & assert
            CreateShouldReturnBindableMinMaxFilterViewModelWithValuesFromQueryRequest(expectedBindingExpression, equippableItem, 0, Resources.Links, queryRequestFilter);
        }

        [Test]
        public void CreateShouldNotReturnLinkFilterViewModelIfItemNoSockets()
        {
            // arrange
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.SocketFilters.Links;
            var equippableItem = new EquippableItem(ItemRarity.Rare);

            var result = AdditionalFilterViewModelsFactory.Create(equippableItem, new SearchQueryRequest());

            Assert.That(result, Has.None.Matches<FilterViewModelBase>(x => x.Text == Resources.Links));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CreateShouldReturnIdentifiedFilterViewModel(bool value)
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.Identified;
            var equippableItem = new EquippableItem(ItemRarity.Rare)
            {
                IsIdentified = value
            };

            CreateShouldReturnBindableFilterViewModel(expectedBindingExpression, equippableItem, null, Resources.Identified);
        }

        [Test]
        public void CreateShouldReturnIdentifiedFilterViewModelWithValueFromQueryRequest()
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.Identified;
            var equippableItem = new EquippableItem(ItemRarity.Rare)
            {
                IsIdentified = true
            };

            var queryRequestFilter = new BoolOptionFilter
            {
                Option = false
            };

            CreateShouldReturnBindableFilterViewModelWithValueFromQueryRequest(expectedBindingExpression, equippableItem, Resources.Identified, queryRequestFilter);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CreateShouldReturnCorruptedFilterViewModel(bool value)
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.Corrupted;
            var equippableItem = new EquippableItem(ItemRarity.Rare)
            {
                IsCorrupted = value
            };

            CreateShouldReturnBindableFilterViewModel(expectedBindingExpression, equippableItem, null, Resources.Corrupted);
        }

        [Test]
        public void CreateShouldReturnCorruptedFilterViewModelWithValueFromQueryRequest()
        {
            Expression<Func<SearchQueryRequest, IFilter>> expectedBindingExpression = x => x.Query.Filters.MiscFilters.Corrupted;
            var equippableItem = new EquippableItem(ItemRarity.Rare)
            {
                IsCorrupted = true
            };

            var queryRequestFilter = new BoolOptionFilter
            {
                Option = false
            };

            CreateShouldReturnBindableFilterViewModelWithValueFromQueryRequest(expectedBindingExpression, equippableItem, Resources.Corrupted, queryRequestFilter);
        }
    }
}