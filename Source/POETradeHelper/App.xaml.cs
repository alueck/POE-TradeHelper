using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using POETradeHelper.Common;
using POETradeHelper.Common.Contract;
using POETradeHelper.Common.UI;
using POETradeHelper.ViewModels;
using POETradeHelper.Views;
using Splat;

namespace POETradeHelper
{
    public class App : Application
    {
        private IUserInputEventProvider userInputEventProvider;

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var viewModel = new MainWindowViewModel(Locator.Current.GetService<IEnumerable<ISettingsViewModel>>(), Locator.Current.GetServices<IInitializable>());
                desktop.MainWindow = new MainWindow
                {
                    DataContext = viewModel
                };
            }

            base.OnFrameworkInitializationCompleted();

            // global key hook does not work with e.g. auto activation in Bootstrapper
            this.userInputEventProvider = Locator.Current.GetService<IUserInputEventProvider>();
        }
    }
}