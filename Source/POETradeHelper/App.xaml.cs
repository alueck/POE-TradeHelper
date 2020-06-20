using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.Options;
using POETradeHelper.Common;
using POETradeHelper.Common.UI;
using POETradeHelper.ViewModels;
using POETradeHelper.Views;
using Splat;
using Application = Avalonia.Application;

namespace POETradeHelper
{
    public class App : Application
    {
        private IEnumerable<IUserInputEventHandler> userInputEventHandlers;

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

            // global key hook does not work with e.g. auto activation in Bootstrapper, so we instantiate our user input event handlers here
            this.userInputEventHandlers = Locator.Current.GetServices<IUserInputEventHandler>();
        }
    }
}