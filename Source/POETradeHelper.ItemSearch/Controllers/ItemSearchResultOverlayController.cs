using Avalonia.Controls;
using POETradeHelper.Common.Contract;
using POETradeHelper.ItemSearch.Contract.Controllers;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.ItemSearch.Views;
using System;
using System.ComponentModel;
using System.Threading;

namespace POETradeHelper.ItemSearch.Controllers
{
    public class ItemSearchResultOverlayController : IItemSearchResultOverlayController, IDisposable
    {
        private readonly IItemSearchResultOverlayViewModel itemSearchResultOverlayViewModel;
        private readonly IUserInputEventProvider userInputEventProvider;
        private readonly IViewLocator viewLocator;
        private readonly IPathOfExileProcessHelper pathOfExileProcessHelper;

        public ItemSearchResultOverlayController(
            IItemSearchResultOverlayViewModel itemSearchResultOverlayViewModel,
            IViewLocator viewLocator,
            IUserInputEventProvider userInputEventProvider,
            IPathOfExileProcessHelper pathOfExileProcessHelper)
        {
            this.itemSearchResultOverlayViewModel = itemSearchResultOverlayViewModel;
            this.viewLocator = viewLocator;
            this.userInputEventProvider = userInputEventProvider;

            this.userInputEventProvider.SearchItem += UserInputEventProvider_SearchItem;
            this.userInputEventProvider.HideOverlay += UserInputEventProvider_HideOverlay;
            this.pathOfExileProcessHelper = pathOfExileProcessHelper;
        }

        private IItemSearchResultOverlayView view;
        private IItemSearchResultOverlayView View => LazyInitializer.EnsureInitialized(ref view, CreateView);

        private IItemSearchResultOverlayView CreateView()
        {
            var view = viewLocator.GetView(itemSearchResultOverlayViewModel);
            if (view is IItemSearchResultOverlayView itemSearchResultOverlay)
            {
                if (view is IControl control)
                {
                    control.DataContext = itemSearchResultOverlayViewModel;
                }

                return itemSearchResultOverlay;
            }

            throw new ArgumentException($"Could not find view for {nameof(IItemSearchResultOverlayViewModel)} that implements {nameof(IItemSearchResultOverlayView)}");
        }

        private async void UserInputEventProvider_SearchItem(object sender, HandledEventArgs e)
        {
            if (!this.pathOfExileProcessHelper.IsPathOfExileActiveWindow())
            {
                return;
            }

            e.Handled = true;

            await this.itemSearchResultOverlayViewModel.SetListingForItemUnderCursorAsync();
            this.View.Show();
        }

        private void UserInputEventProvider_HideOverlay(object sender, HandledEventArgs e)
        {
            if (View.IsVisible)
            {
                View.Hide();
                e.Handled = true;
            }
        }

        public void Dispose()
        {
            this.userInputEventProvider.SearchItem -= UserInputEventProvider_SearchItem;

            this.userInputEventProvider.Dispose();
        }
    }
}