using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using POETradeHelper.Common.Contract;
using POETradeHelper.Common.UI;
using POETradeHelper.ViewModels;
using POETradeHelper.Views;

using Splat;

namespace POETradeHelper
{
    public class App : Application
    {
        public override void Initialize() => AvaloniaXamlLoader.Load(this);

        public override void OnFrameworkInitializationCompleted()
        {
            if (this.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                MainWindowViewModel viewModel = new(
                    Locator.Current.GetServices<ISettingsViewModel>(),
                    Locator.Current.GetServices<IInitializable>());
                desktop.MainWindow = new MainWindow
                {
                    DataContext = viewModel,
                };
                desktop.ShutdownRequested += OnDesktopOnShutdownRequested;
            }

            base.OnFrameworkInitializationCompleted();
        }

        private static void OnDesktopOnShutdownRequested(object? sender, ShutdownRequestedEventArgs e) =>
            Bootstrapper.Shutdown();
    }
}