using System;
using System.Threading;
using System.Threading.Tasks;

using Avalonia.Controls;

using MediatR;

using POETradeHelper.Common.Contract;
using POETradeHelper.Common.Contract.Attributes;
using POETradeHelper.Common.Contract.Commands;
using POETradeHelper.Common.UI;
using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels.Abstractions;
using POETradeHelper.ItemSearch.UI.Avalonia.Views;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Controllers
{
    [Singleton]
    public class ItemSearchResultOverlayController : IRequestHandler<SearchItemCommand>, IRequestHandler<HideOverlayCommand>
    {
        private readonly IItemSearchResultOverlayViewModel itemSearchResultOverlayViewModel;
        private readonly IViewLocator viewLocator;
        private readonly IUiThreadDispatcher uiThreadDispatcher;

        private CancellationTokenSource searchItemCancellationTokenSource = new();

        public ItemSearchResultOverlayController(
            IItemSearchResultOverlayViewModel itemSearchResultOverlayViewModel,
            IViewLocator viewLocator,
            IUiThreadDispatcher uiThreadDispatcher)
        {
            this.itemSearchResultOverlayViewModel = itemSearchResultOverlayViewModel;
            this.viewLocator = viewLocator;
            this.uiThreadDispatcher = uiThreadDispatcher;
        }

        private IItemSearchResultOverlayView? view;
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

            throw new ArgumentException(
                $"Could not find view for {nameof(IItemSearchResultOverlayViewModel)} that implements {nameof(IItemSearchResultOverlayView)}");
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

        public async Task<Unit> Handle(SearchItemCommand request, CancellationToken cancellationToken)
        {
            await this.uiThreadDispatcher.InvokeAsync(async () =>
            {
                try
                {
                    this.CancelSearchItemToken();

                    this.View.Show();

                    await this.itemSearchResultOverlayViewModel
                        .SetListingForItemUnderCursorAsync(this.searchItemCancellationTokenSource.Token)
                        .ConfigureAwait(true);
                }
                catch (Exception exception) when (exception is OperationCanceledException or TaskCanceledException)
                {
                    // do nothing
                }
            });

            return Unit.Value;
        }

        public async Task<Unit> Handle(HideOverlayCommand request, CancellationToken cancellationToken)
        {
            await this.uiThreadDispatcher.InvokeAsync(() =>
            {
                if (View.IsVisible)
                {
                    request.OnHandled();
                    this.CancelSearchItemToken();
                    View.Hide();
                }
            });

            return Unit.Value;
        }
    }
}
