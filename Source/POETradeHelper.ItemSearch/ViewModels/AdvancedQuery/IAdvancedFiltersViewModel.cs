using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Models;

namespace POETradeHelper.ItemSearch.ViewModels
{
    public interface IAdvancedFiltersViewModel
    {
        IList<FilterViewModelBase> AdditionalFilters { get; }

        IEnumerable<StatFilterViewModel> AllStatFilters{ get; }
        
        Task LoadAsync(Item item, IQueryRequest queryRequest, CancellationToken cancellationToken);
    }
}