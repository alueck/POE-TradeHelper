using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.Options;
using POETradeHelper.Common.UI;
using POETradeHelper.ItemSearch.Contract.Controllers;
using POETradeHelper.ViewModels;
using POETradeHelper.Views;
using Splat;
using System.Collections.Generic;
using Application = Avalonia.Application;

namespace POETradeHelper
{
    public class App : Application
    {
        private IItemSearchResultOverlayController itemSearchResultOverlayController;

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var viewModel = new MainWindowViewModel(Locator.Current.GetService<IEnumerable<ISettingsViewModel>>(), Locator.Current.GetService<IOptions<AppSettings>>());
                desktop.MainWindow = new MainWindow
                {
                    DataContext = viewModel
                };
            }

            base.OnFrameworkInitializationCompleted();
            itemSearchResultOverlayController = Locator.Current.GetService<IItemSearchResultOverlayController>();
        }
    }
}