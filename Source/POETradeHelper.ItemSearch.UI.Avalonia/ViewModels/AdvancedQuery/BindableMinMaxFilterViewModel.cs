using System;
using System.Linq.Expressions;

using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;

namespace POETradeHelper.ItemSearch.UI.Avalonia.ViewModels
{
    public class BindableMinMaxFilterViewModel : BindableFilterViewModel, IMinMaxFilterViewModel
    {
        public BindableMinMaxFilterViewModel(Expression<Func<SearchQueryRequest, IFilter>> bindingExpression) : base(bindingExpression)
        {
            this.IsEnabled = false;
        }

        public decimal? Min { get; set; }

        public decimal? Max { get; set; }

        public string Current { get; set; }
    }
}