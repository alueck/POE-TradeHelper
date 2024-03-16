using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Microsoft.Extensions.Options;

using POETradeHelper.Common.Extensions;
using POETradeHelper.ItemSearch.Contract.Configuration;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.UI.Avalonia.Properties;
using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Factories.Implementations
{
    public class EquippableItemAdditionalFilterViewModelsFactory : AdditionalFilterViewModelsFactoryBase
    {
        public EquippableItemAdditionalFilterViewModelsFactory(IOptionsMonitor<ItemSearchOptions> itemSearchOptions) : base(itemSearchOptions)
        {
        }

        public override IEnumerable<FilterViewModelBase> Create(Item item, SearchQueryRequest searchQueryRequest)
        {
            List<FilterViewModelBase> result = [];

            if (item is not EquippableItem equippableItem)
            {
                return result;
            }

            result.AddRange(this.GetArmourFilterViewModels(equippableItem, searchQueryRequest));
            result.AddRange(this.GetWeaponFilterViewModels(equippableItem, searchQueryRequest));
            result.Add(this.GetQualityFilterViewModel(equippableItem, searchQueryRequest));
            result.Add(this.GetItemLevelFilterViewModel(equippableItem, searchQueryRequest));

            if (equippableItem.Sockets?.Count > 0)
            {
                result.Add(GetSocketsFilterViewModel(equippableItem, searchQueryRequest));
                result.Add(GetLinksFilterViewModel(equippableItem, searchQueryRequest));
            }

            result.AddIfNotNull(GetInfluenceFilterViewModel(equippableItem, searchQueryRequest));
            result.Add(this.GetIdentifiedFilterViewModel(searchQueryRequest));
            result.Add(this.GetCorruptedFilterViewModel(searchQueryRequest));
            result.Add(this.GetSynthesisedFilterViewModel(searchQueryRequest));

            return result;
        }

        private IEnumerable<FilterViewModelBase> GetArmourFilterViewModels(EquippableItem equippableItem, SearchQueryRequest searchQueryRequest)
        {
            ArmourValues? armourValues = equippableItem.ArmourValues;

            if (armourValues == null)
            {
                yield break;
            }

            if (armourValues.Armour.HasValue)
            {
                yield return this.CreateBindableMinMaxFilterViewModel(
                    x => x.Query.Filters.ArmourFilters.Armour,
                    Resources.Armour,
                    armourValues.Armour.Value,
                    searchQueryRequest,
                    true);
            }

            if (armourValues.BlockChance.HasValue)
            {
                yield return this.CreateBindableMinMaxFilterViewModel(
                    x => x.Query.Filters.ArmourFilters.Block,
                    Resources.ChanceToBlock,
                    armourValues.BlockChance.Value,
                    searchQueryRequest,
                    true);
            }

            if (armourValues.EnergyShield.HasValue)
            {
                yield return this.CreateBindableMinMaxFilterViewModel(
                    x => x.Query.Filters.ArmourFilters.EnergyShield,
                    Resources.EnergyShield,
                    armourValues.EnergyShield.Value,
                    searchQueryRequest,
                    true);
            }

            if (armourValues.EvasionRating.HasValue)
            {
                yield return this.CreateBindableMinMaxFilterViewModel(
                    x => x.Query.Filters.ArmourFilters.Evasion,
                    Resources.Evasion,
                    armourValues.EvasionRating.Value,
                    searchQueryRequest,
                    true);
            }

            if (armourValues.Ward.HasValue)
            {
                yield return this.CreateBindableMinMaxFilterViewModel(
                    x => x.Query.Filters.ArmourFilters.Ward,
                    Resources.Ward,
                    armourValues.Ward.Value,
                    searchQueryRequest,
                    true);
            }
        }

        private IEnumerable<FilterViewModelBase> GetWeaponFilterViewModels(EquippableItem equippableItem, SearchQueryRequest searchQueryRequest)
        {
            WeaponValues? weaponValues = equippableItem.WeaponValues;

            if (weaponValues == null)
            {
                yield break;
            }

            if (weaponValues.AttacksPerSecond.HasValue)
            {
                yield return this.CreateBindableMinMaxFilterViewModel(
                    x => x.Query.Filters.WeaponFilters.AttacksPerSecond,
                    Resources.AttacksPerSecond,
                    weaponValues.AttacksPerSecond.Value,
                    searchQueryRequest);
            }

            if (weaponValues.CriticalStrikeChance.HasValue)
            {
                yield return this.CreateBindableMinMaxFilterViewModel(
                    x => x.Query.Filters.WeaponFilters.CriticalChance,
                    Resources.CriticalStrikeChance,
                    weaponValues.CriticalStrikeChance.Value,
                    searchQueryRequest);
            }

            if (weaponValues.AverageDamage > 0)
            {
                yield return this.CreateBindableMinMaxFilterViewModel(
                    x => x.Query.Filters.WeaponFilters.Damage,
                    Resources.Damage,
                    weaponValues.AverageDamage.Value,
                    searchQueryRequest);
            }
        }

        private FilterViewModelBase GetItemLevelFilterViewModel(EquippableItem equippableItem, SearchQueryRequest searchQueryRequest) =>
            base.CreateBindableMinMaxFilterViewModel(
                x => x.Query.Filters.MiscFilters.ItemLevel,
                Resources.ItemLevelColumn,
                equippableItem.ItemLevel,
                searchQueryRequest);

        private static FilterViewModelBase? GetInfluenceFilterViewModel(EquippableItem equippableItem, SearchQueryRequest searchQueryRequest)
        {
            Expression<Func<SearchQueryRequest, BoolOptionFilter?>>? bindingExpression = GetInfluenceBindingExpression(equippableItem.Influence);

            return bindingExpression != null
                ? new BindableFilterViewModel<BoolOptionFilter>(bindingExpression)
                {
                    Text = equippableItem.Influence.GetDisplayName(),
                    IsEnabled = bindingExpression.Compile().Invoke(searchQueryRequest) is BoolOptionFilter boolOptionFilter
                        ? boolOptionFilter.Option
                        : null,
                }
                : null;
        }

        private static Expression<Func<SearchQueryRequest, BoolOptionFilter?>>? GetInfluenceBindingExpression(
            InfluenceType influenceType) =>
            influenceType switch
            {
                InfluenceType.Crusader => x => x.Query.Filters.MiscFilters.CrusaderItem,
                InfluenceType.Elder => x => x.Query.Filters.MiscFilters.ElderItem,
                InfluenceType.Hunter => x => x.Query.Filters.MiscFilters.HunterItem,
                InfluenceType.Redeemer => x => x.Query.Filters.MiscFilters.RedeemerItem,
                InfluenceType.Shaper => x => x.Query.Filters.MiscFilters.ShaperItem,
                InfluenceType.Warlord => x => x.Query.Filters.MiscFilters.WarlordItem,
                _ => null,
            };

        private static FilterViewModelBase GetSocketsFilterViewModel(EquippableItem equippableItem, SearchQueryRequest searchQueryRequest) =>
            CreateBindableSocketsFilterViewModel(
                x => x.Query.Filters.SocketFilters.Sockets,
                Resources.Sockets,
                equippableItem.Sockets!.Count,
                searchQueryRequest.Query.Filters.SocketFilters.Sockets);

        private static FilterViewModelBase GetLinksFilterViewModel(EquippableItem equippableItem, SearchQueryRequest searchQueryRequest) =>
            CreateBindableSocketsFilterViewModel(
                x => x.Query.Filters.SocketFilters.Links,
                Resources.Links,
                equippableItem.Sockets!.SocketGroups.Max(x => x.Links),
                searchQueryRequest.Query.Filters.SocketFilters.Links);

        private static FilterViewModelBase CreateBindableSocketsFilterViewModel(
            Expression<Func<SearchQueryRequest, MinMaxFilter?>> bindingExpression,
            string text,
            int currentValue,
            SocketsFilter? queryRequestFilter)
        {
            BindableSocketsFilterViewModel result = new(bindingExpression)
            {
                Current = currentValue.ToString(),
                Text = text,
            };

            if (queryRequestFilter != null)
            {
                result.Min = queryRequestFilter.Min;
                result.Max = queryRequestFilter.Max;
                result.Red = queryRequestFilter.Red;
                result.Green = queryRequestFilter.Green;
                result.Blue = queryRequestFilter.Blue;
                result.White = queryRequestFilter.White;
                result.IsEnabled = true;
            }
            else
            {
                result.Min = currentValue;
                result.Max = currentValue;
            }

            return result;
        }

        private BindableMinMaxFilterViewModel CreateBindableMinMaxFilterViewModel(
            Expression<Func<SearchQueryRequest, MinMaxFilter?>> bindingExpression,
            string text,
            decimal currentValue,
            SearchQueryRequest searchQueryRequest)
        {
            BindableMinMaxFilterViewModel result = new(bindingExpression)
            {
                Current = currentValue.ToString("N2"),
                Text = text,
            };

            MinMaxFilter? queryRequestFilter = GetValueGetter(bindingExpression)(searchQueryRequest);
            if (queryRequestFilter != null)
            {
                result.Min = queryRequestFilter.Min;
                result.Max = queryRequestFilter.Max;
                result.IsEnabled = true;
            }
            else
            {
                result.Min = Math.Round(currentValue * (1 + this.ItemSearchOptions.CurrentValue.AdvancedQueryOptions.MinValuePercentageOffset), 2);
                result.Max = Math.Round(currentValue * (1 + this.ItemSearchOptions.CurrentValue.AdvancedQueryOptions.MaxValuePercentageOffset), 2);
            }

            return result;
        }
    }
}