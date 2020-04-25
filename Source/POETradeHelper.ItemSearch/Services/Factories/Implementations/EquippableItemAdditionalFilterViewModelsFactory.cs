using POETradeHelper.Common.Extensions;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Properties;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace POETradeHelper.ItemSearch.Services.Factories
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
                    result.Add(this.GetSocketsFilterViewModel(equippableItem, searchQueryRequest));
                    result.Add(this.GetLinksFilterViewModel(equippableItem, searchQueryRequest));
                }

                if (equippableItem.Influence != InfluenceType.None)
                {
                    result.Add(this.GetInfluenceFilterViewModel(equippableItem, searchQueryRequest));
                }

                result.Add(this.GetIdentifiedFilterViewModel(equippableItem, searchQueryRequest));
                result.Add(this.GetCorruptedFilterViewModel(equippableItem, searchQueryRequest));
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

        private FilterViewModelBase GetInfluenceFilterViewModel(EquippableItem equippableItem, SearchQueryRequest searchQueryRequest)
        {
            Expression<Func<SearchQueryRequest, IFilter>> bindingExpression = GetInfluenceBindingExpression(equippableItem.Influence);

            return new BindableFilterViewModel(bindingExpression)
            {
                Text = equippableItem.Influence.GetDisplayName(),
                IsEnabled = bindingExpression.Compile().Invoke(searchQueryRequest) is BoolOptionFilter boolOptionFilter && boolOptionFilter.Option
            };
        }

        private static Expression<Func<SearchQueryRequest, IFilter>> GetInfluenceBindingExpression(InfluenceType influenceType)
        {
            Expression<Func<SearchQueryRequest, IFilter>> bindingExpression = null;

            switch (influenceType)
            {
                case InfluenceType.Crusader:
                    bindingExpression = x => x.Query.Filters.MiscFilters.CrusaderItem;
                    break;

                case InfluenceType.Elder:
                    bindingExpression = x => x.Query.Filters.MiscFilters.ElderItem;
                    break;

                case InfluenceType.Hunter:
                    bindingExpression = x => x.Query.Filters.MiscFilters.HunterItem;
                    break;

                case InfluenceType.Redeemer:
                    bindingExpression = x => x.Query.Filters.MiscFilters.RedeemerItem;
                    break;

                case InfluenceType.Shaper:
                    bindingExpression = x => x.Query.Filters.MiscFilters.ShaperItem;
                    break;

                case InfluenceType.Warlord:
                    bindingExpression = x => x.Query.Filters.MiscFilters.WarlordItem;
                    break;
            }

            return bindingExpression;
        }

        private FilterViewModelBase GetSocketsFilterViewModel(EquippableItem equippableItem, SearchQueryRequest searchQueryRequest)
        {
            return this.CreateBindableMinMaxFilterViewModel(
                x => x.Query.Filters.SocketFilters.Sockets,
                Resources.Sockets,
                equippableItem.Sockets.Count,
                searchQueryRequest.Query.Filters.SocketFilters.Sockets);
        }

        private FilterViewModelBase GetLinksFilterViewModel(EquippableItem equippableItem, SearchQueryRequest searchQueryRequest)
        {
            return this.CreateBindableMinMaxFilterViewModel(
                x => x.Query.Filters.SocketFilters.Links,
                Resources.Links,
                equippableItem.Sockets.SocketGroups.Max(x => x.Links),
                searchQueryRequest.Query.Filters.SocketFilters.Links);
        }
    }
}