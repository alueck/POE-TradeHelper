using Microsoft.Extensions.DependencyInjection;

using POETradeHelper.Common.Contract;
using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels;
using POETradeHelper.ItemSearch.UI.Avalonia.Views;

using ReactiveUI;

namespace POETradeHelper.ItemSearch.UI.Avalonia;

public class Module : IModule
{
    public void RegisterServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IViewFor<ItemResultsViewModel>, ItemResultsView>();
        serviceCollection.AddSingleton<IViewFor<ExchangeResultsViewModel>, ExchangeResultsView>();
    }
}