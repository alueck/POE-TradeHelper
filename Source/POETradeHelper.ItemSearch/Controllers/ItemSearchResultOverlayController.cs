using System;
using System.ComponentModel;
using System.Threading;
using Avalonia.Controls;
using POETradeHelper.Common;
using POETradeHelper.Common.Contract;
using POETradeHelper.ItemSearch.ViewModels;
using POETradeHelper.ItemSearch.Views;

namespace POETradeHelper.ItemSearch.Controllers
{
    public class ItemSearchResultOverlayController : IUserInputEventHandler
    {
        private readonly IItemSearchResultOverlayViewModel itemSearchResultOverlayViewModel;
        private readonly IUserInputEventProvider userInputEventProvider;
        private readonly IViewLocator viewLocator;

        private CancellationTokenSource searchItemCancellationTokenSource = new CancellationTokenSource();

        public ItemSearchResultOverlayController(
            IItemSearchResultOverlayViewModel itemSearchResultOverlayViewModel,
            IViewLocator viewLocator,
            IUserInputEventProvider userInputEventProvider)
        {
            this.itemSearchResultOverlayViewModel = itemSearchResultOverlayViewModel;
            this.viewLocator = viewLocator;
            this.userInputEventProvider = userInputEventProvider;

            this.userInputEventProvider.SearchItem += UserInputEventProvider_SearchItem;
            this.userInputEventProvider.HideOverlay += UserInputEventProvider_HideOverlay;
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
            e.Handled = true;

            this.CancelSearchItemToken();

            if (!this.searchItemCancellationTokenSource.IsCancellationRequested)
            {
                this.View.Show();
            }

            await this.itemSearchResultOverlayViewModel.SetListingForItemUnderCursorAsync(this.searchItemCancellationTokenSource.Token).ConfigureAwait(true);
        }

        private void UserInputEventProvider_HideOverlay(object sender, HandledEventArgs e)
        {
            if (View.IsVisible)
            {
                this.CancelSearchItemToken();
                View.Hide();
                e.Handled = true;
            }
        }

        private void CancelSearchItemToken()
        {
            lock (this)
            {
                this.searchItemCancellationTokenSource.Cancel();
                this.searchItemCancellationTokenSource.Dispose();
                this.searchItemCancellationTokenSource = new CancellationTokenSource();
            }
        }

        public void Dispose()
        {
            this.userInputEventProvider.SearchItem -= UserInputEventProvider_SearchItem;

            this.userInputEventProvider.Dispose();
        }
    }
}