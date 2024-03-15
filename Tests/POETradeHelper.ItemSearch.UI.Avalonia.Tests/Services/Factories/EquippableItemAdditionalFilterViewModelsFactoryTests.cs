using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

using FluentAssertions;

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
        public EquippableItemAdditionalFilterViewModelsFactoryTests()
        {
            this.AdditionalFilterViewModelsFactory = new EquippableItemAdditionalFilterViewModelsFactory(this.ItemSearchOptionsMonitorMock);
        }

        [TestCaseSource(nameof(GetNonEquippableItems))]
        public void CreateShouldReturnEmptyEnumerableForNonEquippableItems(Item item)
        {
            IEnumerable<FilterViewModelBase> result =
                this.AdditionalFilterViewModelsFactory.Create(item, new SearchQueryRequest());

            result.Should().BeEmpty();
        }

        [TestCaseSource(nameof(GetMinMaxFilterTestCases))]
        public void CreateShouldReturnArmourFilterViewModel(MinMaxFilter queryRequestFilter)
        {
            // arrange
            Expression<Func<SearchQueryRequest, MinMaxFilter?>> expectedBindingExpression =
                x => x.Query.Filters.ArmourFilters.Armour;
            EquippableItem equippableItem = new(ItemRarity.Unique)
            {
                ArmourValues = new() { Armour = 100 },
            };

            // act & assert
            this.CreateShouldReturnBindableMinMaxFilterViewModel(
                expectedBindingExpression,
                equippableItem,
                Resources.Armour,
                equippableItem.ArmourValues.Armour.Value,
                queryRequestFilter,
                offsetCurrentValue: true);
        }

        [TestCaseSource(nameof(GetMinMaxFilterTestCases))]
        public void CreateShouldReturnBlockChanceFilterViewModel(MinMaxFilter queryRequestFilter)
        {
            // arrange
            Expression<Func<SearchQueryRequest, MinMaxFilter?>> expectedBindingExpression =
                x => x.Query.Filters.ArmourFilters.Block;
            EquippableItem equippableItem = new(ItemRarity.Unique)
            {
                ArmourValues = new() { BlockChance = 30 },
            };

            // act & assert
            this.CreateShouldReturnBindableMinMaxFilterViewModel(
                expectedBindingExpression,
                equippableItem,
                Resources.ChanceToBlock,
                equippableItem.ArmourValues.BlockChance.Value,
                queryRequestFilter,
                offsetCurrentValue: true);
        }

        [TestCaseSource(nameof(GetMinMaxFilterTestCases))]
        public void CreateShouldReturnEnergyShieldFilterViewModel(MinMaxFilter queryRequestFilter)
        {
            // arrange
            Expression<Func<SearchQueryRequest, MinMaxFilter?>> expectedBindingExpression =
                x => x.Query.Filters.ArmourFilters.EnergyShield;
            EquippableItem equippableItem = new(ItemRarity.Unique)
            {
                ArmourValues = new() { EnergyShield = 80 },
            };

            // act & assert
            this.CreateShouldReturnBindableMinMaxFilterViewModel(
                expectedBindingExpression,
                equippableItem,
                Resources.EnergyShield,
                equippableItem.ArmourValues.EnergyShield.Value,
                queryRequestFilter,
                offsetCurrentValue: true);
        }

        [TestCaseSource(nameof(GetMinMaxFilterTestCases))]
        public void CreateShouldReturnEvasionRatingFilterViewModel(MinMaxFilter queryRequestFilter)
        {
            // arrange
            Expression<Func<SearchQueryRequest, MinMaxFilter?>> expectedBindingExpression =
                x => x.Query.Filters.ArmourFilters.Evasion;
            EquippableItem equippableItem = new(ItemRarity.Unique)
            {
                ArmourValues = new() { EvasionRating = 97 },
            };

            // act & assert
            this.CreateShouldReturnBindableMinMaxFilterViewModel(
                expectedBindingExpression,
                equippableItem,
                Resources.Evasion,
                equippableItem.ArmourValues.EvasionRating.Value,
                queryRequestFilter,
                offsetCurrentValue: true);
        }

        [TestCaseSource(nameof(GetMinMaxFilterTestCases))]
        public void CreateShouldReturnWardFilterViewModel(MinMaxFilter queryRequestFilter)
        {
            // arrange
            Expression<Func<SearchQueryRequest, MinMaxFilter?>> expectedBindingExpression =
                x => x.Query.Filters.ArmourFilters.Ward;
            EquippableItem equippableItem = new(ItemRarity.Unique)
            {
                ArmourValues = new() { Ward = 30 },
            };

            // act & assert
            this.CreateShouldReturnBindableMinMaxFilterViewModel(
                expectedBindingExpression,
                equippableItem,
                Resources.Ward,
                equippableItem.ArmourValues.Ward.Value,
                queryRequestFilter,
                offsetCurrentValue: true);
        }

        [TestCaseSource(nameof(GetMinMaxFilterTestCases))]
        public void CreateShouldReturnDamageFilterViewModel(MinMaxFilter queryRequestFilter)
        {
            // arrange
            Expression<Func<SearchQueryRequest, MinMaxFilter?>> expectedBindingExpression =
                x => x.Query.Filters.WeaponFilters.Damage;
            EquippableItem equippableItem = new(ItemRarity.Unique)
            {
                WeaponValues = new()
                {
                    PhysicalDamage = new MinMaxValue { Min = 29, Max = 45 },
                    ElementalDamage =
                    [
                        new MinMaxValue { Min = 15, Max = 29 },
                        new MinMaxValue { Min = 7, Max = 13 },
                    ],
                },
            };

            // act & assert
            this.CreateShouldReturnBindableMinMaxFilterViewModel(
                expectedBindingExpression,
                equippableItem,
                Resources.Damage,
                equippableItem.WeaponValues.AverageDamage!.Value,
                queryRequestFilter);
        }

        [TestCaseSource(nameof(GetMinMaxFilterTestCases))]
        public void CreateShouldReturnAttacksPerSecondFilterViewModel(MinMaxFilter queryRequestFilter)
        {
            // arrange
            Expression<Func<SearchQueryRequest, MinMaxFilter?>> expectedBindingExpression =
                x => x.Query.Filters.WeaponFilters.AttacksPerSecond;
            EquippableItem equippableItem = new(ItemRarity.Unique)
            {
                WeaponValues = new() { AttacksPerSecond = 1.05m },
            };

            // act & assert
            this.CreateShouldReturnBindableMinMaxFilterViewModel(
                expectedBindingExpression,
                equippableItem,
                Resources.AttacksPerSecond,
                equippableItem.WeaponValues.AttacksPerSecond.Value,
                queryRequestFilter);
        }

        [TestCaseSource(nameof(GetMinMaxFilterTestCases))]
        public void CreateShouldReturnCriticalStrikeChanceFilterViewModel(MinMaxFilter queryRequestFilter)
        {
            // arrange
            Expression<Func<SearchQueryRequest, MinMaxFilter?>> expectedBindingExpression =
                x => x.Query.Filters.WeaponFilters.CriticalChance;
            EquippableItem equippableItem = new(ItemRarity.Unique)
            {
                WeaponValues = new() { CriticalStrikeChance = 6.5m },
            };

            // act & assert
            this.CreateShouldReturnBindableMinMaxFilterViewModel(
                expectedBindingExpression,
                equippableItem,
                Resources.CriticalStrikeChance,
                equippableItem.WeaponValues.CriticalStrikeChance.Value,
                queryRequestFilter);
        }

        [TestCaseSource(nameof(GetMinMaxFilterTestCases))]
        public void CreateShouldReturnQualityFilterViewModel(MinMaxFilter queryRequestFilter)
        {
            // arrange
            Expression<Func<SearchQueryRequest, MinMaxFilter?>> expectedBindingExpression =
                x => x.Query.Filters.MiscFilters.Quality;
            EquippableItem equippableItem = new(ItemRarity.Unique)
            {
                Quality = 7,
            };

            // act & assert
            this.CreateShouldReturnBindableMinMaxFilterViewModel(
                expectedBindingExpression,
                equippableItem,
                Resources.QualityColumn,
                equippableItem.Quality,
                queryRequestFilter);
        }

        [TestCaseSource(nameof(GetMinMaxFilterTestCases))]
        public void CreateShouldReturnItemLevelFilterViewModel(MinMaxFilter queryRequestFilter)
        {
            // arrange
            Expression<Func<SearchQueryRequest, MinMaxFilter?>> expectedBindingExpression =
                x => x.Query.Filters.MiscFilters.ItemLevel;
            EquippableItem equippableItem = new(ItemRarity.Unique)
            {
                ItemLevel = 84,
            };

            // act & assert
            this.CreateShouldReturnBindableMinMaxFilterViewModel(
                expectedBindingExpression,
                equippableItem,
                Resources.ItemLevelColumn,
                equippableItem.ItemLevel,
                queryRequestFilter);
        }

        [TestCaseSource(nameof(GetInfluenceFilterViewModelTestData))]
        public void CreateShouldReturnInfluenceFilterViewModelWithValueFromSearchQueryRequest(
            Expression<Func<SearchQueryRequest, BoolOptionFilter?>> expectedBindingExpression,
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
            this.CreateShouldReturnBindableBoolOptionFilterViewModel(
                expectedBindingExpression,
                equippableItem,
                equippableItem.Influence.GetDisplayName(),
                queryRequestFilter);
        }

        [Test]
        public void CreateShouldReturnSocketFilterViewModelIfItemHasAtLeastOneSocket()
        {
            // arrange
            Expression<Func<SearchQueryRequest, MinMaxFilter?>> expectedBindingExpression =
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
                Resources.Sockets,
                equippableItem.Sockets.Count,
                null);
        }

        [Test]
        public void CreateShouldReturnSocketFilterViewModelWithValuesFromSearchQueryRequest()
        {
            // arrange
            Expression<Func<SearchQueryRequest, MinMaxFilter?>> expectedBindingExpression =
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
            this.CreateShouldReturnBindableSocketsFilterViewModel(
                expectedBindingExpression,
                equippableItem,
                Resources.Sockets,
                equippableItem.Sockets.Count,
                queryRequestFilter);
        }

        [Test]
        public void CreateShouldNotReturnSocketFilterViewModelIfItemNoSockets()
        {
            EquippableItem equippableItem = new(ItemRarity.Rare);

            IEnumerable<FilterViewModelBase> result = this.AdditionalFilterViewModelsFactory.Create(equippableItem, new SearchQueryRequest());

            result.Should().NotContain(x => x.Text == Resources.Sockets);
        }

        [Test]
        public void CreateShouldReturnLinksFilterViewModelIfItemHasAtLeastOneSocket()
        {
            // arrange
            Expression<Func<SearchQueryRequest, MinMaxFilter?>> expectedBindingExpression =
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
                Resources.Links,
                0,
                null);
        }

        [Test]
        public void CreateShouldReturnLinksFilterViewModelWithMaxLinkCount()
        {
            // arrange
            Expression<Func<SearchQueryRequest, MinMaxFilter?>> expectedBindingExpression =
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
                Resources.Links,
                3,
                null);
        }

        [Test]
        public void CreateShouldReturnLinkFilterViewModelWithValuesFromSearchQueryRequest()
        {
            // arrange
            Expression<Func<SearchQueryRequest, MinMaxFilter?>> expectedBindingExpression =
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
            this.CreateShouldReturnBindableMinMaxFilterViewModel(
                expectedBindingExpression,
                equippableItem,
                Resources.Links,
                0,
                queryRequestFilter);
        }

        [Test]
        public void CreateShouldNotReturnLinkFilterViewModelIfItemNoSockets()
        {
            EquippableItem equippableItem = new(ItemRarity.Rare);

            IEnumerable<FilterViewModelBase> result = this.AdditionalFilterViewModelsFactory.Create(equippableItem, new SearchQueryRequest());

            result.Should().NotContain(x => x.Text == Resources.Links);
        }

        [TestCaseSource(nameof(GetBoolOptionFilterTestCases))]
        public void CreateShouldReturnIdentifiedFilterViewModel(BoolOptionFilter queryRequestFilter)
        {
            Expression<Func<SearchQueryRequest, BoolOptionFilter?>> expectedBindingExpression =
                x => x.Query.Filters.MiscFilters.Identified;
            EquippableItem equippableItem = new(ItemRarity.Rare)
            {
                IsIdentified = !queryRequestFilter.Option,
            };

            this.CreateShouldReturnBindableBoolOptionFilterViewModel(
                expectedBindingExpression,
                equippableItem,
                Resources.Identified,
                queryRequestFilter);
        }

        [TestCaseSource(nameof(GetBoolOptionFilterTestCases))]
        public void CreateShouldReturnCorruptedFilterViewModel(BoolOptionFilter queryRequestFilter)
        {
            Expression<Func<SearchQueryRequest, BoolOptionFilter?>> expectedBindingExpression =
                x => x.Query.Filters.MiscFilters.Corrupted;
            EquippableItem equippableItem = new(ItemRarity.Rare)
            {
                IsCorrupted = !queryRequestFilter.Option,
            };

            this.CreateShouldReturnBindableBoolOptionFilterViewModel(
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
                (Expression<Func<SearchQueryRequest, BoolOptionFilter?>>)(x => x.Query.Filters.MiscFilters.CrusaderItem),
                InfluenceType.Crusader);
            yield return new TestCaseData(
                (Expression<Func<SearchQueryRequest, BoolOptionFilter?>>)(x => x.Query.Filters.MiscFilters.ElderItem),
                InfluenceType.Elder);
            yield return new TestCaseData(
                (Expression<Func<SearchQueryRequest, BoolOptionFilter?>>)(x => x.Query.Filters.MiscFilters.HunterItem),
                InfluenceType.Hunter);
            yield return new TestCaseData(
                (Expression<Func<SearchQueryRequest, BoolOptionFilter?>>)(x => x.Query.Filters.MiscFilters.RedeemerItem),
                InfluenceType.Redeemer);
            yield return new TestCaseData(
                (Expression<Func<SearchQueryRequest, BoolOptionFilter?>>)(x => x.Query.Filters.MiscFilters.ShaperItem),
                InfluenceType.Shaper);
            yield return new TestCaseData(
                (Expression<Func<SearchQueryRequest, BoolOptionFilter?>>)(x => x.Query.Filters.MiscFilters.WarlordItem),
                InfluenceType.Warlord);
        }
    }
}