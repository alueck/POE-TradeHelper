using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using POETradeHelper.Common.Extensions;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.UI.Avalonia.Properties;
using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Factories.Implementations
{
    public class EquippableItemAdditionalFilterViewModelsFactory : AdditionalFilterViewModelsFactoryBase
    {
        public override IEnumerable<FilterViewModelBase> Create(Item item, SearchQueryRequest searchQueryRequest)
        {
            var result = new List<FilterViewModelBase>();

            if (item is EquippableItem equippableItem)
            {
                result.Add(this.GetQualityFilterViewModel(equippableItem, searchQueryRequest));
                result.Add(this.GetItemLevelFilterViewModel(equippableItem, searchQueryRequest));

                if (equippableItem.Sockets?.Count > 0)
                {
                    result.Add(GetSocketsFilterViewModel(equippableItem, searchQueryRequest));
                    result.Add(GetLinksFilterViewModel(equippableItem, searchQueryRequest));
                }

                if (equippableItem.Influence != InfluenceType.None)
                {
                    result.Add(GetInfluenceFilterViewModel(equippableItem, searchQueryRequest));
                }

                result.Add(this.GetIdentifiedFilterViewModel(searchQueryRequest));
                result.Add(this.GetCorruptedFilterViewModel(searchQueryRequest));
            }

            return result;
        }

        private FilterViewModelBase GetItemLevelFilterViewModel(EquippableItem equippableItem, SearchQueryRequest searchQueryRequest)
        {
            return this.CreateBindableMinMaxFilterViewModel(
                x => x.Query.Filters.MiscFilters.ItemLevel,
                Resources.ItemLevelColumn,
                equippableItem.ItemLevel,
                searchQueryRequest.Query.Filters.MiscFilters.ItemLevel);
        }

        private static FilterViewModelBase GetInfluenceFilterViewModel(EquippableItem equippableItem, SearchQueryRequest searchQueryRequest)
        {
            Expression<Func<SearchQueryRequest, IFilter>> bindingExpression = GetInfluenceBindingExpression(equippableItem.Influence);

            return new BindableFilterViewModel(bindingExpression)
            {
                Text = equippableItem.Influence.GetDisplayName(),
                IsEnabled = bindingExpression.Compile().Invoke(searchQueryRequest) is BoolOptionFilter boolOptionFilter ? boolOptionFilter.Option : null
            };
        }

        private static Expression<Func<SearchQueryRequest, IFilter>> GetInfluenceBindingExpression(InfluenceType influenceType)
        {
            return influenceType switch
            {
                InfluenceType.Crusader => x => x.Query.Filters.MiscFilters.CrusaderItem,
                InfluenceType.Elder => x => x.Query.Filters.MiscFilters.ElderItem,
                InfluenceType.Hunter => x => x.Query.Filters.MiscFilters.HunterItem,
                InfluenceType.Redeemer => x => x.Query.Filters.MiscFilters.RedeemerItem,
                InfluenceType.Shaper => x => x.Query.Filters.MiscFilters.ShaperItem,
                InfluenceType.Warlord => x => x.Query.Filters.MiscFilters.WarlordItem,
                _ => null,
            };
        }

        private static FilterViewModelBase GetSocketsFilterViewModel(EquippableItem equippableItem, SearchQueryRequest searchQueryRequest)
        {
            return CreateBindableSocketsFilterViewModel(
                x => x.Query.Filters.SocketFilters.Sockets,
                Resources.Sockets,
                equippableItem.Sockets.Count,
                searchQueryRequest.Query.Filters.SocketFilters.Sockets);
        }

        private static FilterViewModelBase GetLinksFilterViewModel(EquippableItem equippableItem, SearchQueryRequest searchQueryRequest)
        {
            return CreateBindableSocketsFilterViewModel(
                x => x.Query.Filters.SocketFilters.Links,
                Resources.Links,
                equippableItem.Sockets.SocketGroups.Max(x => x.Links),
                searchQueryRequest.Query.Filters.SocketFilters.Links);
        }

        private static FilterViewModelBase CreateBindableSocketsFilterViewModel(Expression<Func<SearchQueryRequest, IFilter>> bindingExpression, string text, int currentValue, SocketsFilter queryRequestFilter)
        {
            var result = new BindableSocketsFilterViewModel(bindingExpression)
            {
                Current = currentValue.ToString(),
                Text = text
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
    }
}
