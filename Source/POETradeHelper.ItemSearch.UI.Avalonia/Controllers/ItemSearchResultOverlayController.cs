using System;
using System.Threading;
using System.Threading.Tasks;

using Mediator;

using POETradeHelper.Common.Contract;
using POETradeHelper.Common.Contract.Attributes;
using POETradeHelper.Common.Contract.Commands;
using POETradeHelper.Common.UI;
using POETradeHelper.ItemSearch.UI.Avalonia.ViewModels.Abstractions;
using POETradeHelper.ItemSearch.UI.Avalonia.Views;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Controllers
{
    [Singleton]
    public class ItemSearchResultOverlayController : IRequestHandler<SearchItemCommand>, IRequestHandler<HideOverlayCommand>, IOverlayStatusProvider
    {
        private readonly Lock lockObj = new();
        private readonly IItemSearchResultOverlayViewModel itemSearchResultOverlayViewModel;
        private readonly IViewLocator viewLocator;
        private readonly IUiThreadDispatcher uiThreadDispatcher;

        private bool isOverlayVisible;
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

        bool IOverlayStatusProvider.IsVisible => this.isOverlayVisible;

        private IItemSearchResultOverlayView View => LazyInitializer.EnsureInitialized(ref field, this.CreateView);

        public async ValueTask<Unit> Handle(SearchItemCommand request, CancellationToken cancellationToken)
        {
            await this.uiThreadDispatcher.InvokeAsync(async () =>
            {
                try
                {
                    this.CancelSearchItemToken();

                    this.View.Show();
                    this.isOverlayVisible = true;

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

        public async ValueTask<Unit> Handle(HideOverlayCommand request, CancellationToken cancellationToken)
        {
            await this.uiThreadDispatcher.InvokeAsync(() =>
            {
                if (this.View.IsVisible)
                {
                    this.CancelSearchItemToken();
                    this.View.Hide();
                    this.isOverlayVisible = false;
                }
            });

            return Unit.Value;
        }

        private IItemSearchResultOverlayView CreateView()
        {
            if (this.viewLocator.GetView(this.itemSearchResultOverlayViewModel) is IItemSearchResultOverlayView itemSearchResultOverlay)
            {
                itemSearchResultOverlay.DataContext = this.itemSearchResultOverlayViewModel;

                return itemSearchResultOverlay;
            }

            throw new ArgumentException(
                $"Could not find view for {nameof(IItemSearchResultOverlayViewModel)} that implements {nameof(IItemSearchResultOverlayView)}");
        }

        private void CancelSearchItemToken()
        {
            lock (this.lockObj)
            {
                this.searchItemCancellationTokenSource.Cancel();
                this.searchItemCancellationTokenSource.Dispose();
                this.searchItemCancellationTokenSource = new CancellationTokenSource();
            }
        }
    }
}