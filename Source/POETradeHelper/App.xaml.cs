using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using POETradeHelper.ItemSearch.Controllers;
using Splat;
using System;
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
            base.OnFrameworkInitializationCompleted();
            itemSearchResultOverlayController = Locator.Current.GetService<IItemSearchResultOverlayController>();
        }
    }
}