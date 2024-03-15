using System;
using System.Linq.Expressions;

using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Models.Filters;

namespace POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;

public interface IBindableFilterViewModel : IFilterViewModel
{
    Expression<Func<SearchQueryRequest, IFilter?>> BindingExpression { get; }
}