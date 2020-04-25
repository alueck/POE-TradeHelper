using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.PathOfExileTradeApi.Models;
using System.Collections.Generic;

namespace POETradeHelper.ItemSearch.Services.Factories
{
    public class FlaskItemAdditionalFilterViewModelsFactory : AdditionalFilterViewModelsFactoryBase
    {
        public override IEnumerable<FilterViewModelBase> Create(Item item, SearchQueryRequest searchQueryRequest)
        {
            var result = new List<FilterViewModelBase>();

            if (item is FlaskItem flaskItem)
            {
                result.Add(this.GetQualityFilterViewModel(flaskItem, searchQueryRequest));
                result.Add(this.GetIdentifiedFilterViewModel(flaskItem, searchQueryRequest));
            }

            return result;
        }
    }
}